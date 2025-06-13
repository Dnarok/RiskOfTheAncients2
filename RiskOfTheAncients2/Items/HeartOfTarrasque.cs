using BepInEx.Configuration;
using R2API;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;

namespace ROTA2.Items
{
    public class HeartOfTarrasque : ItemBase<HeartOfTarrasque>
    {
        public override string ItemName => "Heart of Tarrasque";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "HEART_OF_TARRASQUE";
        public override string ItemTokenPickup => "Massively increases health and health regeneration.";
        public override string ItemTokenDesc => $"Increases {Healing("base health regeneration")} by {Healing($"{MaximumHealthRegenerationPercentage.Value}% of your maximum health")} per second, and increases {Healing("maximum health")} by {Healing($"{MaximumHealthBase.Value}")} {Stack($"(+{MaximumHealthPerStack.Value} per stack)")}.";
        public override string ItemTokenLore => "Preserved heart of an extinct monster, it bolsters the bearer's fortitude.";
        public override string ItemDefGUID => Assets.HeartOfTarrasque.ItemDef;
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

        public ConfigEntry<float> MaximumHealthBase;
        public ConfigEntry<float> MaximumHealthPerStack;
        public ConfigEntry<float> MaximumHealthRegenerationPercentage;
        public void CreateConfig(ConfigFile configuration)
        {
            MaximumHealthBase = configuration.Bind("Item: " + ItemName, "Initial Maximum Health Bonus", 200.0f, "How much maximum health should be provided by the first stack?");
            ModSettingsManager.AddOption(new FloatFieldOption(MaximumHealthBase));
            MaximumHealthPerStack = configuration.Bind("Item: " + ItemName, "Stacking Maximum Health Bonus", 200.0f, "How much maximum health should be provided by subsequent stacks?");
            ModSettingsManager.AddOption(new FloatFieldOption(MaximumHealthPerStack));
            MaximumHealthRegenerationPercentage = configuration.Bind("Item: " + ItemName, "Percentage of Maximum Health as Bonus Health Regeneration", 1.4f, "What percentage of maximum health should be provided as health regeneration?");
            ModSettingsManager.AddOption(new FloatFieldOption(MaximumHealthRegenerationPercentage));
        }

        private void AddMaximumHealth(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs arguments)
        {
            int count = GetCount(body);
            if (count > 0)
            {
                arguments.baseHealthAdd += MaximumHealthBase.Value + MaximumHealthPerStack.Value * (count - 1);
            }
        }
        private void AddHealthRegeneration(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs arguments)
        {
            if (GetCount(body) > 0)
            {
                HealthComponent health = body.GetComponent<HealthComponent>();
                if (health)
                {
                    arguments.baseRegenAdd += health.fullHealth * MaximumHealthRegenerationPercentage.Value / 100.0f;
                }
            }
        }
    }
}