using ROTA2.Items;
using R2API;
using RoR2;
using UnityEngine;

namespace ROTA2.Buffs
{
    public class OrbOfFrostBuff : BuffBase<OrbOfFrostBuff>
    {
        public override string BuffName => "Lesser Frost";
        public override string BuffTokenName => "ORB_OF_FROST_BUFF";
        public override bool BuffStacks => true;
        public override bool IsDebuff => true;
        public override Color BuffColor => Color.white;
        public override string BuffIconPath => "ROTA2.Icons.orb_of_frost.png";
        public override EliteDef BuffEliteDef => null;
        public override bool IsCooldown => false;
        public override bool IsHidden => false;
        public override NetworkSoundEventDef BuffStartSfx => null;
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
                arguments.moveSpeedReductionMultAdd += OrbOfFrost.Instance.MovementSpeedSlowBase / 100.0f + OrbOfFrost.Instance.MovementSpeedSlowPerStack / 100.0f * (count - 1);
            }
        }
        private void ReduceAttackSpeed(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs arguments)
        {
            int count = GetBuffCount(body);
            if (count > 0)
            {
                arguments.attackSpeedReductionMultAdd += OrbOfFrost.Instance.AttackSpeedSlowBase / 100.0f + OrbOfFrost.Instance.AttackSpeedSlowPerStack / 100.0f * (count - 1);
            }
        }
    }
}