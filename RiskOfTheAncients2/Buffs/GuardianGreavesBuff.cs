using R2API;
using RoR2;
using ROTA2.Equipment;
using UnityEngine;

namespace ROTA2.Buffs
{
    public class GuardianGreavesBuff : BuffBase<GuardianGreavesBuff>
    {
        public override string BuffName => "Guardian Greaves Armor";
        public override string BuffTokenName => "GUARDIAN_GREAVES_BUFF";
        public override bool BuffStacks => false;
        public override bool IsDebuff => false;
        public override Color BuffColor => Color.white;
        public override string BuffIconPath => "ROTA2.Icons.guardian_greaves.png";
        public override EliteDef BuffEliteDef => null;
        public override bool IsCooldown => false;
        public override bool IsHidden => false;
        public override NetworkSoundEventDef BuffStartSfx => null;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddArmor;
        }

        private void AddArmor(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (HasThisBuff(body))
            {
                args.armorAdd += GuardianGreaves.Instance.ArmorBonus;
            }
        }
    }
}