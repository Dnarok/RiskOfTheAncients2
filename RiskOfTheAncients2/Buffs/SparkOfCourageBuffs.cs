using R2API;
using RoR2;
using ROTA2.Items;

namespace ROTA2.Buffs
{
    public class SparkOfCourageDamage : BuffBase<SparkOfCourageDamage>
    {
        public override string BuffName => "Courage";
        public override string BuffTokenName => "SPARK_OF_COURAGE_DAMAGE";
        public override string BuffDefGUID => Assets.SparkOfCourage.DamageBuffDef;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddDamage;
        }

        private void AddDamage(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            int count = SparkOfCourage.GetCount(body);
            if (HasThisBuff(body))
            {
                if (count > 0)
                {
                    args.damageMultAdd += SparkOfCourage.Instance.DamageBase.Value / 100.0f + SparkOfCourage.Instance.DamagePerStack.Value / 100.0f * (count - 1);
                }
                else
                {
                    body.RemoveBuff(BuffDef);
                }
            }
        }
    }
    public class SparkOfCourageArmor : BuffBase<SparkOfCourageArmor>
    {
        public override string BuffName => "Cowardice";
        public override string BuffTokenName => "SPARK_OF_COURAGE_ARMOR";
        public override string BuffDefGUID => Assets.SparkOfCourage.ArmorBuffDef;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddArmor;
        }

        private void AddArmor(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            int count = SparkOfCourage.GetCount(body);
            if (HasThisBuff(body))
            {
                if (count > 0)
                {
                    args.armorAdd += SparkOfCourage.Instance.ArmorBase.Value + SparkOfCourage.Instance.ArmorPerStack.Value * (count - 1);
                }
                else
                {
                    body.RemoveBuff(BuffDef);
                }
            }
        }
    }
}