using RoR2;
using ROTA2.Equipment;
using ROTA2.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace ROTA2
{
    public class RecipeManager
    {
        // items -> items recipes define some list of items and their amounts
        // required, and the amount of outputs produced.
        public class ItemRecipe
        {
            // map of [item in : amount required]
            public Dictionary<ItemDef, int> inputs { get; set; }
            // [item out : amount made]
            public KeyValuePair<ItemDef, int> output { get; set; }
        }
        // equipment -> equipment recipes are necessarily simpler, since you can
        // only swap two equipment at a time.
        public class EquipmentRecipe
        {
            public EquipmentDef input_1 { get; set; }
            public EquipmentDef input_2 { get; set; }
            public EquipmentDef output { get; set; }
        }
        // item + equipment -> equipment
        public class MixedRecipe
        {
            public Dictionary<ItemDef, int> items { get; set; }
            public EquipmentDef equipment { get; set; }
            public EquipmentDef output { get; set; }
        }

        public static List<ItemRecipe> item_recipes = [];
        public static List<ItemDef> itemsToWatchFor = [];
        public static List<EquipmentRecipe> equipment_recipes = [];
        public static List<MixedRecipe> mixed_recipes = [];

        public static void Add(MixedRecipe recipe)
        {
            mixed_recipes.Add(recipe);
        }
        public static void Add(ItemDef[] items, EquipmentDef equipment, EquipmentDef output)
        {
            mixed_recipes.Add(new MixedRecipe
            {
                items = items.Distinct().ToDictionary(x => x, y => 1),
                equipment = equipment,
                output = output
            });
        }
        public static void Add(EquipmentRecipe recipe)
        {
            equipment_recipes.Add(recipe);
        }
        public static void Add(EquipmentDef input_1, EquipmentDef input_2, EquipmentDef output)
        {
            equipment_recipes.Add(new EquipmentRecipe
            {
                input_1 = input_1,
                input_2 = input_2,
                output = output
            });
        }
        public static void Add(ItemRecipe recipe)
        {
            item_recipes.Add(recipe);
        }
        public static void Add(ItemDef[] inputs, ItemDef output)
        {
            item_recipes.Add(new ItemRecipe
            {
                inputs = inputs.Distinct().ToDictionary(x => x, y => 1),
                output = KeyValuePair.Create(output, 1)
            });
        }
        
        public static void AddCraftable(UnityEngine.Object first, UnityEngine.Object second, UnityEngine.Object output, string name, int count = 1)
        {
            Log.Debug($"Adding recipe {name}.");
            var craftable = ScriptableObject.CreateInstance<CraftableDef>();
            craftable.name = name;
            var recipe = new Recipe()
            {
                amountToDrop = count,
                ingredients = new[]
                {
                    new RecipeIngredient { pickup = first },
                    new RecipeIngredient { pickup = second }
                }
            };
            craftable.recipes = new[] { recipe };
            craftable.pickup = output;
            Content.craftables.Add(craftable);
        }
        public static void AddBlankCraftable(string name)
        {
            Log.Debug($"Adding blank recipe {name}.");
            var craftable = ScriptableObject.CreateInstance<CraftableDef>();
            craftable.name = name;
            Content.craftables.Add(craftable);
        }
        public static void FinishCraftable(int index, UnityEngine.Object first, UnityEngine.Object second, UnityEngine.Object output, int count = 1)
        {
            if (index >= Content.craftables.Count)
            {
                Log.Error($"Failed to finish recipe at index {index}.");
            }
            else
            {
                Log.Debug($"Finishing recipe at index {index} ({Content.craftables[index].name}).");
                var recipe = new Recipe()
                {
                    amountToDrop = count,
                    ingredients = new[]
                    {
                        new RecipeIngredient { pickup = first },
                        new RecipeIngredient { pickup = second }
                    }
                };
                Content.craftables[index].recipes = new[] { recipe };
                Content.craftables[index].pickup = output;
            }
        }

        public static void InitNew()
        {
            // excluded from this new one due to being a 3-step without in-between items.
            if (Plugin.ItemsEnabled[OrbOfVenom.Instance] &&
                Plugin.ItemsEnabled[OrbOfFrost.Instance] &&
                Plugin.ItemsEnabled[OrbOfBlight.Instance] &&
                Plugin.ItemsEnabled[OrbOfCorrosion.Instance])
            {
                Add([OrbOfVenom.GetItemDef(), OrbOfBlight.GetItemDef(), OrbOfFrost.GetItemDef()], OrbOfCorrosion.GetItemDef());
                itemsToWatchFor.AddRange([
                    OrbOfFrost.GetItemDef(),
                    OrbOfBlight.GetItemDef(),
                    OrbOfVenom.GetItemDef()
                ]);
            }
            // excluded because doing 2 equipments without the boss item is impossible, so just do it the old way.
            if (Plugin.EquipmentEnabled[ArcaneBoots.Instance] &&
                Plugin.EquipmentEnabled[Mekansm.Instance] &&
                Plugin.EquipmentEnabled[GuardianGreaves.Instance])
            {
                Add(ArcaneBoots.GetEquipmentDef(), Mekansm.GetEquipmentDef(), GuardianGreaves.GetEquipmentDef());
            }

            // these hooks are still needed as well, due to the above two old recipes.
            CharacterBody.onBodyInventoryChangedGlobal += OnInventoryChanged;
            On.RoR2.EquipmentDef.AttemptGrant += OnAttemptGrant;

            // ones that work now, ROTA2 items are already initialized.
            // 0
            if (Plugin.ItemsEnabled[Trident.Instance])
            {
                var craftable = ScriptableObject.CreateInstance<CraftableDef>();
                craftable.name = "Trident";
                craftable.pickup = Trident.GetItemDef();
                Recipe[] recipes = new Recipe[3];
                if (Plugin.ItemsEnabled[Kaya.Instance] &&
                    Plugin.ItemsEnabled[SangeAndYasha.Instance])
                {
                    var recipe = new Recipe()
                    {
                        amountToDrop = 1,
                        ingredients = new[]
                        {
                            new RecipeIngredient { pickup = Kaya.GetItemDef() },
                            new RecipeIngredient { pickup = SangeAndYasha.GetItemDef() }
                        }
                    };
                    recipes[0] = recipe;
                }
                if (Plugin.ItemsEnabled[Sange.Instance] &&
                    Plugin.ItemsEnabled[YashaAndKaya.Instance])
                {
                    var recipe = new Recipe()
                    {
                        amountToDrop = 1,
                        ingredients = new[]
                        {
                            new RecipeIngredient { pickup = Sange.GetItemDef() },
                            new RecipeIngredient { pickup = YashaAndKaya.GetItemDef() }
                        }
                    };
                    recipes[1] = recipe;
                }
                if (Plugin.ItemsEnabled[Yasha.Instance] &&
                    Plugin.ItemsEnabled[KayaAndSange.Instance])
                {
                    var recipe = new Recipe()
                    {
                        amountToDrop = 1,
                        ingredients = new[]
                        {
                            new RecipeIngredient { pickup = Yasha.GetItemDef() },
                            new RecipeIngredient { pickup = KayaAndSange.GetItemDef() }
                        }
                    };
                    recipes[2] = recipe;
                }
                craftable.recipes = recipes;
                // 0
                Content.craftables.Add(craftable);
            }
            if (Plugin.ItemsEnabled[Yasha.Instance] &&
                Plugin.ItemsEnabled[Kaya.Instance])
            {
                // 1
                AddCraftable(Yasha.GetItemDef(), Kaya.GetItemDef(), YashaAndKaya.GetItemDef(), "Yasha and Kaya");
            }
            if (Plugin.ItemsEnabled[Kaya.Instance] &&
                Plugin.ItemsEnabled[Sange.Instance])
            {
                // 2
                AddCraftable(Kaya.GetItemDef(), Sange.GetItemDef(), KayaAndSange.GetItemDef(), "Kaya and Sange");
            }
            if (Plugin.ItemsEnabled[Sange.Instance] &&
                Plugin.ItemsEnabled[Yasha.Instance])
            {
                // 3
                AddCraftable(Sange.GetItemDef(), Yasha.GetItemDef(), SangeAndYasha.GetItemDef(), "Sange and Yasha");
            }

            // ones that rely on vanilla items, and thus need blanks in the meantime.
            if (Plugin.ItemsEnabled[BootsOfSpeed.Instance] &&
                Plugin.ItemsEnabled[TranquilBoots.Instance])
            {
                // 4
                AddBlankCraftable("BootsOfSpeed and HealWhileSafe");
            }
            if (Plugin.ItemsEnabled[BladesOfAttack.Instance] &&
                Plugin.ItemsEnabled[PhaseBoots.Instance])
            {
                // 5
                AddBlankCraftable("BladesOfAttack and SprintBonus");
            }
            if (Plugin.ItemsEnabled[PowerTreads.Instance])
            {
                // 6
                AddBlankCraftable("Hoof and Syringe");
            }
            if (Plugin.ItemsEnabled[TranquilBoots.Instance] &&
                Plugin.EquipmentEnabled[BootsOfBearing.Instance])
            {
                // 7
                AddBlankCraftable("TranquilBoots and TeamWarCry");
            }
            if (Plugin.ItemsEnabled[PhaseBoots.Instance] &&
                Plugin.ItemsEnabled[BootsOfTravel.Instance])
            {
                // 8
                AddBlankCraftable("PhaseBoots and JumpDamageStrike");
            }
            if (Plugin.ItemsEnabled[BootsOfSpeed.Instance] &&
                Plugin.EquipmentEnabled[ArcaneBoots.Instance])
            {
                // 9
                AddBlankCraftable("BootsOfSpeed and EquipmentMagazine");
            }
            if (Plugin.EquipmentEnabled[Mekansm.Instance])
            {
                // 10
                AddBlankCraftable("TPHealingNova and SprintArmor");
            }
            if (Plugin.EquipmentEnabled[Bottle.Instance])
            {
                // 11
                AddBlankCraftable("EquipmentMagazine and HealingPotion");
            }
            if (Plugin.ItemsEnabled[EnchantedMango.Instance])
            {
                // 12
                AddBlankCraftable("ScrapWhite and EnchantedMango");
                // 13
                AddBlankCraftable("FlatHealth and EnchantedMango");
            }
            if (Plugin.ItemsEnabled[HealingSalve.Instance])
            {
                // 14
                AddBlankCraftable("ScrapWhite and HealingSalve");
                // 15
                AddBlankCraftable("HealingPotionConsumed and HealingSalve");
                // 16
                AddBlankCraftable("FlatHealth and HealingSalve");
            }
            if (Plugin.ItemsEnabled[DragonScale.Instance])
            {
                // 17
                AddBlankCraftable("DragonScale and Infusion");
            }
            if (Plugin.ItemsEnabled[HeartOfTarrasque.Instance])
            {
                // 18
                AddBlankCraftable("ScrapWhite and HeartOfTarrasque");
            }
            if (Plugin.ItemsEnabled[Daedalus.Instance])
            {
                // 19
                AddBlankCraftable("ScrapWhite and Daedalus");
            }
            if (Plugin.ItemsEnabled[UnwaveringCondition.Instance] &&
                Plugin.ItemsEnabled[Cloak.Instance])
            {
                // 20
                AddBlankCraftable("ScrapWhite and UnwaveringCondition");
            }
            if (Plugin.ItemsEnabled[BootsOfTravel.Instance] &&
                Plugin.ItemsEnabled[BootsOfSpeed.Instance])
            {
                // 21
                AddBlankCraftable("ScrapWhite and BootsOfTravel");
            }
            if (Plugin.ItemsEnabled[AeonDisk.Instance])
            {
                // 22
                AddBlankCraftable("SprintArmor and TeleportOnLowHealth");
            }
            if (Plugin.ItemsEnabled[Radiance.Instance])
            {
                // 23
                AddBlankCraftable("Thorns and StrengthenBurn");
            }
            if (Plugin.ItemsEnabled[DragonScale.Instance] &&
                Plugin.ItemsEnabled[Radiance.Instance])
            {
                // 24
                AddBlankCraftable("ScrapWhite and Radiance");
            }

            new Content().Initialize();
            PickupCatalog.availability.CallWhenAvailable(InitNewRest);
        }
        public static void InitNewRest()
        {
            // ones that rely on vanilla items, so have to wait for proper initialization.
            if (Plugin.EquipmentEnabled[GhostScepter.Instance])
            {
                Add(GhostScepter.GetEquipmentDef(), DLC1Content.Equipment.BossHunterConsumed, DLC1Content.Equipment.BossHunter);
            }

            if (Plugin.ItemsEnabled[BootsOfSpeed.Instance] &&
                Plugin.ItemsEnabled[TranquilBoots.Instance])
            {
                // 4
                FinishCraftable(4, BootsOfSpeed.GetItemDef(), RoR2Content.Items.HealWhileSafe, TranquilBoots.GetItemDef());
            }
            if (Plugin.ItemsEnabled[BladesOfAttack.Instance] &&
                Plugin.ItemsEnabled[PhaseBoots.Instance])
            {
                // 5
                FinishCraftable(5, BladesOfAttack.GetItemDef(), RoR2Content.Items.SprintBonus, PhaseBoots.GetItemDef());
            }
            if (Plugin.ItemsEnabled[PowerTreads.Instance])
            {
                // 6
                FinishCraftable(6, RoR2Content.Items.Hoof, RoR2Content.Items.Syringe, PowerTreads.GetItemDef());
            }
            if (Plugin.ItemsEnabled[TranquilBoots.Instance] &&
                Plugin.EquipmentEnabled[BootsOfBearing.Instance])
            {
                // 7
                FinishCraftable(7, TranquilBoots.GetItemDef(), RoR2Content.Equipment.TeamWarCry, BootsOfBearing.GetEquipmentDef());
            }
            if (Plugin.ItemsEnabled[PhaseBoots.Instance] &&
                Plugin.ItemsEnabled[BootsOfTravel.Instance])
            {
                // 8
                FinishCraftable(8, PhaseBoots.GetItemDef(), DLC3Content.Items.JumpDamageStrike, BootsOfTravel.GetItemDef());
            }
            if (Plugin.ItemsEnabled[BootsOfSpeed.Instance] &&
                Plugin.EquipmentEnabled[ArcaneBoots.Instance])
            {
                // 9
                FinishCraftable(9, BootsOfSpeed.GetItemDef(), RoR2Content.Items.EquipmentMagazine, ArcaneBoots.GetEquipmentDef());
            }
            if (Plugin.EquipmentEnabled[Mekansm.Instance])
            {
                // 10
                FinishCraftable(10, RoR2Content.Items.TPHealingNova, RoR2Content.Items.SprintArmor, Mekansm.GetEquipmentDef());
            }
            if (Plugin.EquipmentEnabled[Bottle.Instance])
            {
                // 11
                FinishCraftable(11, DLC1Content.Items.HealingPotion, RoR2Content.Items.EquipmentMagazine, Bottle.GetEquipmentDef());
            }
            if (Plugin.ItemsEnabled[EnchantedMango.Instance])
            {
                // 12
                FinishCraftable(12, RoR2Content.Items.ScrapWhite, EnchantedMango.GetItemDef(), DLC3Content.Items.BonusHealthBoost);
                // 13
                FinishCraftable(13, RoR2Content.Items.FlatHealth, EnchantedMango.GetItemDef(), DLC3Content.Items.CookedSteak);
            }
            if (Plugin.ItemsEnabled[HealingSalve.Instance])
            {
                // 14
                FinishCraftable(14, RoR2Content.Items.ScrapWhite, HealingSalve.GetItemDef(), DLC3Content.Items.BonusHealthBoost);
                // 15
                FinishCraftable(15, DLC1Content.Items.HealingPotionConsumed, HealingSalve.GetItemDef(), DLC1Content.Items.HealingPotion);
                // 16
                FinishCraftable(16, RoR2Content.Items.FlatHealth, HealingSalve.GetItemDef(), DLC3Content.Items.CookedSteak);
            }
            if (Plugin.ItemsEnabled[DragonScale.Instance])
            {
                // 17
                FinishCraftable(17, DragonScale.GetItemDef(), RoR2Content.Items.Infusion, DLC1Content.Items.StrengthenBurn);
            }
            if (Plugin.ItemsEnabled[HeartOfTarrasque.Instance])
            {
                // 18
                FinishCraftable(18, RoR2Content.Items.ScrapWhite, HeartOfTarrasque.GetItemDef(), RoR2Content.Items.FlatHealth, 4);
            }
            if (Plugin.ItemsEnabled[Daedalus.Instance])
            {
                // 19
                FinishCraftable(19, RoR2Content.Items.ScrapWhite, Daedalus.GetItemDef(), RoR2Content.Items.CritGlasses, 2);
            }
            if (Plugin.ItemsEnabled[UnwaveringCondition.Instance] &&
                Plugin.ItemsEnabled[Cloak.Instance])
            {
                // 20
                FinishCraftable(20, RoR2Content.Items.ScrapWhite, UnwaveringCondition.GetItemDef(), Cloak.GetItemDef(), 4);
            }
            if (Plugin.ItemsEnabled[BootsOfTravel.Instance] &&
                Plugin.ItemsEnabled[BootsOfSpeed.Instance])
            {
                // 21
                FinishCraftable(21, RoR2Content.Items.ScrapWhite, BootsOfTravel.GetItemDef(), BootsOfSpeed.GetItemDef(), 4);
            }
            if (Plugin.ItemsEnabled[AeonDisk.Instance])
            {
                // 22
                FinishCraftable(22, RoR2Content.Items.SprintArmor, DLC2Content.Items.TeleportOnLowHealth, AeonDisk.GetItemDef());
            }
            if (Plugin.ItemsEnabled[Radiance.Instance])
            {
                // 23
                FinishCraftable(23, RoR2Content.Items.Thorns, DLC1Content.Items.StrengthenBurn, Radiance.GetItemDef());
            }
            if (Plugin.ItemsEnabled[DragonScale.Instance] &&
                Plugin.ItemsEnabled[Radiance.Instance])
            {
                // 24
                FinishCraftable(24, RoR2Content.Items.ScrapWhite, Radiance.GetItemDef(), DragonScale.GetItemDef(), 4);
            }
        }

        public static void InitOld()
        {
            // ITEM RECIPES //
            if (Plugin.ItemsEnabled[OrbOfVenom.Instance] &&
                Plugin.ItemsEnabled[OrbOfFrost.Instance] &&
                Plugin.ItemsEnabled[OrbOfBlight.Instance] &&
                Plugin.ItemsEnabled[OrbOfCorrosion.Instance])
            {
                Add([OrbOfVenom.GetItemDef(), OrbOfBlight.GetItemDef(), OrbOfFrost.GetItemDef()], OrbOfCorrosion.GetItemDef());
                itemsToWatchFor.AddRange([
                    OrbOfFrost.GetItemDef(),
                    OrbOfBlight.GetItemDef(),
                    OrbOfVenom.GetItemDef()
                ]);
            }
            if (Plugin.ItemsEnabled[KayaAndSange.Instance] &&
                Plugin.ItemsEnabled[Yasha.Instance] &&
                Plugin.ItemsEnabled[Trident.Instance])
            {
                Add([KayaAndSange.GetItemDef(), Yasha.GetItemDef()], Trident.GetItemDef());
            }
            if (Plugin.ItemsEnabled[SangeAndYasha.Instance] &&
                Plugin.ItemsEnabled[Kaya.Instance] &&
                Plugin.ItemsEnabled[Trident.Instance])
            {
                Add([SangeAndYasha.GetItemDef(), Kaya.GetItemDef()], Trident.GetItemDef());
            }
            if (Plugin.ItemsEnabled[YashaAndKaya.Instance] &&
                Plugin.ItemsEnabled[Sange.Instance] &&
                Plugin.ItemsEnabled[Trident.Instance])
            {
                Add([YashaAndKaya.GetItemDef(), Sange.GetItemDef()], Trident.GetItemDef());
            }
            if (Plugin.ItemsEnabled[Kaya.Instance] &&
                Plugin.ItemsEnabled[Sange.Instance] &&
                Plugin.ItemsEnabled[KayaAndSange.Instance])
            {
                Add([Kaya.GetItemDef(), Sange.GetItemDef()], KayaAndSange.GetItemDef());
            }
            if (Plugin.ItemsEnabled[Sange.Instance] &&
                Plugin.ItemsEnabled[Yasha.Instance] &&
                Plugin.ItemsEnabled[SangeAndYasha.Instance])
            {
                Add([Sange.GetItemDef(), Yasha.GetItemDef()], SangeAndYasha.GetItemDef());
            }
            if (Plugin.ItemsEnabled[Yasha.Instance] &&
                Plugin.ItemsEnabled[Kaya.Instance] &&
                Plugin.ItemsEnabled[YashaAndKaya.Instance])
            {
                Add([Yasha.GetItemDef(), Kaya.GetItemDef()], YashaAndKaya.GetItemDef());
            }
            if (Plugin.ItemsEnabled[Yasha.Instance] &&
                Plugin.ItemsEnabled[Kaya.Instance] &&
                Plugin.ItemsEnabled[Sange.Instance] &&
                Plugin.ItemsEnabled[Trident.Instance])
            {
                Add([Yasha.GetItemDef(), Kaya.GetItemDef(), Sange.GetItemDef()], Trident.GetItemDef());
                itemsToWatchFor.AddRange([
                    Yasha.GetItemDef(),
                    Kaya.GetItemDef(),
                    Sange.GetItemDef()
                ]);
            }
            if (Plugin.ItemsEnabled[BootsOfSpeed.Instance] &&
                Plugin.ItemsEnabled[TranquilBoots.Instance])
            {
                Add([BootsOfSpeed.GetItemDef(), RoR2Content.Items.HealWhileSafe], TranquilBoots.GetItemDef());
                itemsToWatchFor.AddRange([
                    BootsOfSpeed.GetItemDef(),
                    RoR2Content.Items.HealWhileSafe
                ]);
            }
            if (Plugin.ItemsEnabled[BladesOfAttack.Instance] &&
                Plugin.ItemsEnabled[PhaseBoots.Instance])
            {
                Add([BladesOfAttack.GetItemDef(), RoR2Content.Items.SprintBonus], PhaseBoots.GetItemDef());
                itemsToWatchFor.AddRange([
                    BladesOfAttack.GetItemDef(),
                    RoR2Content.Items.SprintBonus
                ]);
            }
            if (Plugin.ItemsEnabled[PowerTreads.Instance])
            {
                Add([RoR2Content.Items.Hoof, RoR2Content.Items.Syringe], PowerTreads.GetItemDef());
                itemsToWatchFor.AddRange([
                    RoR2Content.Items.Hoof,
                    RoR2Content.Items.Syringe
                ]);
            }

            // EQUIPMENT RECIPES //
            if (Plugin.EquipmentEnabled[ArcaneBoots.Instance] &&
                Plugin.EquipmentEnabled[Mekansm.Instance] &&
                Plugin.EquipmentEnabled[GuardianGreaves.Instance])
            {
                Add(ArcaneBoots.GetEquipmentDef(), Mekansm.GetEquipmentDef(), GuardianGreaves.GetEquipmentDef());
            }
            if (Plugin.EquipmentEnabled[GhostScepter.Instance])
            {
                Add(GhostScepter.GetEquipmentDef(), DLC1Content.Equipment.BossHunterConsumed, DLC1Content.Equipment.BossHunter);
            }

            // MIXED RECIPES //
            if (Plugin.ItemsEnabled[TranquilBoots.Instance] &&
                Plugin.EquipmentEnabled[BootsOfBearing.Instance])
            {
                Add([TranquilBoots.GetItemDef()], RoR2Content.Equipment.TeamWarCry, BootsOfBearing.GetEquipmentDef());
            }

            // HOOKS //
            CharacterBody.onBodyInventoryChangedGlobal += OnInventoryChanged;
            On.RoR2.EquipmentDef.AttemptGrant += OnAttemptGrant;

            Log.Debug($"RecipeManager initialized, added {item_recipes.Count} item recipes and {itemsToWatchFor.Count} items to watch for, and {equipment_recipes.Count} equipment recipes.");
        }

        private static void OnInventoryChanged(CharacterBody body)
        {
            if (!body.GetComponent<RecipeBehavior>())
            {
                foreach (ItemDef item in itemsToWatchFor)
                {
                    if (body.inventory.GetItemCountPermanent(item) > 0)
                    {
                        Log.Debug($"Found at least 1 PERMANENT {item.nameToken}, adding RecipeBehavior to {body.GetDisplayName()}.");
                        body.gameObject.AddComponent<RecipeBehavior>();
                        return;
                    }
                }
            }
        }
        private static void OnAttemptGrant(On.RoR2.EquipmentDef.orig_AttemptGrant orig, ref PickupDef.GrantContext context)
        {
            bool callOriginal = true;
            EquipmentIndex currentEquipmentIndex = context.body.inventory.currentEquipmentIndex;
            EquipmentIndex equipmentIndex = PickupCatalog.GetPickupDef(context.controller._pickupState.pickupIndex)?.equipmentIndex ?? EquipmentIndex.None;
            foreach (EquipmentRecipe recipe in equipment_recipes)
            {
                if ((currentEquipmentIndex == recipe.input_1.equipmentIndex &&
                     equipmentIndex == recipe.input_2.equipmentIndex) ||
                    (currentEquipmentIndex == recipe.input_2.equipmentIndex &&
                     equipmentIndex == recipe.input_1.equipmentIndex))
                {
                    context.body.inventory.SetEquipmentIndex(recipe.output.equipmentIndex, isRemovingEquipment: true);
                    context.controller.StartWaitTime();
                    context.shouldDestroy = true;
                    context.shouldNotify = false;
                    CharacterMasterNotificationQueue.PushEquipmentTransformNotification(context.body.master, currentEquipmentIndex, recipe.output.equipmentIndex, default);
                    CharacterMasterNotificationQueue.PushEquipmentTransformNotification(context.body.master, equipmentIndex, recipe.output.equipmentIndex, default);
                    callOriginal = false;
                    break;
                }
            }

            if (callOriginal)
            {
                orig(ref context);
            }
        }
        private class RecipeBehavior : MonoBehaviour
        {
            CharacterBody body;

            void Awake()
            {
                body = GetComponent<CharacterBody>();
            }
            void FixedUpdate()
            {
                if (NetworkServer.active && body && body.inventory)
                {
                    foreach (ItemRecipe ItemRecipe in item_recipes)
                    {
                        bool bail = false;
                        int amount_to_make = int.MaxValue;
                        foreach (var item in ItemRecipe.inputs.Keys)
                        {
                            int count = body.inventory.GetItemCountPermanent(item);
                            if (count < ItemRecipe.inputs[item])
                            {
                                bail = true;
                                break;
                            }
                            amount_to_make = Math.Min(amount_to_make, count / ItemRecipe.inputs[item]);
                        }

                        if (bail)
                        {
                            continue;
                        }

                        Log.Debug($"Creating {amount_to_make * ItemRecipe.output.Value} {ItemRecipe.output.Key.nameToken}.");

                        foreach (var pair in ItemRecipe.inputs)
                        {
                            body.inventory.RemoveItemPermanent(pair.Key, amount_to_make * pair.Value);

                            CharacterMasterNotificationQueue.PushItemTransformNotification(body.master, pair.Key.itemIndex, ItemRecipe.output.Key.itemIndex, default);
                        }
                        body.inventory.GiveItemPermanent(ItemRecipe.output.Key, amount_to_make * ItemRecipe.output.Value);
                    }

                    foreach (MixedRecipe MixedRecipe in mixed_recipes)
                    {
                        bool bail = false;
                        foreach (var item in MixedRecipe.items.Keys)
                        {
                            int count = body.inventory.GetItemCountPermanent(item);
                            if (count < MixedRecipe.items[item])
                            {
                                bail = true;
                                break;
                            }
                        }

                        if (bail || body.inventory.currentEquipmentIndex != MixedRecipe.equipment.equipmentIndex)
                        {
                            continue;
                        }

                        Log.Debug($"Creating {MixedRecipe.output.nameToken}.");

                        foreach (var pair in MixedRecipe.items)
                        {
                            body.inventory.RemoveItemPermanent(pair.Key, pair.Value);
                        }
                        CharacterMasterNotificationQueue.PushEquipmentTransformNotification(body.master, MixedRecipe.equipment.equipmentIndex, MixedRecipe.output.equipmentIndex, default);
                        body.inventory.SetEquipmentIndexForSlot(MixedRecipe.output.equipmentIndex, body.inventory.activeEquipmentSlot, body.inventory.activeEquipmentSet[body.inventory.activeEquipmentSlot]);
                    }
                }
            }
        }
    }
}