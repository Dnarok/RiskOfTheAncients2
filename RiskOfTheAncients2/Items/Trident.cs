using BepInEx.Configuration;
using R2API;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace ROTA2.Items
{
    public class Trident : ItemBase<Trident>
    {
        public override string ItemName => "Trident";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "TRIDENT";
        public override string ItemTokenPickup => "Reduces skill cooldowns and increases damage, maximum health, base health regeneration, attack speed, and movement speed.";
        public override string ItemTokenDesc => $"Reduces {Utility("skill cooldowns")} by {Utility($"{SkillCooldownReductionBase}%")} {Stack($"(+{SkillCooldownReductionPerStack}% per stack)")} and increases {Damage("damage")} by {Damage($"{DamageBase}%")} {Stack($"(+{DamagePerStack}% per stack)")}, {Healing("maximum health")} by {Healing($"{MaximumHealthBase}")} {Stack($"(+{MaximumHealthPerStack} per stack)")}, {Healing("base health regeneration")} by {Healing($"+{BaseHealthRegenerationBase} hp/s")} {Stack($"(+{BaseHealthRegenerationPerStack} hp/s per stack)")}, {Damage("attack speed")} by {Damage($"{AttackSpeedBase}%")} {Stack($"(+{AttackSpeedPerStack}% per stack)")}, and {Utility("movement speed")} by {Utility($"{MovementSpeedBase}%")} {Stack($"(+{MovementSpeedPerStack}% per stack)")}.";
        public override string ItemTokenLore => "A weapon capable of killing even gods, it was fractured into the three mighty weapons to prevent anyone from misusing its power.";
        public override ItemTier Tier => ItemTier.Tier3;
        public override ItemTag[] ItemTags => [ItemTag.WorldUnique];
        public override string ItemIconPath => "ROTA2.Icons.trident.png";
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddCooldownReduction;
            RecalculateStatsAPI.GetStatCoefficients += AddDamage;
            RecalculateStatsAPI.GetStatCoefficients += AddMaximumHealth;
            RecalculateStatsAPI.GetStatCoefficients += AddBaseHealthRegeneration;
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

        public float SkillCooldownReductionBase;
        public float SkillCooldownReductionPerStack;
        public float DamageBase;
        public float DamagePerStack;
        public float MaximumHealthBase;
        public float MaximumHealthPerStack;
        public float BaseHealthRegenerationBase;
        public float BaseHealthRegenerationPerStack;
        public float AttackSpeedBase;
        public float AttackSpeedPerStack;
        public float MovementSpeedBase;
        public float MovementSpeedPerStack;
        public void CreateConfig(ConfigFile configuration)
        {
            SkillCooldownReductionBase = configuration.Bind("Item: " + ItemName, "Initial Skill Cooldown Reduction", 12.0f, "How much skill cooldown reduction should be provided by the first stack?").Value;
            SkillCooldownReductionPerStack = configuration.Bind("Item: " + ItemName, "Stacking Skill Cooldown Reduction", 12.0f, "How much skill cooldown reduction should be provided by subsequent stacks?").Value;
            DamageBase = configuration.Bind("Item: " + ItemName, "Initial Damage Bonus", 24.0f, "How much damage should be provided by the first stack?").Value;
            DamagePerStack = configuration.Bind("Item: " + ItemName, "Stacking Damage Bonus", 24.0f, "How much damage should be provided by subsequent stacks?").Value;
            MaximumHealthBase = configuration.Bind("Item: " + ItemName, "Initial Maximum Health Bonus", 80.0f, "How much maximum health should be provided by the first stack?").Value;
            MaximumHealthPerStack = configuration.Bind("Item: " + ItemName, "Stacking Maximum Health Bonus", 80.0f, "How much maximum health should be provided by subsequent stacks?").Value;
            BaseHealthRegenerationBase = configuration.Bind("Item: " + ItemName, "Initial Base Health Regeneration Bonus", 3.2f, "How much base health regeneration should be provided by the first stack?").Value;
            BaseHealthRegenerationPerStack = configuration.Bind("Item: " + ItemName, "Stacking Base Health Regeneration Bonus", 3.2f, "How much base health regeneration should be provided by subsequent stacks?").Value;
            AttackSpeedBase = configuration.Bind("Item: " + ItemName, "Initial Attack Speed Bonus", 30.0f, "How much attack speed should be provided by the first stack?").Value;
            AttackSpeedPerStack = configuration.Bind("Item: " + ItemName, "Stacking Attack Speed Bonus", 30.0f, "How much attack speed should be provided by subsequent stacks?").Value;
            MovementSpeedBase = configuration.Bind("Item: " + ItemName, "Initial Movement Speed Bonus", 30.0f, "How much movement speed should be provided by the first stack?").Value;
            MovementSpeedPerStack = configuration.Bind("Item: " + ItemName, "Stacking Movement Speed Bonus", 30.0f, "How much movement speed should be provided by subsequent stacks?").Value;
        }

        private void AddCooldownReduction(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs arguments)
        {
            int count = GetCount(body);
            if (count == 1)
            {
                arguments.cooldownMultAdd -= 1.0f - (1.0f - SkillCooldownReductionBase / 100.0f);
            }
            else if (count > 1)
            {
                arguments.cooldownMultAdd -= 1.0f - (1.0f - SkillCooldownReductionBase / 100.0f) * (float)Math.Pow(1.0f - SkillCooldownReductionPerStack / 100.0f, count - 1);
            }
        }
        private void AddDamage(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs arguments)
        {
            int count = GetCount(body);
            if (count > 0)
            {
                arguments.damageMultAdd += DamageBase / 100.0f + DamagePerStack / 100.0f * (count - 1);
            }
        }
        private void AddMaximumHealth(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs arguments)
        {
            int count = GetCount(body);
            if (count > 0)
            {
                arguments.baseHealthAdd += MaximumHealthBase + MaximumHealthPerStack * (count - 1);
            }
        }
        private void AddBaseHealthRegeneration(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs arguments)
        {
            int count = GetCount(body);
            if (count > 0)
            {
                arguments.baseRegenAdd += BaseHealthRegenerationBase + BaseHealthRegenerationPerStack * (count - 1);
            }
        }
        private void AddAttackSpeed(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs arguments)
        {
            int count = GetCount(body);
            if (count > 0)
            {
                arguments.attackSpeedMultAdd += AttackSpeedBase / 100.0f + AttackSpeedPerStack / 100.0f * (count - 1);
            }
        }
        private void AddMovementSpeed(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs arguments)
        {
            int count = GetCount(body);
            if (count > 0)
            {
                arguments.moveSpeedMultAdd += MovementSpeedBase / 100.0f + MovementSpeedPerStack / 100.0f * (count - 1);
            }
        }
    }
}