using BepInEx.Configuration;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
using ROTA2.Buffs;
using UnityEngine;

namespace ROTA2.Items
{
    public class SparkOfCourage : ItemBase<SparkOfCourage>
    {
        public override string ItemName => "Spark of Courage";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "SPARK_OF_COURAGE";
        public override string ItemTokenPickup => "Increases damage at high health, and armor at low health.";
        public override string ItemTokenDesc => $"Increases {Damage("damage")} by {Damage($"{DamageBase.Value}%")} {Stack($"(+{DamagePerStack.Value}% per stack)")} when {Health($"above {HealthThreshold.Value}% health")}, and increases {Utility("armor")} by {Utility($"{ArmorBase.Value}")} {Stack($"(+{ArmorPerStack.Value} per stack")} when {Health("below")}.";
        public override string ItemTokenLore => "Be strong, fortunes can change on a dime.";
        public override string ItemDefGUID => Assets.SparkOfCourage.ItemDef;
        public override void Hooks()
        {
            CharacterBody.onBodyInventoryChangedGlobal += OnInventoryChanged;
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
        public ConfigEntry<float> ArmorBase;
        public ConfigEntry<float> ArmorPerStack;
        public ConfigEntry<float> HealthThreshold;
        public void CreateConfig(ConfigFile configuration)
        {
            DamageBase = configuration.Bind("Item: " + ItemName, "Initial Damage Bonus", 15.0f, "How much damage should be provided by the first stack?");
            ModSettingsManager.AddOption(new FloatFieldOption(DamageBase));
            DamagePerStack = configuration.Bind("Item: " + ItemName, "Stacking Damage Bonus", 15.0f, "How much damage should be provided by subsequent stacks?");
            ModSettingsManager.AddOption(new FloatFieldOption(DamagePerStack));
            ArmorBase = configuration.Bind("Item: " + ItemName, "Initial Armor Bonus", 10.0f, "How much armor should be provided by the first stack?");
            ModSettingsManager.AddOption(new FloatFieldOption(ArmorBase));
            ArmorPerStack = configuration.Bind("Item: " + ItemName, "Stacking Armor Bonus", 10.0f, "How much armor should be provided by subsequent stacks?");
            ModSettingsManager.AddOption(new FloatFieldOption(ArmorPerStack));
            HealthThreshold = configuration.Bind("Item: " + ItemName, "Health Threshold", 50.0f, "At what percent of maximum health should the bonus flip from damage to armor?");
            ModSettingsManager.AddOption(new FloatFieldOption(HealthThreshold));
        }

        private void OnInventoryChanged(CharacterBody body)
        {
            if (GetCount(body) > 0 && !body.GetComponent<SparkOfCourageBehavior>())
            {
                body.gameObject.AddComponent<SparkOfCourageBehavior>();
            }
        }

        private class SparkOfCourageBehavior : MonoBehaviour
        {
            CharacterBody body;
            bool last_below = false;

            void Awake()
            {
                body = GetComponent<CharacterBody>();
            }
            void Start()
            {
                if (GetCount(body) <= 0)
                {
                    return;
                }

                bool below = body.healthComponent.combinedHealthFraction < Instance.HealthThreshold.Value / 100.0f;
                if (below)
                {
                    body.AddBuff(SparkOfCourageArmor.GetBuffDef());
                }
                else
                {
                    body.AddBuff(SparkOfCourageDamage.GetBuffDef());
                }
                last_below = below;
            }
            void FixedUpdate()
            {
                if (GetCount(body) <= 0)
                {
                    return;
                }

                bool below = body.healthComponent.combinedHealthFraction < Instance.HealthThreshold.Value / 100.0f;
                if (below && !last_below)
                {
                    body.AddBuff(SparkOfCourageArmor.GetBuffDef());
                    body.RemoveBuff(SparkOfCourageDamage.GetBuffDef());
                }
                else if (!below && last_below)
                {
                    body.RemoveBuff(SparkOfCourageArmor.GetBuffDef());
                    body.AddBuff(SparkOfCourageDamage.GetBuffDef());
                }
                last_below = below;
            }
        }
    }
}