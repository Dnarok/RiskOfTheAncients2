using BepInEx.Configuration;
using R2API;
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
        public override string ItemTokenDesc => $"Reduces {Utility("skill cooldowns")} by {Utility($"{SkillCooldownReductionBase}%")} {Stack($"(+{SkillCooldownReductionPerStack}% per stack)")} and increases {Damage("damage")} by {Damage($"{DamageBase}%")} {Stack($"(+{DamagePerStack}% per stack)")}.";
        public override string ItemTokenLore => "The staff of a renowned sorceress, lost for countless millennia.";
        public override ItemTier Tier => ItemTier.Tier2;
        public override string ItemModelPath => "RoR2/Base/Mystery/PickupMystery.prefab";
        public override string ItemIconPath => "ROTA2.Icons.kaya.png";
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

        public float SkillCooldownReductionBase;
        public float SkillCooldownReductionPerStack;
        public float DamageBase;
        public float DamagePerStack;
        public void CreateConfig(ConfigFile configuration)
        {
            SkillCooldownReductionBase = configuration.Bind("Item: " + ItemName, "Initial Skill Cooldown Reduction", 6.0f, "How much skill cooldown reduction should be provided by the first stack?").Value;
            SkillCooldownReductionPerStack = configuration.Bind("Item: " + ItemName, "Stacking Skill Cooldown Reduction", 6.0f, "How much skill cooldown reduction should be provided by subsequent stacks?").Value;
            DamageBase = configuration.Bind("Item: " + ItemName, "Initial Damage Bonus", 12.0f, "How much damage should be provided by the first stack?").Value;
            DamagePerStack = configuration.Bind("Item: " + ItemName, "Stacking Damage Bonus", 12.0f, "How much damage should be provided by subsequent stacks?").Value;
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