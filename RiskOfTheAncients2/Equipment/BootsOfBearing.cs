using BepInEx.Configuration;
using R2API;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
using ROTA2.Buffs;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ROTA2.Equipment
{
    public class BootsOfBearing : EquipmentBase<BootsOfBearing>
    {
        public override string EquipmentName => "Boots of Bearing";
        public override string EquipmentTokenName => "BOOTS_OF_BEARING";
        public override string EquipmentTokenPickup => "Passively increase movement speed and health regneration. Activate to send you and all your allies into a frenzy.";
        public override string EquipmentTokenDesc => $"Passively increases {Utility("base movement speed")} by {Utility($"{PassiveMovementSpeed.Value}")} and {Healing("base health regeneration")} by {Healing($"+{PassiveHealthRegeneration.Value} hp/s")}. Activate to send all allies into a {Damage("frenzy")} for {Utility($"{FrenzyDuration.Value} seconds")}, increasing {Utility("movement speed")} by {Utility($"{ActiveMovementSpeed.Value}%")} and {Damage("attack speed")} by {Damage($"{ActiveAttackSpeed.Value}%")}.";
        public override string EquipmentTokenLore => "Resplendent footwear fashioned for the ancient herald that first dared spread the glory of Stonehall beyond the original borders of its nascent claim.";
        public override float EquipmentCooldown => BootsOfBearingCooldown.Value;
        public override string EquipmentDefGUID => Assets.BootsOfBearing.EquipmentDef;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddMovementSpeed;
            RecalculateStatsAPI.GetStatCoefficients += AddHealthRegeneration;
        }
        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateSounds();
            CreateLanguageTokens();
            CreateEquipmentDef();
            Hooks();
        }

        public ConfigEntry<float> PassiveMovementSpeed;
        public ConfigEntry<float> PassiveHealthRegeneration;
        public ConfigEntry<float> ActiveMovementSpeed;
        public ConfigEntry<float> ActiveAttackSpeed;
        public ConfigEntry<float> FrenzyDuration;
        public ConfigEntry<float> BootsOfBearingCooldown;
        private void CreateConfig(ConfigFile config)
        {
            PassiveMovementSpeed = config.Bind("Equipment: " + EquipmentName, "Passive Base Movement Speed", 1.2f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(PassiveMovementSpeed));
            PassiveHealthRegeneration = config.Bind("Equipment: " + EquipmentName, "Passive Base Health Regeneration", 5.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(PassiveHealthRegeneration));
            ActiveMovementSpeed = config.Bind("Equipment: " + EquipmentName, "Active Movement Speed", 75.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(ActiveMovementSpeed));
            ActiveAttackSpeed = config.Bind("Equipment: " + EquipmentName, "Active Attack Speed", 150.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(ActiveAttackSpeed));
            FrenzyDuration = config.Bind("Equipment: " + EquipmentName, "Frenzy Duration", 7.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(FrenzyDuration));
            BootsOfBearingCooldown = config.Bind("Equipment: " + EquipmentName, "Cooldown", 45.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(BootsOfBearingCooldown));
        }

        NetworkSoundEventDef sound = null;
        protected void CreateSounds()
        {
            Addressables.LoadAssetAsync<NetworkSoundEventDef>(Assets.BootsOfBearing.NetworkSoundEventDef).Completed += (x) => { ContentAddition.AddNetworkSoundEventDef(x.Result); sound = x.Result; };
        }

        protected override bool ActivateEquipment(EquipmentSlot slot)
        {
            if (slot && HasThisEquipment(slot.characterBody))
            {
                var allies = TeamComponent.GetTeamMembers(slot.characterBody.teamComponent.teamIndex);
                foreach (var member in allies)
                {
                    CharacterBody ally = member.GetComponent<CharacterBody>();
                    if (ally && ally.isActiveAndEnabled)
                    {
                        BootsOfBearingBuff.ApplyTo(body: ally, duration: FrenzyDuration.Value);
                    }
                }

                Vector3 position = slot.characterBody.corePosition;
                EffectData effect = new()
                {
                    origin = position
                };
                effect.SetNetworkedObjectReference(slot.characterBody.gameObject);
                EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/TeamWarCryActivation"), effect, true);

                EffectManager.SimpleSoundEffect(sound.index, slot.characterBody.corePosition, true);
            }

            return true;
        }

        private void AddMovementSpeed(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (HasThisEquipment(body))
            {
                args.baseMoveSpeedAdd += PassiveMovementSpeed.Value;
            }
        }
        private void AddHealthRegeneration(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (HasThisEquipment(body))
            {
                args.baseRegenAdd += PassiveHealthRegeneration.Value * (1.0f + 0.2f * (body.level - 1));
            }
        }
    }
}