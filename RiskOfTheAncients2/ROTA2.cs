using BepInEx;
using BepInEx.Bootstrap;
using R2API;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
using RoR2.Skills;
using ROTA2.Buffs;
using ROTA2.Equipment;
using ROTA2.Items;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ROTA2
{
    [BepInDependency(ItemAPI.PluginGUID)]
    [BepInDependency(LanguageAPI.PluginGUID)]
    [BepInDependency(RecalculateStatsAPI.PluginGUID)]
    [BepInDependency(DotAPI.PluginGUID)]
    [BepInDependency("com.rune580.riskofoptions")]
    [BepInDependency("droppod.lookingglass", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "Dnarok";
        public const string PluginName = "RiskOfTheAncients2";
        public const string PluginVersion = "2.0.0";

        public static List<ItemBase> Items = [];
        public static Dictionary<ItemBase, bool> ItemsEnabled = [];
        public static List<EquipmentBase> Equipment = [];
        public static Dictionary<EquipmentBase, bool> EquipmentEnabled = [];
        public static List<BuffBase> Buffs = [];

        public static Dictionary<string, Sprite> SpritesLoaded = [];
        public static SkillDef disabledSkill;
        public static string AddressablesDirectory { get; private set; }

        public static bool isLookingGlassInstalled => Chainloader.PluginInfos.ContainsKey("droppod.lookingglass");

        public void Awake()
        {
            Log.Init(Logger);

            Log.Info("Loading Addressables catalog.");
            AddressablesDirectory = System.IO.Path.GetDirectoryName(Info.Location);
            Addressables.LoadContentCatalogAsync(System.IO.Path.Combine(AddressablesDirectory, "catalog.json")).WaitForCompletion();
            Log.Info("Addressables catalog loaded.");

            ModSettingsManager.SetModDescription("Dota 2 items in Risk of Rain 2!");
            Addressables.LoadAssetAsync<Sprite>(Assets.Default.ModIcon).Completed += (x) => ModSettingsManager.SetModIcon(x.Result);

            var ItemTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(ItemBase)));
            foreach (var type in ItemTypes)
            {
                ItemBase item = (ItemBase)System.Activator.CreateInstance(type);
                var config = Config.Bind("Item: " + item.ConfigItemName, "Enabled", true, "Should this item be available?");
                ModSettingsManager.AddOption(new CheckBoxOption(config, true));
                ItemsEnabled.Add(item, config.Value);
            }

            var EquipmentTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(EquipmentBase)));
            foreach (var type in EquipmentTypes)
            {
                EquipmentBase equipment = (EquipmentBase)System.Activator.CreateInstance(type);
                var config = Config.Bind("Item: " + equipment.EquipmentName, "Enabled", true, "Should this item be available?");
                ModSettingsManager.AddOption(new CheckBoxOption(config, true));
                EquipmentEnabled.Add(equipment, config.Value);
            }

            foreach (var pair in ItemsEnabled)
            {
                if (pair.Value)
                {
                    Items.Add(pair.Key);
                    pair.Key.Init(Config);
                    Log.Info($"Item: {pair.Key.ItemName} initialized!");
                }
                else
                {
                    Log.Warning($"Item: {pair.Key.ItemName} NOT initialized.");
                }
            }

            foreach (var pair in EquipmentEnabled)
            {
                if (pair.Value)
                {
                    Equipment.Add(pair.Key);
                    pair.Key.Init(Config);
                    Log.Info($"Equipment: {pair.Key.EquipmentName} initialized!");
                }
                else
                {
                    Log.Warning($"Equipment: {pair.Key.EquipmentName} NOT initialized.");
                }
            }

            var BuffTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(BuffBase)));
            foreach (var type in BuffTypes)
            {
                BuffBase buff = (BuffBase)System.Activator.CreateInstance(type);
                Buffs.Add(buff);
                buff.Init();
                Log.Info($"Buff: {buff.BuffName} initialized!");
            }

            RoR2Application.onLoad += () =>
            {
                Log.Debug("Initializing RecipeManager.");
                RecipeManager.Init();
            };

            On.RoR2.Items.ContagiousItemManager.Init += (orig) =>
            {
                List<ItemDef.Pair> pairs = new();
                Log.Debug("Adding void items.");

                foreach (ItemBase item in Items)
                {
                    if (item.VoidFor != null)
                    {
                        Log.Debug($"Pairing {item.ItemDef.nameToken} to {item.VoidFor.nameToken}.");
                        pairs.Add(new ItemDef.Pair()
                        {
                            itemDef1 = item.VoidFor,
                            itemDef2 = item.ItemDef
                        });
                    }
                }

                var relationships = ItemCatalog.itemRelationships[DLC1Content.ItemRelationshipTypes.ContagiousItem];
                ItemCatalog.itemRelationships[DLC1Content.ItemRelationshipTypes.ContagiousItem] = relationships.Union(pairs).ToArray();
                orig();
            };

            if (disabledSkill == null)
            {
                Addressables.LoadAssetAsync<SkillDef>(RoR2BepInExPack.GameAssetPaths.RoR2_Base_Captain.CaptainSkillUsedUp_asset).Completed += (x) =>
                {
                    var captain = x.Result;

                    disabledSkill = ScriptableObject.CreateInstance<SkillDef>();
                    disabledSkill.skillName = captain.skillName;
                    disabledSkill.skillNameToken = captain.skillNameToken;
                    disabledSkill.skillDescriptionToken = captain.skillDescriptionToken;
                    disabledSkill.icon = captain.icon;
                    disabledSkill.activationStateMachineName = captain.activationStateMachineName;
                    disabledSkill.activationState = captain.activationState;
                    disabledSkill.interruptPriority = captain.interruptPriority;
                    disabledSkill.baseRechargeInterval = captain.baseRechargeInterval;
                    disabledSkill.baseMaxStock = captain.baseMaxStock;
                    disabledSkill.rechargeStock = captain.rechargeStock;
                    disabledSkill.requiredStock = captain.requiredStock;
                    disabledSkill.stockToConsume = captain.stockToConsume;
                    disabledSkill.beginSkillCooldownOnSkillEnd = captain.beginSkillCooldownOnSkillEnd;
                    disabledSkill.fullRestockOnAssign = captain.fullRestockOnAssign;
                    disabledSkill.dontAllowPastMaxStocks = captain.dontAllowPastMaxStocks;
                    disabledSkill.canceledFromSprinting = captain.canceledFromSprinting;
                    disabledSkill.isCombatSkill = captain.isCombatSkill;
                    disabledSkill.resetCooldownTimerOnUse = captain.resetCooldownTimerOnUse;
                    disabledSkill.cancelSprintingOnActivation = captain.cancelSprintingOnActivation;
                    disabledSkill.canceledFromSprinting = captain.canceledFromSprinting;
                    disabledSkill.forceSprintDuringState = captain.forceSprintDuringState;
                    disabledSkill.mustKeyPress = captain.mustKeyPress;
                    disabledSkill.keywordTokens = captain.keywordTokens;

                    Log.Debug("Disabled skill added.");
                    ContentAddition.AddSkillDef(disabledSkill);
                };
            }

            if (isLookingGlassInstalled)
            {
                RoR2Application.onLoad += Compatibility.LookingGlassCompatibility;
            }

            // network debugging
            // On.RoR2.Networking.NetworkManagerSystemSteam.OnClientConnect += (s, u, t) => { };
        }

        public static void TrySwapShadersInPrefab(GameObject prefab)
        {
            foreach (var renderer in prefab.GetComponentsInChildren<Renderer>())
            {
                var material = renderer.material;
                var shaderName = material.shader.name;
                Log.Debug("Trying to swap " + shaderName + " in " + prefab.name);
                if (shaderName.Contains("Stubbed"))
                {
                    shaderName = shaderName.Replace("Stubbed", string.Empty) + ".shader";
                    var replacementShader = Addressables.LoadAssetAsync<Shader>(shaderName).WaitForCompletion();

                    if (replacementShader != null)
                    {
                        material.shader = replacementShader;
                        Log.Debug("Successfully swapped to " + shaderName);
                    }
                    else
                    {
                        Log.Error("Failed to load shader " + shaderName);
                    }
                }
            }
        }
        public static void SetupSoundEvent(string GUID, NetworkSoundEventDef sound)
        {
            Addressables.LoadAssetAsync<NetworkSoundEventDef>(GUID).Completed += (x) =>
            {
                ContentAddition.AddNetworkSoundEventDef(x.Result);
                sound = x.Result;
            };
        }
    }
}