using BepInEx.Configuration;
using R2API;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;

namespace ROTA2.Items
{
    public class Yasha : ItemBase<Yasha>
    {
        public override string ItemName => "Yasha";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "YASHA";
        public override string ItemTokenPickup => "Increases attack and movement speed. Combines with Sange or Kaya.";
        public override string ItemTokenDesc => $"Increases {Damage("attack speed")} by {Damage($"{AttackSpeedBase.Value}%")} {Stack($"(+{AttackSpeedPerStack.Value}% per stack)")} and {Utility("movement speed")} by {Utility($"{MovementSpeedBase.Value}%")} {Stack($"(+{MovementSpeedPerStack.Value}% per stack)")}.";
        public override string ItemTokenLore => "Yasha is regarded as the swiftest weapon ever created.";
        public override string ItemDefGUID => Assets.Yasha.ItemDef;
        public override void Hooks()
        {
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

        public ConfigEntry<float> AttackSpeedBase;
        public ConfigEntry<float> AttackSpeedPerStack;
        public ConfigEntry<float> MovementSpeedBase;
        public ConfigEntry<float> MovementSpeedPerStack;
        public void CreateConfig(ConfigFile configuration)
        {
            AttackSpeedBase = configuration.Bind("Item: " + ItemName, "Initial Attack Speed Bonus", 15.0f, "How much attack speed should be provided by the first stack?");
            ModSettingsManager.AddOption(new FloatFieldOption(AttackSpeedBase));
            AttackSpeedPerStack = configuration.Bind("Item: " + ItemName, "Stacking Attack Speed Bonus", 15.0f, "How much attack speed should be provided by subsequent stacks?");
            ModSettingsManager.AddOption(new FloatFieldOption(AttackSpeedPerStack));
            MovementSpeedBase = configuration.Bind("Item: " + ItemName, "Initial Movement Speed Bonus", 15.0f, "How much movement speed should be provided by the first stack?");
            ModSettingsManager.AddOption(new FloatFieldOption(MovementSpeedBase));
            MovementSpeedPerStack = configuration.Bind("Item: " + ItemName, "Stacking Movement Speed Bonus", 15.0f, "How much movement speed should be provided by subsequent stacks?");
            ModSettingsManager.AddOption(new FloatFieldOption(MovementSpeedPerStack));
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