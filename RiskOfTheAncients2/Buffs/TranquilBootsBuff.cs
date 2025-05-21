using R2API;
using RoR2;
using ROTA2.Items;
using UnityEngine;

namespace ROTA2.Buffs
{
    public class TranquilBootsBuff : BuffBase<TranquilBootsBuff>
    {
        public override string BuffName => "Safe";
        public override string BuffTokenName => "TRANQUIL_BOOTS_BUFF";
        public override bool BuffStacks => true;
        public override bool IsDebuff => false;
        public override Color BuffColor => Color.white;
        public override string BuffIconPath => "ROTA2.Icons.tranquil_boots.png";
        public override EliteDef BuffEliteDef => null;
        public override bool IsCooldown => false;
        public override bool IsHidden => false;
        public override NetworkSoundEventDef BuffStartSfx => null;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddMovementSpeed;
            RecalculateStatsAPI.GetStatCoefficients += AddHealthRegeneration;
        }

        private void AddMovementSpeed(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            int count = GetBuffCount(body);
            if (count > 0)
            {
                args.attackSpeedMultAdd += TranquilBoots.Instance.OODMovementSpeedBase / 100.0f + TranquilBoots.Instance.OODMovementSpeedPerStack / 100.0f * (count - 1);
            }
        }
        private void AddHealthRegeneration(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            int count = GetBuffCount(body);
            if (count > 0)
            {
                args.baseRegenAdd += TranquilBoots.Instance.OODHealthRegenerationBase + TranquilBoots.Instance.OODHealthRegenerationPerStack * (count - 1) * (1.0f + 0.2f * (body.level - 1));
            }
        }
    }
}