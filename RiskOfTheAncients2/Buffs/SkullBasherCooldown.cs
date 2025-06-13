namespace ROTA2.Buffs
{
    public class SkullBasherCooldown : BuffBase<SkullBasherCooldown>
    {
        public override string BuffName => "Skull Basher Cooldown";
        public override string BuffTokenName => "SKULL_BASHER_COOLDOWN";
        public override string BuffDefGUID => Assets.SkullBasher.BuffDef;
    }
}