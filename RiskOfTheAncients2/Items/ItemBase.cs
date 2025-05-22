using RoR2;
using R2API;
using UnityEngine;
using BepInEx.Configuration;
using System;
using UnityEngine.AddressableAssets;
using RoR2.ExpansionManagement;

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
    }
    public abstract class ItemBase
    {
        public Func<string, string> Damage  = x => "<style=cIsDamage>"  + x + "</style>";
        public Func<string, string> Healing = x => "<style=cIsHealing>" + x + "</style>";
        public Func<string, string> Utility = x => "<style=cIsUtility>" + x + "</style>";
        public Func<string, string> Stack   = x => "<style=cStack>"     + x + "</style>";
        public Func<string, string> Health  = x => "<style=cIsHealth>"  + x + "</style>";
        public Func<string, string> Gold = x => "<style=cShrine>" + x + "</style>";
        public Func<string, string> Death = x => "<style=cDeath>" + x + "</style>";
        public Func<string, string, string> Color = (x, hex) => "<" + hex + ">" + x + "</color>";

        public abstract string ItemName { get; }
        public abstract string ConfigItemName { get; }
        public abstract string ItemTokenName { get; }
        public abstract string ItemTokenPickup { get; }
        public abstract string ItemTokenDesc { get; }
        public abstract string ItemTokenLore { get; }
        public abstract ItemTier Tier { get; }
        public virtual string ItemModelPath { get; } = "item_box.prefab";
        public virtual string ItemIconPath { get; } = "";
        public virtual ItemTag[] ItemTags { get; } = [];
        public virtual bool Removable { get; } = true;
        public virtual bool Hidden { get; } = false;
        public virtual ItemDef VoidFor { get; } = null;
        public abstract void Init(ConfigFile configuration);
        public abstract void Hooks();

        public ItemDef ItemDef;

        protected void CreateLanguageTokens()
        {
            LanguageAPI.Add("ITEM_" + ItemTokenName + "_NAME",        ItemName);
            LanguageAPI.Add("ITEM_" + ItemTokenName + "_PICKUP",      ItemTokenPickup);
            LanguageAPI.Add("ITEM_" + ItemTokenName + "_DESCRIPTION", ItemTokenDesc);
            LanguageAPI.Add("ITEM_" + ItemTokenName + "_LORE",        ItemTokenLore);
        }
        protected void CreateItemDef()
        {
            ItemDef = ScriptableObject.CreateInstance<ItemDef>();
            ItemDef.name = "ITEM_" + ItemTokenName;
            ItemDef.nameToken = "ITEM_" + ItemTokenName + "_NAME";
            ItemDef.pickupToken = "ITEM_" + ItemTokenName + "_PICKUP";
            ItemDef.descriptionToken = "ITEM_" + ItemTokenName + "_DESCRIPTION";
            ItemDef.loreToken = "ITEM_" + ItemTokenName + "_LORE";
            GameObject prefab = Plugin.bundle.LoadAsset<GameObject>(ItemModelPath);
            ModelPanelParameters logbook = prefab.AddComponent<ModelPanelParameters>();
            logbook.minDistance = 4.0f;
            logbook.maxDistance = 10.0f;
            logbook.focusPointTransform = prefab.transform.GetChild(0);
            logbook.cameraPositionTransform = prefab.transform.GetChild(0);
            ItemDef.pickupModelPrefab = prefab;
            ItemDef.hidden = Hidden;
            ItemDef.canRemove = Removable;
            ItemDef.deprecatedTier = Tier;
            ItemDef.tags = ItemTags;
            if (Tier == ItemTier.VoidTier1 || Tier == ItemTier.VoidTier2 || Tier == ItemTier.VoidTier3)
            {
                ItemDef.requiredExpansion = Addressables.LoadAssetAsync<ExpansionDef>("RoR2/DLC1/Common/DLC1.asset").WaitForCompletion();
            }

            if (ItemIconPath == "")
            {
                ItemDef.pickupIconSprite = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/MiscIcons/texMysteryIcon.png").WaitForCompletion();
            }
            else
            {
                ItemDef.pickupIconSprite = Plugin.ExtractSprite(ItemIconPath);
            }

            ItemAPI.Add(new CustomItem(ItemDef, new ItemDisplayRuleDict(null)));
        }

        public int GetCount(CharacterBody body)
        {
            if (body && body.inventory)
            {
                return body.inventory.GetItemCount(ItemDef);
            }
            else
            {
                return 0;
            }
        }
        public int GetCount(CharacterMaster master)
        {
            if (master && master.inventory)
            {
                return master.inventory.GetItemCount(ItemDef);
            }
            else
            {
                return 0;
            }
        }
    }
}