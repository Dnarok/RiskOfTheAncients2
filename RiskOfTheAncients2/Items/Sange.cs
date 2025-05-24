using BepInEx.Configuration;
using R2API;
using RoR2;

namespace ROTA2.Items
{
    public class Sange : ItemBase<Sange>
    {
        public override string ItemName => "Sange";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "SANGE";
        public override string ItemTokenPickup => "Increases attack and movement speed. Combines with Kaya or Yasha.";
        public override string ItemTokenDesc => $"Increases {Healing("maximum health")} by {Healing($"{MaximumHealthBase}")} {Stack($"(+{MaximumHealthPerStack} per stack)")} and {Healing("base health regeneration")} by {Healing($"+{BaseHealthRegenerationBase} hp/s")} {Stack($"(+{BaseHealthRegenerationPerStack} hp/s per stack)")}.";
        public override string ItemTokenLore => "Sange is an unusually accurate weapon, seeking weak points automatically.";
        public override ItemTier Tier => ItemTier.Tier2;
        public override ItemTag[] ItemTags => [ItemTag.Healing];
        public override string ItemIconPath => "ROTA2.Icons.sange.png";
        public override void Hooks()
        {
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

        public float MaximumHealthBase;
        public float MaximumHealthPerStack;
        public float BaseHealthRegenerationBase;
        public float BaseHealthRegenerationPerStack;
        public void CreateConfig(ConfigFile configuration)
        {
            MaximumHealthBase = configuration.Bind("Item: " + ItemName, "Initial Maximum Health Bonus", 40.0f, "How much maximum health should be provided by the first stack?").Value;
            MaximumHealthPerStack = configuration.Bind("Item: " + ItemName, "Stacking Maximum Health Bonus", 40.0f, "How much maximum health should be provided by subsequent stacks?").Value;
            BaseHealthRegenerationBase = configuration.Bind("Item: " + ItemName, "Initial Base Health Regeneration Bonus", 1.6f, "How much base health regeneration should be provided by the first stack?").Value;
            BaseHealthRegenerationPerStack = configuration.Bind("Item: " + ItemName, "Stacking Base Health Regeneration Bonus", 1.6f, "How much base health regeneration should be provided by subsequent stacks?").Value;
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