using R2API;
using RoR2;
using static RoR2.DotController;

namespace ROTA2.Buffs
{
    public class OrbOfVenomBuff : BuffBase<OrbOfVenomBuff>
    {
        public override string BuffName => "Lesser Venom";
        public override string BuffTokenName => "ORB_OF_VENOM_BUFF";
        public override string BuffDefGUID => Assets.OrbOfVenom.BuffDef;

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