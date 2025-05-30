using BepInEx.Configuration;
using MonoMod.RuntimeDetour;
using RoR2;
using RoR2.UI;
using ROTA2.Buffs;
using System;
using UnityEngine;
namespace ROTA2.Equipment
{
    public class ArmletOfMordiggian : EquipmentBase<ArmletOfMordiggian>
    {
        public override string EquipmentName => "Armlet of Mordiggian";
        public override string EquipmentTokenName => "ARMLET_OF_MORDIGGIAN";
        public override string EquipmentTokenPickup => $"Toggle to increase damage, attack speed, and armor... {Health("BUT remove all healing and lose health rapidly.")}";
        public override string EquipmentTokenDesc => $"{Utility("Toggle")}, increasing {Damage("damage")} by {Damage($"{DamageBonus}%")}, {Damage("attack speed")} by {Damage($"{AttackSpeedBonus}%")}, and {Damage("armor")} by {Damage($"{ArmorBonus}")}. While {Utility("toggled")}, {Health("prevent all healing")} and {Death($"lose {MaximumHealthLostPerSecond}%")} of your {Health("maximum health")} per second.";
        public override string EquipmentTokenLore => "Weapon of choice among brutes, the bearer sacrifices his life energy to gain immense strength and power.";
        public override float EquipmentCooldown => ArmletCooldown;
        public override string EquipmentIconPath => "ROTA2.Icons.armlet_of_mordiggian.png";
        public override string EquipmentModelPath => "armlet_of_mordiggian.prefab";
        public override bool EquipmentIsLunar => true;
        public override bool EquipmentCanBeRandomlyTriggered => false;
        public override ColorCatalog.ColorIndex EquipmentColorIndex => ColorCatalog.ColorIndex.LunarItem;
        public override void Hooks()
        {
            var target = typeof(EquipmentIcon).GetMethod(nameof(EquipmentIcon.SetDisplayData), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var destination = typeof(ArmletOfMordiggian).GetMethod(nameof(ModifyDisplayData), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            new Hook(target, destination, this);

            On.RoR2.CharacterBody.OnEquipmentLost += OnEquipmentLost;
        }
        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLanguageTokens();
            CreateEquipmentDef();
            Hooks();
        }

        public float DamageBonus;
        public float AttackSpeedBonus;
        public float ArmorBonus;
        public float MaximumHealthLostPerSecond;
        public float ArmletCooldown;
        private void CreateConfig(ConfigFile config)
        {
            DamageBonus                 = config.Bind("Equipment: " + EquipmentName, "Active Damage Bonus", 60.0f, "").Value;
            AttackSpeedBonus            = config.Bind("Equipment: " + EquipmentName, "Active Attack Speed Bonus", 60.0f, "").Value;
            ArmorBonus                  = config.Bind("Equipment: " + EquipmentName, "Active Armor Bonus", 50.0f, "").Value;
            MaximumHealthLostPerSecond  = config.Bind("Equipment: " + EquipmentName, "Active Maximum Health Loss Per Second", 5.0f, "").Value;
            ArmletCooldown              = config.Bind("Equipment: " + EquipmentName, "Cooldown", 5.0f, "").Value;
        }

        protected override bool ActivateEquipment(EquipmentSlot slot)
        {
            if (slot && HasThisEquipment(slot.characterBody))
            {
                var behavior = slot.characterBody.GetComponent<ArmletOfMordiggianBehavior>();
                if (behavior)
                {
                    slot.characterBody.RemoveBuff(ArmletOfMordiggianBuff.Instance.BuffDef);
                    slot.characterBody.RemoveBuff(RoR2Content.Buffs.HealingDisabled);
                    UnityEngine.Object.Destroy(slot.characterBody.GetComponent<ArmletOfMordiggianBehavior>());
                    Util.PlaySound("ArmletOfMordiggianOff", slot.characterBody.gameObject);
                }
                else
                {
                    slot.characterBody.AddBuff(ArmletOfMordiggianBuff.Instance.BuffDef);
                    slot.characterBody.AddBuff(RoR2Content.Buffs.HealingDisabled);
                    slot.characterBody.gameObject.AddComponent<ArmletOfMordiggianBehavior>();
                    Util.PlaySound("ArmletOfMordiggianOn", slot.characterBody.gameObject);
                }
            }

            return true;
        }

        private string OffPath = "ROTA2.Icons.armlet_of_mordiggian.png";
        private string OnPath = "ROTA2.Icons.armlet_of_mordiggian_toggled.png";
        private void ModifyDisplayData(Action<EquipmentIcon, EquipmentIcon.DisplayData> orig, EquipmentIcon self, EquipmentIcon.DisplayData data)
        {
            orig(self, data);
            if (self && self.targetEquipmentSlot && self.targetEquipmentSlot.characterBody && self.currentDisplayData.equipmentDef == EquipmentDef)
            {
                Texture new_texture = null;
                var behavior = self.targetEquipmentSlot.characterBody.GetComponent<ArmletOfMordiggianBehavior>();
                if (!behavior)
                {
                    new_texture = Plugin.ExtractSprite(OffPath).texture;
                }
                else
                {
                    new_texture = Plugin.ExtractSprite(OnPath).texture;
                }

                if (new_texture)
                {
                    self.iconImage.texture = new_texture;
                }
            }
        }
        private void OnEquipmentLost(On.RoR2.CharacterBody.orig_OnEquipmentLost orig, CharacterBody self, EquipmentDef equipmentDef)
        {
            if (equipmentDef == EquipmentDef)
            {
                self.RemoveBuff(ArmletOfMordiggianBuff.Instance.BuffDef);
                self.RemoveBuff(RoR2Content.Buffs.HealingDisabled);
                UnityEngine.Object.Destroy(self.GetComponent<ArmletOfMordiggianBehavior>());
                Util.PlaySound("ArmletOfMordiggianOff", self.gameObject);
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
                        damage = health.fullCombinedHealth * ArmletOfMordiggian.Instance.MaximumHealthLostPerSecond / 100.0f * tick,
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