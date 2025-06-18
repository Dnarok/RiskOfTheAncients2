using BepInEx.Configuration;
using R2API;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
using ROTA2.Buffs;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace ROTA2.Items
{
    public class AssaultCuirass : ItemBase<AssaultCuirass>
    {
        public override string ItemName => "Assault Cuirass";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "ASSAULT_CUIRASS";
        public override string ItemTokenPickup => "Your allies attack faster and have additional armor.";
        public override string ItemTokenDesc => $"Increase {Damage("attack speed")} by {Damage($"{AttackSpeedBase.Value}%")} {Stack($"(+{AttackSpeedPerStack.Value}% per stack)")} and {Damage("armor")} by {Damage($"{ArmorBase.Value}")} {Stack($"(+{ArmorPerStack.Value} per stack)")} for {Healing("all allies")}.";
        public override string ItemTokenLore => "Forged in the depths of the nether reaches, this hellish mail provides an army with increased armor and attack speed.";
        public override string ItemDefGUID => Assets.AssaultCuirass.ItemDef;
        public override void Hooks()
        {
            CharacterBody.onBodyInventoryChangedGlobal += OnInventoryChanged;
        }
        public override void Init(ConfigFile configuration)
        {
            CreateConfig(configuration);
            CreateLanguageTokens();
            CreateItemDef();
            Hooks();
        }
        protected override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            var prefab = Addressables.LoadAssetAsync<GameObject>(Assets.AssaultCuirass.Display).WaitForCompletion();
            ItemDisplayRuleDict rules = new();
            rules.Add("mdlCommandoDualies",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "Chest",
                    localPos = new Vector3(-0.0001F, 0.28563F, 0.01729F),
                    localAngles = new Vector3(0F, 90F, 0F),
                    localScale = new Vector3(0.54426F, 0.5F, 0.5F)
                }
            ]);
            rules.Add("mdlHuntress",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "Chest",
                    localPos = new Vector3(0.00058F, 0.14624F, 0.04453F),
                    localAngles = new Vector3(0F, 90F, 0F),
                    localScale = new Vector3(0.43131F, 0.4F, 0.39199F)
                }
            ]);
            rules.Add("mdlToolbot",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "Chest",
                    localPos = new Vector3(0.00002F, 2.28599F, -2.01231F),
                    localAngles = new Vector3(0F, 270F, 0F),
                    localScale = new Vector3(2F, 2F, 2F)
                }
            ]);
            rules.Add("mdlEngi",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "Chest",
                    localPos = new Vector3(0F, 0.22566F, 0.07318F),
                    localAngles = new Vector3(0F, 90F, 0F),
                    localScale = new Vector3(0.5F, 0.5F, 0.54017F)
                }
            ]);
            rules.Add("mdlEngiTurret",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "Base",
                    localPos = new Vector3(0f, 0f, 0f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = new Vector3(0f, 0f, 0f),
                }
            ]);
            rules.Add("mdlEngiWalkerTurret",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "Base",
                    localPos = new Vector3(0f, 0f, 0f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = new Vector3(0f, 0f, 0f),
                }
            ]);
            rules.Add("mdlMage",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "Chest",
                    localPos = new Vector3(0F, 0.15323F, 0.02001F),
                    localAngles = new Vector3(0F, 90F, 0F),
                    localScale = new Vector3(0.41025F, 0.31603F, 0.3F)
                }
            ]);
            rules.Add("mdlMerc",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "Chest",
                    localPos = new Vector3(0F, 0.14289F, 0F),
                    localAngles = new Vector3(0F, 90F, 0F),
                    localScale = new Vector3(0.52676F, 0.45F, 0.45F)
                }
            ]);
            rules.Add("mdlTreebot",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "HeadCenter",
                    localPos = new Vector3(0F, 0F, 0F),
                    localAngles = new Vector3(0F, 90F, 0F),
                    localScale = new Vector3(0.28816F, 0.29543F, 0.25F)
                }
            ]);
            rules.Add("mdlLoader",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "Chest",
                    localPos = new Vector3(0F, 0.1767F, 0.00004F),
                    localAngles = new Vector3(0F, 90F, 0F),
                    localScale = new Vector3(0.51703F, 0.45F, 0.45F)
                }
            ]);
            rules.Add("mdlCroco",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "UpperArmR",
                    localPos = new Vector3(-0.00004F, 2.94446F, 0.00006F),
                    localAngles = new Vector3(0F, 90F, 0F),
                    localScale = new Vector3(3F, 3F, 3F)
                }
            ]);
            rules.Add("mdlCaptain",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "Chest",
                    localPos = new Vector3(0F, 0.22809F, 0.05332F),
                    localAngles = new Vector3(0F, 90F, 0F),
                    localScale = new Vector3(0.5F, 0.5F, 0.5F)
                }
            ]);
            rules.Add("mdlBandit2",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "Chest",
                    localPos = new Vector3(0F, 0.24133F, 0.04797F),
                    localAngles = new Vector3(0F, 90F, 0F),
                    localScale = new Vector3(0.45F, 0.4F, 0.4F)
                }
            ]);
            rules.Add("mdlChef",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "Chest",
                    localPos = new Vector3(0.0439F, -0.42776F, 0.12138F),
                    localAngles = new Vector3(0F, 19.89294F, 90F),
                    localScale = new Vector3(0.4192F, 0.4192F, 0.4192F)
                }
            ]);
            rules.Add("mdlRailGunner",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "Chest",
                    localPos = new Vector3(-0.00005F, 0.12137F, 0.01951F),
                    localAngles = new Vector3(0F, 90F, 0F),
                    localScale = new Vector3(0.3F, 0.3F, 0.3F)
                }
            ]);
            rules.Add("mdlVoidSurvivor",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "Chest",
                    localPos = new Vector3(0.00001F, 0.15029F, 0.02864F),
                    localAngles = new Vector3(0F, 90F, 0F),
                    localScale = new Vector3(0.45F, 0.45F, 0.45F)
                }
            ]);
            rules.Add("mdlSeeker",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "Chest",
                    localPos = new Vector3(0F, 0.15083F, -0.03776F),
                    localAngles = new Vector3(0F, 90F, 0F),
                    localScale = new Vector3(0.44328F, 0.40452F, 0.4326F)
                }
            ]);
            rules.Add("mdlFalseSon",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefab,
                    childName = "Chest",
                    localPos = new Vector3(0F, 0.16792F, -0.05655F),
                    localAngles = new Vector3(0F, 90F, 0F),
                    localScale = new Vector3(0.9F, 0.9F, 0.9F)
                }
            ]);
            return rules;
        }

        public ConfigEntry<float> AttackSpeedBase;
        public ConfigEntry<float> AttackSpeedPerStack;
        public ConfigEntry<float> ArmorBase;
        public ConfigEntry<float> ArmorPerStack;
        private void CreateConfig(ConfigFile configuration)
        {
            AttackSpeedBase = configuration.Bind("Item: " + ItemName, "Attack Speed Base", 40f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(AttackSpeedBase));
            AttackSpeedPerStack = configuration.Bind("Item: " + ItemName, "Attack Speed Per Stack", 20f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(AttackSpeedPerStack));
            ArmorBase = configuration.Bind("Item: " + ItemName, "Armor Base", 40f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(ArmorBase));
            ArmorPerStack = configuration.Bind("Item: " + ItemName, "Armor Per Stack", 20f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(ArmorPerStack));
        }

        private void OnInventoryChanged(CharacterBody body)
        {
            if (GetCount(body) > 0 && !body.GetComponent<AssaultCuirassBehavior>())
            {
                body.gameObject.AddComponent<AssaultCuirassBehavior>();
            }
        }

        private class AssaultCuirassBehavior : MonoBehaviour
        {
            CharacterBody body;
            float timer = 0f;
            void Awake()
            {
                body = GetComponent<CharacterBody>();
            }
            void FixedUpdate()
            {
                if (!NetworkServer.active || GetCount(body) <= 0 || (body.healthComponent && !body.healthComponent.alive))
                {
                    return;
                }

                timer += Time.fixedDeltaTime;
                if (timer >= 1f)
                {
                    timer -= 1f;
                    int count = AssaultCuirass.GetCount(body);
                    foreach (var member in TeamComponent.GetTeamMembers(body.teamComponent.teamIndex))
                    {
                        CharacterBody ally = member.GetComponent<CharacterBody>();
                        if (ally && ally.isActiveAndEnabled)
                        {
                            AssaultCuirassBuff.ApplyTo(
                                body: ally,
                                duration: 1.1f,
                                stacks: count,
                                max_stacks: count
                            );
                        }
                    }
                }
            }
        }
    }
}