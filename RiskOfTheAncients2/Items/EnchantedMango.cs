using BepInEx.Configuration;
using R2API;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
using ROTA2.Buffs;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace ROTA2.Items
{
    public class EnchantedMango : ItemBase<EnchantedMango>
    {
        public override string ItemName => "Enchanted Mango";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "ENCHANTED_MANGO";
        public override string ItemTokenPickup => "Receive bonus damage and reset all skill cooldowns at low health. Consumed on use.";
        public override string ItemTokenDesc => $"Taking damage to below {Health($"{HealthThreshold.Value}% health")} {Utility("consumes")} this item, {Utility("resetting all skill cooldowns")} and increasing {Damage("damage")} by {Damage($"{DamageBonus.Value}%")} for {Damage($"{DamageDuration.Value} seconds")}. Regenerates at the start of each stage.";
        public override string ItemTokenLore => "The bittersweet flavors of Jidi Isle are irresistible to amphibians.";
        public override string ItemDefGUID => Assets.EnchantedMango.ItemDef;
        public override void Hooks()
        {
            On.RoR2.HealthComponent.UpdateLastHitTime += OnHit;
        }

        public override void Init(ConfigFile configuration)
        {
            CreateConfig(configuration);
            CreateSounds();
            CreateLanguageTokens();
            CreateItemDef();
            Hooks();
        }

        public ConfigEntry<float> HealthThreshold;
        public ConfigEntry<float> DamageBonus;
        public ConfigEntry<float> DamageDuration;
        public ConfigEntry<bool> PlaySound;
        public void CreateConfig(ConfigFile configuration)
        {
            HealthThreshold = configuration.Bind("Item: " + ItemName, "Health Threshold", 40f, "At what percent of health should this item activate?");
            ModSettingsManager.AddOption(new FloatFieldOption(HealthThreshold));
            DamageBonus = configuration.Bind("Item: " + ItemName, "Damage Bonus", 50f, "How much bonus damage should be provided by activation?");
            ModSettingsManager.AddOption(new FloatFieldOption(DamageBonus));
            DamageDuration = configuration.Bind("Item: " + ItemName, "Damage Duration", 5f, "How long should the bonus damage last?");
            ModSettingsManager.AddOption(new FloatFieldOption(DamageDuration));
            PlaySound = configuration.Bind("Item: " + ItemName, "Play Sound", true, "");
            ModSettingsManager.AddOption(new CheckBoxOption(PlaySound));
        }

        NetworkSoundEventDef sound = null;
        protected void CreateSounds()
        {
            Addressables.LoadAssetAsync<NetworkSoundEventDef>(Assets.EnchantedMango.NetworkSoundEventDef).Completed += (x) => { ContentAddition.AddNetworkSoundEventDef(x.Result); sound = x.Result; };
        }

        private void OnHit(On.RoR2.HealthComponent.orig_UpdateLastHitTime orig, RoR2.HealthComponent self, float damageValue, Vector3 damagePosition, bool damageIsSilent, GameObject attacker, bool delayedDamage, bool firstHitOfDelayedDamage)
        {
            orig(self, damageValue, damagePosition, damageIsSilent, attacker, delayedDamage, firstHitOfDelayedDamage);
            if (NetworkServer.active && self)
            {
                int count = GetCount(self.body);
                bool damaged = self.IsHealthBelowThreshold(HealthThreshold.Value / 100f);
                bool not_buffed = !EnchantedMangoBuff.HasThisBuff(self.body);
                if (count > 0 && damaged && not_buffed)
                {
                    Inventory.ItemTransformation.TryTransformResult result;
                    Inventory.ItemTransformation trans = default;
                    trans.originalItemIndex = ItemDef.itemIndex;
                    trans.newItemIndex = ConsumedMango.GetItemDef().itemIndex;
                    trans.maxToTransform = 1;
                    trans.transformationType = ItemTransformationTypeIndex.None;
                    if (trans.TryTransform(self.body.inventory, out result))
                    {
                        EnchantedMangoBuff.ApplyTo(body: self.body, duration: DamageDuration.Value);

                        if (self.body.skillLocator)
                        {
                            var skills = self.body.skillLocator.allSkills;
                            if (skills != null)
                            {
                                foreach (var skill in skills)
                                {
                                    if (skill && skill.CanApplyAmmoPack() && skill.cooldownRemaining > 0f)
                                    {
                                        skill.ApplyAmmoPack();
                                    }
                                }
                            }
                        }

                        if (PlaySound.Value)
                        {
                            EffectManager.SimpleSoundEffect(sound.index, self.body.corePosition, true);
                        }
                    }
                }

                // self.body.inventory.RemoveItemPermanent(ItemDef);
                // self.body.inventory.GiveItemPermanent(ConsumedMango.GetItemDef());
                // CharacterMasterNotificationQueue.PushItemTransformNotification(self.body.master, GetItemDef().itemIndex, ConsumedMango.GetItemDef().itemIndex, CharacterMasterNotificationQueue.TransformationType.Default);
            }
        }
    }

    public class ConsumedMango : ItemBase<ConsumedMango>
    {
        public override string ItemName => "Consumed Mango";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "CONSUMED_MANGO";
        public override string ItemTokenPickup => "Snack time's over.";
        public override string ItemTokenDesc => $"Increases {Damage("damage")} by {Damage($"{DamageBase.Value}%")} {Stack($"(+{DamagePerStack.Value} per stack)")}.";
        public override string ItemTokenLore => "I miss it already...";
        public override string ItemDefGUID => Assets.EnchantedMango.ConsumedItemDef;
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

        public ConfigEntry<float> DamageBase;
        public ConfigEntry<float> DamagePerStack;
        public void CreateConfig(ConfigFile configuration)
        {
            DamageBase = configuration.Bind("Item: " + ItemName, "Damage Base", 2.5f, "How much damage should be provided by the first stack?");
            ModSettingsManager.AddOption(new FloatFieldOption(DamageBase));
            DamagePerStack = configuration.Bind("Item: " + ItemName, "Damage Per Stack", 2.5f, "How much damage should be provided by subsequent stacks?");
            ModSettingsManager.AddOption(new FloatFieldOption(DamagePerStack));
        }

        private void AddDamage(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            int count = GetCount(body);
            if (count > 0)
            {
                args.damageMultAdd += DamageBase.Value / 100f + DamagePerStack.Value / 100f * (count - 1);
            }
        }
        private void OnStageStart(Stage stage)
        {
            if (CharacterMaster.instancesList != null && NetworkServer.active)
            {
                foreach (CharacterMaster master in CharacterMaster.instancesList)
                {
                    Inventory.ItemTransformation.TryTransformResult result;
                    int count = GetCount(master);
                    if (count > 0)
                    {
                        Inventory.ItemTransformation trans = default;
                        trans.originalItemIndex = ItemDef.itemIndex;
                        trans.newItemIndex = EnchantedMango.GetItemDef().itemIndex;
                        trans.maxToTransform = count;
                        trans.transformationType = ItemTransformationTypeIndex.None;
                        trans.TryTransform(master.inventory, out result);

                        // master.inventory.RemoveItemPermanent(ItemDef, count);
                        // master.inventory.GiveItemPermanent(EnchantedMango.GetItemDef(), count);
                        // CharacterMasterNotificationQueue.PushItemTransformNotification(master, ItemDef.itemIndex, EnchantedMango.GetItemDef().itemIndex, default);
                    }
                }
            }
        }
    }
}