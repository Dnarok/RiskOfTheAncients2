using R2API;
using RoR2;
using ROTA2.Items;

namespace ROTA2.Buffs
{
    public class AssaultCuirassBuff : BuffBase<AssaultCuirassBuff>
    {
        public override string BuffName => "Assault Aura";
        public override string BuffTokenName => "ASSAULT_CUIRASS_BUFF";
        public override string BuffDefGUID => Assets.AssaultCuirass.BuffDef;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddAttackSpeed;
            RecalculateStatsAPI.GetStatCoefficients += AddArmor;
        }

        private void AddAttackSpeed(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            int count = GetBuffCount(body);
            if (count > 0)
            {
                args.attackSpeedMultAdd += AssaultCuirass.Instance.AttackSpeedBase.Value / 100.0f + AssaultCuirass.Instance.AttackSpeedPerStack.Value / 100.0f * (count - 1);
            }
        }
        private void AddArmor(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            int count = GetBuffCount(body);
            if (count > 0)
            {
                args.armorAdd += AssaultCuirass.Instance.ArmorBase.Value + AssaultCuirass.Instance.ArmorPerStack.Value * (count - 1);
            }
        }
    }
}