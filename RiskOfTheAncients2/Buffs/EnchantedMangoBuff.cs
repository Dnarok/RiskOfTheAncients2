using R2API;
using RoR2;
using ROTA2.Items;

namespace ROTA2.Buffs
{
    public class EnchantedMangoBuff : BuffBase<EnchantedMangoBuff>
    {
        public override string BuffName => "Mango Strength";
        public override string BuffTokenName => "ENCHANTED_MANGO_BUFF";
        public override string BuffDefGUID => Assets.EnchantedMango.BuffDef;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddDamage;
        }

        private void AddDamage(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs arguments)
        {
            if (HasThisBuff(body))
            {
                arguments.damageMultAdd += EnchantedMango.Instance.DamageBonus.Value / 100.0f;
            }
        }
    }
}