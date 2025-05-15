using BepInEx.Configuration;
using RoR2;
using ROTA2.Buffs;
using UnityEngine;
using UnityEngine.Networking;

namespace ROTA2.Items
{
    public class AeonDisk : ItemBase<AeonDisk>
    {
        public override string ItemName => "Aeon Disk";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "AEON_DISK";
        public override string ItemTokenPickup => "Become invulnerable and faster at low health. Recharges over time.";
        public override string ItemTokenDesc => $"Taking {Health("lethal damage")} leaves you at {Health("1 health")}, makes you {Utility("invulnerable")} for {Utility($"{InvulnerabilityDurationBase}")} {Stack($"(+{InvulnerabilityDurationPerStack} per stack)")} {Utility("seconds")}, and increases your {Utility("movement speed")} by {Utility($"{MovementSpeed}%")} for {Utility($"{MovementSpeedDurationBase}")} {Stack($"(+{MovementSpeedDurationPerStack} per stack)")} {Utility("seconds")}. Recharges every {Utility($"{Cooldown} seconds")}.";
        public override string ItemTokenLore => "A powerful artifact long ago smuggled out of the Ivory Incubarium. Or so many believe.";
        public override ItemTier Tier => ItemTier.Tier3;
        public override string ItemIconPath => "ROTA2.Icons.aeon_disk.png";
        public override void Hooks()
        {
            On.RoR2.HealthComponent.UpdateLastHitTime += OnHit;
        }

        public override void Init(ConfigFile configuration)
        {
            CreateConfig(configuration);
            CreateLanguageTokens();
            CreateItemDef();
            Hooks();
        }

        public float InvulnerabilityDurationBase;
        public float InvulnerabilityDurationPerStack;
        public float MovementSpeed;
        public float MovementSpeedDurationBase;
        public float MovementSpeedDurationPerStack;
        public float Cooldown;
        public void CreateConfig(ConfigFile configuration)
        {
            InvulnerabilityDurationBase     = configuration.Bind("Item: " + ItemName, "Invulnerability Duration Base",      2.5f,  "").Value;
            InvulnerabilityDurationPerStack = configuration.Bind("Item: " + ItemName, "Invulnerability Duration Per Stack", 1.0f,  "").Value;
            MovementSpeed                   = configuration.Bind("Item: " + ItemName, "Movement Speed Bonus",               50.0f, "").Value;
            MovementSpeedDurationBase       = configuration.Bind("Item: " + ItemName, "Movement Speed Duration Base",       5.0f,  "").Value;
            MovementSpeedDurationPerStack   = configuration.Bind("Item: " + ItemName, "Movement Speed Duration Per Stack",  2.0f,  "").Value;
            Cooldown                        = configuration.Bind("Item: " + ItemName, "Cooldown",                           90.0f, "").Value;
        }

        private void OnHit(On.RoR2.HealthComponent.orig_UpdateLastHitTime orig, RoR2.HealthComponent self, float damageValue, Vector3 damagePosition, bool damageIsSilent, GameObject attacker, bool delayedDamage, bool firstHitOfDelayedDamage)
        {
            int count = GetCount(self.body);
            if (NetworkServer.active && count > 0 && !AeonDiskCooldown.Instance.HasThisBuff(self.body) && !self.alive)
            {
                self.Networkhealth = 1.0f;

                AeonDiskInvulnerability.Instance.ApplyTo(new Buffs.BuffBase.ApplyParameters
                {
                    victim = self.body,
                    duration = InvulnerabilityDurationBase + InvulnerabilityDurationPerStack * (count - 1)
                });
                AeonDiskMovementSpeed.Instance.ApplyTo(new Buffs.BuffBase.ApplyParameters
                {
                    victim = self.body,
                    duration = MovementSpeedDurationBase * MovementSpeedDurationPerStack * (count - 1)
                });
                AeonDiskCooldown.Instance.ApplyTo(new Buffs.BuffBase.ApplyParameters
                {
                    victim = self.body,
                    duration = Cooldown
                });

                Util.PlaySound("AeonDisk", self.body.gameObject);
            }

            orig(self, damageValue, damagePosition, damageIsSilent, attacker, delayedDamage, firstHitOfDelayedDamage);
        }
    }
}