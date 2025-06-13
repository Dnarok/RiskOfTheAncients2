using BepInEx.Configuration;
using R2API;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;

namespace ROTA2.Items
{
    public class SangeAndYasha : ItemBase<SangeAndYasha>
    {
        public override string ItemName => "Sange and Yasha";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "SANGE_AND_YASHA";
        public override string ItemTokenPickup => "Increases maximum health, base health regeneration, attack speed, and movement speed. Combines with Kaya.";
        public override string ItemTokenDesc => $"Increases {Healing("maximum health")} by {Healing($"{MaximumHealthBase.Value}")} {Stack($"(+{MaximumHealthPerStack.Value} per stack)")}, {Healing("base health regeneration")} by {Healing($"+{BaseHealthRegenerationBase.Value} hp/s")} {Stack($"(+{BaseHealthRegenerationPerStack.Value} hp/s per stack)")}, {Damage("attack speed")} by {Damage($"{AttackSpeedBase.Value}%")} {Stack($"(+{AttackSpeedPerStack.Value}% per stack)")}, and {Utility("movement speed")} by {Utility($"{MovementSpeedBase.Value}%")} {Stack($"(+{MovementSpeedPerStack.Value}% per stack)")}.";
        public override string ItemTokenLore => "Sange and Yasha, when attuned by the moonlight and used together, become a very powerful combination.";
        public override string ItemDefGUID => Assets.SangeAndYasha.ItemDef;
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

        public ConfigEntry<float> MaximumHealthBase;
        public ConfigEntry<float> MaximumHealthPerStack;
        public ConfigEntry<float> BaseHealthRegenerationBase;
        public ConfigEntry<float> BaseHealthRegenerationPerStack;
        public ConfigEntry<float> AttackSpeedBase;
        public ConfigEntry<float> AttackSpeedPerStack;
        public ConfigEntry<float> MovementSpeedBase;
        public ConfigEntry<float> MovementSpeedPerStack;
        public void CreateConfig(ConfigFile configuration)
        {
            MaximumHealthBase = configuration.Bind("Item: " + ItemName, "Initial Maximum Health Bonus", 60.0f, "How much maximum health should be provided by the first stack?");
            ModSettingsManager.AddOption(new FloatFieldOption(MaximumHealthBase));
            MaximumHealthPerStack = configuration.Bind("Item: " + ItemName, "Stacking Maximum Health Bonus", 60.0f, "How much maximum health should be provided by subsequent stacks?");
            ModSettingsManager.AddOption(new FloatFieldOption(MaximumHealthPerStack));
            BaseHealthRegenerationBase = configuration.Bind("Item: " + ItemName, "Initial Base Health Regeneration Bonus", 2.4f, "How much base health regeneration should be provided by the first stack?");
            ModSettingsManager.AddOption(new FloatFieldOption(BaseHealthRegenerationBase));
            BaseHealthRegenerationPerStack = configuration.Bind("Item: " + ItemName, "Stacking Base Health Regeneration Bonus", 2.4f, "How much base health regeneration should be provided by subsequent stacks?");
            ModSettingsManager.AddOption(new FloatFieldOption(BaseHealthRegenerationPerStack));
            AttackSpeedBase = configuration.Bind("Item: " + ItemName, "Initial Attack Speed Bonus", 22.5f, "How much attack speed should be provided by the first stack?");
            ModSettingsManager.AddOption(new FloatFieldOption(AttackSpeedBase));
            AttackSpeedPerStack = configuration.Bind("Item: " + ItemName, "Stacking Attack Speed Bonus", 22.5f, "How much attack speed should be provided by subsequent stacks?");
            ModSettingsManager.AddOption(new FloatFieldOption(AttackSpeedPerStack));
            MovementSpeedBase = configuration.Bind("Item: " + ItemName, "Initial Movement Speed Bonus", 22.5f, "How much movement speed should be provided by the first stack?");
            ModSettingsManager.AddOption(new FloatFieldOption(MovementSpeedBase));
            MovementSpeedPerStack = configuration.Bind("Item: " + ItemName, "Stacking Movement Speed Bonus", 22.5f, "How much movement speed should be provided by subsequent stacks?");
            ModSettingsManager.AddOption(new FloatFieldOption(MovementSpeedPerStack));
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
    }
}