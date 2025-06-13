using BepInEx.Configuration;
using R2API;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
using ROTA2.Buffs;
using UnityEngine.AddressableAssets;

namespace ROTA2.Equipment
{
    public class GuardianGreaves : EquipmentBase<GuardianGreaves>
    {
        public override string EquipmentName => "Guardian Greaves";
        public override string EquipmentTokenName => "GUARDIAN_GREAVES";
        public override string EquipmentTokenPickup => "Heal all allies, reset all of their skill cooldowns, and give them armor for a short time.";
        public override string EquipmentTokenDesc => $"{Healing("Heal")} all allies for {Healing($"{MaximumHealthHeal.Value}% of their maximum health")}, {Utility("reset all of their skill cooldowns")}, and increase their {Damage("armor")} by {Damage($"{ArmorBonus.Value}")} for {Utility($"{ArmorBonusDuration.Value} seconds")}.";
        public override string EquipmentTokenLore => "One of many holy instruments constructed to honor the Omniscience.";
        public override float EquipmentCooldown => GuardianGreavesCooldown.Value;
        public override string EquipmentDefGUID => Assets.GuardianGreaves.EquipmentDef;
        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateSounds();
            CreateLanguageTokens();
            CreateEquipmentDef();
        }

        public ConfigEntry<float> MaximumHealthHeal;
        public ConfigEntry<float> ArmorBonus;
        public ConfigEntry<float> ArmorBonusDuration;
        public ConfigEntry<float> GuardianGreavesCooldown;
        private void CreateConfig(ConfigFile config)
        {
            MaximumHealthHeal = config.Bind("Equipment: " + EquipmentName, "Maximum Health Heal", 50.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(MaximumHealthHeal));
            ArmorBonus = config.Bind("Equipment: " + EquipmentName, "Armor Bonus", 100.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(ArmorBonus));
            ArmorBonusDuration = config.Bind("Equipment: " + EquipmentName, "Armor Duration", 8.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(ArmorBonusDuration));
            GuardianGreavesCooldown = config.Bind("Equipment: " + EquipmentName, "Cooldown", 30.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(GuardianGreavesCooldown));
        }

        NetworkSoundEventDef sound = null;
        protected void CreateSounds()
        {
            Addressables.LoadAssetAsync<NetworkSoundEventDef>(Assets.GuardianGreaves.NetworkSoundEventDef).Completed += (x) => { ContentAddition.AddNetworkSoundEventDef(x.Result); sound = x.Result; };
        }

        protected override bool ActivateEquipment(EquipmentSlot slot)
        {
            if (slot && HasThisEquipment(slot.characterBody))
            {
                var allies = TeamComponent.GetTeamMembers(slot.characterBody.teamComponent.teamIndex);
                foreach (var member in allies)
                {
                    CharacterBody ally = member.GetComponent<CharacterBody>();
                    if (ally && ally.isActiveAndEnabled && ally.healthComponent && ally.skillLocator)
                    {
                        var skills = ally.skillLocator.allSkills;
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

                        ally.healthComponent.HealFraction(MaximumHealthHeal.Value / 100.0f, default);
                        GuardianGreavesBuff.ApplyTo(
                            body: ally,
                            duration: ArmorBonusDuration.Value
                        );
                    }
                }

                EffectManager.SimpleSoundEffect(sound.index, slot.characterBody.corePosition, true);
            }

            return true;
        }
    }
}