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
            int buff_count = GetBuffCount(body);
            int item_count = EnchantedMango.GetCount(body);
            if (buff_count > 0 && item_count > 0)
            {
                arguments.damageMultAdd += (EnchantedMango.Instance.DamageBonusBase.Value / 100f + EnchantedMango.Instance.DamageBonusPerStack.Value / 100f * (item_count - 1)) * buff_count;
            }
        }
    }
}