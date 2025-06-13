using BepInEx.Configuration;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
using ROTA2.Buffs;

namespace ROTA2.Items
{
    public class OrbOfVenom : ItemBase<OrbOfVenom>
    {
        public override string ItemName => "Orb of Venom";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "ORB_OF_VENOM";
        public override string ItemTokenPickup => "Poisons for a short time on hit.";
        public override string ItemTokenDesc => $"On hit {Healing("poison")} for {Damage($"{PoisonDamageBase.Value}%")} {Stack($"(+{PoisonDamagePerStack.Value}% per stack)")} base damage over {Damage($"{PoisonDuration.Value} seconds")}.";
        public override string ItemTokenLore => "Envenoms your veapon with the venom of a venomous viper.";
        public override string ItemDefGUID => Assets.OrbOfVenom.ItemDef;
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

        public ConfigEntry<float> PoisonDamageBase;
        public ConfigEntry<float> PoisonDamagePerStack;
        public ConfigEntry<float> PoisonDuration;
        public void CreateConfig(ConfigFile configuration)
        {
            PoisonDamageBase = configuration.Bind("Item: " + ItemName, "Poison Damage Base", 200.0f, "How much base damage should the poison do with the first stack?");
            ModSettingsManager.AddOption(new FloatFieldOption(PoisonDamageBase));
            PoisonDamagePerStack = configuration.Bind("Item: " + ItemName, "Poison Damage Per Stack", 200.0f, "How much base damage should the poison do with subsequent stacks?");
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
                    OrbOfVenomBuff.ApplyTo(
                        victim: self.body,
                        attacker: attacker_body,
                        duration: PoisonDuration.Value,
                        damage: (PoisonDamageBase.Value + PoisonDamagePerStack.Value * (count - 1)) / 100f / PoisonDuration.Value,
                        stacks: 1,
                        max_stacks: 1
                    );
                }
            }

            orig(self, info);
        }
    }
}