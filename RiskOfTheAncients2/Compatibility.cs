using System.Collections.Generic;
using System.Runtime.CompilerServices;
using LookingGlass.ItemStatsNameSpace;
using ROTA2.Items;
using UnityEngine;

// stole it from looking glass because theirs is private...
class Utils
{
    public static float CalculateChanceWithLuck(float baseChance, float luckIn)
    {
        float chanceFloored = Mathf.Floor(baseChance);
        float chanceMod = baseChance % 1f;
        int luck = Mathf.CeilToInt(luckIn);
        if (luck > 0)
            return chanceFloored + (1f - Mathf.Pow(1f - chanceMod, luck + 1));
        if (luck < 0)
            return chanceFloored + Mathf.Pow(chanceMod, Mathf.Abs(luck) + 1);

        return baseChance;
    }

    public static float GetExponentialRechargeTime(float baseCooldown, float extraPercent, int count)
    {
        return baseCooldown * Mathf.Pow(1 - extraPercent, count - 1);
    }

    public static float GetExponentialStacking(float basePercent, float extraPercent, int count)
    {
        return 1 - (1 - basePercent) * Mathf.Pow(1 - extraPercent, count - 1);
    }

    public static float GetHyperbolicStacking(float basePercent, float extraPercent, int count)
    {
        return 1f - 1f / (1f + basePercent + extraPercent * (count - 1));
    }
}

