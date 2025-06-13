namespace ROTA2.Buffs
{
    public class RefresherOrbCooldown : BuffBase<RefresherOrbCooldown>
    {
        public override string BuffName => "Restore Cooldown";
        public override string BuffTokenName => "REFRESHER_ORB_COOLDOWN";
        public override string BuffDefGUID => Assets.RefresherOrb.BuffDef;
    }
}