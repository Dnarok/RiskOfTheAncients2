using BepInEx.Configuration;
using ROTA2.Buffs;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using System;

namespace ROTA2.Items
{
    public class EnchantedMango : ItemBase<EnchantedMango>
    {
        public override string ItemName => "Enchanted Mango";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "ENCHANTED_MANGO";
        public override string ItemTokenPickup => "Receive bonus damage and reset all skill cooldowns at low health. Consumed on use.";
        public override string ItemTokenDesc => $"Taking damage to below {Health($"{HealthThreshold}% health")} {Utility("consumes")} this item, {Utility("resetting all skill cooldowns")} and increasing {Damage("damage")} by {Damage($"{DamageBonus}%")} for {Damage($"{DamageDuration} seconds")}. Regenerates at the start of each stage.";
        public override string ItemTokenLore => "The bittersweet flavors of Jidi Isle are irresistible to amphibians.";
        public override ItemTier Tier => ItemTier.Tier1;
        public override ItemTag[] ItemTags => [ItemTag.Utility, ItemTag.Damage];
        public override string ItemModelPath => "enchanted_mango.prefab";
        public override string ItemIconPath => "ROTA2.Icons.enchanted_mango.png";
        public override void Hooks()
        {
            On.RoR2.HealthComponent.UpdateLastHitTime += OnHit;
        }

        public override void Init(ConfigFile configuration)
        {
            CreateConfig(configuration);
            CreateLanguageTokens();
            CreateItemDef();
            Hooks();
        }

        public float HealthThreshold;
        public float DamageBonus;
        public float DamageDuration;
        public void CreateConfig(ConfigFile configuration)
        {
            HealthThreshold = configuration.Bind("Item: " + ItemName, "Health Threshold", 40.0f, "At what percent of health should this item activate?").Value;
            DamageBonus     = configuration.Bind("Item: " + ItemName, "Damage Bonus",     50.0f, "How much bonus damage should be provided by activation?").Value;
            DamageDuration  = configuration.Bind("Item: " + ItemName, "Damage Duration",  5.0f,  "How long should the bonus damage last?").Value;
        }

        private void OnHit(On.RoR2.HealthComponent.orig_UpdateLastHitTime orig, RoR2.HealthComponent self, float damageValue, Vector3 damagePosition, bool damageIsSilent, GameObject attacker, bool delayedDamage, bool firstHitOfDelayedDamage)
        {
            orig(self, damageValue, damagePosition, damageIsSilent, attacker, delayedDamage, firstHitOfDelayedDamage);
            if (NetworkServer.active && self && GetCount(self.body) > 0 && self.IsHealthBelowThreshold(HealthThreshold / 100.0f) && !EnchantedMangoBuff.Instance.HasThisBuff(self.body))
            {
                EnchantedMangoBuff.Instance.ApplyTo(new BuffBase.ApplyParameters
                {
                    victim = self.body,
                    duration = DamageDuration
                });

                if (self.body.skillLocator)
                {
                    var skills = self.body.skillLocator.allSkills;
                    if (skills != null)
                    {
                        foreach (var skill in skills)
                        {
                            if (skill && skill.CanApplyAmmoPack() && skill.cooldownRemaining > 0.0f)
                            {
                                skill.ApplyAmmoPack();
                            }
                        }
                    }
                }

                self.body.inventory.RemoveItem(ItemDef);
                self.body.inventory.GiveItem(ConsumedMango.Instance.ItemDef);
                CharacterMasterNotificationQueue.PushItemTransformNotification(self.body.master, EnchantedMango.Instance.ItemDef.itemIndex, ConsumedMango.Instance.ItemDef.itemIndex, CharacterMasterNotificationQueue.TransformationType.Default);

                Util.PlaySound("EnchantedMango", self.body.gameObject);
            }
        }
    }

    public class ConsumedMango : ItemBase<ConsumedMango>
    {
        public override string ItemName => "Consumed Mango";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "CONSUMED_MANGO";
        public override string ItemTokenPickup => "Snack time's over.";
        public override string ItemTokenDesc => $"Increases {Damage("damage")} by {Damage($"{DamageBase}%")} {Stack($"(+{DamagePerStack} per stack)")}.";
        public override string ItemTokenLore => "I miss it already...";
        public override ItemTier Tier => ItemTier.NoTier;
        public override ItemTag[] ItemTags => [ItemTag.WorldUnique];
        public override string ItemIconPath => "ROTA2.Icons.consumed_mango.png";
        public override bool Removable => false;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddDamage;
            RoR2.Stage.onStageStartGlobal += OnStageStart;
        }
        public override void Init(ConfigFile configuration)
        {
            CreateConfig(configuration);
            CreateLanguageTokens();
            CreateItemDef();
            Hooks();
        }

        public float DamageBase;
        public float DamagePerStack;
        public void CreateConfig(ConfigFile configuration)
        {
            DamageBase     = configuration.Bind("Item: " + ItemName, "Damage Base",      2.5f, "How much damage should be provided by the first stack?").Value;
            DamagePerStack = configuration.Bind("Item: " + ItemName, "Damage Per Stack", 2.5f, "How much damage should be provided by subsequent stacks?").Value;
        }

        private void AddDamage(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            int count = GetCount(body);
            if (count > 0)
            {
                args.damageMultAdd += DamageBase / 100.0f + DamagePerStack / 100.0f * (count - 1);
            }
        }
        private void OnStageStart(Stage stage)
        {
            if (CharacterMaster.instancesList != null)
            {
                foreach (CharacterMaster master in CharacterMaster.instancesList)
                {
                    int count = GetCount(master);
                    if (count > 0)
                    {
                        master.inventory.RemoveItem(ItemDef, count);
                        master.inventory.GiveItem(EnchantedMango.Instance.ItemDef, count);
                        CharacterMasterNotificationQueue.PushItemTransformNotification(master, ItemDef.itemIndex, EnchantedMango.Instance.ItemDef.itemIndex, default);
                    }
                }
            }
        }
    }
}