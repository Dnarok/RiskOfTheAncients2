using R2API;
using RoR2;
using ROTA2.Items;

namespace ROTA2.Buffs
{
    public class PowerTreadsStrength : BuffBase<PowerTreadsStrength>
    {
        public override string BuffName => "Strength";
        public override string BuffTokenName => "POWER_TREADS_STRENGTH";
        public override string BuffDefGUID => Assets.PowerTreads.StrengthBuffDef;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddRegen;
        }

        private void AddRegen(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (HasThisBuff(body))
            {
                args.baseRegenAdd += PowerTreads.Instance.StrengthRegen.Value * (1.0f + 0.2f * (body.level - 1));
            }
        }
    }
    public class PowerTreadsAgility : BuffBase<PowerTreadsAgility>
    {
        public override string BuffName => "Agility";
        public override string BuffTokenName => "POWER_TREADS_AGILITY";
        public override string BuffDefGUID => Assets.PowerTreads.AgilityBuffDef;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddArmor;
        }

        private void AddArmor(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (HasThisBuff(body))
            {
                args.armorAdd += PowerTreads.Instance.AgilityArmor.Value;
            }
        }
    }
    public class PowerTreadsIntelligence : BuffBase<PowerTreadsIntelligence>
    {
        public override string BuffName => "Intelligence";
        public override string BuffTokenName => "POWER_TREADS_INTELLIGENCE";
        public override string BuffDefGUID => Assets.PowerTreads.IntelligenceBuffDef;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddDamage;
        }

        private void AddDamage(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (HasThisBuff(body))
            {
                args.damageMultAdd += PowerTreads.Instance.IntelligenceDamage.Value / 100.0f;
            }
        }
    }
}