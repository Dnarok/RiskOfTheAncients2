using R2API;
using RoR2;
using ROTA2.Equipment;

namespace ROTA2.Buffs
{
    public class GuardianGreavesBuff : BuffBase<GuardianGreavesBuff>
    {
        public override string BuffName => "Guardian Greaves Armor";
        public override string BuffTokenName => "GUARDIAN_GREAVES_BUFF";
        public override string BuffDefGUID => Assets.GuardianGreaves.BuffDef;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddArmor;
        }

        private void AddArmor(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (HasThisBuff(body))
            {
                args.armorAdd += GuardianGreaves.Instance.ArmorBonus.Value;
            }
        }
    }
}