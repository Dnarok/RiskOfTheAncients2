using R2API;
using RoR2;
using ROTA2.Items;
using UnityEngine;

namespace ROTA2.Buffs
{
    public class ShadowAmuletBuff : BuffBase<ShadowAmuletBuff>
    {
        public override string BuffName => "Fade";
        public override string BuffTokenName => "SHADOW_AMULET_BUFF";
        public override bool BuffStacks => true;
        public override bool IsDebuff => false;
        public override Color BuffColor => Color.white;
        public override string BuffIconPath => "ROTA2.Icons.shadow_amulet.png";
        public override EliteDef BuffEliteDef => null;
        public override bool IsCooldown => false;
        public override bool IsHidden => false;
        public override NetworkSoundEventDef BuffStartSfx => null;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddAttackSpeed;
        }

        private void AddAttackSpeed(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            int count = GetBuffCount(body);
            if (count > 0)
            {
                args.attackSpeedMultAdd += ShadowAmulet.Instance.AttackSpeedBase / 100.0f + ShadowAmulet.Instance.AttackSpeedPerStack / 100.0f * (count - 1);
            }
        }
    }
}