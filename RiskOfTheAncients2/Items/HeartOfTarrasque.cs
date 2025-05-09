using BepInEx.Configuration;
using R2API;
using RoR2;

namespace ROTA2.Items
{
    public class HeartOfTarrasque : ItemBase<HeartOfTarrasque>
    {
        public override string ItemName => "Heart of Tarrasque";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "HEART_OF_TARRASQUE";
        public override string ItemTokenPickup => "Massively increases health and health regeneration.";
        public override string ItemTokenDesc => $"Increases {Healing("base health regeneration")} by {Healing($"{MaximumHealthRegenerationPercentage}% of your maximum health")} per second, and increases {Healing("maximum health")} by {Healing($"{MaximumHealthBase}")} {Stack($"(+{MaximumHealthPerStack} per stack)")}.";
        public override string ItemTokenLore => "Preserved heart of an extinct monster, it bolsters the bearer's fortitude.";
        public override ItemTier Tier => ItemTier.Tier3;
        public override string ItemModelPath => "RoR2/Base/Mystery/PickupMystery.prefab";
        public override string ItemIconPath => "RiskOfTheAncients2.Icons.heart_of_tarrasque.png";
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddMaximumHealth;
            RecalculateStatsAPI.GetStatCoefficients += AddHealthRegeneration;
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
        public float MaximumHealthRegenerationPercentage;
        public void CreateConfig(ConfigFile configuration)
        {
            MaximumHealthBase                   = configuration.Bind("Item: " + ItemName, "Initial Maximum Health Bonus",                               200.0f, "How much maximum health should be provided by the first stack?").Value;
            MaximumHealthPerStack               = configuration.Bind("Item: " + ItemName, "Stacking Maximum Health Bonus",                              200.0f, "How much maximum health should be provided by subsequent stacks?").Value;
            MaximumHealthRegenerationPercentage = configuration.Bind("Item: " + ItemName, "Percentage of Maximum Health as Bonus Health Regeneration",  1.4f,   "What percentage of maximum health should be provided as health regeneration?").Value;
        }

        private void AddMaximumHealth(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs arguments)
        {
            int count = GetCount(body);
            if (count > 0)
            {
                arguments.baseHealthAdd += MaximumHealthBase + MaximumHealthPerStack * (count - 1);
            }
        }
        private void AddHealthRegeneration(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs arguments)
        {
            if (GetCount(body) > 0)
            {
                HealthComponent health = body.GetComponent<HealthComponent>();
                if (health)
                {
                    arguments.baseRegenAdd += health.fullHealth * MaximumHealthRegenerationPercentage / 100.0f;
                }
            }
        }
    }
}