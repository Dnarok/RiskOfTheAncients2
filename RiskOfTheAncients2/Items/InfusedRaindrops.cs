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
            IL.RoR2.HealthComponent.TakeDamageProcess += (il) =>
            {
                ILCursor cursor = new(il);
                cursor.GotoNext(
                    x => x.MatchLdloc(0),
                    x => x.MatchLdfld(out _),
                    x => x.MatchLdfld(out _),
                    x => x.MatchBrfalse(out _),
                    x => x.MatchRet()
                );
                cursor.Emit(OpCodes.Ldarg_0);
                cursor.Emit(OpCodes.Ldarg_1);
                cursor.EmitDelegate<Action<HealthComponent, DamageInfo>>((self, info) =>
                {
                    int count = GetCount(self.body);
                    if (info.rejected || info.damageType.damageType.HasFlag(DamageType.BypassBlock) || info.damage <= self.fullCombinedHealth * DamageThreshold / 100.0f)
                    {
                        if (count > 0)
                        {
                            counts[self.body].count = count;
                        }

                        return;
                    }

                    if (count > 0)
                    {
                        CountAndCharges value = counts[self.body];
                        if (value.charges > 0)
                        {
                            info.damage -= DamageBlock;
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
                            if (value.charges % BlockCount == 0)
                            {
                                ignore = true;
                                self.body.inventory.RemoveItem(ItemDef);
                                self.body.inventory.GiveItem(DehydratedRaindrops.Instance.ItemDef);
                                ignore = false;
                                --value.count;

                                CharacterMasterNotificationQueue.PushItemTransformNotification(self.body.master, ItemDef.itemIndex, DehydratedRaindrops.Instance.ItemDef.itemIndex, CharacterMasterNotificationQueue.TransformationType.Default);
                                Util.PlaySound("InfusedRaindrops", self.body.gameObject);
                            }
                        }
                    }
                });
            };
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
        private void OnTakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo info)
        {
            int count = GetCount(self.body);
            if (!info.rejected && info.damage > self.fullCombinedHealth * DamageThreshold / 100.0f && count > 0)
            {
                if (counts[self.body].charges > 0)
                {
                    info.damage -= DamageBlock;
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

                    --counts[self.body].charges;
                    if (counts[self.body].charges % 10 == 0)
                    {
                        ignore = true;
                        self.body.inventory.RemoveItem(ItemDef);
                        self.body.inventory.GiveItem(DehydratedRaindrops.Instance.ItemDef);
                        ignore = false;
                        --counts[self.body].count;

                        CharacterMasterNotificationQueue.PushItemTransformNotification(self.body.master, ItemDef.itemIndex, DehydratedRaindrops.Instance.ItemDef.itemIndex, CharacterMasterNotificationQueue.TransformationType.Default);
                        Util.PlaySound("InfusedRaindrops", self.body.gameObject);
                    }
                }
            }
            else if (count > 0)
            {
                counts[self.body].count = count;
            }

            orig(self, info);
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