using R2API;
using RoR2;
using ROTA2.Equipment;

namespace ROTA2.Buffs
{
    public class BootsOfBearingBuff : BuffBase<BootsOfBearingBuff>
    {
        public override string BuffName => "Endurance";
        public override string BuffTokenName => "BOOTS_OF_BEARING_BUFF";
        public override string BuffDefGUID => Assets.BootsOfBearing.BuffDef;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddAttackSpeed;
            RecalculateStatsAPI.GetStatCoefficients += AddMovementSpeed;
        }

        private void AddAttackSpeed(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (HasThisBuff(body))
            {
                args.attackSpeedMultAdd += BootsOfBearing.Instance.ActiveAttackSpeed.Value / 100.0f;
            }
        }
        private void AddMovementSpeed(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (HasThisBuff(body))
            {
                args.moveSpeedMultAdd += BootsOfBearing.Instance.ActiveMovementSpeed.Value / 100.0f;
            }
        }
    }
}