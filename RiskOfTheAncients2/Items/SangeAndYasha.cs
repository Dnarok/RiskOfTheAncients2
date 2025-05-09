using BepInEx.Configuration;
using R2API;
using RoR2;

namespace ROTA2.Items
{
    public class SangeAndYasha : ItemBase<SangeAndYasha>
    {
        public override string ItemName => "Sange and Yasha";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "SANGE_AND_YASHA";
        public override string ItemTokenPickup => "Increases maximum health, base health regeneration, attack speed, and movement speed. Combines with Kaya.";
        public override string ItemTokenDesc => $"Increases {Healing("maximum health")} by {Healing($"{MaximumHealthBase}")} {Stack($"(+{MaximumHealthPerStack} per stack)")}, {Healing("base health regeneration")} by {Healing($"+{BaseHealthRegenerationBase} hp/s")} {Stack($"(+{BaseHealthRegenerationPerStack} hp/s per stack)")}, {Damage("attack speed")} by {Damage($"{AttackSpeedBase}%")} {Stack($"(+{AttackSpeedPerStack}% per stack)")}, and {Utility("movement speed")} by {Utility($"{MovementSpeedBase}%")} {Stack($"(+{MovementSpeedPerStack}% per stack)")}.";
        public override string ItemTokenLore => "Sange and Yasha, when attuned by the moonlight and used together, become a very powerful combination.";
        public override ItemTier Tier => ItemTier.Tier2;
        public override ItemTag[] ItemTags => [ItemTag.WorldUnique];
        public override string ItemModelPath => "RoR2/Base/Mystery/PickupMystery.prefab";
        public override string ItemIconPath => "RiskOfTheAncients2.Icons.sange_and_yasha.png";
        public override void Hooks()
        {
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
            MaximumHealthBase = configuration.Bind("Item: " + ItemName, "Initial Maximum Health Bonus", 60.0f, "How much maximum health should be provided by the first stack?").Value;
            MaximumHealthPerStack = configuration.Bind("Item: " + ItemName, "Stacking Maximum Health Bonus", 60.0f, "How much maximum health should be provided by subsequent stacks?").Value;
            BaseHealthRegenerationBase = configuration.Bind("Item: " + ItemName, "Initial Base Health Regeneration Bonus", 2.4f, "How much base health regeneration should be provided by the first stack?").Value;
            BaseHealthRegenerationPerStack = configuration.Bind("Item: " + ItemName, "Stacking Base Health Regeneration Bonus", 2.4f, "How much base health regeneration should be provided by subsequent stacks?").Value;
            AttackSpeedBase = configuration.Bind("Item: " + ItemName, "Initial Attack Speed Bonus", 18.0f, "How much attack speed should be provided by the first stack?").Value;
            AttackSpeedPerStack = configuration.Bind("Item: " + ItemName, "Stacking Attack Speed Bonus", 18.0f, "How much attack speed should be provided by subsequent stacks?").Value;
            MovementSpeedBase = configuration.Bind("Item: " + ItemName, "Initial Movement Speed Bonus", 18.0f, "How much movement speed should be provided by the first stack?").Value;
            MovementSpeedPerStack = configuration.Bind("Item: " + ItemName, "Stacking Movement Speed Bonus", 18.0f, "How much movement speed should be provided by subsequent stacks?").Value;
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