using BepInEx.Configuration;
using R2API;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
using UnityEngine.AddressableAssets;

namespace ROTA2.Items
{
    public class ExMachina : ItemBase<ExMachina>
    {
        public override string ItemName => "Ex Machina";
        public override string ItemTokenName => "EX_MACHINA";
        public override string ItemTokenPickup => "Chance to restore equipment on equipment use.";
        public override string ItemTokenDesc => $"{Utility($"{RestoreChanceBase.Value}%")} {Stack($"(+{RestoreChancePerStack.Value}% per stack, hyperbolic)")} chance on equipment use to {Utility("instantly restore the cooldown")}.";
        public override string ItemTokenLore => "The remains of an ancient universe, preserved within a single sphere.";
        public override string ItemDefGUID => Assets.ExMachina.ItemDef;
        public override void Hooks()
        {
            On.RoR2.EquipmentSlot.OnEquipmentExecuted += EquipmentExecuted;
        }
        public override void Init(ConfigFile configuration)
        {
            CreateConfig(configuration);
            CreateSounds();
            CreateLanguageTokens();
            CreateItemDef();
            Hooks();
        }

        public ConfigEntry<float> RestoreChanceBase;
        public ConfigEntry<float> RestoreChancePerStack;
        public ConfigEntry<bool> PlaySound;
        private void CreateConfig(ConfigFile configuration)
        {
            RestoreChanceBase = configuration.Bind("Item: " + ItemName, "Equipment Restore Chance Base", 30.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(RestoreChanceBase));
            RestoreChancePerStack = configuration.Bind("Item: " + ItemName, "Equipment Restore Chance Per Stack", 15.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(RestoreChancePerStack));
            PlaySound = configuration.Bind("Item: " + ItemName, "Play Sound", true, "");
            ModSettingsManager.AddOption(new CheckBoxOption(PlaySound));
        }

        NetworkSoundEventDef sound = null;
        protected void CreateSounds()
        {
            Addressables.LoadAssetAsync<NetworkSoundEventDef>(Assets.ExMachina.NetworkSoundEventDef).Completed += (x) => { ContentAddition.AddNetworkSoundEventDef(x.Result); sound = x.Result; };
        }

        private void EquipmentExecuted(On.RoR2.EquipmentSlot.orig_OnEquipmentExecuted orig, EquipmentSlot self)
        {
            orig(self);

            int count = GetCount(self.characterBody);
            if (count > 0)
            {
                float chance = 1f - (1f / (1f + 1.5f * (RestoreChanceBase.Value / 100f + RestoreChancePerStack.Value / 100f * (count - 1))));
                if (Util.CheckRoll0To1(chance, self.characterBody.master))
                {
                    self.inventory.RestockEquipmentCharges(self.activeEquipmentSlot, 1);

                    if (PlaySound.Value)
                    {
                        EffectManager.SimpleSoundEffect(sound.index, self.characterBody.corePosition, true);
                    }
                }
            }
        }
    }
}