using ROTA2.Equipment;
using R2API;
using RoR2;
using UnityEngine;

namespace ROTA2.Buffs
{
    public class AmplifyDamageBuff : BuffBase<AmplifyDamageBuff>
    {
        public override string BuffName => "Amplify Damage";
        public override string BuffTokenName => "AMPLIFY_DAMAGE_RUNE";
        public override bool BuffStacks => false;
        public override bool IsDebuff => false;
        public override Color BuffColor => Color.white;
        public override string BuffIconPath => "ROTA2.Icons.bottle_amplify_damage.png";
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
                arguments.damageMultAdd += Bottle.Instance.AmplifyDamageBonus / 100.0f;
            }
        }
    }

    public class ArcaneBuff : BuffBase<ArcaneBuff>
    {
        public override string BuffName => "Arcane";
        public override string BuffTokenName => "ARCANE_RUNE";
        public override bool BuffStacks => false;
        public override bool IsDebuff => false;
        public override Color BuffColor => Color.white;
        public override string BuffIconPath => "ROTA2.Icons.bottle_arcane.png";
        public override EliteDef BuffEliteDef => null;
        public override bool IsCooldown => false;
        public override bool IsHidden => false;
        public override NetworkSoundEventDef BuffStartSfx => null;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddSkillCooldownReduction;
        }

        private void AddSkillCooldownReduction(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs arguments)
        {
            if (HasThisBuff(body))
            {
                arguments.cooldownMultAdd -= 1.0f - (1.0f - Bottle.Instance.ArcaneReduction / 100.0f);
            }
        }
    }

    public class HasteBuff : BuffBase<HasteBuff>
    {
        public override string BuffName => "Haste";
        public override string BuffTokenName => "HASTE_RUNE";
        public override bool BuffStacks => false;
        public override bool IsDebuff => false;
        public override Color BuffColor => Color.white;
        public override string BuffIconPath => "ROTA2.Icons.bottle_haste.png";
        public override EliteDef BuffEliteDef => null;
        public override bool IsCooldown => false;
        public override bool IsHidden => false;
        public override NetworkSoundEventDef BuffStartSfx => null;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddMovementSpeed;
        }

        private void AddMovementSpeed(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs arguments)
        {
            if (HasThisBuff(body))
            {
                arguments.moveSpeedMultAdd += Bottle.Instance.HasteBonus / 100.0f;
            }
        }
    }

    public class RegenerationBuff : BuffBase<RegenerationBuff>
    {
        public override string BuffName => "Regeneration";
        public override string BuffTokenName => "REGENERATION_RUNE";
        public override bool BuffStacks => false;
        public override bool IsDebuff => false;
        public override Color BuffColor => Color.white;
        public override string BuffIconPath => "ROTA2.Icons.bottle_regeneration.png";
        public override EliteDef BuffEliteDef => null;
        public override bool IsCooldown => false;
        public override bool IsHidden => false;
        public override NetworkSoundEventDef BuffStartSfx => null;
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
                    arguments.baseRegenAdd += health.fullHealth * Bottle.Instance.RegenerationMaximumHealthPercentage / 100.0f;
                }
            }
        }
    }
}