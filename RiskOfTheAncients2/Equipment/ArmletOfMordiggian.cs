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

        public ConfigEntry<float> DamageBonus;
        public ConfigEntry<float> AttackSpeedBonus;
        public ConfigEntry<float> ArmorBonus;
        public ConfigEntry<float> MaximumHealthLostPerSecond;
        public ConfigEntry<float> ArmletCooldown;
        private void CreateConfig(ConfigFile config)
        {
            DamageBonus = config.Bind("Equipment: " + EquipmentName, "Active Damage Bonus", 60.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(DamageBonus));
            AttackSpeedBonus = config.Bind("Equipment: " + EquipmentName, "Active Attack Speed Bonus", 60.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(AttackSpeedBonus));
            ArmorBonus = config.Bind("Equipment: " + EquipmentName, "Active Armor Bonus", 50.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(ArmorBonus));
            MaximumHealthLostPerSecond = config.Bind("Equipment: " + EquipmentName, "Active Maximum Health Loss Per Second", 5.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(MaximumHealthLostPerSecond));
            ArmletCooldown = config.Bind("Equipment: " + EquipmentName, "Cooldown", 5.0f, "");
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
            float elapsed = 0.0f;
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
                        damageType = DamageType.NonLethal | DamageType.BypassBlock | DamageType.Silent
                    };
                    DamageInfo info = new()
                    {
                        damage = health.fullCombinedHealth * Instance.MaximumHealthLostPerSecond.Value / 100.0f * tick,
                        procCoefficient = 0.0f,
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