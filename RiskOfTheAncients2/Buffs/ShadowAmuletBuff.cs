using R2API;
using RoR2;
using ROTA2.Items;

namespace ROTA2.Buffs
{
    public class ShadowAmuletBuff : BuffBase<ShadowAmuletBuff>
    {
        public override string BuffName => "Fade";
        public override string BuffTokenName => "SHADOW_AMULET_BUFF";
        public override string BuffDefGUID => Assets.ShadowAmulet.BuffDef;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddAttackSpeed;
        }

        private void AddAttackSpeed(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (HasThisBuff(body))
            {
                int count = ShadowAmulet.GetCount(body);
                if (count > 0)
                {
                    args.attackSpeedMultAdd += ShadowAmulet.Instance.AttackSpeedBase.Value / 100.0f + ShadowAmulet.Instance.AttackSpeedPerStack.Value / 100.0f * (count - 1);
                }
                else
                {
                    body.RemoveBuff(BuffDef);
                }
            }
        }
    }
}