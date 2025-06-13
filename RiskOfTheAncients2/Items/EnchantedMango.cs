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
            HealthThreshold = configuration.Bind("Item: " + ItemName, "Health Threshold", 40.0f, "At what percent of health should this item activate?");
            ModSettingsManager.AddOption(new FloatFieldOption(HealthThreshold));
            DamageBonus = configuration.Bind("Item: " + ItemName, "Damage Bonus", 50.0f, "How much bonus damage should be provided by activation?");
            ModSettingsManager.AddOption(new FloatFieldOption(DamageBonus));
            DamageDuration = configuration.Bind("Item: " + ItemName, "Damage Duration", 5.0f, "How long should the bonus damage last?");
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
            if (NetworkServer.active && self && GetCount(self.body) > 0 && self.IsHealthBelowThreshold(HealthThreshold.Value / 100.0f) && !EnchantedMangoBuff.HasThisBuff(self.body))
            {
                EnchantedMangoBuff.ApplyTo(body: self.body, duration: DamageDuration.Value);

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
                self.body.inventory.GiveItem(ConsumedMango.GetItemDef());
                CharacterMasterNotificationQueue.PushItemTransformNotification(self.body.master, GetItemDef().itemIndex, ConsumedMango.GetItemDef().itemIndex, CharacterMasterNotificationQueue.TransformationType.Default);

                if (PlaySound.Value)
                {
                    EffectManager.SimpleSoundEffect(sound.index, self.body.corePosition, true);
                }
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
                args.damageMultAdd += DamageBase.Value / 100.0f + DamagePerStack.Value / 100.0f * (count - 1);
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
                        master.inventory.GiveItem(EnchantedMango.GetItemDef(), count);
                        CharacterMasterNotificationQueue.PushItemTransformNotification(master, ItemDef.itemIndex, EnchantedMango.GetItemDef().itemIndex, default);
                    }
                }
            }
        }
    }
}