namespace ROTA2
{
    public static class Compatibility
    {
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static void LookingGlassCompatibility()
        {
            // Boots of Speed
            ItemStatsDef item = new();
            item.descriptions.Add("Base Movement Speed: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
            item.calculateValuesNew = (luck, count, proc) =>
            {
                List<float> values = 
                [
                    BootsOfSpeed.Instance.MovementSpeedBase + BootsOfSpeed.Instance.MovementSpeedPerStack * (count - 1)
                ];
                return values;
            };
            ItemDefinitions.RegisterItemStatsDef(item, BootsOfSpeed.Instance.ItemDef.itemIndex);

            // Enchanted Mango
            item = new();
            item.descriptions.Add("Mangos: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
            item.calculateValuesNew = (luck, count, proc) =>
            {
                List<float> values = 
                [
                    count
                ];
                return values;
            };
            ItemDefinitions.RegisterItemStatsDef(item, EnchantedMango.Instance.ItemDef.itemIndex);

            // Fairy's Trinket
            item = new();
            item.descriptions.Add("Cooldown Reduction: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            item.descriptions.Add("Damage: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            item.descriptions.Add("Bonus Health: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Health);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.FlatHealth);
            item.calculateValuesNew = (luck, count, proc) =>
            {
                List<float> values =
                [
                    Utils.GetExponentialStacking(FairysTrinket.Instance.SkillCooldownReductionBase / 100.0f, FairysTrinket.Instance.SkillCooldownReductionPerStack / 100.0f, count),
                    FairysTrinket.Instance.DamageBase / 100.0f + FairysTrinket.Instance.DamagePerStack / 100.0f * (count - 1),
                    FairysTrinket.Instance.MaximumHealthBase + FairysTrinket.Instance.MaximumHealthPerStack * (count - 1)
                ];
                return values;
            };
            ItemDefinitions.RegisterItemStatsDef(item, FairysTrinket.Instance.ItemDef.itemIndex);

            // Healing Salve
            item = new();
            item.descriptions.Add("Total Heal: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Healing);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.PercentHealth);
            item.calculateValuesNew = (luck, count, proc) =>
            {
                List<float> values = 
                [
                    (HealingSalve.Instance.MaximumHealthRegenerationBase / 100.0f + HealingSalve.Instance.MaximumHealthRegenerationPerStack / 100.0f * (count - 1)) * HealingSalve.Instance.BuffDuration
                ];
                return values;
            };
            ItemDefinitions.RegisterItemStatsDef(item, HealingSalve.Instance.ItemDef.itemIndex);
            
            // Javelin
            item = new();
            item.descriptions.Add("Bonus Hit Chance: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            item.descriptions.Add("Damage: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            item.hasChance = true;
            item.chanceScaling = ItemStatsDef.ChanceScaling.DoesNotScale;
            item.calculateValuesNew = (luck, count, proc) =>
            {
                List<float> values = 
                [
                    Utils.CalculateChanceWithLuck(Javelin.Instance.ProcChance / 100.0f, luck),
                    Javelin.Instance.DamageBase / 100.0f + Javelin.Instance.DamagePerStack / 100.0f * (count - 1)
                ];
                return values;
            };
            ItemDefinitions.RegisterItemStatsDef(item, Javelin.Instance.ItemDef.itemIndex);

            // Orb of Blight
            item = new();
            item.descriptions.Add("Max Armor Reduction: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
            item.calculateValuesNew = (luck, count, proc) =>
            {
                List<float> values =
                [
                    OrbOfBlight.Instance.ArmorReduction * (OrbOfBlight.Instance.MaxStacksBase + OrbOfBlight.Instance.MaxStacksPerStack * (count - 1))
                ];
                return values;
            };
            ItemDefinitions.RegisterItemStatsDef(item, OrbOfBlight.Instance.ItemDef.itemIndex);
            
            // Orb of Frost
            item = new();
            item.descriptions.Add("Movement Speed Slow: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            item.descriptions.Add("Attack Speed Slow: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            item.calculateValuesNew = (luck, count, proc) =>
            {
                List<float> values =
                [
                    OrbOfFrost.Instance.MovementSpeedSlowBase / 100.0f + OrbOfFrost.Instance.MovementSpeedSlowPerStack / 100.0f * (count - 1),
                    OrbOfFrost.Instance.AttackSpeedSlowBase / 100.0f + OrbOfFrost.Instance.AttackSpeedSlowPerStack / 100.0f * (count - 1),
                ];
                return values;
            };
            ItemDefinitions.RegisterItemStatsDef(item, OrbOfFrost.Instance.ItemDef.itemIndex);
            
            // Orb of Venom
            item = new();
            item.descriptions.Add("Total Poison Damage: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            item.calculateValuesNew = (luck, count, proc) =>
            {
                List<float> values =
                [
                    (OrbOfVenom.Instance.PoisonDamageBase / 100.0f + OrbOfVenom.Instance.PoisonDamagePerStack / 100.0f * (count - 1)) * OrbOfVenom.Instance.PoisonDuration
                ];
                return values;
            };
            ItemDefinitions.RegisterItemStatsDef(item, OrbOfVenom.Instance.ItemDef.itemIndex);

            // Quelling Blade
            item = new();
            item.descriptions.Add("Non-Boss Damage: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
            item.calculateValuesNew = (luck, count, proc) =>
            {
                List<float> values =
                [
                    QuellingBlade.Instance.DamageBase + QuellingBlade.Instance.DamagePerStack * (count - 1)
                ];
                return values;
            };
            ItemDefinitions.RegisterItemStatsDef(item, QuellingBlade.Instance.ItemDef.itemIndex);

            // Spark of Courage
            item = new();
            item.descriptions.Add("Damage: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            item.descriptions.Add("Armor: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
            item.calculateValuesNew = (luck, count, proc) =>
            {
                List<float> values =
                [
                    SparkOfCourage.Instance.DamageBase / 100.0f + SparkOfCourage.Instance.DamagePerStack / 100.0f * (count - 1),
                    SparkOfCourage.Instance.ArmorBase + SparkOfCourage.Instance.ArmorPerStack * (count - 1)
                ];
                return values;
            };
            ItemDefinitions.RegisterItemStatsDef(item, SparkOfCourage.Instance.ItemDef.itemIndex);

            // Dragon Scale
            item = new();
            item.descriptions.Add("Burn Chance: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            item.descriptions.Add("Total Damage: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            item.hasChance = true;
            item.chanceScaling = ItemStatsDef.ChanceScaling.DoesNotScale;
            item.calculateValuesNew = (luck, count, proc) =>
            {
                List<float> values =
                [
                    Utils.CalculateChanceWithLuck(DragonScale.Instance.ProcChance / 100.0f, luck),
                    (DragonScale.Instance.DamageBase / 100.0f + DragonScale.Instance.DamagePerStack / 100.0f * (count - 1)) * DragonScale.Instance.BurnDuration
                ];
                return values;
            };
            ItemDefinitions.RegisterItemStatsDef(item, DragonScale.Instance.ItemDef.itemIndex);

            // Lance of Pursuit
            item = new();
            item.descriptions.Add("Rear Damage: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            item.calculateValuesNew = (luck, count, proc) =>
            {
                List<float> values =
                [
                    LanceOfPursuit.Instance.DamageBase / 100.0f + LanceOfPursuit.Instance.DamagePerStack / 100.0f * (count - 1)
                ];
                return values;
            };
            ItemDefinitions.RegisterItemStatsDef(item, LanceOfPursuit.Instance.ItemDef.itemIndex);

            // Kaya
            item = new();
            item.descriptions.Add("Cooldown Reduction: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            item.descriptions.Add("Damage: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            item.calculateValuesNew = (luck, count, proc) =>
            {
                List<float> values =
                [
                    Utils.GetExponentialStacking(Kaya.Instance.SkillCooldownReductionBase / 100.0f, Kaya.Instance.SkillCooldownReductionPerStack / 100.0f, count),
                    Kaya.Instance.DamageBase / 100.0f + Kaya.Instance.DamagePerStack / 100.0f * (count - 1)
                ];
                return values;
            };
            ItemDefinitions.RegisterItemStatsDef(item, Kaya.Instance.ItemDef.itemIndex);

            // Sange
            item = new();
            item.descriptions.Add("Bonus Health: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Health);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.FlatHealth);
            item.descriptions.Add("Healing: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Healing);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.FlatHealing);
            item.calculateValuesNew = (luck, count, proc) =>
            {
                List<float> values =
                [
                    Sange.Instance.MaximumHealthBase + Sange.Instance.MaximumHealthPerStack * (count - 1),
                    Sange.Instance.BaseHealthRegenerationBase + Sange.Instance.BaseHealthRegenerationPerStack * (count - 1)
                ];
                return values;
            };
            ItemDefinitions.RegisterItemStatsDef(item, Sange.Instance.ItemDef.itemIndex);

            // Yasha
            item = new();
            item.descriptions.Add("Attack Speed: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            item.descriptions.Add("Movement Speed: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            item.calculateValuesNew = (luck, count, proc) =>
            {
                List<float> values =
                [
                    Yasha.Instance.AttackSpeedBase / 100.0f + Yasha.Instance.AttackSpeedPerStack / 100.0f * (count - 1),
                    Yasha.Instance.MovementSpeedBase / 100.0f + Yasha.Instance.MovementSpeedPerStack / 100.0f * (count - 1)
                ];
                return values;
            };
            ItemDefinitions.RegisterItemStatsDef(item, Yasha.Instance.ItemDef.itemIndex);

            // Skull Basher
            item = new();
            item.descriptions.Add("Bash Chance: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            item.descriptions.Add("Damage: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            item.hasChance = true;
            item.chanceScaling = ItemStatsDef.ChanceScaling.DoesNotScale;
            item.calculateValuesNew = (luck, count, proc) =>
            {
                List<float> values =
                [
                    Utils.CalculateChanceWithLuck(SkullBasher.Instance.ProcChance / 100.0f, luck),
                    SkullBasher.Instance.DamageBase / 100.0f + SkullBasher.Instance.DamagePerStack / 100.0f * (count - 1)
                ];
                return values;
            };
            ItemDefinitions.RegisterItemStatsDef(item, SkullBasher.Instance.ItemDef.itemIndex);

            // Daedalus
            item = new();
            item.descriptions.Add("Crit Chance: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            item.descriptions.Add("Crit Damage: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            item.calculateValuesNew = (luck, count, proc) =>
            {
                List<float> values =
                [
                    Utils.CalculateChanceWithLuck(Daedalus.Instance.CriticalChance / 100.0f, luck),
                    Daedalus.Instance.CriticalDamageBase / 100.0f + Daedalus.Instance.CriticalDamagePerStack / 100.0f * (count - 1)
                ];
                return values;
            };
            ItemDefinitions.RegisterItemStatsDef(item, Daedalus.Instance.ItemDef.itemIndex);

            // QuicksilverAmulet
            item = new();
            item.descriptions.Add("Max Attack Speed: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            item.descriptions.Add("Max Movement Speed: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            item.calculateValues = (master, count) =>
            {
                int cooldowns = 0;
                var skills = master.GetBody()?.skillLocator?.allSkills;
                if (skills != null)
                {
                    foreach (var skill in skills)
                    {
                        // only count skills that can actually have a cooldown.
                        // e.g. Commando's primary *wouldn't* count.
                        if (skill && skill.baseRechargeInterval > 0.0f)
                        {
                            ++cooldowns;
                        }
                    }
                }
                List<float> values =
                [
                    (QuicksilverAmulet.Instance.AttackSpeedBase / 100.0f + QuicksilverAmulet.Instance.AttackSpeedPerStack / 100.0f * (count - 1)) * cooldowns,
                    (QuicksilverAmulet.Instance.MovementSpeedBase / 100.0f + QuicksilverAmulet.Instance.MovementSpeedPerStack / 100.0f * (count - 1)) * cooldowns
                ];
                return values;
            };
            ItemDefinitions.RegisterItemStatsDef(item, QuicksilverAmulet.Instance.ItemDef.itemIndex);

            // Heart of Tarrasque
            item = new();
            item.descriptions.Add("Bonus Health: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Health);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.FlatHealth);
            item.calculateValuesNew = (luck, count, proc) =>
            {
                List<float> values =
                [
                    HeartOfTarrasque.Instance.MaximumHealthBase + HeartOfTarrasque.Instance.MaximumHealthPerStack * (count - 1)
                ];
                return values;
            };
            ItemDefinitions.RegisterItemStatsDef(item, HeartOfTarrasque.Instance.ItemDef.itemIndex);

            // Radiance
            item = new();
            item.descriptions.Add("Radius: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Meters);
            item.descriptions.Add("Ignite Damage: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            item.descriptions.Add("Burn Total Damage: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            item.calculateValuesNew = (luck, count, proc) =>
            {
                List<float> values =
                [
                    Radiance.Instance.RadiusBase + Radiance.Instance.RadiusPerStack * (count - 1),
                    Radiance.Instance.IgniteBase / 100.0f + Radiance.Instance.IgnitePerStack / 100.0f * (count - 1),
                    (Radiance.Instance.BurnBase / 100.0f + Radiance.Instance.BurnPerStack / 100.0f * (count - 1)) * Radiance.Instance.BurnDuration
                ];
                return values;
            };
            ItemDefinitions.RegisterItemStatsDef(item, Radiance.Instance.ItemDef.itemIndex);

            // Aeon Disk
            item = new();
            item.descriptions.Add("Invulnerability Duration: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Seconds);
            item.descriptions.Add("Movement Speed Duration: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Seconds);
            item.calculateValuesNew = (luck, count, proc) =>
            {
                List<float> values =
                [
                    AeonDisk.Instance.InvulnerabilityDurationBase + AeonDisk.Instance.InvulnerabilityDurationPerStack * (count - 1),
                    AeonDisk.Instance.MovementSpeedDurationBase + AeonDisk.Instance.MovementSpeedDurationPerStack * (count - 1)
                ];
                return values;
            };
            ItemDefinitions.RegisterItemStatsDef(item, AeonDisk.Instance.ItemDef.itemIndex);

            // Assault Cuirass
            item = new();
            item.descriptions.Add("Attack Speed: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            item.descriptions.Add("Armor: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
            item.calculateValuesNew = (luck, count, proc) =>
            {
                List<float> values =
                [
                    AssaultCuirass.Instance.AttackSpeedBase / 100.0f + AssaultCuirass.Instance.AttackSpeedPerStack / 100.0f * (count - 1),
                    AssaultCuirass.Instance.ArmorBase + AssaultCuirass.Instance.ArmorPerStack * (count - 1)
                ];
                return values;
            };
            ItemDefinitions.RegisterItemStatsDef(item, AssaultCuirass.Instance.ItemDef.itemIndex);

            // Nemesis Curse
            item = new();
            item.descriptions.Add("Curse Damage Multiplier: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
            item.calculateValuesNew = (luck, count, proc) =>
            {
                List<float> values =
                [
                    (1.0f + NemesisCurse.Instance.DamageBase / 100.0f) * Mathf.Pow(1.0f + NemesisCurse.Instance.DamagePerStack / 100.0f, count - 1)
                ];
                return values;
            };
            ItemDefinitions.RegisterItemStatsDef(item, NemesisCurse.Instance.ItemDef.itemIndex);

            // Orb of Corrosion
            item = new();
            item.descriptions.Add("Max Armor Reduction: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
            item.descriptions.Add("Movement Speed Slow: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            item.descriptions.Add("Attack Speed Slow: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            item.descriptions.Add("Total Poison Damage: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            item.calculateValuesNew = (luck, count, proc) =>
            {
                List<float> values =
                [
                    OrbOfCorrosion.Instance.ArmorReduction * (OrbOfCorrosion.Instance.MaxStacksBase + OrbOfCorrosion.Instance.MaxStacksPerStack * (count - 1)),
                    OrbOfCorrosion.Instance.MovementSpeedSlowBase / 100.0f + OrbOfCorrosion.Instance.MovementSpeedSlowPerStack / 100.0f * (count - 1),
                    OrbOfCorrosion.Instance.AttackSpeedSlowBase / 100.0f + OrbOfCorrosion.Instance.AttackSpeedSlowPerStack / 100.0f * (count - 1),
                    (OrbOfCorrosion.Instance.PoisonDamageBase / 100.0f + OrbOfCorrosion.Instance.PoisonDamagePerStack / 100.0f * (count - 1)) * OrbOfCorrosion.Instance.PoisonDuration
                ];
                return values;
            };
            ItemDefinitions.RegisterItemStatsDef(item, OrbOfCorrosion.Instance.ItemDef.itemIndex);

            // Kaya and Sange
            item = new();
            item.descriptions.Add("Cooldown Reduction: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            item.descriptions.Add("Damage: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            item.descriptions.Add("Bonus Health: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Health);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.FlatHealth);
            item.descriptions.Add("Healing: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Healing);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.FlatHealing);
            item.calculateValuesNew = (luck, count, proc) =>
            {
                List<float> values =
                [
                    Utils.GetExponentialStacking(KayaAndSange.Instance.SkillCooldownReductionBase / 100.0f, KayaAndSange.Instance.SkillCooldownReductionPerStack / 100.0f, count),
                    KayaAndSange.Instance.DamageBase / 100.0f + KayaAndSange.Instance.DamagePerStack / 100.0f * (count - 1),
                    KayaAndSange.Instance.MaximumHealthBase + KayaAndSange.Instance.MaximumHealthPerStack * (count - 1),
                    KayaAndSange.Instance.BaseHealthRegenerationBase + KayaAndSange.Instance.BaseHealthRegenerationPerStack * (count - 1)
                ];
                return values;
            };
            ItemDefinitions.RegisterItemStatsDef(item, KayaAndSange.Instance.ItemDef.itemIndex);

            // Sange and Yasha
            item = new();
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            item.descriptions.Add("Bonus Health: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Health);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.FlatHealth);
            item.descriptions.Add("Healing: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Healing);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.FlatHealing);
            item.descriptions.Add("Attack Speed: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            item.descriptions.Add("Movement Speed: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            item.calculateValuesNew = (luck, count, proc) =>
            {
                List<float> values =
                [
                    SangeAndYasha.Instance.MaximumHealthBase + SangeAndYasha.Instance.MaximumHealthPerStack * (count - 1),
                    SangeAndYasha.Instance.BaseHealthRegenerationBase + SangeAndYasha.Instance.BaseHealthRegenerationPerStack * (count - 1),
                    SangeAndYasha.Instance.AttackSpeedBase / 100.0f + SangeAndYasha.Instance.AttackSpeedPerStack / 100.0f * (count - 1),
                    SangeAndYasha.Instance.MovementSpeedBase / 100.0f + SangeAndYasha.Instance.MovementSpeedPerStack / 100.0f * (count - 1)
                ];
                return values;
            };
            ItemDefinitions.RegisterItemStatsDef(item, SangeAndYasha.Instance.ItemDef.itemIndex);

            // Yasha and Kaya
            item = new();
            item.descriptions.Add("Cooldown Reduction: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            item.descriptions.Add("Attack Speed: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            item.descriptions.Add("Movement Speed: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            item.descriptions.Add("Damage: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            item.calculateValuesNew = (luck, count, proc) =>
            {
                List<float> values =
                [
                    Utils.GetExponentialStacking(YashaAndKaya.Instance.SkillCooldownReductionBase / 100.0f, YashaAndKaya.Instance.SkillCooldownReductionPerStack / 100.0f, count),
                    YashaAndKaya.Instance.AttackSpeedBase / 100.0f + YashaAndKaya.Instance.AttackSpeedPerStack / 100.0f * (count - 1),
                    YashaAndKaya.Instance.MovementSpeedBase / 100.0f + YashaAndKaya.Instance.MovementSpeedPerStack / 100.0f * (count - 1),
                    YashaAndKaya.Instance.DamageBase / 100.0f + YashaAndKaya.Instance.DamagePerStack / 100.0f * (count - 1)
                ];
                return values;
            };
            ItemDefinitions.RegisterItemStatsDef(item, YashaAndKaya.Instance.ItemDef.itemIndex);

            // Trident
            item = new();
            item.descriptions.Add("Cooldown Reduction: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            item.descriptions.Add("Damage: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            item.descriptions.Add("Bonus Health: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Health);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.FlatHealth);
            item.descriptions.Add("Healing: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Healing);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.FlatHealing);
            item.descriptions.Add("Attack Speed: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Damage);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            item.descriptions.Add("Movement Speed: ");
            item.valueTypes.Add(ItemStatsDef.ValueType.Utility);
            item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
            item.calculateValuesNew = (luck, count, proc) =>
            {
                List<float> values =
                [
                    Utils.GetExponentialStacking(Trident.Instance.SkillCooldownReductionBase / 100.0f, Trident.Instance.SkillCooldownReductionPerStack / 100.0f, count),
                    Trident.Instance.DamageBase / 100.0f + Trident.Instance.DamagePerStack / 100.0f * (count - 1),
                    Trident.Instance.MaximumHealthBase + Trident.Instance.MaximumHealthPerStack * (count - 1),
                    Trident.Instance.BaseHealthRegenerationBase + Trident.Instance.BaseHealthRegenerationPerStack * (count - 1),
                    Trident.Instance.AttackSpeedBase / 100.0f + Trident.Instance.AttackSpeedPerStack / 100.0f * (count - 1),
                    Trident.Instance.MovementSpeedBase / 100.0f + Trident.Instance.MovementSpeedPerStack / 100.0f * (count - 1)
                ];
                return values;
            };
            ItemDefinitions.RegisterItemStatsDef(item, Trident.Instance.ItemDef.itemIndex);
        }
    }
}