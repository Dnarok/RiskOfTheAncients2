using BepInEx.Configuration;
using R2API;
using RoR2;

namespace ROTA2.Items
{
    public class BootsOfSpeed : ItemBase<BootsOfSpeed>
    {
        public override string ItemName => "Boots of Speed";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "BOOTS_OF_SPEED";
        public override string ItemTokenPickup => "Slightly increases base movement speed.";
        public override string ItemTokenDesc => $"Increases {Utility("base movement speed")} by {Utility($"{MovementSpeedBase}")} {Stack($"(+{MovementSpeedPerStack} per stack)")}.";
        public override string ItemTokenLore => "Fleet footwear, increasing movement.";
        public override ItemTier Tier => ItemTier.Tier1;
        public override ItemTag[] ItemTags => [ItemTag.Utility];
        public override string ItemModelPath => "boots_of_speed.prefab";
        public override string ItemIconPath => "ROTA2.Icons.boots_of_speed.png";
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddMovementSpeed;
        }
        public override void Init(ConfigFile configuration)
        {
            CreateConfig(configuration);
            CreateLanguageTokens();
            CreateItemDef();
            Hooks();
        }

        public float MovementSpeedBase;
        public float MovementSpeedPerStack;
        public void CreateConfig(ConfigFile configuration)
        {
            MovementSpeedBase       = configuration.Bind("Item: " + ItemName, "Initial Base Movement Speed Bonus",  0.7f, "How much base movement speed should be provided by the first stack?").Value;
            MovementSpeedPerStack   = configuration.Bind("Item: " + ItemName, "Stacking Base Movement Speed Bonus", 0.7f, "How much base movement speed should be provided by subsequent stacks?").Value;
        }

        private void AddMovementSpeed(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs arguments)
        {
            int count = GetCount(body);
            if (count > 0)
            {
                arguments.baseMoveSpeedAdd += MovementSpeedBase + MovementSpeedPerStack * (count - 1);
            }
        }
    }
}