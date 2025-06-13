using R2API;
using RoR2;
using ROTA2.Items;

namespace ROTA2.Buffs
{
    public class AeonDiskInvulnerability : BuffBase<AeonDiskInvulnerability>
    {
        public override string BuffName => "Combo Breaker";
        public override string BuffTokenName => "AEON_DISK_INVULNERABILITY";
        public override string BuffDefGUID => Assets.AeonDisk.InvulnerabilityBuffDef;
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
        public override string BuffDefGUID => Assets.AeonDisk.MoveSpeedBuffDef;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddMovementSpeed;
        }

        private void AddMovementSpeed(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (HasThisBuff(body))
            {
                args.moveSpeedMultAdd += AeonDisk.Instance.MovementSpeed.Value / 100.0f;
            }
        }
    }
    public class AeonDiskCooldown : BuffBase<AeonDiskCooldown>
    {
        public override string BuffName => "Aeon Disk Cooldown";
        public override string BuffTokenName => "AEON_DISK_COOLDOWN";
        public override string BuffDefGUID => Assets.AeonDisk.CooldownBuffDef;
    }
}