using BepInEx.Configuration;
using R2API;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
using ROTA2.Buffs;
using UnityEngine.AddressableAssets;

namespace ROTA2.Equipment
{
    public class Mekansm : EquipmentBase<Mekansm>
    {
        public override string EquipmentName => "Mekansm";
        public override string EquipmentTokenName => "MEKANSM";
        public override string EquipmentTokenPickup => "Heal all allies and give them armor for a short time.";
        public override string EquipmentTokenDesc => $"{Healing("Heal")} all allies for {Healing($"{MaximumHealthHeal.Value}% of their maximum health")}, and increase their {Damage("armor")} by {Damage($"{ArmorBonus.Value}")} for {Utility($"{ArmorBonusDuration.Value} seconds")}.";
        public override string EquipmentTokenLore => "A glowing jewel formed out of assorted parts that somehow fit together perfectly.";
        public override float EquipmentCooldown => MekansmCooldown.Value;
        public override string EquipmentDefGUID => Assets.Mekansm.EquipmentDef;
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
        public ConfigEntry<float> MekansmCooldown;
        private void CreateConfig(ConfigFile config)
        {
            MaximumHealthHeal = config.Bind("Equipment: " + EquipmentName, "Maximum Health Heal", 25.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(MaximumHealthHeal));
            ArmorBonus = config.Bind("Equipment: " + EquipmentName, "Armor Bonus", 50.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(ArmorBonus));
            ArmorBonusDuration = config.Bind("Equipment: " + EquipmentName, "Armor Duration", 8.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(ArmorBonusDuration));
            MekansmCooldown = config.Bind("Equipment: " + EquipmentName, "Cooldown", 30.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(MekansmCooldown));
        }

        NetworkSoundEventDef sound = null;
        protected void CreateSounds()
        {
            Addressables.LoadAssetAsync<NetworkSoundEventDef>(Assets.Mekansm.NetworkSoundEventDef).Completed += (x) => { ContentAddition.AddNetworkSoundEventDef(x.Result); sound = x.Result; };
        }

        protected override bool ActivateEquipment(EquipmentSlot slot)
        {
            if (slot && HasThisEquipment(slot.characterBody) && slot.characterBody.teamComponent)
            {
                var allies = TeamComponent.GetTeamMembers(slot.characterBody.teamComponent.teamIndex);
                foreach (var member in allies)
                {
                    HealthComponent ally = member.GetComponent<HealthComponent>();
                    if (ally && ally.isActiveAndEnabled)
                    {
                        ally.HealFraction(MaximumHealthHeal.Value / 100.0f, default);
                        MekansmBuff.ApplyTo(
                            body: ally.body,
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