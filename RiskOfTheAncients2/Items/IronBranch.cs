using BepInEx.Configuration;
using R2API;
using RiskOfOptions;
using RiskOfOptions.Options;

namespace ROTA2.Items
{
    public class IronBranch : ItemBase<IronBranch>
    {
        public override string ItemName => "Iron Branch";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "IRON_BRANCH";
        public override string ItemTokenPickup => "Increases all stats by a small amount.";
        public override string ItemTokenDesc => $"Grants {Utility($"{StatIncreaseBase.Value}%")} {Stack($"(+{StatIncreasePerStack.Value}% per stack)")} increase to {Utility("ALL stats")}.";
        public override string ItemTokenLore => "A seemingly ordinary branch, its ironlike qualities are bestowed upon the bearer.";
        public override string ItemDefGUID => Assets.IronBranch.ItemDef;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += (body, args) =>
            {
                int count = GetCount(body);
                if (count > 0)
                {
                    float multiplier = 1f + (StatIncreaseBase.Value / 100.0f + StatIncreasePerStack.Value / 100.0f * (count - 1));
                    args.healthTotalMult *= multiplier;
                    args.shieldTotalMult *= multiplier;
                    args.moveSpeedTotalMult *= multiplier;
                    args.damageTotalMult *= multiplier;
                    args.attackSpeedTotalMult *= multiplier;
                    args.critTotalMult *= multiplier;
                    args.regenTotalMult *= multiplier;
                    args.armorTotalMult *= multiplier;
                }
            };
        }

        public override void Init(ConfigFile configuration)
        {
            CreateConfig(configuration);
            CreateLanguageTokens();
            CreateItemDef();
            Hooks();
        }

        public ConfigEntry<float> StatIncreaseBase;
        public ConfigEntry<float> StatIncreasePerStack;
        private void CreateConfig(ConfigFile configuration)
        {
            StatIncreaseBase = configuration.Bind("Item: " + ItemName, "All Stats Increase Base", 1.5f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(StatIncreaseBase));
            StatIncreasePerStack = configuration.Bind("Item: " + ItemName, "All Stats Increase Per Stack", 1.5f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(StatIncreasePerStack));
        }
    }
}