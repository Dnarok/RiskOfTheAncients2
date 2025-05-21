using R2API;
using RoR2;
using ROTA2.Equipment;
using UnityEngine;

namespace ROTA2.Buffs
{
    public class MekansmBuff : BuffBase<MekansmBuff>
    {
        public override string BuffName => "Mekansm Armor";
        public override string BuffTokenName => "MEKANSM_BUFF";
        public override bool BuffStacks => false;
        public override bool IsDebuff => false;
        public override Color BuffColor => Color.white;
        public override string BuffIconPath => "ROTA2.Icons.mekansm.png";
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
                args.armorAdd += Mekansm.Instance.ArmorBonus;
            }
        }
    }
}