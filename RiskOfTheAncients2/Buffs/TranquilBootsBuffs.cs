using R2API;
using RoR2;
using ROTA2.Items;

namespace ROTA2.Buffs
{
    public class TranqbuilBootsOn : BuffBase<TranqbuilBootsOn>
    {
        public override string BuffName => "Safe";
        public override string BuffTokenName => "TRANQUIL_BOOTS_ON";
        public override string BuffDefGUID => Assets.TranquilBoots.OnBuffDef;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddStats;
        }

        private void AddStats(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (HasThisBuff(body))
            {
                int count = TranquilBoots.GetCount(body);
                if (count > 0)
                {
                    args.baseMoveSpeedAdd += TranquilBoots.Instance.OODMovementSpeedBase.Value + TranquilBoots.Instance.OODMovementSpeedPerStack.Value * (count - 1);
                    args.baseRegenAdd += (TranquilBoots.Instance.OODHealthRegenerationBase.Value + TranquilBoots.Instance.OODHealthRegenerationPerStack.Value * (count - 1)) * (1 + 0.2f * (body.level - 1));
                }
                else
                {
                    body.RemoveBuff(BuffDef);
                }
            }
        }
    }
    public class TranquilBootsOff : BuffBase<TranquilBootsOff>
    {
        public override string BuffName => "Unsafe";
        public override string BuffTokenName => "TRANQUIL_BOOTS_OFF";
        public override string BuffDefGUID => Assets.TranquilBoots.OffBuffDef;
    }
}