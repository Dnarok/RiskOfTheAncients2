using ROTA2.Items;
using R2API;
using RoR2;
using UnityEngine;

namespace ROTA2.Buffs
{
    public class EnchantedMangoBuff : BuffBase<EnchantedMangoBuff>
    {
        public override string BuffName => "Mango Strength";
        public override string BuffTokenName => "ENCHANTED_MANGO_BUFF";
        public override bool BuffStacks => false;
        public override bool IsDebuff => false;
        public override Color BuffColor => Color.white;
        public override string BuffIconPath => "RiskOfTheAncients2.Icons.enchanted_mango.png";
        public override EliteDef BuffEliteDef => null;
        public override bool IsCooldown => false;
        public override bool IsHidden => false;
        public override NetworkSoundEventDef BuffStartSfx => null;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddDamage;
        }

        private void AddDamage(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs arguments)
        {
            if (HasThisBuff(body))
            {
                arguments.damageMultAdd += EnchantedMango.Instance.DamageBonus / 100.0f;
            }
        }
    }
}