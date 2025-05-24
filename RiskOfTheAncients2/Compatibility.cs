using System.Collections.Generic;
using System.Runtime.CompilerServices;
using LookingGlass.ItemStatsNameSpace;
using ROTA2.Equipment;
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
            Log.Debug("Adding LookingGlass item definitions.");
            ItemStatsDef item;
            // Boots of Speed
            if (Plugin.ItemsEnabled[BootsOfSpeed.Instance])
            {
                item = new();
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
            }

            // Enchanted Mango
            if (Plugin.ItemsEnabled[EnchantedMango.Instance])
            {
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
            }

            // Consumed Mango
            if (Plugin.ItemsEnabled[ConsumedMango.Instance])
            {
                item = new();
                item.descriptions.Add("Damage: ");
                item.valueTypes.Add(ItemStatsDef.ValueType.Damage);
                item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
                item.calculateValuesNew = (luck, count, proc) =>
                {
                    List<float> values =
                    [
                        ConsumedMango.Instance.DamageBase / 100.0f + ConsumedMango.Instance.DamagePerStack / 100.0f * (count - 1)
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, ConsumedMango.Instance.ItemDef.itemIndex);
            }

            // Fairy's Trinket
            if (Plugin.ItemsEnabled[FairysTrinket.Instance])
            { 
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
            }

            // Healing Salve
            if (Plugin.ItemsEnabled[HealingSalve.Instance])
            {
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
            }
            
            // Javelin
            if (Plugin.ItemsEnabled[Javelin.Instance])
            {
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
            }

            // Orb of Blight
            if (Plugin.ItemsEnabled[OrbOfBlight.Instance])
            {
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
            }

            // Orb of Frost
            if (Plugin.ItemsEnabled[OrbOfFrost.Instance])
            {
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
            }

            // Orb of Venom
            if (Plugin.ItemsEnabled[OrbOfVenom.Instance])
            {
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
            }

            // Quelling Blade
            if (Plugin.ItemsEnabled[QuellingBlade.Instance])
            {
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
            }

            // Spark of Courage
            if (Plugin.ItemsEnabled[SparkOfCourage.Instance])
            {
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
            }

            // Dragon Scale
            if (Plugin.ItemsEnabled[DragonScale.Instance])
            {
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
            }

            // Lance of Pursuit
            if (Plugin.ItemsEnabled[LanceOfPursuit.Instance])
            {
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
            }

            // Iron Branch
            if (Plugin.ItemsEnabled[IronBranch.Instance])
            {
                item = new();
                item.descriptions.Add("Stats Increase: ");
                item.valueTypes.Add(ItemStatsDef.ValueType.Utility);
                item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
                item.calculateValuesNew = (luck, count, proc) =>
                {
                    List<float> values =
                    [
                        IronBranch.Instance.StatIncreaseBase / 100.0f + IronBranch.Instance.StatIncreasePerStack / 100.0f * (count - 1)
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, IronBranch.Instance.ItemDef.itemIndex);
            }

            // Kaya
            if (Plugin.ItemsEnabled[Kaya.Instance])
            {
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
            }

            // Sange
            if (Plugin.ItemsEnabled[Sange.Instance])
            {
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
            }

            // Yasha
            if (Plugin.ItemsEnabled[Yasha.Instance])
            {
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
            }

            // Skull Basher
            if (Plugin.ItemsEnabled[SkullBasher.Instance])
            {
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
            }

            // Daedalus
            if (Plugin.ItemsEnabled[Daedalus.Instance])
            {
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
            }

            // QuicksilverAmulet
            if (Plugin.ItemsEnabled[QuicksilverAmulet.Instance])
            {
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
            }

            // Shadow Amulet
            if (Plugin.ItemsEnabled[ShadowAmulet.Instance])
            {
                item = new();
                item.descriptions.Add("Attack Speed: ");
                item.valueTypes.Add(ItemStatsDef.ValueType.Damage);
                item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
                item.calculateValuesNew = (luck, count, proc) =>
                {
                    List<float> values =
                    [
                        ShadowAmulet.Instance.AttackSpeedBase / 100.0f + ShadowAmulet.Instance.AttackSpeedPerStack / 100.0f * (count - 1)
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, ShadowAmulet.Instance.ItemDef.itemIndex);
            }

            // Iron Talon
            if (Plugin.ItemsEnabled[IronTalon.Instance])
            {
                item = new();
                item.descriptions.Add("Damage: ");
                item.valueTypes.Add(ItemStatsDef.ValueType.Damage);
                item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
                item.calculateValuesNew = (luck, count, proc) =>
                {
                    List<float> values =
                    [
                        RoR2.Util.ConvertAmplificationPercentageIntoReductionNormalized(IronTalon.Instance.HealthDamageBase / 100.0f + IronTalon.Instance.HealthDamagePerStack / 100.0f * (count - 1))
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, IronTalon.Instance.ItemDef.itemIndex);
            }

            // Heart of Tarrasque
            if (Plugin.ItemsEnabled[HeartOfTarrasque.Instance])
            {
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
            }

            // Radiance
            if (Plugin.ItemsEnabled[Radiance.Instance])
            {
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
            }

            // Aeon Disk
            if (Plugin.ItemsEnabled[AeonDisk.Instance])
            {
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
            }

            // Assault Cuirass
            if (Plugin.ItemsEnabled[AssaultCuirass.Instance])
            {
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
            }

            // Infused Raindrops
            if (Plugin.ItemsEnabled[InfusedRaindrops.Instance])
            {
                item = new();
                item.descriptions.Add("Damage Reduction: ");
                item.valueTypes.Add(ItemStatsDef.ValueType.Armor);
                item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
                item.calculateValuesNew = (luck, count, proc) =>
                {
                    List<float> values =
                    [
                        InfusedRaindrops.Instance.DamageBlockBase + InfusedRaindrops.Instance.DamageBlockPerStack * (count - 1)
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, InfusedRaindrops.Instance.ItemDef.itemIndex);
            }

            // Pirate Hat
            if (Plugin.ItemsEnabled[PirateHat.Instance])
            {
                item = new();
                item.descriptions.Add("Scrap Chance: ");
                item.valueTypes.Add(ItemStatsDef.ValueType.Utility);
                item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
                item.hasChance = true;
                item.chanceScaling = ItemStatsDef.ChanceScaling.Linear;
                item.calculateValuesNew = (luck, count, proc) =>
                {
                    List<float> values =
                    [
                        Utils.CalculateChanceWithLuck(PirateHat.Instance.DropChanceBase / 100.0f + PirateHat.Instance.DropChancePerStack / 100.0f * (count - 1), luck)
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, PirateHat.Instance.ItemDef.itemIndex);
            }

            // Nemesis Curse
            if (Plugin.ItemsEnabled[NemesisCurse.Instance])
            {
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
            }

            // Orb of Corrosion
            if (Plugin.ItemsEnabled[OrbOfCorrosion.Instance])
            {
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
            }

            // Tranquil Boots
            if (Plugin.ItemsEnabled[TranquilBoots.Instance])
            {
                item = new();
                item.descriptions.Add("Base Movement Speed: ");
                item.valueTypes.Add(ItemStatsDef.ValueType.Utility);
                item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
                item.descriptions.Add("Safe Healing: ");
                item.valueTypes.Add(ItemStatsDef.ValueType.Healing);
                item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.FlatHealing);
                item.descriptions.Add("Safe Movement Speed: ");
                item.valueTypes.Add(ItemStatsDef.ValueType.Utility);
                item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
                item.calculateValuesNew = (luck, count, proc) =>
                {
                    List<float> values =
                    [
                        TranquilBoots.Instance.MovementSpeedBase + TranquilBoots.Instance.MovementSpeedPerStack * (count - 1),
                        TranquilBoots.Instance.OODHealthRegenerationBase + TranquilBoots.Instance.OODHealthRegenerationPerStack * (count - 1),
                        TranquilBoots.Instance.OODMovementSpeedBase + TranquilBoots.Instance.OODMovementSpeedPerStack * (count - 1)
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, TranquilBoots.Instance.ItemDef.itemIndex);
            }

            // Kaya and Sange
            if (Plugin.ItemsEnabled[KayaAndSange.Instance])
            {
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
            }

            // Sange and Yasha
            if (Plugin.ItemsEnabled[SangeAndYasha.Instance])
            {
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
            }

            // Yasha and Kaya
            if (Plugin.ItemsEnabled[YashaAndKaya.Instance])
            {
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
            }

            // Trident
            if (Plugin.ItemsEnabled[Trident.Instance])
            {
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

            // Consumed Moon Shard
            if (Plugin.ItemsEnabled[ConsumedMoonShard.Instance])
            {
                item = new();
                item.descriptions.Add("Attack Speed: ");
                item.valueTypes.Add(ItemStatsDef.ValueType.Damage);
                item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
                item.calculateValuesNew = (luck, count, proc) =>
                {
                    List<float> values =
                    [
                        ConsumedMoonShard.Instance.AttackSpeedBase / 100.0f + ConsumedMoonShard.Instance.AttackSpeedPerStack / 100.0f * (count - 1)
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, ConsumedMoonShard.Instance.ItemDef.itemIndex);
            }
        }
    }
}