using BepInEx.Configuration;
using R2API;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
using ROTA2.Buffs;
using System;
using UnityEngine.AddressableAssets;

namespace ROTA2.Items
{
    public class RefresherOrb : ItemBase<RefresherOrb>
    {
        public override string ItemName => "Refresher Orb";
        public override string ItemTokenName => "REFRESHER_ORB";
        public override string ItemTokenPickup => "Chance on equipment use to restore skill cooldowns.";
        public override string ItemTokenDesc => $"{Utility($"{RestoreChance.Value}%")} chance on equipment use to {Utility("instantly restore all skill cooldowns")}. Recharges every {Utility($"{RestoreCooldown.Value} seconds")} {Stack($"(-{RestoreCooldownReductionPerStack.Value}% per stack)")}.";
        public override string ItemTokenLore => "A powerful artifact created for wizards.";
        public override string ItemDefGUID => Assets.RefresherOrb.ItemDef;
        public override ItemDef VoidFor => ExMachina.GetItemDef();
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

        public ConfigEntry<float> RestoreChance;
        public ConfigEntry<float> RestoreCooldown;
        public ConfigEntry<float> RestoreCooldownReductionPerStack;
        public ConfigEntry<bool> PlaySound;
        private void CreateConfig(ConfigFile configuration)
        {
            RestoreChance = configuration.Bind("Item: " + ItemName, "Skill Restore Chance", 100.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(RestoreChance));
            RestoreCooldown = configuration.Bind("Item: " + ItemName, "Restore Cooldown", 10.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(RestoreCooldown));
            RestoreCooldownReductionPerStack = configuration.Bind("Item: " + ItemName, "Restore Cooldown Reduction Per Stack", 50.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(RestoreCooldownReductionPerStack));
            PlaySound = configuration.Bind("Item: " + ItemName, "Play Sound", true, "");
            ModSettingsManager.AddOption(new CheckBoxOption(PlaySound));
        }

        NetworkSoundEventDef sound = null;
        protected void CreateSounds()
        {
            Addressables.LoadAssetAsync<NetworkSoundEventDef>(Assets.RefresherOrb.NetworkSoundEventDef).Completed += (x) => { ContentAddition.AddNetworkSoundEventDef(x.Result); sound = x.Result; };
        }

        private void EquipmentExecuted(On.RoR2.EquipmentSlot.orig_OnEquipmentExecuted orig, EquipmentSlot self)
        {
            orig(self);

            int count = GetCount(self.characterBody);
            if (count > 0 && !RefresherOrbCooldown.HasThisBuff(self.characterBody) && self.characterBody.skillLocator)
            {
                var skills = self.characterBody.skillLocator.allSkills;
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

                RefresherOrbCooldown.ApplyTo(
                    body: self.characterBody,
                    duration: RestoreCooldown.Value * MathF.Pow(RestoreCooldownReductionPerStack.Value / 100.0f, count - 1)
                );

                if (PlaySound.Value)
                {
                    EffectManager.SimpleSoundEffect(sound.index, self.characterBody.corePosition, true);
                }
            }
        }
    }
}