using BepInEx.Configuration;
using R2API;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
using System;

namespace ROTA2.Items
{
    public class YashaAndKaya : ItemBase<YashaAndKaya>
    {
        public override string ItemName => "Yasha and Kaya";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "YASHA_AND_KAYA";
        public override string ItemTokenPickup => "Reduces skill cooldowns and increases attack speed, movement speed, and damage. Combines with Sange.";
        public override string ItemTokenDesc => $"Reduces {Utility("skill cooldowns")} by {Utility($"{SkillCooldownReductionBase.Value}%")} {Stack($"(+{SkillCooldownReductionPerStack.Value}% per stack)")} and increases {Damage("attack speed")} by {Damage($"{AttackSpeedBase.Value}%")} {Stack($"(+{AttackSpeedPerStack.Value}% per stack)")}, {Utility("movement speed")} by {Utility($"{MovementSpeedBase.Value}%")} {Stack($"(+{MovementSpeedPerStack.Value}% per stack)")}, and {Damage("damage")} by {Damage($"{DamageBase.Value}%")} {Stack($"(+{DamagePerStack.Value}% per stack)")}.";
        public override string ItemTokenLore => "Yasha and Kaya when paired together share a natural resonance.";
        public override string ItemDefGUID => Assets.YashaAndKaya.ItemDef;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddAttackSpeed;
            RecalculateStatsAPI.GetStatCoefficients += AddMovementSpeed;
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

        public ConfigEntry<float> AttackSpeedBase;
        public ConfigEntry<float> AttackSpeedPerStack;
        public ConfigEntry<float> MovementSpeedBase;
        public ConfigEntry<float> MovementSpeedPerStack;
        public ConfigEntry<float> SkillCooldownReductionBase;
        public ConfigEntry<float> SkillCooldownReductionPerStack;
        public ConfigEntry<float> DamageBase;
        public ConfigEntry<float> DamagePerStack;
        public void CreateConfig(ConfigFile configuration)
        {
            AttackSpeedBase = configuration.Bind("Item: " + ItemName, "Initial Attack Speed Bonus", 22.5f, "How much attack speed should be provided by the first stack?");
            ModSettingsManager.AddOption(new FloatFieldOption(AttackSpeedBase));
            AttackSpeedPerStack = configuration.Bind("Item: " + ItemName, "Stacking Attack Speed Bonus", 22.5f, "How much attack speed should be provided by subsequent stacks?");
            ModSettingsManager.AddOption(new FloatFieldOption(AttackSpeedPerStack));
            MovementSpeedBase = configuration.Bind("Item: " + ItemName, "Initial Movement Speed Bonus", 22.5f, "How much movement speed should be provided by the first stack?");
            ModSettingsManager.AddOption(new FloatFieldOption(MovementSpeedBase));
            MovementSpeedPerStack = configuration.Bind("Item: " + ItemName, "Stacking Movement Speed Bonus", 22.5f, "How much movement speed should be provided by subsequent stacks?");
            ModSettingsManager.AddOption(new FloatFieldOption(MovementSpeedPerStack));
            SkillCooldownReductionBase = configuration.Bind("Item: " + ItemName, "Initial Skill Cooldown Reduction", 9.0f, "How much skill cooldown reduction should be provided by the first stack?");
            ModSettingsManager.AddOption(new FloatFieldOption(SkillCooldownReductionBase));
            SkillCooldownReductionPerStack = configuration.Bind("Item: " + ItemName, "Stacking Skill Cooldown Reduction", 9.0f, "How much skill cooldown reduction should be provided by subsequent stacks?");
            ModSettingsManager.AddOption(new FloatFieldOption(SkillCooldownReductionPerStack));
            DamageBase = configuration.Bind("Item: " + ItemName, "Initial Damage Bonus", 18.0f, "How much damage should be provided by the first stack?");
            ModSettingsManager.AddOption(new FloatFieldOption(DamageBase));
            DamagePerStack = configuration.Bind("Item: " + ItemName, "Stacking Damage Bonus", 18.0f, "How much damage should be provided by subsequent stacks?");
            ModSettingsManager.AddOption(new FloatFieldOption(DamagePerStack));
        }

        private void AddAttackSpeed(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs arguments)
        {
            int count = GetCount(body);
            if (count > 0)
            {
                arguments.attackSpeedMultAdd += AttackSpeedBase.Value / 100.0f + AttackSpeedPerStack.Value / 100.0f * (count - 1);
            }
        }
        private void AddMovementSpeed(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs arguments)
        {
            int count = GetCount(body);
            if (count > 0)
            {
                arguments.moveSpeedMultAdd += MovementSpeedBase.Value / 100.0f + MovementSpeedPerStack.Value / 100.0f * (count - 1);
            }
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