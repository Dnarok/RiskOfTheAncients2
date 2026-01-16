using LookingGlass.ItemStatsNameSpace;
using RoR2;
using ROTA2.Equipment;
using ROTA2.Items;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ROTA2
{
    public static class Compatibility
    {
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static void LookingGlassCompatibility()
        {
            Log.Debug("Adding LookingGlass stat display definitions.");

            Func<float, string> utility = x => LookingGlass.StatsDisplay.StatsDisplayClass.builtInColors.Value ? "<style=\"cIsUtility\">" + x.ToString(LookingGlass.StatsDisplay.StatsDisplayDefinitions.floatPrecision) + "</style" : x.ToString(LookingGlass.StatsDisplay.StatsDisplayDefinitions.floatPrecision);
            LookingGlass.StatsDisplay.StatsDisplayClass.statDictionary.Add("magicResistance", cachedUserBody =>
            {
                var component = cachedUserBody.GetComponent<StatsAPI.CustomStats>();
                if (component)
                {
                    return utility((1f - component.MagicResistanceMultiplier) * 100f) + "%";
                }
                else
                {
                    return utility(0f) + "%";
                }
            });

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
                        BootsOfSpeed.Instance.MovementSpeedBase.Value + BootsOfSpeed.Instance.MovementSpeedPerStack.Value * (count - 1)
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, BootsOfSpeed.GetItemDef().itemIndex);
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
                ItemDefinitions.RegisterItemStatsDef(item, EnchantedMango.GetItemDef().itemIndex);
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
                        ConsumedMango.Instance.DamageBase.Value / 100f + ConsumedMango.Instance.DamagePerStack.Value / 100f * (count - 1)
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, ConsumedMango.GetItemDef().itemIndex);
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
                        LookingGlass.Utils.GetExponentialStacking(FairysTrinket.Instance.SkillCooldownReductionBase.Value / 100f, FairysTrinket.Instance.SkillCooldownReductionPerStack.Value / 100f, count),
                        FairysTrinket.Instance.DamageBase.Value / 100f + FairysTrinket.Instance.DamagePerStack.Value / 100f * (count - 1),
                        FairysTrinket.Instance.MaximumHealthBase.Value + FairysTrinket.Instance.MaximumHealthPerStack.Value * (count - 1)
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, FairysTrinket.GetItemDef().itemIndex);
            }

            // Healing Salve
            if (Plugin.ItemsEnabled[HealingSalve.Instance])
            {
                item = new();
                item.descriptions.Add("Total Heal: ");
                item.valueTypes.Add(ItemStatsDef.ValueType.Healing);
                item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.FlatHealth);
                item.calculateValues = (master, count) =>
                {
                    List<float> values =
                    [
                        (HealingSalve.Instance.RegenerationBase.Value + HealingSalve.Instance.RegenerationPerStack.Value * (count - 1)) * (1 + 0.2f * master.GetBody().level) * HealingSalve.Instance.BuffDuration.Value
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, HealingSalve.GetItemDef().itemIndex);
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
                        LookingGlass.Utils.CalculateChanceWithLuck(Javelin.Instance.ProcChance.Value / 100f, luck),
                        Javelin.Instance.DamageBase.Value / 100f + Javelin.Instance.DamagePerStack.Value / 100f * (count - 1)
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, Javelin.GetItemDef().itemIndex);
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
                        OrbOfBlight.Instance.ArmorReduction.Value * (OrbOfBlight.Instance.MaxStacksBase.Value + OrbOfBlight.Instance.MaxStacksPerStack.Value * (count - 1))
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, OrbOfBlight.GetItemDef().itemIndex);
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
                        OrbOfFrost.Instance.MovementSpeedSlowBase.Value / 100f + OrbOfFrost.Instance.MovementSpeedSlowPerStack.Value / 100f * (count - 1),
                        OrbOfFrost.Instance.AttackSpeedSlowBase.Value / 100f + OrbOfFrost.Instance.AttackSpeedSlowPerStack.Value / 100f * (count - 1),
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, OrbOfFrost.GetItemDef().itemIndex);
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
                        (OrbOfVenom.Instance.PoisonDamageBase.Value / 100f + OrbOfVenom.Instance.PoisonDamagePerStack.Value / 100f * (count - 1)) * OrbOfVenom.Instance.PoisonDuration.Value
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, OrbOfVenom.GetItemDef().itemIndex);
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
                        QuellingBlade.Instance.DamageBase.Value + QuellingBlade.Instance.DamagePerStack.Value * (count - 1)
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, QuellingBlade.GetItemDef().itemIndex);
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
                        SparkOfCourage.Instance.DamageBase.Value / 100f + SparkOfCourage.Instance.DamagePerStack.Value / 100f * (count - 1),
                        SparkOfCourage.Instance.ArmorBase.Value + SparkOfCourage.Instance.ArmorPerStack.Value * (count - 1)
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, SparkOfCourage.GetItemDef().itemIndex);
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
                        LookingGlass.Utils.CalculateChanceWithLuck(DragonScale.Instance.ProcChance.Value / 100f, luck),
                        (DragonScale.Instance.DamageBase.Value / 100f + DragonScale.Instance.DamagePerStack.Value / 100f * (count - 1))
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, DragonScale.GetItemDef().itemIndex);
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
                        LanceOfPursuit.Instance.DamageBase.Value / 100f + LanceOfPursuit.Instance.DamagePerStack.Value / 100f * (count - 1)
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, LanceOfPursuit.GetItemDef().itemIndex);
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
                        IronBranch.Instance.StatIncreaseBase.Value / 100f + IronBranch.Instance.StatIncreasePerStack.Value / 100f * (count - 1)
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, IronBranch.GetItemDef().itemIndex);
            }

            // Blades of Attack
            if (Plugin.ItemsEnabled[BladesOfAttack.Instance])
            {
                item = new();
                item.descriptions.Add("Damage: ");
                item.valueTypes.Add(ItemStatsDef.ValueType.Damage);
                item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
                item.calculateValuesNew = (luck, count, proc) =>
                {
                    List<float> values =
                    [
                        BladesOfAttack.Instance.DamageBase.Value / 100f + BladesOfAttack.Instance.DamagePerStack.Value / 100f * (count - 1)
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, BladesOfAttack.GetItemDef().itemIndex);
            }

            // Cloak
            if (Plugin.ItemsEnabled[Cloak.Instance])
            {
                item = new();
                item.descriptions.Add("Magic Resistance: ");
                item.valueTypes.Add(ItemStatsDef.ValueType.Utility);
                item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
                item.calculateValuesNew = (luck, count, proc) =>
                {
                    List<float> values =
                    [
                        1f - ((1f - Cloak.Instance.MagicResistanceBase.Value / 100f) * Mathf.Pow(1f - Cloak.Instance.MagicResistancePerStack.Value / 100f, count - 1)),
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, Cloak.GetItemDef().itemIndex);
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
                        LookingGlass.Utils.GetExponentialStacking(Kaya.Instance.SkillCooldownReductionBase.Value / 100f, Kaya.Instance.SkillCooldownReductionPerStack.Value / 100f, count),
                        Kaya.Instance.DamageBase.Value / 100f + Kaya.Instance.DamagePerStack.Value / 100f * (count - 1)
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, Kaya.GetItemDef().itemIndex);
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
                        Sange.Instance.MaximumHealthBase.Value + Sange.Instance.MaximumHealthPerStack.Value * (count - 1),
                        Sange.Instance.BaseHealthRegenerationBase.Value + Sange.Instance.BaseHealthRegenerationPerStack.Value * (count - 1)
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, Sange.GetItemDef().itemIndex);
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
                        Yasha.Instance.AttackSpeedBase.Value / 100f + Yasha.Instance.AttackSpeedPerStack.Value / 100f * (count - 1),
                        Yasha.Instance.MovementSpeedBase.Value / 100f + Yasha.Instance.MovementSpeedPerStack.Value / 100f * (count - 1)
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, Yasha.GetItemDef().itemIndex);
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
                        LookingGlass.Utils.CalculateChanceWithLuck(SkullBasher.Instance.ProcChance.Value / 100f, luck),
                        SkullBasher.Instance.DamageBase.Value / 100f + SkullBasher.Instance.DamagePerStack.Value / 100f * (count - 1)
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, SkullBasher.GetItemDef().itemIndex);
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
                        LookingGlass.Utils.CalculateChanceWithLuck(Daedalus.Instance.CriticalChance.Value / 100f, luck),
                        Daedalus.Instance.CriticalDamageBase.Value / 100f + Daedalus.Instance.CriticalDamagePerStack.Value / 100f * (count - 1)
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, Daedalus.GetItemDef().itemIndex);
            }

            // Quicksilver Amulet
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
                            if (skill && skill.baseRechargeInterval > 0f)
                            {
                                ++cooldowns;
                            }
                        }
                    }
                    List<float> values =
                    [
                        (QuicksilverAmulet.Instance.AttackSpeedBase.Value / 100f + QuicksilverAmulet.Instance.AttackSpeedPerStack.Value / 100f * (count - 1)) * cooldowns,
                        (QuicksilverAmulet.Instance.MovementSpeedBase.Value / 100f + QuicksilverAmulet.Instance.MovementSpeedPerStack.Value / 100f * (count - 1)) * cooldowns
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, QuicksilverAmulet.GetItemDef().itemIndex);
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
                        ShadowAmulet.Instance.AttackSpeedBase.Value / 100f + ShadowAmulet.Instance.AttackSpeedPerStack.Value / 100f * (count - 1)
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, ShadowAmulet.GetItemDef().itemIndex);
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
                        RoR2.Util.ConvertAmplificationPercentageIntoReductionNormalized(IronTalon.Instance.HealthDamageBase.Value / 100f + IronTalon.Instance.HealthDamagePerStack.Value / 100f * (count - 1))
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, IronTalon.GetItemDef().itemIndex);
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
                        HeartOfTarrasque.Instance.MaximumHealthBase.Value + HeartOfTarrasque.Instance.MaximumHealthPerStack.Value * (count - 1)
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, HeartOfTarrasque.GetItemDef().itemIndex);
            }

            // Radiance
            if (Plugin.ItemsEnabled[Radiance.Instance])
            {
                item = new();
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
                        Radiance.Instance.IgniteBase.Value / 100f + Radiance.Instance.IgnitePerStack.Value / 100f * (count - 1),
                        (Radiance.Instance.BurnBase.Value / 100f + Radiance.Instance.BurnPerStack.Value / 100f * (count - 1))
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, Radiance.GetItemDef().itemIndex);
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
                        AeonDisk.Instance.InvulnerabilityDuration.Value,
                        AeonDisk.Instance.MovementSpeedDuration.Value
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, AeonDisk.GetItemDef().itemIndex);
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
                        AssaultCuirass.Instance.AttackSpeedBase.Value / 100f + AssaultCuirass.Instance.AttackSpeedPerStack.Value / 100f * (count - 1),
                        AssaultCuirass.Instance.ArmorBase.Value + AssaultCuirass.Instance.ArmorPerStack.Value * (count - 1)
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, AssaultCuirass.GetItemDef().itemIndex);
            }

            // Boots of Travel
            if (Plugin.ItemsEnabled[BootsOfTravel.Instance])
            {
                item = new();
                item.descriptions.Add("Damage: ");
                item.valueTypes.Add(ItemStatsDef.ValueType.Damage);
                item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
                item.calculateValues = (master, count) =>
                {
                    CharacterBody body = master.GetBody();
                    List<float> values =
                    [
                        1 - 1 / (body.moveSpeed / (body.baseMoveSpeed * (1f + BootsOfTravel.Instance.MovementSpeedBonus.Value / 100f))) * (BootsOfTravel.Instance.DamageBonusBase.Value / 100f + BootsOfTravel.Instance.DamageBonusPerStack.Value / 100f * (count - 1))
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, BootsOfTravel.GetItemDef().itemIndex);
            }

            // Ex Machina
            if (Plugin.ItemsEnabled[ExMachina.Instance])
            {
                item = new();
                item.descriptions.Add("Chance: ");
                item.valueTypes.Add(ItemStatsDef.ValueType.Utility);
                item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
                item.calculateValuesNew = (luck, count, proc) =>
                {
                    float chance = 1f - (1f / (1f + 1.5f * (ExMachina.Instance.RestoreChanceBase.Value / 100f + ExMachina.Instance.RestoreChancePerStack.Value / 100f * (count - 1))));
                    List<float> values =
                    [
                        LookingGlass.Utils.CalculateChanceWithLuck(chance, luck)
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, ExMachina.GetItemDef().itemIndex);
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
                        InfusedRaindrops.Instance.DamageBlockBase.Value + InfusedRaindrops.Instance.DamageBlockPerStack.Value * (count - 1)
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, InfusedRaindrops.GetItemDef().itemIndex);
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
                        LookingGlass.Utils.CalculateChanceWithLuck(PirateHat.Instance.DropChanceBase.Value / 100f + PirateHat.Instance.DropChancePerStack.Value / 100f * (count - 1), luck)
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, PirateHat.GetItemDef().itemIndex);
            }

            // Refresher Orb
            if (Plugin.ItemsEnabled[RefresherOrb.Instance])
            {
                item = new();
                item.descriptions.Add("Restore Cooldown: ");
                item.valueTypes.Add(ItemStatsDef.ValueType.Utility);
                item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Seconds);
                item.calculateValuesNew = (luck, count, proc) =>
                {
                    List<float> values =
                    [
                        RefresherOrb.Instance.RestoreCooldown.Value * MathF.Pow(RefresherOrb.Instance.RestoreCooldownReductionPerStack.Value / 100f, count - 1)
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, RefresherOrb.GetItemDef().itemIndex);
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
                        (1f + NemesisCurse.Instance.DamageBase.Value / 100f) * Mathf.Pow(1f + NemesisCurse.Instance.DamagePerStack.Value / 100f, count - 1)
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, NemesisCurse.GetItemDef().itemIndex);
            }

            // Unwavering Condition
            if (Plugin.ItemsEnabled[UnwaveringCondition.Instance])
            {
                item = new();
                item.descriptions.Add("Magic Resistance: ");
                item.valueTypes.Add(ItemStatsDef.ValueType.Utility);
                item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
                item.descriptions.Add("Maximum Health: ");
                item.valueTypes.Add(ItemStatsDef.ValueType.Health);
                item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Number);
                item.calculateValuesNew = (luck, count, proc) =>
                {
                    float multiplier = 1f - UnwaveringCondition.Instance.MagicResistanceBase.Value / 100f;
                    for (int i = 1; i < count; ++i)
                    {
                        multiplier *= 1f - UnwaveringCondition.Instance.MagicResistancePerStack.Value / 100f;
                    }
                    List<float> values =
                    [
                        1f - ((1f - UnwaveringCondition.Instance.MagicResistanceBase.Value / 100f) * Mathf.Pow(1f - UnwaveringCondition.Instance.MagicResistancePerStack.Value / 100f, count - 1)),
                        UnwaveringCondition.Instance.CombinedHealthLimit.Value * Mathf.Pow(1f - UnwaveringCondition.Instance.LimitReductionPerStack.Value / 100f, count - 1)
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, UnwaveringCondition.GetItemDef().itemIndex);
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
                        OrbOfCorrosion.Instance.ArmorReduction.Value * (OrbOfCorrosion.Instance.MaxStacksBase.Value + OrbOfCorrosion.Instance.MaxStacksPerStack.Value * (count - 1)),
                        OrbOfCorrosion.Instance.MovementSpeedSlowBase.Value / 100f + OrbOfCorrosion.Instance.MovementSpeedSlowPerStack.Value / 100f * (count - 1),
                        OrbOfCorrosion.Instance.AttackSpeedSlowBase.Value / 100f + OrbOfCorrosion.Instance.AttackSpeedSlowPerStack.Value / 100f * (count - 1),
                        (OrbOfCorrosion.Instance.PoisonDamageBase.Value / 100f + OrbOfCorrosion.Instance.PoisonDamagePerStack.Value / 100f * (count - 1)) * OrbOfCorrosion.Instance.PoisonDuration.Value
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, OrbOfCorrosion.GetItemDef().itemIndex);
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
                        TranquilBoots.Instance.MovementSpeedBase.Value + TranquilBoots.Instance.MovementSpeedPerStack.Value * (count - 1),
                        TranquilBoots.Instance.OODHealthRegenerationBase.Value + TranquilBoots.Instance.OODHealthRegenerationPerStack.Value * (count - 1),
                        TranquilBoots.Instance.OODMovementSpeedBase.Value + TranquilBoots.Instance.OODMovementSpeedPerStack.Value * (count - 1)
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, TranquilBoots.GetItemDef().itemIndex);
            }

            // Phase Boots
            if (Plugin.ItemsEnabled[PhaseBoots.Instance])
            {
                item = new();
                item.descriptions.Add("Damage: ");
                item.valueTypes.Add(ItemStatsDef.ValueType.Damage);
                item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
                item.descriptions.Add("Sprint Bonus: ");
                item.valueTypes.Add(ItemStatsDef.ValueType.Utility);
                item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
                item.calculateValuesNew = (luck, count, proc) =>
                {
                    List<float> values =
                    [
                        PhaseBoots.Instance.DamageBase.Value / 100f + PhaseBoots.Instance.DamagePerStack.Value / 100f * (count - 1),
                        PhaseBoots.Instance.SprintSpeedBase.Value / 100f + PhaseBoots.Instance.SprintSpeedPerStack.Value / 100f * (count - 1)
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, PhaseBoots.GetItemDef().itemIndex);
            }

            // Power Treads
            if (Plugin.ItemsEnabled[PowerTreads.Instance])
            {
                item = new();
                item.descriptions.Add("Movement Speed: ");
                item.valueTypes.Add(ItemStatsDef.ValueType.Utility);
                item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
                item.descriptions.Add("Attack Speed: ");
                item.valueTypes.Add(ItemStatsDef.ValueType.Damage);
                item.measurementUnits.Add(ItemStatsDef.MeasurementUnits.Percentage);
                item.calculateValuesNew = (luck, count, proc) =>
                {
                    List<float> values =
                    [
                        PowerTreads.Instance.MovementSpeedBase.Value / 100f + PowerTreads.Instance.MovementSpeedPerStack.Value / 100f * (count - 1),
                        PowerTreads.Instance.AttackSpeedBase.Value / 100f + PowerTreads.Instance.AttackSpeedPerStack.Value / 100f * (count - 1)
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, PowerTreads.GetItemDef().itemIndex);
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
                        LookingGlass.Utils.GetExponentialStacking(KayaAndSange.Instance.SkillCooldownReductionBase.Value / 100f, KayaAndSange.Instance.SkillCooldownReductionPerStack.Value / 100f, count),
                        KayaAndSange.Instance.DamageBase.Value / 100f + KayaAndSange.Instance.DamagePerStack.Value / 100f * (count - 1),
                        KayaAndSange.Instance.MaximumHealthBase.Value + KayaAndSange.Instance.MaximumHealthPerStack.Value * (count - 1),
                        KayaAndSange.Instance.BaseHealthRegenerationBase.Value + KayaAndSange.Instance.BaseHealthRegenerationPerStack.Value * (count - 1)
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, KayaAndSange.GetItemDef().itemIndex);
            }

            // Sange and Yasha
            if (Plugin.ItemsEnabled[SangeAndYasha.Instance])
            {
                item = new();
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
                        SangeAndYasha.Instance.MaximumHealthBase.Value + SangeAndYasha.Instance.MaximumHealthPerStack.Value * (count - 1),
                        SangeAndYasha.Instance.BaseHealthRegenerationBase.Value + SangeAndYasha.Instance.BaseHealthRegenerationPerStack.Value * (count - 1),
                        SangeAndYasha.Instance.AttackSpeedBase.Value / 100f + SangeAndYasha.Instance.AttackSpeedPerStack.Value / 100f * (count - 1),
                        SangeAndYasha.Instance.MovementSpeedBase.Value / 100f + SangeAndYasha.Instance.MovementSpeedPerStack.Value / 100f * (count - 1)
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, SangeAndYasha.GetItemDef().itemIndex);
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
                        LookingGlass.Utils.GetExponentialStacking(YashaAndKaya.Instance.SkillCooldownReductionBase.Value / 100f, YashaAndKaya.Instance.SkillCooldownReductionPerStack.Value / 100f, count),
                        YashaAndKaya.Instance.AttackSpeedBase.Value / 100f + YashaAndKaya.Instance.AttackSpeedPerStack.Value / 100f * (count - 1),
                        YashaAndKaya.Instance.MovementSpeedBase.Value / 100f + YashaAndKaya.Instance.MovementSpeedPerStack.Value / 100f * (count - 1),
                        YashaAndKaya.Instance.DamageBase.Value / 100f + YashaAndKaya.Instance.DamagePerStack.Value / 100f * (count - 1)
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, YashaAndKaya.GetItemDef().itemIndex);
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
                        LookingGlass.Utils.GetExponentialStacking(Trident.Instance.SkillCooldownReductionBase.Value / 100f, Trident.Instance.SkillCooldownReductionPerStack.Value / 100f, count),
                        Trident.Instance.DamageBase.Value / 100f + Trident.Instance.DamagePerStack.Value / 100f * (count - 1),
                        Trident.Instance.MaximumHealthBase.Value + Trident.Instance.MaximumHealthPerStack.Value * (count - 1),
                        Trident.Instance.BaseHealthRegenerationBase.Value + Trident.Instance.BaseHealthRegenerationPerStack.Value * (count - 1),
                        Trident.Instance.AttackSpeedBase.Value / 100f + Trident.Instance.AttackSpeedPerStack.Value / 100f * (count - 1),
                        Trident.Instance.MovementSpeedBase.Value / 100f + Trident.Instance.MovementSpeedPerStack.Value / 100f * (count - 1)
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, Trident.GetItemDef().itemIndex);
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
                        ConsumedMoonShard.Instance.AttackSpeedBase.Value / 100f + ConsumedMoonShard.Instance.AttackSpeedPerStack.Value / 100f * (count - 1)
                    ];
                    return values;
                };
                ItemDefinitions.RegisterItemStatsDef(item, ConsumedMoonShard.GetItemDef().itemIndex);
            }
        }
    }
}