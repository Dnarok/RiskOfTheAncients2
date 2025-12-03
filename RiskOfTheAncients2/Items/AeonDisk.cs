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
        public override string ItemTokenPickup => "Become invulnerable and faster instead of dying. Usable once per stage.";
        public override string ItemTokenDesc => $"Taking {Health("lethal damage")} leaves you at {Health("1 health")}, makes you {Utility("invulnerable")} for {Utility($"{InvulnerabilityDuration.Value}")} {Utility("seconds")}, {Utility("cleanses")} negative effects, and increases your {Utility("movement speed")} by {Utility($"{MovementSpeed.Value}%")} for {Utility($"{MovementSpeedDuration.Value}")} {Utility("seconds")}. Recharges every stage.";
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

        public ConfigEntry<float> InvulnerabilityDuration;
        public ConfigEntry<float> MovementSpeed;
        public ConfigEntry<float> MovementSpeedDuration;
        public ConfigEntry<bool> PlaySound;
        public void CreateConfig(ConfigFile configuration)
        {
            InvulnerabilityDuration = configuration.Bind("Item: " + ItemName, "Invulnerability Duration", 2.5f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(InvulnerabilityDuration));
            MovementSpeed = configuration.Bind("Item: " + ItemName, "Movement Speed Bonus", 50f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(MovementSpeed));
            MovementSpeedDuration = configuration.Bind("Item: " + ItemName, "Movement Speed Duration", 5f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(MovementSpeedDuration));
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
            if (NetworkServer.active && count > 0 && !AeonDiskInvulnerability.HasThisBuff(self.body) && !self.alive)
            {
                self.Networkhealth = 1f;

                AeonDiskInvulnerability.ApplyTo(
                    body: self.body,
                    duration: InvulnerabilityDuration.Value
                );
                AeonDiskMovementSpeed.ApplyTo(
                    body: self.body,
                    duration: MovementSpeedDuration.Value
                );

                Vector3 corePosition = self.body.corePosition;
                EffectData effectData = new EffectData
                {
                    origin = corePosition
                };
                effectData.SetHurtBoxReference(self.body.mainHurtBox);
                EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/CleanseEffect"), effectData, transmit: true);

                self.body.inventory.RemoveItemPermanent(ItemDef);
                self.body.inventory.GiveItemPermanent(UsedAeonDisk.GetItemDef());
                CharacterMasterNotificationQueue.PushItemTransformNotification(self.body.master, GetItemDef().itemIndex, UsedAeonDisk.GetItemDef().itemIndex, CharacterMasterNotificationQueue.TransformationType.Default);

                if (PlaySound.Value)
                {
                    EffectManager.SimpleSoundEffect(sound.index, self.body.corePosition, true);
                }

                if (NetworkServer.active)
                {
                    CleanseSystem.CleanseBodyServer(self.body, removeDebuffs: true, removeBuffs: false, removeCooldownBuffs: true, removeDots: true, removeStun: true, removeNearbyProjectiles: false);
                }
            }

            orig(self, damageValue, damagePosition, damageIsSilent, attacker, delayedDamage, firstHitOfDelayedDamage);
        }
    }

    public class UsedAeonDisk : ItemBase<UsedAeonDisk>
    {
        public override string ItemName => "Used Aeon Disk";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "USED_AEON_DISK";
        public override string ItemTokenPickup => "Combo broken!";
        public override string ItemTokenDesc => $"At the start of each stage, it reverts to Aeon Disk.";
        public override string ItemTokenLore => "...AND LIVE!";
        public override string ItemDefGUID => Assets.AeonDisk.UsedItemDef;
        public override void Hooks()
        {
            RoR2.Stage.onStageStartGlobal += OnStageStart;
        }
        public override void Init(ConfigFile configuration)
        {
            CreateLanguageTokens();
            CreateItemDef();
            Hooks();
        }

        private void OnStageStart(Stage stage)
        {
            if (CharacterMaster.instancesList != null)
            {
                foreach (CharacterMaster master in CharacterMaster.instancesList)
                {
                    int count = GetCount(master);
                    if (count > 0)
                    {
                        master.inventory.RemoveItemPermanent(ItemDef, count);
                        master.inventory.GiveItemPermanent(AeonDisk.GetItemDef(), count);
                        CharacterMasterNotificationQueue.PushItemTransformNotification(master, ItemDef.itemIndex, AeonDisk.GetItemDef().itemIndex, default);
                    }
                }
            }
        }
    }
}