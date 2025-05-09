using BepInEx.Configuration;
using R2API;
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
        public override string ItemTokenDesc => $"Reduces {Utility("skill cooldowns")} by {Utility($"{SkillCooldownReductionBase}%")} {Stack($"(+{SkillCooldownReductionPerStack}% per stack)")} and increases {Damage("attack speed")} by {Damage($"{AttackSpeedBase}%")} {Stack($"(+{AttackSpeedPerStack}% per stack)")}, {Utility("movement speed")} by {Utility($"{MovementSpeedBase}%")} {Stack($"(+{MovementSpeedPerStack}% per stack)")}, and {Damage("damage")} by {Damage($"{DamageBase}%")} {Stack($"(+{DamagePerStack}% per stack)")}.";
        public override string ItemTokenLore => "Yasha and Kaya when paired together share a natural resonance.";
        public override ItemTier Tier => ItemTier.Tier2;
        public override ItemTag[] ItemTags => [ItemTag.WorldUnique];
        public override string ItemModelPath => "RoR2/Base/Mystery/PickupMystery.prefab";
        public override string ItemIconPath => "RiskOfTheAncients2.Icons.yasha_and_kaya.png";
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

        public float AttackSpeedBase;
        public float AttackSpeedPerStack;
        public float MovementSpeedBase;
        public float MovementSpeedPerStack;
        public float SkillCooldownReductionBase;
        public float SkillCooldownReductionPerStack;
        public float DamageBase;
        public float DamagePerStack;
        public void CreateConfig(ConfigFile configuration)
        {
            AttackSpeedBase = configuration.Bind("Item: " + ItemName, "Initial Attack Speed Bonus", 18.0f, "How much attack speed should be provided by the first stack?").Value;
            AttackSpeedPerStack = configuration.Bind("Item: " + ItemName, "Stacking Attack Speed Bonus", 18.0f, "How much attack speed should be provided by subsequent stacks?").Value;
            MovementSpeedBase = configuration.Bind("Item: " + ItemName, "Initial Movement Speed Bonus", 18.0f, "How much movement speed should be provided by the first stack?").Value;
            MovementSpeedPerStack = configuration.Bind("Item: " + ItemName, "Stacking Movement Speed Bonus", 18.0f, "How much movement speed should be provided by subsequent stacks?").Value;
            SkillCooldownReductionBase = configuration.Bind("Item: " + ItemName, "Initial Skill Cooldown Reduction", 12.0f, "How much skill cooldown reduction should be provided by the first stack?").Value;
            SkillCooldownReductionPerStack = configuration.Bind("Item: " + ItemName, "Stacking Skill Cooldown Reduction", 12.0f, "How much skill cooldown reduction should be provided by subsequent stacks?").Value;
            DamageBase = configuration.Bind("Item: " + ItemName, "Initial Damage Bonus", 15.0f, "How much damage should be provided by the first stack?").Value;
            DamagePerStack = configuration.Bind("Item: " + ItemName, "Stacking Damage Bonus", 15.0f, "How much damage should be provided by subsequent stacks?").Value;
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
    }
}