using Newtonsoft.Json.Utilities;
using RoR2;
using ROTA2.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace ROTA2
{
    public class RecipeManager
    {
        public class Recipe
        {
            // map of [item in : amount required]
            public Dictionary<ItemDef, int> inputs { get; set; }
            // [item out : amount made]
            public KeyValuePair<ItemDef, int> output { get; set; }
        }

        public static List<Recipe> recipes = [];
        public static List<ItemDef> itemsToWatchFor = [];

        public static void Add(Recipe recipe)
        {
            recipes.Add(recipe);
        }
        public static void Add(ItemDef[] inputs, ItemDef output)
        {
            Recipe recipe = new()
            {
                inputs = inputs.Distinct().ToDictionary(x => x, y => 1),
                output = KeyValuePair.Create(output, 1)
            };
            recipes.Add(recipe);
        }

        public static void Init()
        {
            if (Plugin.ItemsEnabled[OrbOfVenom.Instance] &&
                Plugin.ItemsEnabled[OrbOfFrost.Instance] &&
                Plugin.ItemsEnabled[OrbOfBlight.Instance] &&
                Plugin.ItemsEnabled[OrbOfCorrosion.Instance])
            {
                Add([OrbOfVenom.Instance.ItemDef, OrbOfBlight.Instance.ItemDef, OrbOfFrost.Instance.ItemDef], OrbOfCorrosion.Instance.ItemDef);
                itemsToWatchFor.AddRange([
                    OrbOfFrost.Instance.ItemDef,
                    OrbOfBlight.Instance.ItemDef,
                    OrbOfVenom.Instance.ItemDef
                ]);
            }
            if (Plugin.ItemsEnabled[KayaAndSange.Instance] &&
                Plugin.ItemsEnabled[Yasha.Instance] &&
                Plugin.ItemsEnabled[Trident.Instance])
            {
                Add([KayaAndSange.Instance.ItemDef, Yasha.Instance.ItemDef], Trident.Instance.ItemDef);
            }
            if (Plugin.ItemsEnabled[SangeAndYasha.Instance] &&
                Plugin.ItemsEnabled[Kaya.Instance] &&
                Plugin.ItemsEnabled[Trident.Instance])
            {
                Add([SangeAndYasha.Instance.ItemDef, Kaya.Instance.ItemDef], Trident.Instance.ItemDef);
            }
            if (Plugin.ItemsEnabled[YashaAndKaya.Instance] &&
                Plugin.ItemsEnabled[Sange.Instance] &&
                Plugin.ItemsEnabled[Trident.Instance])
            {
                Add([YashaAndKaya.Instance.ItemDef, Sange.Instance.ItemDef], Trident.Instance.ItemDef);
            }
            if (Plugin.ItemsEnabled[Kaya.Instance] &&
                Plugin.ItemsEnabled[Sange.Instance] &&
                Plugin.ItemsEnabled[Trident.Instance])
            {
                Add([Kaya.Instance.ItemDef, Sange.Instance.ItemDef], KayaAndSange.Instance.ItemDef);
            }
            if (Plugin.ItemsEnabled[Sange.Instance] &&
                Plugin.ItemsEnabled[Yasha.Instance] &&
                Plugin.ItemsEnabled[Trident.Instance])
            {
                Add([Sange.Instance.ItemDef, Yasha.Instance.ItemDef], SangeAndYasha.Instance.ItemDef);
            }
            if (Plugin.ItemsEnabled[Yasha.Instance] &&
                Plugin.ItemsEnabled[Kaya.Instance] &&
                Plugin.ItemsEnabled[Trident.Instance])
            {
                Add([Yasha.Instance.ItemDef, Kaya.Instance.ItemDef], YashaAndKaya.Instance.ItemDef);
            }
            if (Plugin.ItemsEnabled[Yasha.Instance] &&
                Plugin.ItemsEnabled[Kaya.Instance] &&
                Plugin.ItemsEnabled[Sange.Instance] &&
                Plugin.ItemsEnabled[Trident.Instance])
            {
                Add([Yasha.Instance.ItemDef, Kaya.Instance.ItemDef, Sange.Instance.ItemDef], Trident.Instance.ItemDef);
                itemsToWatchFor.AddRange([
                    Yasha.Instance.ItemDef,
                    Kaya.Instance.ItemDef,
                    Sange.Instance.ItemDef
                ]);
            }
            if (Plugin.ItemsEnabled[BootsOfSpeed.Instance] &&
                Plugin.ItemsEnabled[TranquilBoots.Instance])
            {
                Add([BootsOfSpeed.Instance.ItemDef, RoR2Content.Items.HealWhileSafe], TranquilBoots.Instance.ItemDef);
                itemsToWatchFor.AddRange([
                    BootsOfSpeed.Instance.ItemDef,
                    RoR2Content.Items.HealWhileSafe
                ]);
            }

            CharacterBody.onBodyInventoryChangedGlobal += OnInventoryChanged;
            Log.Debug($"RecipeManager initialized, added {recipes.Count} recipes and {itemsToWatchFor.Count} items to watch for.");
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
        private class RecipeBehavior : MonoBehaviour
        {
            CharacterBody body;

            void Awake()
            {
                body = GetComponent<CharacterBody>();
            }
            void Update()
            {
                if (NetworkServer.active && body && body.inventory)
                {
                    foreach (Recipe recipe in recipes)
                    {
                        bool bail = false;
                        int amount_to_make = int.MaxValue; 
                        foreach (var item in recipe.inputs.Keys)
                        {
                            int count = body.inventory.GetItemCount(item);
                            if (count < recipe.inputs[item])
                            {
                                bail = true;
                                break;
                            }
                            amount_to_make = Math.Min(amount_to_make, count / recipe.inputs[item]);
                        }

                        if (bail)
                        {
                            continue;
                        }

                        Log.Debug($"Creating {amount_to_make * recipe.output.Value} {recipe.output.Key.nameToken}.");

                        foreach (var pair in recipe.inputs)
                        {
                            body.inventory.RemoveItem(pair.Key, amount_to_make * pair.Value);

                            CharacterMasterNotificationQueue.PushItemTransformNotification(body.master, pair.Key.itemIndex, recipe.output.Key.itemIndex, default);
                        }
                        body.inventory.GiveItem(recipe.output.Key, amount_to_make * recipe.output.Value);
                    }
                }
            }
        }
    }
}