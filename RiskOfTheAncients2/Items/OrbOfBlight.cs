using BepInEx.Configuration;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
using ROTA2.Buffs;

namespace ROTA2.Items
{
    public class OrbOfBlight : ItemBase<OrbOfBlight>
    {
        public override string ItemName => "Orb of Blight";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "ORB_OF_BLIGHT";
        public override string ItemTokenPickup => "Reduces armor on hit, up to a max.";
        public override string ItemTokenDesc => $"On hit reduce {Damage("armor")} by {Damage($"{ArmorReduction.Value}")} for {Damage($"{ArmorReductionDuration.Value} seconds")}, up to {Damage($"{MaxStacksBase.Value} times")} {Stack($"(+{MaxStacksPerStack.Value} per stack)")}.";
        public override string ItemTokenLore => "An unnerving stone unearthed beneath the Fields of Endless Carnage.";
        public override string ItemDefGUID => Assets.OrbOfBlight.ItemDef;
        public override void Hooks()
        {
            On.RoR2.HealthComponent.TakeDamage += OnHit;
        }
        public override void Init(ConfigFile configuration)
        {
            CreateConfig(configuration);
            CreateLanguageTokens();
            CreateItemDef();
            Hooks();
        }

        public ConfigEntry<float> ArmorReduction;
        public ConfigEntry<int> MaxStacksBase;
        public ConfigEntry<int> MaxStacksPerStack;
        public ConfigEntry<float> ArmorReductionDuration;
        public void CreateConfig(ConfigFile configuration)
        {
            ArmorReduction = configuration.Bind("Item: " + ItemName, "Armor Reduction Per Stack", 5.0f, "How much armor should be removed per stack of the debuff?");
            ModSettingsManager.AddOption(new FloatFieldOption(ArmorReduction));
            MaxStacksBase = configuration.Bind("Item: " + ItemName, "Base Max Stacks", 3, "How many debuff stacks can be applied by the first stack?");
            ModSettingsManager.AddOption(new IntFieldOption(MaxStacksBase));
            MaxStacksPerStack = configuration.Bind("Item: " + ItemName, "Stacking Max Stacks", 2, "How many debuff stacks can be applied by subsequent stack?");
            ModSettingsManager.AddOption(new IntFieldOption(MaxStacksPerStack));
            ArmorReductionDuration = configuration.Bind("Item: " + ItemName, "Armor Reduction Duration", 3.0f, "How long should the armor reduction last?");
            ModSettingsManager.AddOption(new FloatFieldOption(ArmorReductionDuration));
        }

        private void OnHit(On.RoR2.HealthComponent.orig_TakeDamage orig, RoR2.HealthComponent self, RoR2.DamageInfo info)
        {
            if (self && self.alive && info.attacker && info.procCoefficient > 0.0f)
            {
                var attacker_body = info.attacker.GetComponent<CharacterBody>();
                int count = GetCount(attacker_body);
                if (count > 0)
                {
                    OrbOfBlightBuff.ApplyTo(
                        body: self.body,
                        duration: ArmorReductionDuration.Value,
                        stacks: 1,
                        max_stacks: MaxStacksBase.Value + MaxStacksPerStack.Value * (count - 1)
                    );
                }
            }

            orig(self, info);
        }
    }
}