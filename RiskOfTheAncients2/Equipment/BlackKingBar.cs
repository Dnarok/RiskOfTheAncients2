using BepInEx.Configuration;
using R2API;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
using ROTA2.Buffs;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ROTA2.Equipment
{
    public class BlackKingBar : EquipmentBase<BlackKingBar>
    {
        public override string EquipmentName => "Black King Bar";
        public override string EquipmentTokenName => "BLACK_KING_BAR";
        public override string EquipmentTokenPickup => "Become immune to negative effects and reduce incoming damage for a short time.";
        public override string EquipmentTokenDesc => $"Become {Utility("immune")} to all negative effects and reduce all {Damage("incoming damage")} by {Damage($"{DamageReduction.Value}%")} for {Utility($"{AvatarDuration.Value} seconds")}.";
        public override string EquipmentTokenLore => "A powerful staff imbued with the strength of giants.";
        public override float EquipmentCooldown => BlackKingBarCooldown.Value;
        public override string EquipmentDefGUID => Assets.BlackKingBar.EquipmentDef;
        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateSounds();
            CreateLanguageTokens();
            CreateEquipmentDef();
        }
        protected override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            var prefab = Addressables.LoadAssetAsync<GameObject>(Assets.BlackKingBar.Display).WaitForCompletion();
            ItemDisplayRuleDict rules = new();
            rules.Add("mdlCommandoDualies",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "Chest",
                    localPos = new Vector3(0F, 0.32288F, -0.21657F),
                    localAngles = new Vector3(0F, 270F, 0F),
                    localScale = new Vector3(0.25F, 0.25F, 0.25F)
                }
            ]);
            rules.Add("mdlHuntress",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "Muzzle",
                    localPos = new Vector3(0F, 0F, 0F),
                    localAngles = new Vector3(0F, 90F, 90F),
                    localScale = new Vector3(0.15F, 0.15F, 0.15F)
                }
            ]);
            rules.Add("mdlToolbot",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "HandR",
                    localPos = new Vector3(-0.00007F, 0.72823F, 1.34924F),
                    localAngles = new Vector3(90F, 0F, 0F),
                    localScale = new Vector3(2F, 2F, 2F)
                }
            ]);
            rules.Add("mdlEngi",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "HandR",
                    localPos = new Vector3(0.1689F, 0.13944F, -0.02592F),
                    localAngles = new Vector3(90F, 90F, 0F),
                    localScale = new Vector3(0.25F, 0.25F, 0.25F)
                }
            ]);
            rules.Add("mdlEngiTurret",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "Base",
                    localPos = new Vector3(0f, 0f, 0f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = new Vector3(0f, 0f, 0f),
                }
            ]);
            rules.Add("mdlEngiWalkerTurret",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "Base",
                    localPos = new Vector3(0f, 0f, 0f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = new Vector3(0f, 0f, 0f),
                }
            ]);
            rules.Add("mdlMage",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "Chest",
                    localPos = new Vector3(-0.14083F, 0.40487F, -0.20451F),
                    localAngles = new Vector3(8.57235F, 268.1937F, 0F),
                    localScale = new Vector3(0.2F, 0.2F, 0.2F)
                }
            ]);
            rules.Add("mdlMerc",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "HandR",
                    localPos = new Vector3(0.18137F, 0.16401F, 0.014F),
                    localAngles = new Vector3(90F, 90F, 0F),
                    localScale = new Vector3(0.25F, 0.25F, 0.25F)
                }
            ]);
            rules.Add("mdlTreebot",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "FootBackL",
                    localPos = new Vector3(0.00006F, 0.50691F, -0.0688F),
                    localAngles = new Vector3(0F, 90F, 180F),
                    localScale = new Vector3(0.4F, 0.4F, 0.4F)
                }
            ]);
            rules.Add("mdlLoader",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "MechHandRight",
                    localPos = new Vector3(0.18757F, 0.31668F, 0.00001F),
                    localAngles = new Vector3(90F, 90F, 0F),
                    localScale = new Vector3(0.3F, 0.3F, 0.3F)
                }
            ]);
            rules.Add("mdlCroco",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "MouthMuzzle",
                    localPos = new Vector3(-1.58213F, 2.13522F, 3.09165F),
                    localAngles = new Vector3(344.793F, 0F, 90F),
                    localScale = new Vector3(2.5F, 2.5F, 2.5F)
                }
            ]);
            rules.Add("mdlCaptain",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "HandR",
                    localPos = new Vector3(0.00992F, 0.11026F, 0.18148F),
                    localAngles = new Vector3(90F, 0F, 0F),
                    localScale = new Vector3(0.25F, 0.25F, 0.25F)
                }
            ]);
            rules.Add("mdlBandit2",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "ThighR",
                    localPos = new Vector3(-0.11807F, 0.27893F, 0.00001F),
                    localAngles = new Vector3(-0.00001F, 180F, 180F),
                    localScale = new Vector3(0.2F, 0.2F, 0.2F)
                }
            ]);
            rules.Add("mdlChef",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "PizzaCutter",
                    localPos = new Vector3(0.00002F, -0.19456F, -0.00007F),
                    localAngles = new Vector3(0F, 0F, 180F),
                    localScale = new Vector3(0.3F, 0.3F, 0.3F)
                }
            ]);
            rules.Add("mdlRailGunner",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "MuzzleSniper",
                    localPos = new Vector3(0.19567F, 0F, -0.03756F),
                    localAngles = new Vector3(90F, 0F, 0F),
                    localScale = new Vector3(0.25F, 0.25F, 0.25F)
                }
            ]);
            rules.Add("mdlVoidSurvivor",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "Hand",
                    localPos = new Vector3(-0.04788F, 0.1147F, 0.21336F),
                    localAngles = new Vector3(90F, 0F, 0F),
                    localScale = new Vector3(0.25F, 0.25F, 0.25F)
                }
            ]);
            rules.Add("mdlSeeker",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "HandR",
                    localPos = new Vector3(0.20863F, 0.08456F, -0.02071F),
                    localAngles = new Vector3(90F, 90F, 0F),
                    localScale = new Vector3(0.25F, 0.25F, 0.25F)
                }
            ]);
            rules.Add("mdlFalseSon",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "HandR",
                    localPos = new Vector3(0.28669F, 0.21788F, -0.04943F),
                    localAngles = new Vector3(90F, 90F, 0F),
                    localScale = new Vector3(0.4F, 0.4F, 0.4F)
                }
            ]);
            return rules;
        }

        public ConfigEntry<float> AvatarDuration;
        public ConfigEntry<float> DamageReduction;
        public ConfigEntry<float> BlackKingBarCooldown;
        private void CreateConfig(ConfigFile config)
        {
            AvatarDuration = config.Bind("Equipment: " + EquipmentName, "Avatar Duration", 6f, "How long should the immunity and damage reduction last?");
            ModSettingsManager.AddOption(new FloatFieldOption(AvatarDuration));
            DamageReduction = config.Bind("Equipment: " + EquipmentName, "Incoming Damage Reduction", 35f, "What percentage of incoming damage should be reduced while active?");
            ModSettingsManager.AddOption(new FloatFieldOption(DamageReduction));
            BlackKingBarCooldown = config.Bind("Equipment: " + EquipmentName, "Cooldown", 65f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(BlackKingBarCooldown));
        }

        NetworkSoundEventDef sound = null;
        protected void CreateSounds()
        {
            Addressables.LoadAssetAsync<NetworkSoundEventDef>(Assets.BlackKingBar.NetworkSoundEventDef).Completed += (x) => { ContentAddition.AddNetworkSoundEventDef(x.Result); sound = x.Result; };
        }

        protected override bool ActivateEquipment(EquipmentSlot slot)
        {
            var body = slot.characterBody;
            if (HasThisEquipment(body))
            {
                BlackKingBarBuff.ApplyTo(
                    body: body,
                    duration: AvatarDuration.Value
                );

                Vector3 corePosition = body.corePosition;
                EffectData effectData = new EffectData
                {
                    origin = corePosition
                };
                effectData.SetHurtBoxReference(body.mainHurtBox);
                EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/CleanseEffect"), effectData, transmit: true);
                Util.CleanseBody(body, removeDebuffs: true, removeBuffs: false, removeCooldownBuffs: true, removeDots: true, removeStun: true, removeNearbyProjectiles: false);

                EffectManager.SimpleSoundEffect(sound.index, body.corePosition, true);
            }

            return true;
        }
    }
}