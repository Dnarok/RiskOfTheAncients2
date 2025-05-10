using R2API;
using RoR2;
using ROTA2.Items;
using UnityEngine;

namespace ROTA2.Buffs
{
    public class AeonDiskInvulnerability : BuffBase<AeonDiskInvulnerability>
    {
        public override string BuffName => "Combo Breaker";
        public override string BuffTokenName => "AEON_DISK_INVULNERABILITY";
        public override bool BuffStacks => false;
        public override bool IsDebuff => false;
        public override Color BuffColor => Color.white;
        public override string BuffIconPath => "RiskOfTheAncients2.Icons.aeon_disk.png";
        public override EliteDef BuffEliteDef => null;
        public override bool IsCooldown => false;
        public override bool IsHidden => false;
        public override NetworkSoundEventDef BuffStartSfx => null;
        public override void Hooks()
        {
            On.RoR2.HealthComponent.TakeDamage += OnTakeDamage;
        }

        private void OnTakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo info)
        {
            if (self && HasThisBuff(self.body))
            {
                info.rejected = true;
            }

            orig(self, info);
        }
    }
    public class AeonDiskMovementSpeed : BuffBase<AeonDiskMovementSpeed>
    {
        public override string BuffName => "Movement Speed";
        public override string BuffTokenName => "AEON_DISK_MOVEMENT_SPEED";
        public override bool BuffStacks => false;
        public override bool IsDebuff => false;
        public override Color BuffColor => Color.white;
        public override string BuffIconPath => "RiskOfTheAncients2.Icons.aeon_disk.png";
        public override EliteDef BuffEliteDef => null;
        public override bool IsCooldown => false;
        public override bool IsHidden => false;
        public override NetworkSoundEventDef BuffStartSfx => null;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddMovementSpeed;
        }

        private void AddMovementSpeed(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (HasThisBuff(body))
            {
                args.moveSpeedMultAdd += AeonDisk.Instance.MovementSpeed / 100.0f;
            }
        }
    }
    public class AeonDiskCooldown : BuffBase<AeonDiskCooldown>
    {
        public override string BuffName => "Aeon Disk Cooldown";
        public override string BuffTokenName => "AEON_DISK_COOLDOWN";
        public override bool BuffStacks => false;
        public override bool IsDebuff => false;
        public override Color BuffColor => Color.white;
        public override string BuffIconPath => "RiskOfTheAncients2.Icons.aeon_disk_cooldown.png";
        public override EliteDef BuffEliteDef => null;
        public override bool IsCooldown => true;
        public override bool IsHidden => false;
        public override NetworkSoundEventDef BuffStartSfx => null;
    }
}