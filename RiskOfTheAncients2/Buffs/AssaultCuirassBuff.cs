using R2API;
using RoR2;
using ROTA2.Items;
using System;
using UnityEngine;

namespace ROTA2.Buffs
{
    public class AssaultCuirassBuff : BuffBase<AssaultCuirassBuff>
    {
        public override string BuffName => "Assault Aura";
        public override string BuffTokenName => "ASSAULT_CUIRASS_BUFF";
        public override bool BuffStacks => true;
        public override bool IsDebuff => false;
        public override Color BuffColor => Color.white;
        public override string BuffIconPath => "ROTA2.Icons.assault_cuirass.png";
        public override EliteDef BuffEliteDef => null;
        public override bool IsCooldown => false;
        public override bool IsHidden => false;
        public override NetworkSoundEventDef BuffStartSfx => null;
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
                args.attackSpeedMultAdd += AssaultCuirass.Instance.AttackSpeedBase / 100.0f + AssaultCuirass.Instance.AttackSpeedPerStack / 100.0f * (count - 1);
            }
        }
        private void AddArmor(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            int count = GetBuffCount(body);
            if (count > 0)
            {
                args.armorAdd += AssaultCuirass.Instance.ArmorBase + AssaultCuirass.Instance.ArmorPerStack * (count - 1);
            }
        }
    }
}