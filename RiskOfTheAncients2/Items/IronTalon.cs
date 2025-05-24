using BepInEx.Configuration;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
namespace ROTA2.Items
{
    public class IronTalon : ItemBase<IronTalon>
    {
        public override string ItemName => "Iron Talon";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "IRON_TALON";
        public override string ItemTokenPickup => "Deal large damage to Boss monsters on your first hit.";
        public override string ItemTokenDesc => $"{Damage("Damage")} boss monsters on {Damage("first hit")} for {Health($"{HealthDamageBase}%")} {Stack($"(+{HealthDamagePerStack} per stack)")} of their {Health("current health")}.";
        public override string ItemTokenLore => "A simple but effective weapon devised to quell a great Hellbear uprising.";
        public override ItemTier Tier => ItemTier.Tier2;
        public override ItemTag[] ItemTags => [ItemTag.Damage];
        public override string ItemIconPath => "ROTA2.Icons.iron_talon.png";
        public override void Hooks()
        {
            On.RoR2.GlobalEventManager.OnHitEnemy += OnHit;
        }
        public override void Init(ConfigFile configuration)
        {
            CreateConfig(configuration);
            CreateLanguageTokens();
            CreateItemDef();
            Hooks();
        }

        public float HealthDamageBase;
        public float HealthDamagePerStack;
        private void CreateConfig(ConfigFile configuration)
        {
            HealthDamageBase     = configuration.Bind("Item: " + ItemName, "Current Health Damage Base",      10.0f, "").Value;
            HealthDamagePerStack = configuration.Bind("Item: " + ItemName, "Current Health Damage Per Stack", 10.0f, "").Value;
        }

        private void OnHit(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo info, GameObject victim)
        {
            if (!NetworkServer.active || info.rejected || info.procCoefficient <= 0)
            {
                orig(self, info, victim);
                return;
            }

            GameObject attacker = info.attacker;
            if (attacker && victim)
            {
                var attacker_body = attacker.GetComponent<CharacterBody>();
                var victim_body = victim.GetComponent<CharacterBody>();
                if (attacker_body && victim_body && victim_body.healthComponent && victim_body.isChampion)
                {
                    int count = GetCount(attacker_body);
                    if (count > 0 && victim.GetComponent<IronTalonBehavior>() == null)
                    {
                        DamageInfo damage = new()
                        {
                            attacker = attacker,
                            damage = victim_body.healthComponent.combinedHealth * Util.ConvertAmplificationPercentageIntoReductionNormalized(HealthDamageBase / 100.0f + HealthDamagePerStack / 100.0f * (count - 1)),
                            crit = false,
                            position = info.position,
                            damageColorIndex = DamageColorIndex.WeakPoint,
                            procCoefficient = 0.0f,
                            damageType = new DamageTypeCombo
                            {
                                damageType = DamageType.BypassArmor
                            }
                        };
                        victim_body.healthComponent.TakeDamage(damage);

                        victim.AddComponent<IronTalonBehavior>();

                        Util.PlaySound("IronTalon", victim);
                    }
                }
            }

            orig(self, info, victim);
        }

        // this currently serves only to tag enemies that we have already hit.
        public class IronTalonBehavior : MonoBehaviour
        {}
    }
}