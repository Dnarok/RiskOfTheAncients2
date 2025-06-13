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
    public class BlackKingBar : EquipmentBase<BlackKingBar>
    {
        public override string EquipmentName => "Black King Bar";
        public override string EquipmentTokenName => "BLACK_KING_BAR";
        public override string EquipmentTokenPickup => "Become immune to negative effects and reduce incoming damage for a short time.";
        public override string EquipmentTokenDesc => $"Become {Utility("immune")} to all negative effects and reduce all {Damage("incoming damage")} by {Damage($"{DamageReduction.Value}%")} for {Utility($"{AvatarDuration.Value} seconds")}.";
        public override string EquipmentTokenLore => "A powerful staff imbued with the strength of giants.";
        public override float EquipmentCooldown => BlackKingBarCooldown.Value;
        public override string EquipmentDefGUID => Assets.BlackKingBar.EquipmentDef;
        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateSounds();
            CreateLanguageTokens();
            CreateEquipmentDef();
        }

        public ConfigEntry<float> AvatarDuration;
        public ConfigEntry<float> DamageReduction;
        public ConfigEntry<float> BlackKingBarCooldown;
        private void CreateConfig(ConfigFile config)
        {
            AvatarDuration = config.Bind("Equipment: " + EquipmentName, "Avatar Duration", 6.0f, "How long should the immunity and damage reduction last?");
            ModSettingsManager.AddOption(new FloatFieldOption(AvatarDuration));
            DamageReduction = config.Bind("Equipment: " + EquipmentName, "Incoming Damage Reduction", 35.0f, "What percentage of incoming damage should be reduced while active?");
            ModSettingsManager.AddOption(new FloatFieldOption(DamageReduction));
            BlackKingBarCooldown = config.Bind("Equipment: " + EquipmentName, "Cooldown", 65.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(BlackKingBarCooldown));
        }

        NetworkSoundEventDef sound = null;
        protected void CreateSounds()
        {
            Addressables.LoadAssetAsync<NetworkSoundEventDef>(Assets.BlackKingBar.NetworkSoundEventDef).Completed += (x) => { ContentAddition.AddNetworkSoundEventDef(x.Result); sound = x.Result; };
        }

        protected override bool ActivateEquipment(EquipmentSlot slot)
        {
            var body = slot.characterBody;
            if (HasThisEquipment(body))
            {
                BlackKingBarBuff.ApplyTo(
                    body: body,
                    duration: AvatarDuration.Value
                );

                Vector3 corePosition = body.corePosition;
                EffectData effectData = new EffectData
                {
                    origin = corePosition
                };
                effectData.SetHurtBoxReference(body.mainHurtBox);
                EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/CleanseEffect"), effectData, transmit: true);
                Util.CleanseBody(body, removeDebuffs: true, removeBuffs: false, removeCooldownBuffs: true, removeDots: true, removeStun: true, removeNearbyProjectiles: false);

                EffectManager.SimpleSoundEffect(sound.index, body.corePosition, true);
            }

            return true;
        }
    }
}