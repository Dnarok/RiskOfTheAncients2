using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public class CountAndCharges
        {
            public int count;
            public int charges;
        }
        public Dictionary<CharacterBody, CountAndCharges> counts = [];
        private bool ignore = false;
        private void OnInventoryChanged(CharacterBody body)
        {
            if (ignore)
            {
                return;
            }

            int count = GetCount(body);
            if (GetCount(body) > 0)
            {
                if (!body.GetComponent<RaindropsBehavior>())
                {
                    body.gameObject.AddComponent<RaindropsBehavior>();
                }

                if (!counts.ContainsKey(body))
                {
                    counts.Add(body, new ()
                    {
                        charges = BlockCount * count,
                        count = count
                    });

                    return;
                }
                if (counts[body].count < count)
                {
                    counts[body].charges += BlockCount * (count - counts[body].count);
                    counts[body].count = count;
                }
                else if (counts[body].count > count)
                {
                    counts[body].charges = BlockCount * count;
                    counts[body].count = count;
                }
            }
        }

        public class RaindropsBehavior : MonoBehaviour, IOnIncomingDamageServerReceiver
        {
            CharacterBody body;

            void Awake()
            {
                body = GetComponent<CharacterBody>();
                if (body && body.healthComponent)
                {
                    HG.ArrayUtils.ArrayAppend(ref body.healthComponent.onIncomingDamageReceivers, this);
                }
            }
            void OnDestroy()
            {
                if (body && body.healthComponent)
                {
                    int i = Array.IndexOf(body.healthComponent.onIncomingDamageReceivers, this);
                    if (i != -1)
                    {
                        HG.ArrayUtils.ArrayRemoveAtAndResize(ref body.healthComponent.onIncomingDamageReceivers, body.healthComponent.onIncomingDamageReceivers.Length, i);
                    }
                }
            }
            public void OnIncomingDamageServer(DamageInfo info)
            {
                int count = Instance.GetCount(body);
                if (info.rejected || info.damageType.damageType.HasFlag(DamageType.BypassBlock) || info.damage <= body.healthComponent.fullCombinedHealth * Instance.DamageThreshold / 100.0f)
                {
                    if (count > 0)
                    {
                        Instance.counts[body].count = count;
                    }

                    return;
                }

                if (count > 0)
                {
                    CountAndCharges value = Instance.counts[body];
                    if (value.charges > 0)
                    {
                        info.damage -= Instance.DamageBlock;
                        info.damage = Mathf.Max(0.0f, info.damage);
                        if (info.damage == 0)
                        {
                            EffectData effectData = new EffectData
                            {
                                origin = info.position,
                                rotation = Util.QuaternionSafeLookRotation((info.force != Vector3.zero) ? info.force : UnityEngine.Random.onUnitSphere)
                            };
                            EffectManager.SpawnEffect(HealthComponent.AssetReferences.bearEffectPrefab, effectData, transmit: true);
                            info.rejected = true;
                        }

                        --value.charges;
                        if (value.charges % Instance.BlockCount == 0)
                        {
                            Instance.ignore = true;
                            body.inventory.RemoveItem(Instance.ItemDef);
                            body.inventory.GiveItem(DehydratedRaindrops.Instance.ItemDef);
                            Instance.ignore = false;
                            --value.count;

                            CharacterMasterNotificationQueue.PushItemTransformNotification(body.master, Instance.ItemDef.itemIndex, DehydratedRaindrops.Instance.ItemDef.itemIndex, CharacterMasterNotificationQueue.TransformationType.Default);
                            Util.PlaySound("InfusedRaindrops", body.gameObject);
                        }
                    }
                }
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
        public override bool Removable => false;
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