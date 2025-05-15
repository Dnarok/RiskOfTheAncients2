using BepInEx.Configuration;
using R2API;
using RoR2;
using System;

namespace ROTA2.Items
{
    public class KayaAndSange : ItemBase<KayaAndSange>
    {
        public override string ItemName => "Kaya and Sange";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "KAYA_AND_SANGE";
        public override string ItemTokenPickup => "Reduces skill cooldowns and increases damage, maximum health, and base health regeneration. Combines with Yasha.";
        public override string ItemTokenDesc => $"Reduces {Utility("skill cooldowns")} by {Utility($"{SkillCooldownReductionBase}%")} {Stack($"(+{SkillCooldownReductionPerStack}% per stack)")} and increases {Damage("damage")} by {Damage($"{DamageBase}%")} {Stack($"(+{DamagePerStack}% per stack)")}, {Healing("maximum health")} by {Healing($"{MaximumHealthBase}")} {Stack($"(+{MaximumHealthPerStack} per stack)")}, and {Healing("base health regeneration")} by {Healing($"+{BaseHealthRegenerationBase} hp/s")} {Stack($"(+{BaseHealthRegenerationPerStack} hp/s per stack)")}.";
        public override string ItemTokenLore => "Two of three known items of unimaginable power that many believe were crafted at the same enchanter's forge.";
        public override ItemTier Tier => ItemTier.Tier2;
        public override ItemTag[] ItemTags => [ItemTag.WorldUnique];
        public override string ItemIconPath => "ROTA2.Icons.kaya_and_sange.png";
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddCooldownReduction;
            RecalculateStatsAPI.GetStatCoefficients += AddDamage;
            RecalculateStatsAPI.GetStatCoefficients += AddMaximumHealth;
            RecalculateStatsAPI.GetStatCoefficients += AddBaseHealthRegeneration;
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
        public void CreateConfig(ConfigFile configuration)
        {
            SkillCooldownReductionBase = configuration.Bind("Item: " + ItemName, "Initial Skill Cooldown Reduction", 9.0f, "How much skill cooldown reduction should be provided by the first stack?").Value;
            SkillCooldownReductionPerStack = configuration.Bind("Item: " + ItemName, "Stacking Skill Cooldown Reduction", 9.0f, "How much skill cooldown reduction should be provided by subsequent stacks?").Value;
            DamageBase = configuration.Bind("Item: " + ItemName, "Initial Damage Bonus", 18.0f, "How much damage should be provided by the first stack?").Value;
            DamagePerStack = configuration.Bind("Item: " + ItemName, "Stacking Damage Bonus", 18.0f, "How much damage should be provided by subsequent stacks?").Value;
            MaximumHealthBase = configuration.Bind("Item: " + ItemName, "Initial Maximum Health Bonus", 60.0f, "How much maximum health should be provided by the first stack?").Value;
            MaximumHealthPerStack = configuration.Bind("Item: " + ItemName, "Stacking Maximum Health Bonus", 60.0f, "How much maximum health should be provided by subsequent stacks?").Value;
            BaseHealthRegenerationBase = configuration.Bind("Item: " + ItemName, "Initial Base Health Regeneration Bonus", 2.4f, "How much base health regeneration should be provided by the first stack?").Value;
            BaseHealthRegenerationPerStack = configuration.Bind("Item: " + ItemName, "Stacking Base Health Regeneration Bonus", 2.4f, "How much base health regeneration should be provided by subsequent stacks?").Value;
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
    }
}