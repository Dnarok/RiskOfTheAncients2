using BepInEx.Configuration;
using R2API;
using RoR2;
using System;
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
                throw new InvalidOperationException($"Singleton class \"{typeof(T).Name}\" inheriting ItemBase was instantiated twice.");
            }
            else
            {
                Instance = this as T;
            }
        }
        public static EquipmentDef GetEquipmentDef()
        {
            return Instance.EquipmentDef;
        }
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
        public abstract string EquipmentDefGUID { get; }
        public virtual string[] EquipmentSoundGUIDs { get; } = [];
        public abstract void Init(ConfigFile config);
        public virtual void Hooks()
        {

        }

        protected EquipmentDef EquipmentDef;
        protected void CreateLanguageTokens()
        {
            LanguageAPI.Add("EQUIPMENT_" + EquipmentTokenName + "_NAME", EquipmentName);
            LanguageAPI.Add("EQUIPMENT_" + EquipmentTokenName + "_PICKUP", EquipmentTokenPickup);
            LanguageAPI.Add("EQUIPMENT_" + EquipmentTokenName + "_DESCRIPTION", EquipmentTokenDesc);
            LanguageAPI.Add("EQUIPMENT_" + EquipmentTokenName + "_LORE", EquipmentTokenLore);
        }
        protected void CreateEquipmentDef()
        {
            EquipmentDef = Addressables.LoadAssetAsync<EquipmentDef>(EquipmentDefGUID).WaitForCompletion();
            EquipmentDef.name = "EQUIPMENT_" + EquipmentTokenName;
            EquipmentDef.cooldown = EquipmentCooldown;

            ItemAPI.Add(new CustomEquipment(EquipmentDef, new ItemDisplayRuleDict(null)));

            foreach (string GUID in EquipmentSoundGUIDs)
            {
                var nsed = Addressables.LoadAssetAsync<NetworkSoundEventDef>(GUID).WaitForCompletion();
                ContentAddition.AddNetworkSoundEventDef(nsed);
            }

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
    }
}