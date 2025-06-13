using BepInEx.Configuration;
using R2API;
using RiskOfOptions;
using RiskOfOptions.Options;
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
        public override string ItemTokenDesc => $"Reduces {Utility("skill cooldowns")} by {Utility($"{SkillCooldownReductionBase.Value}%")} {Stack($"(+{SkillCooldownReductionPerStack.Value}% per stack)")} and increases {Damage("damage")} by {Damage($"{DamageBase.Value}%")} {Stack($"(+{DamagePerStack.Value}% per stack)")}, {Healing("maximum health")} by {Healing($"{MaximumHealthBase.Value}")} {Stack($"(+{MaximumHealthPerStack.Value} per stack)")}, and {Healing("base health regeneration")} by {Healing($"+{BaseHealthRegenerationBase.Value} hp/s")} {Stack($"(+{BaseHealthRegenerationPerStack.Value} hp/s per stack)")}.";
        public override string ItemTokenLore => "Two of three known items of unimaginable power that many believe were crafted at the same enchanter's forge.";
        public override string ItemDefGUID => Assets.KayaAndSange.ItemDef;
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

        public ConfigEntry<float> SkillCooldownReductionBase;
        public ConfigEntry<float> SkillCooldownReductionPerStack;
        public ConfigEntry<float> DamageBase;
        public ConfigEntry<float> DamagePerStack;
        public ConfigEntry<float> MaximumHealthBase;
        public ConfigEntry<float> MaximumHealthPerStack;
        public ConfigEntry<float> BaseHealthRegenerationBase;
        public ConfigEntry<float> BaseHealthRegenerationPerStack;
        public void CreateConfig(ConfigFile configuration)
        {
            SkillCooldownReductionBase = configuration.Bind("Item: " + ItemName, "Initial Skill Cooldown Reduction", 9.0f, "How much skill cooldown reduction should be provided by the first stack?");
            ModSettingsManager.AddOption(new FloatFieldOption(SkillCooldownReductionBase));
            SkillCooldownReductionPerStack = configuration.Bind("Item: " + ItemName, "Stacking Skill Cooldown Reduction", 9.0f, "How much skill cooldown reduction should be provided by subsequent stacks?");
            ModSettingsManager.AddOption(new FloatFieldOption(SkillCooldownReductionPerStack));
            DamageBase = configuration.Bind("Item: " + ItemName, "Initial Damage Bonus", 18.0f, "How much damage should be provided by the first stack?");
            ModSettingsManager.AddOption(new FloatFieldOption(DamageBase));
            DamagePerStack = configuration.Bind("Item: " + ItemName, "Stacking Damage Bonus", 18.0f, "How much damage should be provided by subsequent stacks?");
            ModSettingsManager.AddOption(new FloatFieldOption(DamagePerStack));
            MaximumHealthBase = configuration.Bind("Item: " + ItemName, "Initial Maximum Health Bonus", 60.0f, "How much maximum health should be provided by the first stack?");
            ModSettingsManager.AddOption(new FloatFieldOption(MaximumHealthBase));
            MaximumHealthPerStack = configuration.Bind("Item: " + ItemName, "Stacking Maximum Health Bonus", 60.0f, "How much maximum health should be provided by subsequent stacks?");
            ModSettingsManager.AddOption(new FloatFieldOption(MaximumHealthPerStack));
            BaseHealthRegenerationBase = configuration.Bind("Item: " + ItemName, "Initial Base Health Regeneration Bonus", 2.4f, "How much base health regeneration should be provided by the first stack?");
            ModSettingsManager.AddOption(new FloatFieldOption(BaseHealthRegenerationBase));
            BaseHealthRegenerationPerStack = configuration.Bind("Item: " + ItemName, "Stacking Base Health Regeneration Bonus", 2.4f, "How much base health regeneration should be provided by subsequent stacks?");
            ModSettingsManager.AddOption(new FloatFieldOption(BaseHealthRegenerationPerStack));
        }

        private void AddCooldownReduction(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs arguments)
        {
            int count = GetCount(body);
            if (count == 1)
            {
                arguments.cooldownMultAdd -= 1.0f - (1.0f - SkillCooldownReductionBase.Value / 100.0f);
            }
            else if (count > 1)
            {
                arguments.cooldownMultAdd -= 1.0f - (1.0f - SkillCooldownReductionBase.Value / 100.0f) * (float)Math.Pow(1.0f - SkillCooldownReductionPerStack.Value / 100.0f, count - 1);
            }
        }
        private void AddDamage(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs arguments)
        {
            int count = GetCount(body);
            if (count > 0)
            {
                arguments.damageMultAdd += DamageBase.Value / 100.0f + DamagePerStack.Value / 100.0f * (count - 1);
            }
        }
        private void AddMaximumHealth(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs arguments)
        {
            int count = GetCount(body);
            if (count > 0)
            {
                arguments.baseHealthAdd += MaximumHealthBase.Value + MaximumHealthPerStack.Value * (count - 1);
            }
        }
        private void AddBaseHealthRegeneration(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs arguments)
        {
            int count = GetCount(body);
            if (count > 0)
            {
                arguments.baseRegenAdd += (BaseHealthRegenerationBase.Value + BaseHealthRegenerationPerStack.Value * (count - 1)) * (1 + 0.2f * body.level);
            }
        }
    }
}