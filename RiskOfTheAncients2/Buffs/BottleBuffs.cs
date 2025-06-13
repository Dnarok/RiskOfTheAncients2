using R2API;
using RoR2;
using ROTA2.Equipment;

namespace ROTA2.Buffs
{
    public class AmplifyDamageBuff : BuffBase<AmplifyDamageBuff>
    {
        public override string BuffName => "Amplify Damage";
        public override string BuffTokenName => "AMPLIFY_DAMAGE_RUNE";
        public override string BuffDefGUID => Assets.Bottle.AmplifyDamage.BuffDef;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddDamage;
        }

        private void AddDamage(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs arguments)
        {
            if (HasThisBuff(body))
            {
                arguments.damageMultAdd += Bottle.Instance.AmplifyDamageBonus.Value / 100.0f;
            }
        }
    }

    public class ArcaneBuff : BuffBase<ArcaneBuff>
    {
        public override string BuffName => "Arcane";
        public override string BuffTokenName => "ARCANE_RUNE";
        public override string BuffDefGUID => Assets.Bottle.Arcane.BuffDef;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddSkillCooldownReduction;
        }

        private void AddSkillCooldownReduction(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs arguments)
        {
            if (HasThisBuff(body))
            {
                arguments.cooldownMultAdd -= 1.0f - (1.0f - Bottle.Instance.ArcaneReduction.Value / 100.0f);
            }
        }
    }

    public class HasteBuff : BuffBase<HasteBuff>
    {
        public override string BuffName => "Haste";
        public override string BuffTokenName => "HASTE_RUNE";
        public override string BuffDefGUID => Assets.Bottle.Haste.BuffDef;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddMovementSpeed;
        }

        private void AddMovementSpeed(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs arguments)
        {
            if (HasThisBuff(body))
            {
                arguments.moveSpeedMultAdd += Bottle.Instance.HasteBonus.Value / 100.0f;
            }
        }
    }

    public class RegenerationBuff : BuffBase<RegenerationBuff>
    {
        public override string BuffName => "Regeneration";
        public override string BuffTokenName => "REGENERATION_RUNE";
        public override string BuffDefGUID => Assets.Bottle.Regeneration.BuffDef;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddHealthRegeneration;
        }

        private void AddHealthRegeneration(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs arguments)
        {
            if (HasThisBuff(body))
            {
                HealthComponent health = body.GetComponent<HealthComponent>();
                if (health)
                {
                    arguments.baseRegenAdd += health.fullHealth * Bottle.Instance.RegenerationMaximumHealthPercentage.Value / 100.0f;
                }
            }
        }
    }
}