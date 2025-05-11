using ROTA2.Items;
using R2API;
using RoR2;
using UnityEngine;
using static RoR2.DotController;

namespace ROTA2.Buffs
{
    public class OrbOfCorrosionArmor : BuffBase<OrbOfCorrosionArmor>
    {
        public override string BuffName => "Greater Corruption";
        public override string BuffTokenName => "ORB_OF_CORROSION_ARMOR";
        public override bool BuffStacks => true;
        public override bool IsDebuff => true;
        public override Color BuffColor => Color.red;
        public override string BuffIconPath => "ROTA2.Icons.orb_of_corrosion.png";
        public override EliteDef BuffEliteDef => null;
        public override bool IsCooldown => false;
        public override bool IsHidden => false;
        public override NetworkSoundEventDef BuffStartSfx => null;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += RemoveArmor;
        }

        private void RemoveArmor(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs arguments)
        {
            int count = GetBuffCount(body);
            if (count > 0)
            {
                arguments.armorAdd -= OrbOfCorrosion.Instance.ArmorReduction * count;
            }
        }
    }

    public class OrbOfCorrosionSlow : BuffBase<OrbOfCorrosionSlow>
    {
        public override string BuffName => "Greater Frost";
        public override string BuffTokenName => "ORB_OF_CORROSION_SLOW";
        public override bool BuffStacks => true;
        public override bool IsDebuff => true;
        public override Color BuffColor => Color.blue;
        public override string BuffIconPath => "ROTA2.Icons.orb_of_corrosion.png";
        public override EliteDef BuffEliteDef => null;
        public override bool IsCooldown => false;
        public override bool IsHidden => false;
        public override NetworkSoundEventDef BuffStartSfx => null;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += ReduceMovementSpeed;
            RecalculateStatsAPI.GetStatCoefficients += ReduceAttackSpeed;
        }

        private void ReduceMovementSpeed(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs arguments)
        {
            int count = GetBuffCount(body);
            if (count > 0)
            {
                arguments.moveSpeedReductionMultAdd += OrbOfCorrosion.Instance.MovementSpeedSlowBase / 100.0f + OrbOfCorrosion.Instance.MovementSpeedSlowPerStack / 100.0f * (count - 1);
            }
        }
        private void ReduceAttackSpeed(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs arguments)
        {
            int count = GetBuffCount(body);
            if (count > 0)
            {
                arguments.attackSpeedReductionMultAdd += OrbOfCorrosion.Instance.AttackSpeedSlowBase / 100.0f + OrbOfCorrosion.Instance.AttackSpeedSlowPerStack / 100.0f * (count - 1);
            }
        }
    }

    public class OrbOfCorrosionPoison : BuffBase<OrbOfCorrosionPoison>
    {
        public override string BuffName => "Greater Venom";
        public override string BuffTokenName => "ORB_OF_CORROSION_POISON";
        public override bool BuffStacks => false;
        public override bool IsDebuff => true;
        public override Color BuffColor => Color.green;
        public override string BuffIconPath => "ROTA2.Icons.orb_of_corrosion.png";
        public override EliteDef BuffEliteDef => null;
        public override bool IsCooldown => false;
        public override bool IsHidden => false;
        public override NetworkSoundEventDef BuffStartSfx => null;

        public override void Init()
        {
            base.Init();
            CreateDot();
        }
        public void CreateDot()
        {
            DotDef dot = new()
            {
                associatedBuff = BuffDef,
                damageCoefficient = 1.0f,
                damageColorIndex = DamageColorIndex.Poison,
                interval = 1.0f,
                resetTimerOnAdd = false
            };

            Index = DotAPI.RegisterDotDef(dot);
        }
    }
}