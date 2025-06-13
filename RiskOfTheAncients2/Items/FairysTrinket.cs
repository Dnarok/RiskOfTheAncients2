using BepInEx.Configuration;
using R2API;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
using System;

namespace ROTA2.Items
{
    public class FairysTrinket : ItemBase<FairysTrinket>
    {
        public override string ItemName => "Fairy's Trinket";
        public override string ConfigItemName => "Fairys Trinket";
        public override string ItemTokenName => "FAIRYS_TRINKET";
        public override string ItemTokenPickup => "Reduces skill cooldowns and increases damage and health.";
        public override string ItemTokenDesc => $"Reduces {Utility("skill cooldowns")} by {Utility($"{SkillCooldownReductionBase.Value}%")} {Stack($"(+{SkillCooldownReductionPerStack.Value}% per stack)")}, and increases {Damage("damage")} by {Damage($"{DamageBase.Value}%")} {Stack($"(+{DamagePerStack.Value}% per stack)")} and {Healing("maximum health")} by {Healing($"{MaximumHealthBase.Value}")} {Stack($"(+{MaximumHealthPerStack.Value} per stack)")}.";
        public override string ItemTokenLore => "A small token imbued with the fortune of the fae in recognition of an intriguing display of mortal kindness.";
        public override string ItemDefGUID => Assets.FairysTrinket.ItemDef;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddCooldownReduction;
            RecalculateStatsAPI.GetStatCoefficients += AddDamage;
            RecalculateStatsAPI.GetStatCoefficients += AddMaximumHealth;
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
        public void CreateConfig(ConfigFile configuration)
        {
            SkillCooldownReductionBase = configuration.Bind("Item: " + ConfigItemName, "Initial Skill Cooldown Reduction", 2.5f, "How much skill cooldown reduction should be provided by the first stack?");
            ModSettingsManager.AddOption(new FloatFieldOption(SkillCooldownReductionBase));
            SkillCooldownReductionPerStack = configuration.Bind("Item: " + ConfigItemName, "Stacking Skill Cooldown Reduction", 2.5f, "How much skill cooldown reduction should be provided by subsequent stacks?");
            ModSettingsManager.AddOption(new FloatFieldOption(SkillCooldownReductionPerStack));
            DamageBase = configuration.Bind("Item: " + ConfigItemName, "Initial Damage Bonus", 3.5f, "How much damage should be provided by the first stack?");
            ModSettingsManager.AddOption(new FloatFieldOption(DamageBase));
            DamagePerStack = configuration.Bind("Item: " + ConfigItemName, "Stacking Damage Bonus", 3.5f, "How much damage should be provided by subsequent stacks?");
            ModSettingsManager.AddOption(new FloatFieldOption(DamagePerStack));
            MaximumHealthBase = configuration.Bind("Item: " + ConfigItemName, "Initial Maximum Health Bonus", 10.0f, "How much maximum health should be provided by the first stack?");
            ModSettingsManager.AddOption(new FloatFieldOption(MaximumHealthBase));
            MaximumHealthPerStack = configuration.Bind("Item: " + ConfigItemName, "Stacking Maximum Health Bonus", 10.0f, "How much maximum health should be provided by subsequent stacks?");
            ModSettingsManager.AddOption(new FloatFieldOption(MaximumHealthPerStack));
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
    }
}