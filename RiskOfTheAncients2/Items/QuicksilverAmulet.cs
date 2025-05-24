using BepInEx.Configuration;
using R2API;
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
        public override string ItemTokenDesc => $"Increase {Damage("attack speed")} by {Damage($"{AttackSpeedBase}%")} {Stack($"(+{AttackSpeedPerStack}% per stack)")} and {Utility("movement speed")} by {Utility($"{MovementSpeedBase}%")} {Stack($"(+{MovementSpeedPerStack}% per stack)")} for each {Utility("skill on cooldown")}.";
        public override string ItemTokenLore => "An enchanted talisman brimming with a mysterious substance.";
        public override ItemTier Tier => ItemTier.Tier2;
        public override ItemTag[] ItemTags => [ItemTag.Damage, ItemTag.Utility];
        public override string ItemIconPath => "ROTA2.Icons.quicksilver_amulet.png";
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

        public float AttackSpeedBase;
        public float AttackSpeedPerStack;
        public float MovementSpeedBase;
        public float MovementSpeedPerStack;
        private void CreateConfig(ConfigFile configuration)
        {
            AttackSpeedBase       = configuration.Bind("Item: " + ItemName, "Attack Speed Base", 10.0f, "").Value;
            AttackSpeedPerStack   = configuration.Bind("Item: " + ItemName, "Attack Speed Per Stack", 10.0f, "").Value;
            MovementSpeedBase     = configuration.Bind("Item: " + ItemName, "Movement Speed Base", 10.0f, "").Value;
            MovementSpeedPerStack = configuration.Bind("Item: " + ItemName, "Movement Speed Per Stack", 10.0f, "").Value;
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

                args.attackSpeedMultAdd += (AttackSpeedBase / 100.0f + AttackSpeedPerStack / 100.0f * (count - 1)) * cooldowns;
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

                args.moveSpeedMultAdd += (MovementSpeedBase / 100.0f + MovementSpeedPerStack / 100.0f * (count - 1)) * cooldowns;
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
            void Update()
            {
                if (QuicksilverAmulet.Instance.GetCount(body) <= 0 || !body.skillLocator || !NetworkServer.active || (body.healthComponent && !body.healthComponent.alive))
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
                        body.RecalculateStats();
                    }
                }
            }
        }
    }
}