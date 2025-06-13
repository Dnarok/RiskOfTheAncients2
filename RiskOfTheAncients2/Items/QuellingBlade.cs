using BepInEx.Configuration;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;

namespace ROTA2.Items
{
    public class QuellingBlade : ItemBase<QuellingBlade>
    {
        public override string ItemName => "Quelling Blade";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "QUELLING_BLADE";
        public override string ItemTokenPickup => "Receive flat skill damage increase against non-boss enemies.";
        public override string ItemTokenDesc => $"Increase {Damage($"outgoing skill damage")} by {Damage($"{DamageBase.Value}")} {Stack($"(+{DamagePerStack.Value} per stack)")} against {Damage("non-Boss enemies")}.";
        public override string ItemTokenLore => "The axe of a fallen gnome, it allows you to effectively maneuver the forest.";
        public override string ItemDefGUID => Assets.QuellingBlade.ItemDef;
        public override void Hooks()
        {
            On.RoR2.HealthComponent.TakeDamage += OnDamageDealt;
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
        public void CreateConfig(ConfigFile configuration)
        {
            DamageBase = configuration.Bind("Item: " + ItemName, "Damage Base", 8.0f, "How much flat damage should the first stack provide?");
            ModSettingsManager.AddOption(new FloatFieldOption(DamageBase));
            DamagePerStack = configuration.Bind("Item: " + ItemName, "Damage Per Stack", 8.0f, "How much flat damage should subsequent stacks provide?");
            ModSettingsManager.AddOption(new FloatFieldOption(DamagePerStack));
        }

        private void OnDamageDealt(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo info)
        {
            if (info.rejected || info.procCoefficient == 0.0f || !info.damageType.IsDamageSourceSkillBased)
            {
                orig(self, info);
                return;
            }

            if (info.attacker && self && self.body && !self.body.isBoss)
            {
                CharacterBody attacker_body = info.attacker.GetComponent<CharacterBody>();
                int count = GetCount(attacker_body);
                if (count > 0)
                {
                    info.damage += (DamageBase.Value + DamagePerStack.Value * (count - 1)) * info.procCoefficient;
                }
            }

            orig(self, info);
        }
    }
}