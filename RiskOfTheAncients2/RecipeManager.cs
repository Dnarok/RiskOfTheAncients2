using RoR2;
using ROTA2.Equipment;
using ROTA2.Items;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public static void Init()
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
                Plugin.ItemsEnabled[Trident.Instance])
            {
                Add([Kaya.GetItemDef(), Sange.GetItemDef()], KayaAndSange.GetItemDef());
            }
            if (Plugin.ItemsEnabled[Sange.Instance] &&
                Plugin.ItemsEnabled[Yasha.Instance] &&
                Plugin.ItemsEnabled[Trident.Instance])
            {
                Add([Sange.GetItemDef(), Yasha.GetItemDef()], SangeAndYasha.GetItemDef());
            }
            if (Plugin.ItemsEnabled[Yasha.Instance] &&
                Plugin.ItemsEnabled[Kaya.Instance] &&
                Plugin.ItemsEnabled[Trident.Instance])
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
                Add([RoR2Content.Items.SprintBonus, RoR2Content.Items.BoostAttackSpeed], PowerTreads.GetItemDef());
                itemsToWatchFor.AddRange([
                    RoR2Content.Items.SprintBonus,
                    RoR2Content.Items.BoostAttackSpeed
                ]);
            }

            // EQUIPMENT RECIPES //
            if (Plugin.EquipmentEnabled[ArcaneBoots.Instance] &&
                Plugin.EquipmentEnabled[Mekansm.Instance] &&
                Plugin.EquipmentEnabled[GuardianGreaves.Instance])
            {
                Add(ArcaneBoots.GetEquipmentDef(), Mekansm.GetEquipmentDef(), GuardianGreaves.GetEquipmentDef());
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
                    if (body.inventory.GetItemCount(item) > 0)
                    {
                        Log.Debug($"Found at least 1 {item.nameToken}, adding RecipeBehavior to {body.GetDisplayName()}.");
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
            EquipmentIndex equipmentIndex = PickupCatalog.GetPickupDef(context.controller.pickupIndex)?.equipmentIndex ?? EquipmentIndex.None;
            foreach (EquipmentRecipe recipe in equipment_recipes)
            {
                if ((currentEquipmentIndex == recipe.input_1.equipmentIndex &&
                     equipmentIndex == recipe.input_2.equipmentIndex) ||
                    (currentEquipmentIndex == recipe.input_2.equipmentIndex &&
                     equipmentIndex == recipe.input_1.equipmentIndex))
                {
                    context.body.inventory.SetEquipmentIndex(recipe.output.equipmentIndex);
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
                            int count = body.inventory.GetItemCount(item);
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
                            body.inventory.RemoveItem(pair.Key, amount_to_make * pair.Value);

                            CharacterMasterNotificationQueue.PushItemTransformNotification(body.master, pair.Key.itemIndex, ItemRecipe.output.Key.itemIndex, default);
                        }
                        body.inventory.GiveItem(ItemRecipe.output.Key, amount_to_make * ItemRecipe.output.Value);
                    }

                    foreach (MixedRecipe MixedRecipe in mixed_recipes)
                    {
                        bool bail = false;
                        foreach (var item in MixedRecipe.items.Keys)
                        {
                            int count = body.inventory.GetItemCount(item);
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
                            body.inventory.RemoveItem(pair.Key, pair.Value);
                        }
                        CharacterMasterNotificationQueue.PushEquipmentTransformNotification(body.master, MixedRecipe.equipment.equipmentIndex, MixedRecipe.output.equipmentIndex, default);
                        body.inventory.SetEquipmentIndexForSlot(MixedRecipe.output.equipmentIndex, body.inventory.activeEquipmentSlot);
                    }
                }
            }
        }
    }
}