using BepInEx.Configuration;
using ROTA2.Buffs;
using RoR2;
using UnityEngine;

namespace ROTA2.Equipment
{
    public class BlackKingBar : EquipmentBase<BlackKingBar>
    {
        public override string EquipmentName => "Black King Bar";
        public override string EquipmentTokenName => "BLACK_KING_BAR";
        public override string EquipmentTokenPickup => "Become immune to negative effects and reduce incoming damage for a short time.";
        public override string EquipmentTokenDesc => $"Become {Utility("immune")} to all negative effects and reduce all {Damage("incoming damage")} by {Damage($"{DamageReduction}%")} for {Utility($"{AvatarDuration} seconds")}.";
        public override string EquipmentTokenLore => "A powerful staff imbued with the strength of giants.";
        public override string EquipmentIconPath => "RiskOfTheAncients2.Icons.black_king_bar.png";
        public override float EquipmentCooldown => BlackKingBarCooldown;
        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLanguageTokens();
            CreateEquipmentDef();
        }

        public float AvatarDuration;
        public float DamageReduction;
        public float BlackKingBarCooldown;
        private void CreateConfig(ConfigFile config)
        {
            AvatarDuration       = config.Bind("Equipment: " + EquipmentName, "Avatar Duration",           6.0f,  "How long should the immunity and damage reduction last?").Value;
            DamageReduction      = config.Bind("Equipment: " + EquipmentName, "Incoming Damage Reduction", 25.0f, "What percentage of incoming damage should be reduced while active?").Value;
            BlackKingBarCooldown = config.Bind("Equipment: " + EquipmentName, "Cooldown",                  75.0f, "").Value;
        }

        protected override bool ActivateEquipment(EquipmentSlot slot)
        {
            var body = slot.characterBody;
            if (body)
            {
                BlackKingBarBuff.Instance.ApplyTo(new BuffBase.ApplyParameters
                {
                    victim = body,
                    duration = AvatarDuration
                });

                Vector3 corePosition = body.corePosition;
                EffectData effectData = new EffectData
                {
                    origin = corePosition
                };
                effectData.SetHurtBoxReference(body.mainHurtBox);
                EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/CleanseEffect"), effectData, transmit: true);
                Util.CleanseBody(body, removeDebuffs: true, removeBuffs: false, removeCooldownBuffs: true, removeDots: true, removeStun: true, removeNearbyProjectiles: false);
            }

            return true;
        }
    }
}