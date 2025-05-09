using RoR2;
using UnityEngine;

namespace ROTA2.Buffs
{
    public class SkullBasherCooldown : BuffBase<SkullBasherCooldown>
    {
        public override string BuffName => "Skull Basher Cooldown";
        public override string BuffTokenName => "SKULL_BASHER_COOLDOWN";
        public override bool BuffStacks => false;
        public override bool IsDebuff => false;
        public override Color BuffColor => Color.white;
        public override string BuffIconPath => "RiskOfTheAncients2.Icons.skull_basher_cooldown.png";
        public override EliteDef BuffEliteDef => null;
        public override bool IsCooldown => true;
        public override bool IsHidden => false;
        public override NetworkSoundEventDef BuffStartSfx => null;
    }
}