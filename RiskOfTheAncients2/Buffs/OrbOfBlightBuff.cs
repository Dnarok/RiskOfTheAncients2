using ROTA2.Items;
using R2API;
using RoR2;
using UnityEngine;

namespace ROTA2.Buffs
{
    public class OrbOfBlightBuff : BuffBase<OrbOfBlightBuff>
    {
        public override string BuffName => "Lesser Corruption";
        public override string BuffTokenName => "ORB_OF_BLIGHT_BUFF";
        public override bool BuffStacks => true;
        public override bool IsDebuff => true;
        public override Color BuffColor => Color.white;
        public override string BuffIconPath => "RiskOfTheAncients2.Icons.orb_of_blight.png";
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
                arguments.armorAdd -= OrbOfBlight.Instance.ArmorReduction * count;
            }
        }
    }
}