using BepInEx.Configuration;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
using ROTA2.Buffs;

namespace ROTA2.Items
{
    public class OrbOfFrost : ItemBase<OrbOfFrost>
    {
        public override string ItemName => "Orb of Frost";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "ORB_OF_FROST";
        public override string ItemTokenPickup => "Slightly slow enemies on hit.";
        public override string ItemTokenDesc => $"{Utility("Slow")} enemies on hit for {Utility($"{MovementSpeedSlowBase.Value}% movement speed")} {Stack($"(-{MovementSpeedSlowPerStack.Value}% per stack)")} and {Utility($"-{AttackSpeedSlowBase.Value}% attack speed")} {Stack($"-{AttackSpeedSlowPerStack.Value}% per stack)")} for {Utility($"{DebuffDuration.Value} seconds")}.";
        public override string ItemTokenLore => "Freezes your foes with a frosty force.";
        public override string ItemDefGUID => Assets.OrbOfFrost.ItemDef;
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

        public ConfigEntry<float> MovementSpeedSlowBase;
        public ConfigEntry<float> MovementSpeedSlowPerStack;
        public ConfigEntry<float> AttackSpeedSlowBase;
        public ConfigEntry<float> AttackSpeedSlowPerStack;
        public ConfigEntry<float> DebuffDuration;
        public void CreateConfig(ConfigFile configuration)
        {
            MovementSpeedSlowBase = configuration.Bind("Item: " + ItemName, "Movement Speed Reduction Base", 10.0f, "How much should movement speed be reduced initially?");
            ModSettingsManager.AddOption(new FloatFieldOption(MovementSpeedSlowBase));
            MovementSpeedSlowPerStack = configuration.Bind("Item: " + ItemName, "Movement Speed Reduction Per Stack", 10.0f, "How much should movement speed be reduced per stack?");
            ModSettingsManager.AddOption(new FloatFieldOption(MovementSpeedSlowPerStack));
            AttackSpeedSlowBase = configuration.Bind("Item: " + ItemName, "Attack Speed Reduction Base", 10.0f, "How much should attack speed be reduced initially?");
            ModSettingsManager.AddOption(new FloatFieldOption(AttackSpeedSlowBase));
            AttackSpeedSlowPerStack = configuration.Bind("Item: " + ItemName, "Attack Speed Reduction Per Stack", 10.0f, "How much should attack speed be reduced per stack?");
            ModSettingsManager.AddOption(new FloatFieldOption(AttackSpeedSlowPerStack));
            DebuffDuration = configuration.Bind("Item: " + ItemName, "Slow Duration", 3.0f, "How long should the slows last?");
            ModSettingsManager.AddOption(new FloatFieldOption(DebuffDuration));
        }

        private void OnHit(On.RoR2.HealthComponent.orig_TakeDamage orig, RoR2.HealthComponent self, RoR2.DamageInfo info)
        {
            if (self && self.alive && info.attacker && info.procCoefficient > 0.0f)
            {
                var attacker_body = info.attacker.GetComponent<CharacterBody>();
                int count = GetCount(attacker_body);
                if (count > 0)
                {
                    OrbOfFrostBuff.ApplyTo(
                        body: self.body,
                        duration: DebuffDuration.Value,
                        stacks: count,
                        max_stacks: count
                    );
                }
            }

            orig(self, info);
        }
    }
}