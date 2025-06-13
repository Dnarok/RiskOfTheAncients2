using BepInEx.Configuration;
using R2API;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;

namespace ROTA2.Items
{
    public class BladesOfAttack : ItemBase<BladesOfAttack>
    {
        public override string ItemName => "Blades of Attack";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "BLADES_OF_ATTACK";
        public override string ItemTokenPickup => "Increases damage.";
        public override string ItemTokenDesc => $"Increases {Damage("damage")} by {Damage($"{DamageBase.Value}%")} {Stack($"(+{DamagePerStack.Value}% per stack)")}.";
        public override string ItemTokenLore => "The damage of these small, concealable blades should not be underestimated.";
        public override string ItemDefGUID => Assets.BladesOfAttack.ItemDef;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddDamage;
        }
        public override void Init(ConfigFile configuration)
        {
            CreateConfig(configuration);
            CreateLanguageTokens();
            CreateItemDef();
            Hooks();
        }

        public ConfigEntry<float> DamageBase;
        public ConfigEntry<float> DamagePerStack;
        private void CreateConfig(ConfigFile configuration)
        {
            DamageBase = configuration.Bind("Item: " + ItemName, "Damage Base", 10.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(DamageBase));
            DamagePerStack = configuration.Bind("Item: " + ItemName, "Damage Per Stack", 10.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(DamagePerStack));
        }

        private void AddDamage(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            int count = GetCount(body);
            if (count > 0)
            {
                args.damageMultAdd += DamageBase.Value / 100.0f + DamagePerStack.Value / 100.0f * (count - 1);
            }
        }
    }
}