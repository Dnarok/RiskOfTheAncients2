using BepInEx.Configuration;
using RiskOfOptions;
using RiskOfOptions.Options;

namespace ROTA2.Items
{
    public class Cloak : ItemBase<Cloak>
    {
        public override string ItemName => "Cloak";
        public override string ItemTokenName => "CLOAK";
        public override string ItemTokenPickup => "Increase 'Magic Resistance', reducing incoming direct damage from enemies.";
        public override string ItemTokenDesc => $"Increases '{Utility("Magic Resistance")}' by {Utility($"{MagicResistanceBase.Value}%")} {Stack($"(+{MagicResistancePerStack.Value}% per stack)")}, {Utility("reducing incoming direct damage from enemies")}.";
        public override string ItemTokenLore => "A cloak made of a magical material that works to dispel any magic cast on it.";
        public override string ItemDefGUID => Assets.Cloak.ItemDef;
        public override void Hooks()
        {
            StatsAPI.Recalculate += (body, args) =>
            {
                int count = GetCount(body);
                if (count > 0)
                {
                    args.MagicResistance.Add(MagicResistanceBase.Value / 100f);
                    args.MagicResistance.AddRange(System.Linq.Enumerable.Repeat(MagicResistancePerStack.Value / 100f, count - 1));
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

        public ConfigEntry<float> MagicResistanceBase;
        public ConfigEntry<float> MagicResistancePerStack;
        private void CreateConfig(ConfigFile configuration)
        {
            MagicResistanceBase = configuration.Bind("Item: " + ItemName, "Magic Resistance Base", 8f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(MagicResistanceBase));
            MagicResistancePerStack = configuration.Bind("Item: " + ItemName, "Magic Resistance Increase Per Stack", 8f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(MagicResistancePerStack));
        }
    }
}