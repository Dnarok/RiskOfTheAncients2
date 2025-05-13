using BepInEx.Configuration;
using R2API;
using RoR2;
using System;
using UnityEngine;

namespace ROTA2.Items
{
    public class InfusedRaindrops : ItemBase<InfusedRaindrops>
    {
        public override string ItemName => "Infused Raindrops";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "INFUSED_RAINDROPS";
        public override string ItemTokenPickup => "Reduces the damage of large hits until consumed.";
        public override string ItemTokenDesc => $"Taking {Health($"more than {DamageThreshold}%")} of your {Health("maximum health")} as {Damage("damage")} will {Damage("reduce the damage")} by {Damage($"{DamageBlock}")}. Can occur {Utility($"{BlockCount} times")} per stack before the stack is {Utility("consumed")}.";
        public override string ItemTokenLore => "Elemental protection from magical assaults.";
        public override ItemTier Tier => ItemTier.Tier1;
        public override string ItemIconPath => "ROTA2.Icons.infused_raindrops.png";
        public override void Hooks()
        {
            CharacterBody.onBodyInventoryChangedGlobal += OnInventoryChanged;
        }
        public override void Init(ConfigFile configuration)
        {
            CreateConfig(configuration);
            CreateLanguageTokens();
            CreateItemDef();
            Hooks();
        }

        public float DamageThreshold;
        public float DamageBlock;
        public int BlockCount;
        private void CreateConfig(ConfigFile configuration)
        {
            DamageThreshold = configuration.Bind("Item: " + ItemName, "Incoming Damage Percent Threshold", 5.0f, "").Value;
            DamageBlock     = configuration.Bind("Item: " + ItemName, "Flat Damage Block", 50.0f, "").Value;
            BlockCount      = configuration.Bind("Item: " + ItemName, "Block Count Per Stack", 10, "").Value;
        }

        private void OnInventoryChanged(CharacterBody body)
        {
            int count = GetCount(body);
            if (GetCount(body) > 0 && !body.GetComponent<InfusedRaindropsBehavior>())
            {
                body.gameObject.AddComponent<InfusedRaindropsBehavior>();
            }
        }

        public class InfusedRaindropsBehavior : MonoBehaviour
        {
            CharacterBody body;
            public int remaining_charges;
            int last_count;
            private bool ignore;

            public void Awake()
            {
                body = GetComponent<CharacterBody>();
                last_count = InfusedRaindrops.Instance.GetCount(body);
                remaining_charges = last_count * InfusedRaindrops.Instance.BlockCount;
                ignore = false;

                On.RoR2.CharacterBody.OnInventoryChanged += OnInventoryChanged;
                On.RoR2.HealthComponent.TakeDamage += OnTakeDamage;
            }

            private void OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody body)
            {
                if (ignore || body != this.body)
                {
                    orig(body);
                    return;
                }

                int count = InfusedRaindrops.Instance.GetCount(body);
                if (count > last_count)
                {
                    remaining_charges += (count - last_count) * InfusedRaindrops.Instance.BlockCount;
                    last_count = count;
                }
                else if (count < last_count)
                {
                    remaining_charges -= count * InfusedRaindrops.Instance.BlockCount;
                    last_count = count;
                }

                orig(body);
            }
            private void OnTakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo info)
            {
                if (info.rejected || !self || self.body != body || remaining_charges <= 0)
                {
                    orig(self, info);
                    return;
                }

                if (info.damage > self.fullCombinedHealth * InfusedRaindrops.Instance.DamageThreshold / 100.0f)
                {
                    info.damage = Math.Max(0.0f, info.damage - InfusedRaindrops.Instance.DamageBlock);
                    if (info.damage == 0.0f)
                    {
                        EffectData effect = new EffectData
                        {
                            origin = info.position,
                            rotation = Util.QuaternionSafeLookRotation((info.force != Vector3.zero) ? info.force : UnityEngine.Random.onUnitSphere)
                        };
                        EffectManager.SpawnEffect(HealthComponent.AssetReferences.bearEffectPrefab, effect, transmit: true);
                        info.rejected = true;
                    }

                    --remaining_charges;
                    if (remaining_charges % 10 == 0)
                    {
                        ignore = true;
                        body.inventory.RemoveItem(InfusedRaindrops.Instance.ItemDef, 1);
                        body.inventory.GiveItem(DehydratedRaindrops.Instance.ItemDef, 1);
                        ignore = false;

                        CharacterMasterNotificationQueue.PushItemTransformNotification(body.master, InfusedRaindrops.Instance.ItemDef.itemIndex, DehydratedRaindrops.Instance.ItemDef.itemIndex, CharacterMasterNotificationQueue.TransformationType.Default);
                        body.RecalculateStats();
                    }
                }

                orig(self, info);
            }
        }
    }

    public class DehydratedRaindrops : ItemBase<DehydratedRaindrops>
    {
        public override string ItemName => "Dehydrated Raindrops";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "DEHYDRATED_RAINDROPS";
        public override string ItemTokenPickup => "Reduces skill cooldowns.";
        public override string ItemTokenDesc => $"Reduces {Utility("skill cooldowns")} by {Utility($"{SkillCooldownReductionBase}%")} {Stack($"(+{SkillCooldownReductionPerStack}% per stack)")}.";
        public override string ItemTokenLore => "Dried up.";
        public override ItemTier Tier => ItemTier.NoTier;
        public override ItemTag[] ItemTags => [ItemTag.WorldUnique];
        public override string ItemIconPath => "ROTA2.Icons.dehydrated_raindrops.png";
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddCooldownReduction;
        }
        public override void Init(ConfigFile configuration)
        {
            CreateConfig(configuration);
            CreateLanguageTokens();
            CreateItemDef();
            Hooks();
        }

        public float SkillCooldownReductionBase;
        public float SkillCooldownReductionPerStack;
        public void CreateConfig(ConfigFile configuration)
        {
            SkillCooldownReductionBase = configuration.Bind("Item: " + ItemName, "Initial Skill Cooldown Reduction", 2.5f, "How much skill cooldown reduction should be provided by the first stack?").Value;
            SkillCooldownReductionPerStack = configuration.Bind("Item: " + ItemName, "Stacking Skill Cooldown Reduction", 2.5f, "How much skill cooldown reduction should be provided by subsequent stacks?").Value;
        }

        private void AddCooldownReduction(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs arguments)
        {
            int count = GetCount(body);
            if (count == 1)
            {
                arguments.cooldownMultAdd -= 1.0f - (1.0f - SkillCooldownReductionBase / 100.0f);
            }
            else if (count > 1)
            {
                arguments.cooldownMultAdd -= 1.0f - (1.0f - SkillCooldownReductionBase / 100.0f) * (float)Math.Pow(1.0f - SkillCooldownReductionPerStack / 100.0f, count - 1);
            }
        }
    }
}