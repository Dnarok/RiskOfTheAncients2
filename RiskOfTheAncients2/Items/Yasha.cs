using BepInEx.Configuration;
using R2API;
using RoR2;

namespace ROTA2.Items
{
    public class Yasha : ItemBase<Yasha>
    {
        public override string ItemName => "Yasha";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "YASHA";
        public override string ItemTokenPickup => "Increases attack and movement speed. Combines with Sange or Kaya.";
        public override string ItemTokenDesc => $"Increases {Damage("attack speed")} by {Damage($"{AttackSpeedBase}%")} {Stack($"(+{AttackSpeedPerStack}% per stack)")} and {Utility("movement speed")} by {Utility($"{MovementSpeedBase}%")} {Stack($"(+{MovementSpeedPerStack}% per stack)")}.";
        public override string ItemTokenLore => "Yasha is regarded as the swiftest weapon ever created.";
        public override ItemTier Tier => ItemTier.Tier2;
        public override string ItemIconPath => "ROTA2.Icons.yasha.png";
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

        public float AttackSpeedBase;
        public float AttackSpeedPerStack;
        public float MovementSpeedBase;
        public float MovementSpeedPerStack;
        public void CreateConfig(ConfigFile configuration)
        {
            AttackSpeedBase = configuration.Bind("Item: " + ItemName, "Initial Attack Speed Bonus", 15.0f, "How much attack speed should be provided by the first stack?").Value;
            AttackSpeedPerStack = configuration.Bind("Item: " + ItemName, "Stacking Attack Speed Bonus", 15.0f, "How much attack speed should be provided by subsequent stacks?").Value;
            MovementSpeedBase = configuration.Bind("Item: " + ItemName, "Initial Movement Speed Bonus", 15.0f, "How much movement speed should be provided by the first stack?").Value;
            MovementSpeedPerStack = configuration.Bind("Item: " + ItemName, "Stacking Movement Speed Bonus", 15.0f, "How much movement speed should be provided by subsequent stacks?").Value;
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