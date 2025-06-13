using BepInEx.Configuration;
using R2API;
using RoR2;
using RoR2.ExpansionManagement;
using System;
using UnityEngine.AddressableAssets;

namespace ROTA2.Items
{
    public abstract class ItemBase<T> : ItemBase where T : ItemBase<T>
    {
        public static T Instance { get; private set; }

        public ItemBase()
        {
            if (Instance != null)
            {
                throw new InvalidOperationException($"Singleton class \"{typeof(T).Name}\" inheriting ItemBase was instantiated twice.");
            }
            else
            {
                Instance = this as T;
            }
        }

        public static int GetCount(CharacterBody body)
        {
            if (body && body.inventory)
            {
                return body.inventory.GetItemCount(GetItemDef());
            }
            else
            {
                return 0;
            }
        }
        public static int GetCount(CharacterMaster master)
        {
            if (master && master.inventory)
            {
                return master.inventory.GetItemCount(GetItemDef());
            }
            else
            {
                return 0;
            }
        }
        public static ItemDef GetItemDef()
        {
            return Instance.ItemDef;
        }
    }
    public abstract class ItemBase
    {
        public Func<string, string> Damage = x => "<style=cIsDamage>" + x + "</style>";
        public Func<string, string> Healing = x => "<style=cIsHealing>" + x + "</style>";
        public Func<string, string> Utility = x => "<style=cIsUtility>" + x + "</style>";
        public Func<string, string> Stack = x => "<style=cStack>" + x + "</style>";
        public Func<string, string> Health = x => "<style=cIsHealth>" + x + "</style>";
        public Func<string, string> Gold = x => "<style=cShrine>" + x + "</style>";
        public Func<string, string> Death = x => "<style=cDeath>" + x + "</style>";
        public Func<string, string, string> Color = (x, hex) => "<" + hex + ">" + x + "</color>";

        public abstract string ItemName { get; }
        public virtual string ConfigItemName => ItemName;
        public abstract string ItemTokenName { get; }
        public abstract string ItemTokenPickup { get; }
        public abstract string ItemTokenDesc { get; }
        public abstract string ItemTokenLore { get; }
        public abstract string ItemDefGUID { get; }
        public virtual ItemDef VoidFor { get; } = null;
        public abstract void Init(ConfigFile configuration);
        public abstract void Hooks();

        public ItemDef ItemDef;
        protected void CreateLanguageTokens()
        {
            LanguageAPI.Add("ITEM_" + ItemTokenName + "_NAME", ItemName);
            LanguageAPI.Add("ITEM_" + ItemTokenName + "_PICKUP", ItemTokenPickup);
            LanguageAPI.Add("ITEM_" + ItemTokenName + "_DESCRIPTION", ItemTokenDesc);
            LanguageAPI.Add("ITEM_" + ItemTokenName + "_LORE", ItemTokenLore);
        }
        protected void CreateItemDef()
        {
            Addressables.LoadAssetAsync<ItemDef>(ItemDefGUID).Completed += (x) =>
            {
                ItemDef = x.Result;
                ItemDef.name = "ITEM_" + ItemTokenName;
                if (ItemDef.tier == ItemTier.VoidTier1 ||
                    ItemDef.tier == ItemTier.VoidTier2 ||
                    ItemDef.tier == ItemTier.VoidTier3 ||
                    ItemDef.tier == ItemTier.VoidBoss)
                {
                    Addressables.LoadAssetAsync<ExpansionDef>(RoR2BepInExPack.GameAssetPaths.RoR2_DLC1_Common.DLC1_asset).Completed += (y) =>
                    {
                        ItemDef.requiredExpansion = y.Result;
                    };
                }
                ItemAPI.Add(new CustomItem(ItemDef, new ItemDisplayRuleDict(null)));
            };
        }
    }
}