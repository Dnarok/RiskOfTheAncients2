using R2API;
using RoR2;
using ROTA2.Items;

namespace ROTA2.Buffs
{
    public class OrbOfBlightBuff : BuffBase<OrbOfBlightBuff>
    {
        public override string BuffName => "Lesser Corruption";
        public override string BuffTokenName => "ORB_OF_BLIGHT_BUFF";
        public override string BuffDefGUID => Assets.OrbOfBlight.BuffDef;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += RemoveArmor;
        }

        private void RemoveArmor(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs arguments)
        {
            int count = GetBuffCount(body);
            if (count > 0)
            {
                arguments.armorAdd -= OrbOfBlight.Instance.ArmorReduction.Value * count;
            }
        }
    }
}