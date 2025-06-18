using BepInEx.Configuration;
using R2API;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
using RoR2.UI;
using ROTA2.Buffs;
using UnityEngine;
using UnityEngine.AddressableAssets;
namespace ROTA2.Equipment
{
    public class ArmletOfMordiggian : EquipmentBase<ArmletOfMordiggian>
    {
        public override string EquipmentName => "Armlet of Mordiggian";
        public override string EquipmentTokenName => "ARMLET_OF_MORDIGGIAN";
        public override string EquipmentTokenPickup => $"Toggle to increase damage, attack speed, and armor... {Health("BUT remove all healing and lose health rapidly.")}";
        public override string EquipmentTokenDesc => $"{Utility("Toggle")}, increasing {Damage("damage")} by {Damage($"{DamageBonus.Value}%")}, {Damage("attack speed")} by {Damage($"{AttackSpeedBonus.Value}%")}, and {Damage("armor")} by {Damage($"{ArmorBonus.Value}")}. While {Utility("toggled")}, {Health("prevent all healing")} and {Death($"lose {MaximumHealthLostPerSecond.Value}%")} of your {Health("maximum health")} per second.";
        public override string EquipmentTokenLore => "Weapon of choice among brutes, the bearer sacrifices his life energy to gain immense strength and power.";
        public override float EquipmentCooldown => ArmletCooldown.Value;
        public override string EquipmentDefGUID => Assets.ArmletOfMordiggian.EquipmentDef;
        public override void Hooks()
        {
            On.RoR2.UI.EquipmentIcon.SetDisplayData += ModifyDisplayData;
            On.RoR2.CharacterBody.OnEquipmentLost += OnEquipmentLost;
        }
        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateSounds();
            CreateTextures();
            CreateLanguageTokens();
            CreateEquipmentDef();
            Hooks();
        }
        protected override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            var prefab = Addressables.LoadAssetAsync<GameObject>(Assets.ArmletOfMordiggian.Display).WaitForCompletion();
            ItemDisplayRuleDict rules = new();
            rules.Add("mdlCommandoDualies",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "LowerArmL",
                    localPos = new Vector3(0.00662F, 0.19009F, -0.03836F),
                    localAngles = new Vector3(25.64926F, 38.6189F, 229.9474F),
                    localScale = new Vector3(0.25F, 0.25F, 0.25F)
                }
            ]);
            rules.Add("mdlHuntress",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "LowerArmL",
                    localPos = new Vector3(-0.03505F, 0.15087F, 0.01517F),
                    localAngles = new Vector3(25.54721F, 132.3588F, 224.4509F),
                    localScale = new Vector3(0.18F, 0.18F, 0.18F)
                }
            ]);
            rules.Add("mdlToolbot",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "LowerArmL",
                    localPos = new Vector3(0.3379F, 2.09508F, 0.65033F),
                    localAngles = new Vector3(20.35008F, 226.4485F, 218.6916F),
                    localScale = new Vector3(3F, 3F, 3F)
                }
            ]);
            rules.Add("mdlEngi",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "LowerArmL",
                    localPos = new Vector3(0.00761F, 0.18047F, -0.07358F),
                    localAngles = new Vector3(22.26755F, 39.19514F, 227.847F),
                    localScale = new Vector3(0.25112F, 0.25221F, 0.25617F)
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
                    childName = "Head",
                    localPos = new Vector3(-0.00772F, 0.12793F, 0.02797F),
                    localAngles = new Vector3(29.91999F, 128.0866F, 4.47263F),
                    localScale = new Vector3(0.27F, 0.27F, 0.27F)
                }
            ]);
            rules.Add("mdlMerc",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "LowerArmL",
                    localPos = new Vector3(-0.00004F, 0.12637F, -0.04767F),
                    localAngles = new Vector3(23.69618F, 43.86812F, 228.6077F),
                    localScale = new Vector3(0.25112F, 0.25221F, 0.25617F)
                }
            ]);
            rules.Add("mdlTreebot",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "FootBackL",
                    localPos = new Vector3(0.0019F, 0.73911F, -0.06439F),
                    localAngles = new Vector3(17.82564F, 40.64308F, 230.0784F),
                    localScale = new Vector3(0.5F, 0.5F, 0.5F)
                }
            ]);
            rules.Add("mdlLoader",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "LowerArmL",
                    localPos = new Vector3(-0.02437F, 0.13945F, -0.01861F),
                    localAngles = new Vector3(15.58784F, 83.94258F, 231.6736F),
                    localScale = new Vector3(0.2F, 0.2F, 0.2F)
                }
            ]);
            rules.Add("mdlCroco",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "LowerArmL",
                    localPos = new Vector3(-0.4549F, 1.58741F, 0.03962F),
                    localAngles = new Vector3(15.84318F, 135.3776F, 223.8735F),
                    localScale = new Vector3(2.39148F, 2.39148F, 2.39148F)
                }
            ]);
            rules.Add("mdlCaptain",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "MuzzleGun",
                    localPos = new Vector3(0.07529F, 0.00868F, -0.17878F),
                    localAngles = new Vector3(45.02224F, 240.2036F, 109.8633F),
                    localScale = new Vector3(0.25F, 0.25F, 0.25F)
                }
            ]);
            rules.Add("mdlBandit2",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "LowerArmL",
                    localPos = new Vector3(-0.03596F, 0.11842F, 0.00503F),
                    localAngles = new Vector3(9.31973F, 134.5163F, 224.7896F),
                    localScale = new Vector3(0.2F, 0.2F, 0.2F)
                }
            ]);
            rules.Add("mdlChef",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "LowerArmL",
                    localPos = new Vector3(0.19228F, 0.02053F, -0.02746F),
                    localAngles = new Vector3(13.72011F, 226.8655F, 317.0612F),
                    localScale = new Vector3(0.2F, 0.2F, 0.2F)
                }
            ]);
            rules.Add("mdlRailGunner",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "LowerArmL",
                    localPos = new Vector3(-0.01191F, 0.12651F, -0.02463F),
                    localAngles = new Vector3(16.01123F, 66.31157F, 227.4979F),
                    localScale = new Vector3(0.17F, 0.17F, 0.17F)
                }
            ]);
            rules.Add("mdlVoidSurvivor",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "ForeArmL",
                    localPos = new Vector3(0.03101F, 0.27247F, 0.01259F),
                    localAngles = new Vector3(20.40562F, 303.2962F, 237.4383F),
                    localScale = new Vector3(0.25112F, 0.25221F, 0.25617F)
                }
            ]);
            rules.Add("mdlSeeker",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "LowerArmL",
                    localPos = new Vector3(0.0053F, 0.16928F, -0.04099F),
                    localAngles = new Vector3(17.19487F, 28.56992F, 226.1967F),
                    localScale = new Vector3(0.21F, 0.21F, 0.24F)
                }
            ]);
            rules.Add("mdlFalseSon",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "LowerArmR",
                    localPos = new Vector3(0.03182F, 0.35667F, 0.0521F),
                    localAngles = new Vector3(9.07225F, 266.4364F, 221.3455F),
                    localScale = new Vector3(0.45F, 0.45F, 0.45F)
                }
            ]);
            return rules;
        }

        public ConfigEntry<float> DamageBonus;
        public ConfigEntry<float> AttackSpeedBonus;
        public ConfigEntry<float> ArmorBonus;
        public ConfigEntry<float> MaximumHealthLostPerSecond;
        public ConfigEntry<float> ArmletCooldown;
        private void CreateConfig(ConfigFile config)
        {
            DamageBonus = config.Bind("Equipment: " + EquipmentName, "Active Damage Bonus", 75f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(DamageBonus));
            AttackSpeedBonus = config.Bind("Equipment: " + EquipmentName, "Active Attack Speed Bonus", 75f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(AttackSpeedBonus));
            ArmorBonus = config.Bind("Equipment: " + EquipmentName, "Active Armor Bonus", 100f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(ArmorBonus));
            MaximumHealthLostPerSecond = config.Bind("Equipment: " + EquipmentName, "Active Maximum Health Loss Per Second", 5f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(MaximumHealthLostPerSecond));
            ArmletCooldown = config.Bind("Equipment: " + EquipmentName, "Cooldown", 5f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(ArmletCooldown));
        }

        NetworkSoundEventDef soundOn = null;
        NetworkSoundEventDef soundOff = null;
        protected void CreateSounds()
        {
            Addressables.LoadAssetAsync<NetworkSoundEventDef>(Assets.ArmletOfMordiggian.SoundOn).Completed += (x) => { ContentAddition.AddNetworkSoundEventDef(x.Result); soundOn = x.Result; };
            Addressables.LoadAssetAsync<NetworkSoundEventDef>(Assets.ArmletOfMordiggian.SoundOff).Completed += (x) => { ContentAddition.AddNetworkSoundEventDef(x.Result); soundOff = x.Result; };
        }

        Texture2D OnIcon = null;
        Texture2D OffIcon = null;
        protected void CreateTextures()
        {
            Addressables.LoadAssetAsync<Sprite>(Assets.ArmletOfMordiggian.IconOn).Completed += (x) => { OnIcon = x.Result.texture; };
            Addressables.LoadAssetAsync<Sprite>(Assets.ArmletOfMordiggian.IconOff).Completed += (x) => { OffIcon = x.Result.texture; };
        }

        protected override bool ActivateEquipment(EquipmentSlot slot)
        {
            if (slot && HasThisEquipment(slot.characterBody))
            {
                var behavior = slot.characterBody.GetComponent<ArmletOfMordiggianBehavior>();
                if (behavior)
                {
                    slot.characterBody.RemoveBuff(ArmletOfMordiggianBuff.GetBuffDef());
                    slot.characterBody.RemoveBuff(RoR2Content.Buffs.HealingDisabled);
                    Object.Destroy(slot.characterBody.GetComponent<ArmletOfMordiggianBehavior>());

                    EffectManager.SimpleSoundEffect(soundOff.index, slot.characterBody.corePosition, true);
                }
                else
                {
                    slot.characterBody.AddBuff(ArmletOfMordiggianBuff.GetBuffDef());
                    slot.characterBody.AddBuff(RoR2Content.Buffs.HealingDisabled);
                    slot.characterBody.gameObject.AddComponent<ArmletOfMordiggianBehavior>();

                    EffectManager.SimpleSoundEffect(soundOn.index, slot.characterBody.corePosition, true);
                }
            }

            return true;
        }

        private void ModifyDisplayData(On.RoR2.UI.EquipmentIcon.orig_SetDisplayData orig, EquipmentIcon self, EquipmentIcon.DisplayData data)
        {
            orig(self, data);
            if (self && self.targetEquipmentSlot && self.targetEquipmentSlot.characterBody && self.currentDisplayData.equipmentDef == EquipmentDef)
            {
                var behavior = self.targetEquipmentSlot.characterBody.GetComponent<ArmletOfMordiggianBehavior>();
                if (!behavior)
                {
                    self.iconImage.texture = OffIcon;
                }
                else
                {
                    self.iconImage.texture = OnIcon;
                }
            }
        }
        private void OnEquipmentLost(On.RoR2.CharacterBody.orig_OnEquipmentLost orig, CharacterBody self, EquipmentDef equipmentDef)
        {
            if (equipmentDef == EquipmentDef)
            {
                self.RemoveBuff(ArmletOfMordiggianBuff.GetBuffDef());
                self.RemoveBuff(RoR2Content.Buffs.HealingDisabled);
                var behavior = self.GetComponent<ArmletOfMordiggianBehavior>();
                if (behavior)
                {
                    Object.Destroy(self.GetComponent<ArmletOfMordiggianBehavior>());

                    EffectManager.SimpleSoundEffect(soundOff.index, self.corePosition, true);
                }
            }

            orig(self, equipmentDef);
        }

        public class ArmletOfMordiggianBehavior : MonoBehaviour
        {
            HealthComponent health;
            float elapsed = 0f;
            float tick = 0.2f;

            void Awake()
            {
                health = GetComponent<HealthComponent>();
            }

            void FixedUpdate()
            {
                elapsed += Time.fixedDeltaTime;
                if (elapsed >= tick)
                {
                    elapsed -= tick;
                    DamageTypeCombo combo = new()
                    {
                        damageSource = DamageSource.Equipment,
                        damageType = DamageType.NonLethal | DamageType.BypassBlock | DamageType.BypassArmor | DamageType.Silent
                    };
                    DamageInfo info = new()
                    {
                        damage = health.fullCombinedHealth * Instance.MaximumHealthLostPerSecond.Value / 100f * tick,
                        procCoefficient = 0f,
                        damageType = combo,
                        damageColorIndex = DamageColorIndex.Bleed,
                        position = health.body.transform.position
                    };
                    health.TakeDamage(info);
                }
            }
        }
    }
}