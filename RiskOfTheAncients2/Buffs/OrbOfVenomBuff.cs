using R2API;
using RoR2;
using UnityEngine;
using static RoR2.DotController;

namespace ROTA2.Buffs
{
    public class OrbOfVenomBuff : BuffBase<OrbOfVenomBuff>
    {
        public override string BuffName => "Lesser Venom";
        public override string BuffTokenName => "ORB_OF_VENOM_BUFF";
        public override bool BuffStacks => false;
        public override bool IsDebuff => true;
        public override Color BuffColor => Color.white;
        public override string BuffIconPath => "RiskOfTheAncients2.Icons.orb_of_venom.png";
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