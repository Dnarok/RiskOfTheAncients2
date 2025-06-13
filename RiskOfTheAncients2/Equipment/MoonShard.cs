using BepInEx.Configuration;
using R2API;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
using ROTA2.Items;
using UnityEngine.AddressableAssets;

namespace ROTA2.Equipment
{
    public class MoonShard : EquipmentBase<MoonShard>
    {
        public override string EquipmentName => "Moon Shard";
        public override string EquipmentTokenName => "MOON_SHARD";
        public override string EquipmentTokenPickup => "Passively increases attack speed. Consume to gain permanent attack speed.";
        public override string EquipmentTokenDesc => $"Passively increases {Damage("attack speed")} by {Damage($"{AttackSpeed.Value}%")} while held. {Utility("Consume")} on use to {Damage("permanently")} increase {Damage("attack speed")} by {Damage($"{ConsumedMoonShard.Instance.AttackSpeedBase.Value}%")} {Stack($"(+{ConsumedMoonShard.Instance.AttackSpeedPerStack.Value}% per stack)")}.";
        public override string EquipmentTokenLore => "Said to be a tear from the lunar goddess Selemene.";
        public override string EquipmentDefGUID => Assets.MoonShard.EquipmentDef;
        public override float EquipmentCooldown => 0.5f;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddAttackSpeed;
        }
        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateSounds();
            CreateLanguageTokens();
            CreateEquipmentDef();
            Hooks();
        }

        public ConfigEntry<float> AttackSpeed;
        private void CreateConfig(ConfigFile config)
        {
            AttackSpeed = config.Bind("Equipment: " + EquipmentName, "Held Attack Speed", 60.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(AttackSpeed));
        }

        NetworkSoundEventDef sound = null;
        protected void CreateSounds()
        {
            Addressables.LoadAssetAsync<NetworkSoundEventDef>(Assets.MoonShard.NetworkSoundEventDef).Completed += (x) => { ContentAddition.AddNetworkSoundEventDef(x.Result); sound = x.Result; };
        }

        protected override bool ActivateEquipment(EquipmentSlot slot)
        {
            if (slot && HasThisEquipment(slot.characterBody))
            {
                slot.characterBody.inventory.SetEquipmentIndexForSlot(EquipmentIndex.None, slot.activeEquipmentSlot);
                slot.characterBody.inventory.GiveItem(ConsumedMoonShard.GetItemDef());

                EffectManager.SimpleSoundEffect(sound.index, slot.characterBody.corePosition, true);
            }

            return true;
        }

        private void AddAttackSpeed(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (HasThisEquipment(body))
            {
                args.attackSpeedMultAdd += AttackSpeed.Value / 100.0f;
            }
        }
    }

    public class ConsumedMoonShard : ItemBase<ConsumedMoonShard>
    {
        public override string ItemName => "Consumed Moon Shard";
        public override string ItemTokenName => "CONSUMED_MOON_SHARD";
        public override string ItemTokenPickup => "Increase attack speed.";
        public override string ItemTokenDesc => $"Increases {Damage("attack speed")} by {Damage($"{AttackSpeedBase.Value}%")} {Stack($"(+{AttackSpeedPerStack.Value} per stack)")}.";
        public override string ItemTokenLore => "The power has waned.";
        public override string ItemDefGUID => Assets.MoonShard.ItemDef;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddAttackSpeed;
        }
        public override void Init(ConfigFile configuration)
        {
            CreateConfig(configuration);
            CreateLanguageTokens();
            CreateItemDef();
            Hooks();
        }

        public ConfigEntry<float> AttackSpeedBase;
        public ConfigEntry<float> AttackSpeedPerStack;
        public void CreateConfig(ConfigFile configuration)
        {
            AttackSpeedBase = configuration.Bind("Item: " + ItemName, "Attack Speed Base", 20.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(AttackSpeedBase));
            AttackSpeedPerStack = configuration.Bind("Item: " + ItemName, "Attack Speed Per Stack", 20.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(AttackSpeedPerStack));
        }

        private void AddAttackSpeed(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            int count = GetCount(body);
            if (count > 0)
            {
                args.attackSpeedMultAdd += AttackSpeedBase.Value / 100.0f + AttackSpeedPerStack.Value / 100.0f * (count - 1);
            }
        }
    }
}