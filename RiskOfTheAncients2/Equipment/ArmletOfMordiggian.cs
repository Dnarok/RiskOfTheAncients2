using BepInEx.Configuration;
using RoR2;
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
        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLanguageTokens();
            CreateEquipmentDef();
        }

        public float DamageBonus;
        public float AttackSpeedBonus;
        public float ArmorBonus;
        public float MaximumHealthLostPerSecond;
        public float ArmletCooldown;
        private void CreateConfig(ConfigFile config)
        {
            DamageBonus                 = config.Bind("Equipment: " + EquipmentName, "Active Damage Bonus", 50.0f, "").Value;
            AttackSpeedBonus            = config.Bind("Equipment: " + EquipmentName, "Active Attack Speed Bonus", 50.0f, "").Value;
            ArmorBonus                  = config.Bind("Equipment: " + EquipmentName, "Active Armor Bonus", 50.0f, "").Value;
            MaximumHealthLostPerSecond  = config.Bind("Equipment: " + EquipmentName, "Active Maximum Health Loss Per Second", 5.0f, "").Value;
            ArmletCooldown              = config.Bind("Equipment: " + EquipmentName, "Cooldown", 6.0f, "").Value;
        }

        protected override bool ActivateEquipment(EquipmentSlot slot)
        {
            if (slot && HasThisEquipment(slot.characterBody))
            {
                EquipmentState state = new()
                {
                    equipmentIndex = ArmletOfMordiggianToggled.Instance.EquipmentDef.equipmentIndex,
                    chargeFinishTime = Run.FixedTimeStamp.now + ArmletCooldown,
                    charges = 0,
                    equipmentDef = ArmletOfMordiggianToggled.Instance.EquipmentDef
                };
                slot.inventory.SetEquipment(state, slot.activeEquipmentSlot);
            }

            return true;
        }
    }

    public class ArmletOfMordiggianToggled : EquipmentBase<ArmletOfMordiggianToggled>
    {
        public override string EquipmentName => "Armlet of Mordiggian (Toggled)";
        public override string EquipmentTokenName => "ARMLET_OF_MORDIGGIAN_TOGGLED";
        public override string EquipmentTokenPickup => "Toggle to stop.";
        public override string EquipmentTokenDesc => $"{Utility("Toggle")} to stop.";
        public override string EquipmentTokenLore => "Weapon of choice among brutes, the bearer sacrifices his life energy to gain immense strength and power.";
        public override float EquipmentCooldown => ArmletCooldown;
        public override string EquipmentIconPath => "ROTA2.Icons.armlet_of_mordiggian_toggled.png";
        public override string EquipmentModelPath => "armlet_of_mordiggian.prefab";
        public override bool EquipmentIsLunar => true;
        public override bool EquipmentCanDrop => false;
        public override bool EquipmentCanBeRandomlyTriggered => false;
        public override ColorCatalog.ColorIndex EquipmentColorIndex => ColorCatalog.ColorIndex.LunarItem;
        public override void Hooks()
        {
            On.RoR2.CharacterBody.OnEquipmentGained += OnEquipmentGained;
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
            DamageBonus = config.Bind("Equipment: " + EquipmentName, "Active Damage Bonus", 50.0f, "").Value;
            AttackSpeedBonus = config.Bind("Equipment: " + EquipmentName, "Active Attack Speed Bonus", 50.0f, "").Value;
            ArmorBonus = config.Bind("Equipment: " + EquipmentName, "Active Armor Bonus", 50.0f, "").Value;
            MaximumHealthLostPerSecond = config.Bind("Equipment: " + EquipmentName, "Active Maximum Health Loss Per Second", 5.0f, "").Value;
            ArmletCooldown = config.Bind("Equipment: " + EquipmentName, "Cooldown", 6.0f, "").Value;
        }

        protected override bool ActivateEquipment(EquipmentSlot slot)
        {
            if (slot && HasThisEquipment(slot.characterBody))
            {
                EquipmentState state = new()
                {
                    equipmentIndex = ArmletOfMordiggian.Instance.EquipmentDef.equipmentIndex,
                    chargeFinishTime = Run.FixedTimeStamp.now + ArmletCooldown,
                    charges = slot.inventory.GetEquipment(slot.activeEquipmentSlot).charges,
                    equipmentDef = ArmletOfMordiggian.Instance.EquipmentDef
                };
                slot.inventory.SetEquipment(state, slot.activeEquipmentSlot);
            }

            return true;
        }

        private void OnEquipmentGained(On.RoR2.CharacterBody.orig_OnEquipmentGained orig, CharacterBody self, EquipmentDef equipmentDef)
        {
            if (equipmentDef == EquipmentDef)
            {
                self.AddBuff(ArmletOfMordiggianBuff.Instance.BuffDef);
                self.AddBuff(RoR2Content.Buffs.HealingDisabled);
                self.gameObject.AddComponent<ArmletOfMordiggianBehavior>();
                Util.PlaySound("ArmletOfMordiggianOn", self.gameObject);
            }

            orig(self, equipmentDef);
        }
        private void OnEquipmentLost(On.RoR2.CharacterBody.orig_OnEquipmentLost orig, CharacterBody self, EquipmentDef equipmentDef)
        {
            if (equipmentDef == EquipmentDef)
            {
                self.RemoveBuff(ArmletOfMordiggianBuff.Instance.BuffDef);
                self.RemoveBuff(RoR2Content.Buffs.HealingDisabled);
                ArmletOfMordiggianBehavior.Destroy(self.GetComponent<ArmletOfMordiggianBehavior>());
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

            void Update()
            {
                elapsed += Time.deltaTime;
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
                        damage = health.fullCombinedHealth * ArmletOfMordiggianToggled.Instance.MaximumHealthLostPerSecond / 100.0f * tick,
                        procCoefficient = 0.0f,
                        damageType = combo,
                        damageColorIndex = DamageColorIndex.Bleed,
                        inflictor = this.gameObject,
                        position = health.body.transform.position
                    };
                    health.TakeDamage(info);
                }
            }
        }
    }
}