using BepInEx.Configuration;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
using ROTA2.Buffs;

namespace ROTA2.Items
{
    public class OrbOfCorrosion : ItemBase<OrbOfCorrosion>
    {
        public override string ItemName => "Orb of Corrosion";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "ORB_OF_CORROSION";
        public override string ItemTokenPickup => "Reduces armor, movement speed, and attack speed, and poisons on hit.";
        public override string ItemTokenDesc =>
@$"On hit:
    Reduce {Damage("armor")} by {Damage($"{ArmorReduction.Value}")} for {Damage($"{ArmorReductionDuration.Value} seconds")}, up to {Damage($"{MaxStacksBase.Value}")} {Stack($"(+{MaxStacksPerStack.Value} per stack)")} times.
    {Utility("Slow")} enemies for {Utility($"-{MovementSpeedSlowBase.Value}%")} {Stack($"(-{MovementSpeedSlowPerStack.Value}% per stack)")} {Utility("movement speed")} and {Utility($"-{AttackSpeedSlowBase.Value}%")} {Stack($"(-{AttackSpeedSlowPerStack.Value}% per stack)")} {Utility("attack speed")} for {Utility($"{SlowDuration.Value} seconds")}.
    {Healing("Poison")} enemies for {Damage($"{PoisonDamageBase.Value}%")} {Stack($"(+{PoisonDamagePerStack.Value}% per stack)")} base damage over {Damage($"{PoisonDuration.Value} seconds")}.
Effects stack with component items.";
        public override string ItemTokenLore => "Seepage from the wounds of a warrior deity, sealed in an arcanist's orb following a campaign of vicious slaughter.";
        public override string ItemDefGUID => Assets.OrbOfCorrosion.ItemDef;
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
        public ConfigEntry<float> MovementSpeedSlowBase;
        public ConfigEntry<float> MovementSpeedSlowPerStack;
        public ConfigEntry<float> AttackSpeedSlowBase;
        public ConfigEntry<float> AttackSpeedSlowPerStack;
        public ConfigEntry<float> SlowDuration;
        public ConfigEntry<float> PoisonDamageBase;
        public ConfigEntry<float> PoisonDamagePerStack;
        public ConfigEntry<float> PoisonDuration;
        public void CreateConfig(ConfigFile configuration)
        {
            ArmorReduction = configuration.Bind("Item: " + ItemName, "Armor Reduction Per Stack", 7.5f, "How much armor should be removed per stack of the debuff?");
            ModSettingsManager.AddOption(new FloatFieldOption(ArmorReduction));
            MaxStacksBase = configuration.Bind("Item: " + ItemName, "Base Max Stacks", 3, "How many debuff stacks can be applied by the first stack?");
            ModSettingsManager.AddOption(new IntFieldOption(MaxStacksBase));
            MaxStacksPerStack = configuration.Bind("Item: " + ItemName, "Stacking Max Stacks", 2, "How many debuff stacks can be applied by subsequent stack?");
            ModSettingsManager.AddOption(new IntFieldOption(MaxStacksPerStack));
            ArmorReductionDuration = configuration.Bind("Item: " + ItemName, "Armor Reduction Duration", 3.0f, "How long should the armor reduction last?");
            ModSettingsManager.AddOption(new FloatFieldOption(ArmorReductionDuration));

            MovementSpeedSlowBase = configuration.Bind("Item: " + ItemName, "Movement Speed Reduction Base", 15.0f, "How much should movement speed be reduced initially?");
            ModSettingsManager.AddOption(new FloatFieldOption(MovementSpeedSlowBase));
            MovementSpeedSlowPerStack = configuration.Bind("Item: " + ItemName, "Movement Speed Reduction Per Stack", 15.0f, "How much should movement speed be reduced per stack?");
            ModSettingsManager.AddOption(new FloatFieldOption(MovementSpeedSlowPerStack));
            AttackSpeedSlowBase = configuration.Bind("Item: " + ItemName, "Attack Speed Reduction Base", 15.0f, "How much should attack speed be reduced initially?");
            ModSettingsManager.AddOption(new FloatFieldOption(AttackSpeedSlowBase));
            AttackSpeedSlowPerStack = configuration.Bind("Item: " + ItemName, "Attack Speed Reduction Per Stack", 15.0f, "How much should attack speed be reduced per stack?");
            ModSettingsManager.AddOption(new FloatFieldOption(AttackSpeedSlowPerStack));
            SlowDuration = configuration.Bind("Item: " + ItemName, "Slow Duration", 3.0f, "How long should the slows last?");
            ModSettingsManager.AddOption(new FloatFieldOption(SlowDuration));

            PoisonDamageBase = configuration.Bind("Item: " + ItemName, "Poison Damage Base", 300.0f, "How much base damage should the poison do with the first stack?");
            ModSettingsManager.AddOption(new FloatFieldOption(PoisonDamageBase));
            PoisonDamagePerStack = configuration.Bind("Item: " + ItemName, "Poison Damage Per Stack", 300.0f, "How much base damage should the poison do with subsequent stacks?");
            ModSettingsManager.AddOption(new FloatFieldOption(PoisonDamagePerStack));
            PoisonDuration = configuration.Bind("Item: " + ItemName, "Poison Duration", 3.0f, "How long should the poison last?");
            ModSettingsManager.AddOption(new FloatFieldOption(PoisonDuration));
        }

        private void OnHit(On.RoR2.HealthComponent.orig_TakeDamage orig, RoR2.HealthComponent self, RoR2.DamageInfo info)
        {
            if (self && self.alive && info.attacker && info.procCoefficient > 0.0f)
            {
                var attacker_body = info.attacker.GetComponent<CharacterBody>();
                int count = GetCount(attacker_body);
                if (count > 0)
                {
                    OrbOfCorrosionArmor.ApplyTo(
                        body: self.body,
                        duration: ArmorReductionDuration.Value,
                        stacks: 1,
                        max_stacks: MaxStacksBase.Value + MaxStacksPerStack.Value * (count - 1)
                    );
                    OrbOfCorrosionSlow.ApplyTo(
                        body: self.body,
                        duration: SlowDuration.Value,
                        stacks: count,
                        max_stacks: count
                    );
                    OrbOfCorrosionPoison.ApplyTo(
                        victim: self.body,
                        attacker: attacker_body,
                        duration: PoisonDuration.Value,
                        damage: (PoisonDamageBase.Value + PoisonDamagePerStack.Value * (count - 1)) / 100.0f / PoisonDuration.Value,
                        stacks: 1,
                        max_stacks: 1
                    );
                }
            }

            orig(self, info);
        }
    }
}