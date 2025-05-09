using BepInEx.Configuration;
using R2API;
using RoR2;

namespace ROTA2.Items
{
    public class SparkOfCourage : ItemBase<SparkOfCourage>
    {
        public override string ItemName => "Spark of Courage";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "SPARK_OF_COURAGE";
        public override string ItemTokenPickup => "Increases damage at high health, and armor at low health.";
        public override string ItemTokenDesc => $"Increases {Damage("damage")} by {Damage($"{DamageBase}%")} {Stack($"(+{DamagePerStack}% per stack)")} when {Health($"above {HealthThreshold}% health")}, and increases {Utility("armor")} by {Utility($"{ArmorBase}")} {Stack($"(+{ArmorPerStack} per stack")} when {Health("below")}.";
        public override string ItemTokenLore => "Be strong, fortunes can change on a dime.";
        public override ItemTier Tier => ItemTier.Tier1;
        public override string ItemModelPath => "RoR2/Base/Mystery/PickupMystery.prefab";
        public override string ItemIconPath => "RiskOfTheAncients2.Icons.spark_of_courage.png";
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddDamage;
            RecalculateStatsAPI.GetStatCoefficients += AddArmor;
        }
        public override void Init(ConfigFile configuration)
        {
            CreateConfig(configuration);
            CreateLanguageTokens();
            CreateItemDef();
            Hooks();
        }

        public float DamageBase;
        public float DamagePerStack;
        public float ArmorBase;
        public float ArmorPerStack;
        public float HealthThreshold;
        public void CreateConfig(ConfigFile configuration)
        {
            DamageBase = configuration.Bind("Item: " + ItemName, "Initial Damage Bonus", 8.0f, "How much damage should be provided by the first stack?").Value;
            DamagePerStack = configuration.Bind("Item: " + ItemName, "Stacking Damage Bonus", 8.0f, "How much damage should be provided by subsequent stacks?").Value;
            ArmorBase = configuration.Bind("Item: " + ItemName, "Initial Armor Bonus", 8.0f, "How much armor should be provided by the first stack?").Value;
            ArmorPerStack = configuration.Bind("Item: " + ItemName, "Stacking Armor Bonus", 8.0f, "How much armor should be provided by subsequent stacks?").Value;
            HealthThreshold = configuration.Bind("Item: " + ItemName, "Health Threshold", 50.0f, "At what percent of maximum health should the bonus flip from damage to armor?").Value;
        }

        private void AddDamage(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs arguments)
        {
            int count = GetCount(body);
            if (count > 0)
            {
                HealthComponent health = body.GetComponent<HealthComponent>();
                if (health && health.combinedHealthFraction >= HealthThreshold / 100.0f)
                {
                    arguments.damageMultAdd += DamageBase / 100.0f + DamagePerStack / 100.0f * (count - 1);
                }
            }
        }
        private void AddArmor(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs arguments)
        {
            int count = GetCount(body);
            if (count > 0)
            {
                HealthComponent health = body.GetComponent<HealthComponent>();
                if (health && health.combinedHealthFraction < HealthThreshold / 100.0f)
                {
                    arguments.armorAdd += ArmorBase + ArmorPerStack * (count - 1);
                }
            }
        }
    }
}