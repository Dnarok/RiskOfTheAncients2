using BepInEx;
using ROTA2.Buffs;
using ROTA2.Equipment;
using ROTA2.Items;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Skills;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using BepInEx.Bootstrap;
using UnityEngine.AddressableAssets;
using Newtonsoft.Json.Utilities;

namespace ROTA2
{
    [BepInDependency(ItemAPI.PluginGUID)]
    [BepInDependency(LanguageAPI.PluginGUID)]
    [BepInDependency(RecalculateStatsAPI.PluginGUID)]
    [BepInDependency(CommandHelper.PluginGUID)]
    [BepInDependency(DotAPI.PluginGUID)]
    [BepInDependency("droppod.lookingglass", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]

    public class Plugin : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "Dnarok";
        public const string PluginName = "RiskOfTheAncients2";
        public const string PluginVersion = "1.1.7";

        public static List<ItemBase> Items = [];
        public static Dictionary<ItemBase, bool> ItemsEnabled = [];
        public static List<EquipmentBase> Equipment = [];
        public static Dictionary<EquipmentBase, bool> EquipmentEnabled = [];
        public static List<BuffBase> Buffs = [];

        public static Dictionary<string, Sprite> SpritesLoaded = [];
        public static SkillDef disabledSkill;
        public static AssetBundle bundle;

        public static bool isLookingGlassInstalled => Chainloader.PluginInfos.ContainsKey("droppod.lookingglass");

        public void Awake()
        {
            Log.Init(Logger);
            CommandHelper.AddToConsoleWhenReady();
            string[] resources = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            foreach (string resource in resources)
            {
                Log.Info(resource);
            }

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ROTA2.models"))
            {
                bundle = AssetBundle.LoadFromStream(stream);
            }

            string[] assets = bundle.GetAllAssetNames();
            foreach (string asset in assets)
            {
                Log.Info(asset);
            }

            // foreach (var material in bundle.LoadAllAssets<Material>())
            // {
            //     var shaderName = material.shader.name;
            //     Log.Debug("Trying to swap " + shaderName);
            //     if (shaderName.Contains("Stubbed"))
            //     {
            //         shaderName = shaderName.Replace("Stubbed", string.Empty) + ".shader";
            //         var replacementShader = Addressables.LoadAssetAsync<Shader>(shaderName).WaitForCompletion();
            // 
            //         if (replacementShader != null)
            //         {
            //             material.shader = replacementShader;
            //             Log.Debug("Successfully swapped to " + shaderName);
            //         }
            //         else
            //         {
            //             Log.Error("Failed to load shader " + shaderName);
            //         }
            //     }
            // }

            var ItemTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(ItemBase)));
            foreach (var type in ItemTypes)
            {
                ItemBase item = (ItemBase)System.Activator.CreateInstance(type);
                ItemsEnabled.Add(item, Config.Bind("Item: " + item.ConfigItemName, "Enabled", true, "Should this item be available?").Value);
            }

