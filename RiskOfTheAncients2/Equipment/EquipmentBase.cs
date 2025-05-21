using BepInEx.Configuration;
using RoR2;
using R2API;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ROTA2.Equipment
{
    public abstract class EquipmentBase<T> : EquipmentBase where T : EquipmentBase<T>
    {
        public static T Instance { get; private set; }

        public EquipmentBase()
        {
            if (Instance != null)
            {
                throw new InvalidOperationException($"Singleton class \"{typeof(T).Name}\" inherting ItemBase was instantiated twice.");
            }
            else
            {
                Instance = this as T;
            }
        }
    }
    public abstract class EquipmentBase
    {
        public Func<string, string> Damage = x => "<style=cIsDamage>" + x + "</style>";
        public Func<string, string> Healing = x => "<style=cIsHealing>" + x + "</style>";
        public Func<string, string> Utility = x => "<style=cIsUtility>" + x + "</style>";
        public Func<string, string> Stack = x => "<style=cStack>" + x + "</style>";
        public Func<string, string> Health = x => "<style=cIsHealth>" + x + "</style>";
        public Func<string, string> Gold = x => "<style=cShrine>" + x + "</style>";
        public Func<string, string> Death = x => "<style=cDeath>" + x + "</style>";
        public Func<string, string, string> Color = (x, hex) => "<" + hex + ">" + x + "</color>";

        public abstract string EquipmentName { get; }
        public abstract string EquipmentTokenName { get; }
        public abstract string EquipmentTokenPickup { get; }
        public abstract string EquipmentTokenDesc { get; }
        public abstract string EquipmentTokenLore { get; }
        public abstract float EquipmentCooldown { get; }
        public virtual string EquipmentModelPath { get; } = "item_box.prefab";
        public virtual string EquipmentIconPath { get; } = "";
        public virtual bool EquipmentCanDrop { get; } = true;
        public virtual bool EquipmentIsLunar { get; } = false;
        public virtual bool EquipmentCanBeRandomlyTriggered { get; } = true;
        public virtual ColorCatalog.ColorIndex EquipmentColorIndex { get; } = ColorCatalog.ColorIndex.Equipment;
        public abstract void Init(ConfigFile config);
        public virtual void Hooks()
        { 
        
        }

        public EquipmentDef EquipmentDef;

        protected void CreateLanguageTokens()
        {
            LanguageAPI.Add("EQUIPMENT_" + EquipmentTokenName + "_NAME",        EquipmentName);
            LanguageAPI.Add("EQUIPMENT_" + EquipmentTokenName + "_PICKUP",      EquipmentTokenPickup);
            LanguageAPI.Add("EQUIPMENT_" + EquipmentTokenName + "_DESCRIPTION", EquipmentTokenDesc);
            LanguageAPI.Add("EQUIPMENT_" + EquipmentTokenName + "_LORE",        EquipmentTokenLore);
        }
        protected void CreateEquipmentDef()
        {
            EquipmentDef = ScriptableObject.CreateInstance<EquipmentDef>();
            EquipmentDef.name = "EQUIPMENT_" + EquipmentTokenName;
            EquipmentDef.nameToken = "EQUIPMENT_" + EquipmentTokenName + "_NAME";
            EquipmentDef.pickupToken = "EQUIPMENT_" + EquipmentTokenName + "_PICKUP";
            EquipmentDef.descriptionToken = "EQUIPMENT_" + EquipmentTokenName + "_DESCRIPTION";
            EquipmentDef.loreToken = "EQUIPMENT_" + EquipmentTokenName + "_LORE";
            GameObject prefab = Plugin.bundle.LoadAsset<GameObject>(EquipmentModelPath);
            ModelPanelParameters logbook = prefab.AddComponent<ModelPanelParameters>();
            logbook.minDistance = 4.0f;
            logbook.maxDistance = 10.0f;
            logbook.focusPointTransform = prefab.transform.GetChild(0);
            logbook.cameraPositionTransform = prefab.transform.GetChild(0);
            EquipmentDef.pickupModelPrefab = prefab;
            EquipmentDef.appearsInMultiPlayer = true;
            EquipmentDef.appearsInSinglePlayer = true;
            EquipmentDef.canDrop = EquipmentCanDrop;
            EquipmentDef.cooldown = EquipmentCooldown;
            EquipmentDef.enigmaCompatible = true;
            EquipmentDef.isBoss = false;
            EquipmentDef.isLunar = EquipmentIsLunar;
            EquipmentDef.canBeRandomlyTriggered = EquipmentCanBeRandomlyTriggered;
            EquipmentDef.colorIndex = EquipmentColorIndex;

            if (EquipmentIconPath == "")
            {
                EquipmentDef.pickupIconSprite = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/MiscIcons/texMysteryIcon.png").WaitForCompletion();
            }
            else
            {
                EquipmentDef.pickupIconSprite = Plugin.ExtractSprite(EquipmentIconPath);
            }

            ItemAPI.Add(new CustomEquipment(EquipmentDef, new ItemDisplayRuleDict(null)));

            On.RoR2.EquipmentSlot.PerformEquipmentAction += PerformEquipmentAction;
        }

        private bool PerformEquipmentAction(On.RoR2.EquipmentSlot.orig_PerformEquipmentAction orig, RoR2.EquipmentSlot slot, EquipmentDef equipmentDef)
        {
            if (equipmentDef == EquipmentDef)
            {
                return ActivateEquipment(slot);
            }
            else
            {
                return orig(slot, equipmentDef);
            }
        }
        protected abstract bool ActivateEquipment(EquipmentSlot slot);

        public bool HasThisEquipment(CharacterBody body)
        {
            if (body && body.inventory)
            {
                for (uint i = 0; i < body.inventory.GetEquipmentSlotCount(); i++)
                {
                    var equipment = body.inventory.GetEquipment(i);
                    if (equipment.equipmentIndex == EquipmentDef.equipmentIndex)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        public bool HasThisEquipment(CharacterMaster master)
        {
            if (master && master.inventory)
            {
                for (uint i = 0; i < master.inventory.GetEquipmentSlotCount(); i++)
                {
                    var equipment = master.inventory.GetEquipment(i);
                    if (equipment.equipmentIndex == EquipmentDef.equipmentIndex)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}