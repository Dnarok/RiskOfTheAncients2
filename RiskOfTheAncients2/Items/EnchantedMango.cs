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
        public override string ItemTokenPickup => "Activating your Secondary skill reduces your Special skill's cooldown and increases damage.";
        public override string ItemTokenDesc => $"Activating {Utility("Secondary skill")} reduces {Utility("Special skill remaining cooldown")} by {Utility($"{SpecialCooldownReduction.Value}%")}, and increases {Damage("damage")} by {Damage($"{DamageBonusBase.Value}%")} {Stack($"(+{DamageBonusPerStack.Value}% per stack)")} for {Damage($"{DamageDuration.Value} seconds")}.";
        public override string ItemTokenLore => "The bittersweet flavors of Jidi Isle are irresistible to amphibians.";
        public override string ItemDefGUID => Assets.EnchantedMango.ItemDef;
        public override void Hooks()
        {
            On.RoR2.CharacterBody.OnSkillActivated += OnSkill;
        }

        public override void Init(ConfigFile configuration)
        {
            CreateConfig(configuration);
            CreateSounds();
            CreateLanguageTokens();
            CreateItemDef();
            Hooks();
        }

        public ConfigEntry<float> SpecialCooldownReduction;
        public ConfigEntry<float> DamageBonusBase;
        public ConfigEntry<float> DamageBonusPerStack;
        public ConfigEntry<float> DamageDuration;
        public ConfigEntry<bool> PlaySound;
        public void CreateConfig(ConfigFile configuration)
        {
            SpecialCooldownReduction = configuration.Bind("Item: " + ItemName, "Special Cooldown Reduction", 15f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(SpecialCooldownReduction));
            DamageBonusBase = configuration.Bind("Item: " + ItemName, "Damage Bonus Base", 5f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(DamageBonusBase));
            DamageBonusPerStack = configuration.Bind("Item: " + ItemName, "Damage Bonus Per Stack", 5f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(DamageBonusPerStack));
            DamageDuration = configuration.Bind("Item: " + ItemName, "Damage Duration", 5f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(DamageDuration));
            PlaySound = configuration.Bind("Item: " + ItemName, "Play Sound", true, "");
            ModSettingsManager.AddOption(new CheckBoxOption(PlaySound));
        }

        NetworkSoundEventDef sound = null;
        protected void CreateSounds()
        {
            Addressables.LoadAssetAsync<NetworkSoundEventDef>(Assets.EnchantedMango.NetworkSoundEventDef).Completed += (x) => { ContentAddition.AddNetworkSoundEventDef(x.Result); sound = x.Result; };
        }

        private void OnSkill(On.RoR2.CharacterBody.orig_OnSkillActivated orig, CharacterBody body, GenericSkill skill)
        {
            int count = GetCount(body);
            if (count > 0 && skill == body.skillLocator.secondary && skill.cooldownRemaining > 0f)
            {
                EnchantedMangoBuff.ApplyTo(
                    body: body,
                    duration: DamageDuration.Value
                );

                var special = body.skillLocator.special;
                if (special.stock < special.maxStock)
                {
                    special.rechargeStopwatch += special.baseRechargeInterval * (SpecialCooldownReduction.Value / 100f);
                }

                if (PlaySound.Value)
                {
                    EffectManager.SimpleSoundEffect(sound.index, body.corePosition, true);
                }
            }

            orig(body, skill);
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