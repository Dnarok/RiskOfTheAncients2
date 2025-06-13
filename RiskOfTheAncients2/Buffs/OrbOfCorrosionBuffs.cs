using R2API;
using RoR2;
using ROTA2.Items;
using static RoR2.DotController;

namespace ROTA2.Buffs
{
    public class OrbOfCorrosionArmor : BuffBase<OrbOfCorrosionArmor>
    {
        public override string BuffName => "Greater Corruption";
        public override string BuffTokenName => "ORB_OF_CORROSION_ARMOR";
        public override string BuffDefGUID => Assets.OrbOfCorrosion.ArmorBuffDef;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += RemoveArmor;
        }

        private void RemoveArmor(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs arguments)
        {
            int count = GetBuffCount(body);
            if (count > 0)
            {
                arguments.armorAdd -= OrbOfCorrosion.Instance.ArmorReduction.Value * count;
            }
        }
    }

    public class OrbOfCorrosionSlow : BuffBase<OrbOfCorrosionSlow>
    {
        public override string BuffName => "Greater Frost";
        public override string BuffTokenName => "ORB_OF_CORROSION_SLOW";
        public override string BuffDefGUID => Assets.OrbOfCorrosion.SlowBuffDef;
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
                arguments.moveSpeedReductionMultAdd += OrbOfCorrosion.Instance.MovementSpeedSlowBase.Value / 100.0f + OrbOfCorrosion.Instance.MovementSpeedSlowPerStack.Value / 100.0f * (count - 1);
            }
        }
        private void ReduceAttackSpeed(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs arguments)
        {
            int count = GetBuffCount(body);
            if (count > 0)
            {
                arguments.attackSpeedReductionMultAdd += OrbOfCorrosion.Instance.AttackSpeedSlowBase.Value / 100.0f + OrbOfCorrosion.Instance.AttackSpeedSlowPerStack.Value / 100.0f * (count - 1);
            }
        }
    }

    public class OrbOfCorrosionPoison : BuffBase<OrbOfCorrosionPoison>
    {
        public override string BuffName => "Greater Venom";
        public override string BuffTokenName => "ORB_OF_CORROSION_POISON";
        public override string BuffDefGUID => Assets.OrbOfCorrosion.PoisonBuffDef;

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