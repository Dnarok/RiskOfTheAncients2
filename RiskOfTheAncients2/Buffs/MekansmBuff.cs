using R2API;
using RoR2;
using ROTA2.Equipment;

namespace ROTA2.Buffs
{
    public class MekansmBuff : BuffBase<MekansmBuff>
    {
        public override string BuffName => "Mekansm Armor";
        public override string BuffTokenName => "MEKANSM_BUFF";
        public override string BuffDefGUID => Assets.Mekansm.BuffDef;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddArmor;
        }

        private void AddArmor(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (HasThisBuff(body))
            {
                args.armorAdd += Mekansm.Instance.ArmorBonus.Value;
            }
        }
    }
}