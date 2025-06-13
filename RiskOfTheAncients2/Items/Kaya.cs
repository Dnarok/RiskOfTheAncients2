using BepInEx.Configuration;
using R2API;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
using System;

namespace ROTA2.Items
{
    public class Kaya : ItemBase<Kaya>
    {
        public override string ItemName => "Kaya";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "KAYA";
        public override string ItemTokenPickup => "Reduces skill cooldowns and increases damage. Combines with Yasha or Sange.";
        public override string ItemTokenDesc => $"Reduces {Utility("skill cooldowns")} by {Utility($"{SkillCooldownReductionBase.Value}%")} {Stack($"(+{SkillCooldownReductionPerStack.Value}% per stack)")} and increases {Damage("damage")} by {Damage($"{DamageBase.Value}%")} {Stack($"(+{DamagePerStack.Value}% per stack)")}.";
        public override string ItemTokenLore => "The staff of a renowned sorceress, lost for countless millennia.";
        public override string ItemDefGUID => Assets.Kaya.ItemDef;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddCooldownReduction;
            RecalculateStatsAPI.GetStatCoefficients += AddDamage;
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
        public void CreateConfig(ConfigFile configuration)
        {
            SkillCooldownReductionBase = configuration.Bind("Item: " + ItemName, "Initial Skill Cooldown Reduction", 6.0f, "How much skill cooldown reduction should be provided by the first stack?");
            ModSettingsManager.AddOption(new FloatFieldOption(SkillCooldownReductionBase));
            SkillCooldownReductionPerStack = configuration.Bind("Item: " + ItemName, "Stacking Skill Cooldown Reduction", 6.0f, "How much skill cooldown reduction should be provided by subsequent stacks?");
            ModSettingsManager.AddOption(new FloatFieldOption(SkillCooldownReductionPerStack));
            DamageBase = configuration.Bind("Item: " + ItemName, "Initial Damage Bonus", 12.0f, "How much damage should be provided by the first stack?");
            ModSettingsManager.AddOption(new FloatFieldOption(DamageBase));
            DamagePerStack = configuration.Bind("Item: " + ItemName, "Stacking Damage Bonus", 12.0f, "How much damage should be provided by subsequent stacks?");
            ModSettingsManager.AddOption(new FloatFieldOption(DamagePerStack));
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
    }
}