            var EquipmentTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(EquipmentBase)));
            foreach (var type in EquipmentTypes)
            {
                EquipmentBase equipment = (EquipmentBase)System.Activator.CreateInstance(type);
                EquipmentEnabled.Add(equipment, Config.Bind("Item: " + equipment.EquipmentName, "Enabled", true, "Should this item be available?").Value);
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
                var captain = LegacyResourcesAPI.Load<SkillDef>("SkillDefs/CaptainBody/CaptainSkillUsedUp");

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

                ContentAddition.AddSkillDef(disabledSkill);
            }

            if (isLookingGlassInstalled)
            {
                RoR2Application.onLoad += Compatibility.LookingGlassCompatibility;
            }

            // network debugging
            // On.RoR2.Networking.NetworkManagerSystemSteam.OnClientConnect += (s, u, t) => { };
        }

        private void Update()
        {

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

        public static byte[] ExtractResource(string filename)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using Stream resource = assembly.GetManifestResourceStream(filename);
            if (resource == null)
            {
                Log.Warning($"Manifest resource stream was null for {filename}.");
                return null;
            }

            byte[] array = new byte[resource.Length];
            int amount = resource.Read(array, 0, array.Length);
            if (amount == 0)
            {
                Log.Warning($"Read 0 bytes from resource {filename}.");
                return null;
            }
            else
            {
                return array;
            }
        }
        public static Sprite ExtractSprite(string filename)
        {
            Sprite sprite = null;
            if (SpritesLoaded.TryGetValue(filename, out sprite))
            {
                return sprite;
            }

            if (filename == "")
            {
                return Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/MiscIcons/texMysteryIcon.png").WaitForCompletion();
            }
            byte[]? icon_bytes = ExtractResource(filename);
            if (icon_bytes == null)
            {
                Log.Warning($"Failed to extract bytes from resource \"{filename}\". Falling back to mystery icon.");
                return Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/MiscIcons/texMysteryIcon.png").WaitForCompletion();
            }
            else
            {
                Texture2D icon_texture = new(512, 512);
                if (!ImageConversion.LoadImage(icon_texture, icon_bytes))
                {
                    Log.Warning($"Failed to convert resource bytes from \"{filename}\" into texture. Falling back to mystery icon.");
                    return Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/MiscIcons/texMysteryIcon.png").WaitForCompletion();
                }
                else
                {
                    Log.Info($"Successfully loaded texture from resource \"{filename}\": {icon_texture.width}x{icon_texture.height}, {icon_texture.graphicsFormat:G}.");
                    Sprite result = Sprite.Create(icon_texture, new Rect(0.0f, 0.0f, icon_texture.width, icon_texture.height), new Vector2(0.5f, 0.5f));
                    SpritesLoaded.Add(filename, result);
                    return result;
                }
            }
        }

        [ConCommand(commandName = "D2M_GiveItem", flags = ConVarFlags.None, helpText = "Gives the item by its token name (e.g. HEART_OF_TARRASQUE).")]
        private static void GiveItemCommand(ConCommandArgs arguments)
        {
            if (arguments.senderBody && arguments.senderBody.inventory)
            {
                string token = arguments.GetArgString(0);
                ItemBase? item = ItemsEnabled.Keys.ToList().Find(x => x.ItemTokenName == token);
                if (item != null)
                {
                    Log.Info($"Command and item recognized. Spawning {item.ItemName} at coordinates {arguments.senderMaster.GetBodyObject().transform.position}.");
                    PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(item.ItemDef.itemIndex), arguments.senderMaster.GetBodyObject().transform.position, arguments.senderMaster.GetBodyObject().transform.forward * 20.0f);
                }
                else
                {
                    EquipmentBase? equipment = EquipmentEnabled.Keys.ToList().Find(x => x.EquipmentTokenName == token);
                    if (equipment != null)
                    {
                        Log.Info($"Command and equipment recognized. Spawning {equipment.EquipmentName} at coordinates {arguments.senderMaster.GetBodyObject().transform.position}.");
                        PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(equipment.EquipmentDef.equipmentIndex), arguments.senderMaster.GetBodyObject().transform.position, arguments.senderMaster.GetBodyObject().transform.forward * 20.0f);
                    }
                }
            }
        }
        [ConCommand(commandName = "D2M_RemoveItem", flags = ConVarFlags.None, helpText = "Removes the item by its token name (e.g. HEART_OF_TARRASQUE).")]
        private static void RemoveItemCommand(ConCommandArgs arguments)
        {
            if (arguments.senderBody && arguments.senderBody.inventory)
            {
                string token = arguments.GetArgString(0);
                ItemBase? item = ItemsEnabled.Keys.ToList().Find(x => x.ItemTokenName == token);
                if (item != null && arguments.senderBody.inventory)
                {
                    Log.Info($"Command and item recognized. Removing {item.ItemName} from {arguments.sender.name}.");
                    arguments.senderBody.inventory.RemoveItem(item.ItemDef);
                }
                else
                {
                    EquipmentBase? equipment = EquipmentEnabled.Keys.ToList().Find(x => x.EquipmentTokenName == token);
                    if (equipment != null && arguments.senderBody.inventory)
                    {
                        Log.Info($"Command and item recognized. Removing {equipment.EquipmentName} from {arguments.sender.name}.");
                        arguments.senderBody.inventory.SetEquipmentIndex(EquipmentIndex.None);
                    }
                }
            }
        }
    }
}