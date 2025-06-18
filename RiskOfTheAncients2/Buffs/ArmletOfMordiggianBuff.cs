using R2API;
using ROTA2.Equipment;

namespace ROTA2.Buffs
{
    public class ArmletOfMordiggianBuff : BuffBase<ArmletOfMordiggianBuff>
    {
        public override string BuffName => "Unholy Strength";
        public override string BuffTokenName => "ARMLET_OF_MORDIGGIAN_BUFF";
        public override string BuffDefGUID => Assets.ArmletOfMordiggian.BuffDef;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += (body, args) =>
            {
                if (HasThisBuff(body))
                {
                    args.damageTotalMult *= 1f + ArmletOfMordiggian.Instance.DamageBonus.Value / 100f;
                    args.attackSpeedTotalMult *= 1f + ArmletOfMordiggian.Instance.AttackSpeedBonus.Value / 100f;
                    args.armorAdd += ArmletOfMordiggian.Instance.ArmorBonus.Value;
                }
            };
        }
    }
}