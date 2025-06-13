using R2API;
using RoR2;
using ROTA2.Items;

namespace ROTA2.Buffs
{
    public class OrbOfFrostBuff : BuffBase<OrbOfFrostBuff>
    {
        public override string BuffName => "Lesser Frost";
        public override string BuffTokenName => "ORB_OF_FROST_BUFF";
        public override string BuffDefGUID => Assets.OrbOfFrost.BuffDef;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += ReduceMovementSpeed;
            RecalculateStatsAPI.GetStatCoefficients += ReduceAttackSpeed;
        }

        private void ReduceMovementSpeed(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs arguments)
        {
            int count = GetBuffCount(body);
            if (count > 0)
            {
                arguments.moveSpeedReductionMultAdd += OrbOfFrost.Instance.MovementSpeedSlowBase.Value / 100.0f + OrbOfFrost.Instance.MovementSpeedSlowPerStack.Value / 100.0f * (count - 1);
            }
        }
        private void ReduceAttackSpeed(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs arguments)
        {
            int count = GetBuffCount(body);
            if (count > 0)
            {
                arguments.attackSpeedReductionMultAdd += OrbOfFrost.Instance.AttackSpeedSlowBase.Value / 100.0f + OrbOfFrost.Instance.AttackSpeedSlowPerStack.Value / 100.0f * (count - 1);
            }
        }
    }
}