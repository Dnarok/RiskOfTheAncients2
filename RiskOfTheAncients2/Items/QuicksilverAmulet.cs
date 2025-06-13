using BepInEx.Configuration;
using R2API;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ROTA2.Items
{
    public class QuicksilverAmulet : ItemBase<QuicksilverAmulet>
    {
        public override string ItemName => "Quicksilver Amulet";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "QUICKSILVER_AMULET";
        public override string ItemTokenPickup => "Increase attack and movement speed for each skill on cooldown.";
        public override string ItemTokenDesc => $"Increase {Damage("attack speed")} by {Damage($"{AttackSpeedBase.Value}%")} {Stack($"(+{AttackSpeedPerStack.Value}% per stack)")} and {Utility("movement speed")} by {Utility($"{MovementSpeedBase.Value}%")} {Stack($"(+{MovementSpeedPerStack.Value}% per stack)")} for each {Utility("skill on cooldown")}.";
        public override string ItemTokenLore => "An enchanted talisman brimming with a mysterious substance.";
        public override string ItemDefGUID => Assets.QuicksilverAmulet.ItemDef;
        public override void Hooks()
        {
            CharacterBody.onBodyInventoryChangedGlobal += OnInventoryChanged;
            RecalculateStatsAPI.GetStatCoefficients += AddAttackSpeed;
            RecalculateStatsAPI.GetStatCoefficients += AddMovementSpeed;
        }

        public override void Init(ConfigFile configuration)
        {
            CreateConfig(configuration);
            CreateLanguageTokens();
            CreateItemDef();
            Hooks();
        }

        public ConfigEntry<float> AttackSpeedBase;
        public ConfigEntry<float> AttackSpeedPerStack;
        public ConfigEntry<float> MovementSpeedBase;
        public ConfigEntry<float> MovementSpeedPerStack;
        private void CreateConfig(ConfigFile configuration)
        {
            AttackSpeedBase = configuration.Bind("Item: " + ItemName, "Attack Speed Base", 10.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(AttackSpeedBase));
            AttackSpeedPerStack = configuration.Bind("Item: " + ItemName, "Attack Speed Per Stack", 10.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(AttackSpeedPerStack));
            MovementSpeedBase = configuration.Bind("Item: " + ItemName, "Movement Speed Base", 10.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(MovementSpeedBase));
            MovementSpeedPerStack = configuration.Bind("Item: " + ItemName, "Movement Speed Per Stack", 10.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(MovementSpeedPerStack));
        }

        private void AddAttackSpeed(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            int count = GetCount(body);
            if (count > 0)
            {
                int cooldowns = 0;
                var skills = body.skillLocator.allSkills;
                if (skills != null)
                {
                    foreach (var skill in skills)
                    {
                        if (skill && skill.cooldownRemaining > 0.0f)
                        {
                            ++cooldowns;
                        }
                    }
                }

                args.attackSpeedMultAdd += (AttackSpeedBase.Value / 100.0f + AttackSpeedPerStack.Value / 100.0f * (count - 1)) * cooldowns;
            }
        }
        private void AddMovementSpeed(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            int count = GetCount(body);
            if (count > 0)
            {
                int cooldowns = 0;
                var skills = body.skillLocator.allSkills;
                if (skills != null)
                {
                    foreach (var skill in skills)
                    {
                        if (skill && skill.cooldownRemaining > 0.0f)
                        {
                            ++cooldowns;
                        }
                    }
                }

                args.moveSpeedMultAdd += (MovementSpeedBase.Value / 100.0f + MovementSpeedPerStack.Value / 100.0f * (count - 1)) * cooldowns;
            }
        }
        private void OnInventoryChanged(CharacterBody body)
        {
            if (GetCount(body) > 0 && !body.GetComponent<QuicksilverAmuletBehavior>())
            {
                body.gameObject.AddComponent<QuicksilverAmuletBehavior>();
            }
        }

        private class QuicksilverAmuletBehavior : MonoBehaviour
        {
            CharacterBody body;
            Dictionary<GenericSkill, int> stocks;

            void Awake()
            {
                body = GetComponent<CharacterBody>();
                stocks = new();
            }
            void FixedUpdate()
            {
                if (QuicksilverAmulet.GetCount(body) <= 0 || !body.skillLocator || !NetworkServer.active || (body.healthComponent && !body.healthComponent.alive))
                {
                    return;
                }

                var skills = body.skillLocator.allSkills;
                if (skills != null)
                {
                    bool recalculate = false;
                    foreach (var skill in skills)
                    {
                        int last_stock = -1;
                        stocks.TryGetValue(skill, out last_stock);
                        if (skill.stock < last_stock)
                        {
                            recalculate = true;
                        }
                        else if (skill.stock == skill.maxStock && skill.stock > last_stock)
                        {
                            recalculate = true;
                        }

                        stocks[skill] = skill.stock;
                    }

                    if (recalculate)
                    {
                        body.MarkAllStatsDirty();
                    }
                }
            }
        }
    }
}