using BepInEx.Configuration;
using R2API;
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
        public override string ItemTokenDesc => $"Reduces {Utility("skill cooldowns")} by {Utility($"{SkillCooldownReductionBase}%")} {Stack($"(+{SkillCooldownReductionPerStack}% per stack)")}, and increases {Damage("damage")} by {Damage($"{DamageBase}%")} {Stack($"(+{DamagePerStack}% per stack)")} and {Healing("maximum health")} by {Healing($"{MaximumHealthBase}")} {Stack($"(+{MaximumHealthPerStack} per stack)")}.";
        public override string ItemTokenLore => "A small token imbued with the fortune of the fae in recognition of an intriguing display of mortal kindness.";
        public override ItemTier Tier => ItemTier.Tier1;
        public override string ItemIconPath => "RiskOfTheAncients2.Icons.fairys_trinket.png";
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

        public float SkillCooldownReductionBase;
        public float SkillCooldownReductionPerStack;
        public float DamageBase;
        public float DamagePerStack;
        public float MaximumHealthBase;
        public float MaximumHealthPerStack;
        public void CreateConfig(ConfigFile configuration)
        {
            SkillCooldownReductionBase      = configuration.Bind("Item: " + ConfigItemName, "Initial Skill Cooldown Reduction",   2.5f,   "How much skill cooldown reduction should be provided by the first stack?").Value;
            SkillCooldownReductionPerStack  = configuration.Bind("Item: " + ConfigItemName, "Stacking Skill Cooldown Reduction",  2.5f,   "How much skill cooldown reduction should be provided by subsequent stacks?").Value;
            DamageBase                      = configuration.Bind("Item: " + ConfigItemName, "Initial Damage Bonus",               3.5f,   "How much damage should be provided by the first stack?").Value;
            DamagePerStack                  = configuration.Bind("Item: " + ConfigItemName, "Stacking Damage Bonus",              3.5f,   "How much damage should be provided by subsequent stacks?").Value;
            MaximumHealthBase               = configuration.Bind("Item: " + ConfigItemName, "Initial Maximum Health Bonus",       10.0f,  "How much maximum health should be provided by the first stack?").Value;
            MaximumHealthPerStack           = configuration.Bind("Item: " + ConfigItemName, "Stacking Maximum Health Bonus",      10.0f,  "How much maximum health should be provided by subsequent stacks?").Value;
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
    }
}