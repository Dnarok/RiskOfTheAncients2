using BepInEx.Configuration;
using R2API;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;

namespace ROTA2.Items
{
    public class Sange : ItemBase<Sange>
    {
        public override string ItemName => "Sange";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "SANGE";
        public override string ItemTokenPickup => "Increases attack and movement speed. Combines with Kaya or Yasha.";
        public override string ItemTokenDesc => $"Increases {Healing("maximum health")} by {Healing($"{MaximumHealthBase.Value}")} {Stack($"(+{MaximumHealthPerStack.Value} per stack)")} and {Healing("base health regeneration")} by {Healing($"+{BaseHealthRegenerationBase.Value} hp/s")} {Stack($"(+{BaseHealthRegenerationPerStack.Value} hp/s per stack)")}.";
        public override string ItemTokenLore => "Sange is an unusually accurate weapon, seeking weak points automatically.";
        public override string ItemDefGUID => Assets.Sange.ItemDef;
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

        public ConfigEntry<float> MaximumHealthBase;
        public ConfigEntry<float> MaximumHealthPerStack;
        public ConfigEntry<float> BaseHealthRegenerationBase;
        public ConfigEntry<float> BaseHealthRegenerationPerStack;
        public void CreateConfig(ConfigFile configuration)
        {
            MaximumHealthBase = configuration.Bind("Item: " + ItemName, "Initial Maximum Health Bonus", 40.0f, "How much maximum health should be provided by the first stack?");
            ModSettingsManager.AddOption(new FloatFieldOption(MaximumHealthBase));
            MaximumHealthPerStack = configuration.Bind("Item: " + ItemName, "Stacking Maximum Health Bonus", 40.0f, "How much maximum health should be provided by subsequent stacks?");
            ModSettingsManager.AddOption(new FloatFieldOption(MaximumHealthPerStack));
            BaseHealthRegenerationBase = configuration.Bind("Item: " + ItemName, "Initial Base Health Regeneration Bonus", 1.6f, "How much base health regeneration should be provided by the first stack?");
            ModSettingsManager.AddOption(new FloatFieldOption(BaseHealthRegenerationBase));
            BaseHealthRegenerationPerStack = configuration.Bind("Item: " + ItemName, "Stacking Base Health Regeneration Bonus", 1.6f, "How much base health regeneration should be provided by subsequent stacks?");
            ModSettingsManager.AddOption(new FloatFieldOption(BaseHealthRegenerationPerStack));
        }

        private void AddMaximumHealth(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs arguments)
        {
            int count = GetCount(body);
            if (count > 0)
            {
                arguments.baseHealthAdd += MaximumHealthBase.Value + MaximumHealthPerStack.Value * (count - 1);
            }
        }
        private void AddBaseHealthRegeneration(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs arguments)
        {
            int count = GetCount(body);
            if (count > 0)
            {
                arguments.baseRegenAdd += (BaseHealthRegenerationBase.Value + BaseHealthRegenerationPerStack.Value * (count - 1)) * (1 + 0.2f * body.level);
            }
        }
    }
}