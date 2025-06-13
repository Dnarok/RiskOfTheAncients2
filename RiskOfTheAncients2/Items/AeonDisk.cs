using BepInEx.Configuration;
using R2API;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
using ROTA2.Buffs;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace ROTA2.Items
{
    public class AeonDisk : ItemBase<AeonDisk>
    {
        public override string ItemName => "Aeon Disk";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "AEON_DISK";
        public override string ItemTokenPickup => "Become invulnerable and faster at low health. Recharges over time.";
        public override string ItemTokenDesc => $"Taking {Health("lethal damage")} leaves you at {Health("1 health")}, makes you {Utility("invulnerable")} for {Utility($"{InvulnerabilityDurationBase.Value}")} {Stack($"(+{InvulnerabilityDurationPerStack.Value} per stack)")} {Utility("seconds")}, {Utility("cleanses")} negative effects, and increases your {Utility("movement speed")} by {Utility($"{MovementSpeed.Value}%")} for {Utility($"{MovementSpeedDurationBase.Value}")} {Stack($"(+{MovementSpeedDurationPerStack.Value} per stack)")} {Utility("seconds")}. Recharges every {Utility($"{Cooldown.Value} seconds")}.";
        public override string ItemTokenLore => "A powerful artifact long ago smuggled out of the Ivory Incubarium. Or so many believe.";
        public override string ItemDefGUID => Assets.AeonDisk.ItemDef;
        public override void Hooks()
        {
            On.RoR2.HealthComponent.UpdateLastHitTime += OnHit;
        }
        public override void Init(ConfigFile configuration)
        {
            CreateConfig(configuration);
            CreateSounds();
            CreateLanguageTokens();
            CreateItemDef();
            Hooks();
        }

        public ConfigEntry<float> InvulnerabilityDurationBase;
        public ConfigEntry<float> InvulnerabilityDurationPerStack;
        public ConfigEntry<float> MovementSpeed;
        public ConfigEntry<float> MovementSpeedDurationBase;
        public ConfigEntry<float> MovementSpeedDurationPerStack;
        public ConfigEntry<float> Cooldown;
        public ConfigEntry<bool> PlaySound;
        public void CreateConfig(ConfigFile configuration)
        {
            InvulnerabilityDurationBase = configuration.Bind("Item: " + ItemName, "Invulnerability Duration Base", 2.5f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(InvulnerabilityDurationBase));
            InvulnerabilityDurationPerStack = configuration.Bind("Item: " + ItemName, "Invulnerability Duration Per Stack", 1.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(InvulnerabilityDurationPerStack));
            MovementSpeed = configuration.Bind("Item: " + ItemName, "Movement Speed Bonus", 50.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(MovementSpeed));
            MovementSpeedDurationBase = configuration.Bind("Item: " + ItemName, "Movement Speed Duration Base", 5.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(MovementSpeedDurationBase));
            MovementSpeedDurationPerStack = configuration.Bind("Item: " + ItemName, "Movement Speed Duration Per Stack", 2.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(MovementSpeedDurationPerStack));
            Cooldown = configuration.Bind("Item: " + ItemName, "Cooldown", 90.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(Cooldown));
            PlaySound = configuration.Bind("Item: " + ItemName, "Play Sound", true, "");
            ModSettingsManager.AddOption(new CheckBoxOption(PlaySound));
        }

        NetworkSoundEventDef sound = null;
        protected void CreateSounds()
        {
            Addressables.LoadAssetAsync<NetworkSoundEventDef>(Assets.AeonDisk.NetworkSoundEventDef).Completed += (x) => { ContentAddition.AddNetworkSoundEventDef(x.Result); sound = x.Result; };
        }

        private void OnHit(On.RoR2.HealthComponent.orig_UpdateLastHitTime orig, HealthComponent self, float damageValue, Vector3 damagePosition, bool damageIsSilent, GameObject attacker, bool delayedDamage, bool firstHitOfDelayedDamage)
        {
            int count = GetCount(self.body);
            if (NetworkServer.active && count > 0 && !AeonDiskCooldown.HasThisBuff(self.body) && !self.alive)
            {
                self.Networkhealth = 1.0f;

                AeonDiskInvulnerability.ApplyTo(
                    body: self.body,
                    duration: InvulnerabilityDurationBase.Value + InvulnerabilityDurationPerStack.Value * (count - 1)
                );
                AeonDiskMovementSpeed.ApplyTo(
                    body: self.body,
                    duration: MovementSpeedDurationBase.Value + MovementSpeedDurationPerStack.Value * (count - 1)
                );
                AeonDiskCooldown.ApplyTo(
                    body: self.body,
                    duration: Cooldown.Value
                );

                Vector3 corePosition = self.body.corePosition;
                EffectData effectData = new EffectData
                {
                    origin = corePosition
                };
                effectData.SetHurtBoxReference(self.body.mainHurtBox);
                EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/CleanseEffect"), effectData, transmit: true);
                Util.CleanseBody(self.body, removeDebuffs: true, removeBuffs: false, removeCooldownBuffs: true, removeDots: true, removeStun: true, removeNearbyProjectiles: false);

                if (PlaySound.Value)
                {
                    EffectManager.SimpleSoundEffect(sound.index, self.body.corePosition, true);
                }
            }

            orig(self, damageValue, damagePosition, damageIsSilent, attacker, delayedDamage, firstHitOfDelayedDamage);
        }
    }
}