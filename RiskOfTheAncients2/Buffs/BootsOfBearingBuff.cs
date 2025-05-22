using R2API;
using RoR2;
using ROTA2.Equipment;
using ROTA2.Items;
using UnityEngine;

namespace ROTA2.Buffs
{
    public class BootsOfBearingBuff : BuffBase<BootsOfBearingBuff>
    {
        public override string BuffName => "Endurance";
        public override string BuffTokenName => "BOOTS_OF_BEARING_BUFF";
        public override bool BuffStacks => false;
        public override bool IsDebuff => false;
        public override Color BuffColor => Color.white;
        public override string BuffIconPath => "ROTA2.Icons.boots_of_bearing.png";
        public override EliteDef BuffEliteDef => null;
        public override bool IsCooldown => false;
        public override bool IsHidden => false;
        public override NetworkSoundEventDef BuffStartSfx => null;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddAttackSpeed;
            RecalculateStatsAPI.GetStatCoefficients += AddMovementSpeed;
        }

        private void AddAttackSpeed(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (HasThisBuff(body))
            {
                args.attackSpeedMultAdd += BootsOfBearing.Instance.ActiveAttackSpeed / 100.0f;
            }
        }
        private void AddMovementSpeed(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (HasThisBuff(body))
            {
                args.moveSpeedMultAdd += BootsOfBearing.Instance.ActiveMovementSpeed / 100.0f;
            }
        }
    }
}