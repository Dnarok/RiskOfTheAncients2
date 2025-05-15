using BepInEx.Configuration;
using ROTA2.Buffs;
using RoR2;

namespace ROTA2.Items
{
    public class OrbOfVenom : ItemBase<OrbOfVenom>
    {
        public override string ItemName => "Orb of Venom";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "ORB_OF_VENOM";
        public override string ItemTokenPickup => "Poisons for a short time on hit.";
        public override string ItemTokenDesc => $"On hit {Healing("poison")} for {Damage($"{PoisonDamageBase}%")} {Stack($"(+{PoisonDamagePerStack}% per stack)")} base damage over {Damage($"{PoisonDuration} seconds")}.";
        public override string ItemTokenLore => "Envenoms your veapon with the venom of a venomous viper.";
        public override ItemTier Tier => ItemTier.Tier1;
        public override string ItemIconPath => "ROTA2.Icons.orb_of_venom.png";
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

        public float PoisonDamageBase;
        public float PoisonDamagePerStack;
        public float PoisonDuration;
        public void CreateConfig(ConfigFile configuration)
        {
            PoisonDamageBase = configuration.Bind("Item: " + ItemName, "Poison Damage Base", 200.0f, "How much base damage should the poison do with the first stack?").Value;
            PoisonDamagePerStack = configuration.Bind("Item: " + ItemName, "Poison Damage Per Stack", 200.0f, "How much base damage should the poison do with subsequent stacks?").Value;
            PoisonDuration = configuration.Bind("Item: " + ItemName, "Poison Duration", 3.0f, "How long should the poison last?").Value;
        }

        private void OnHit(On.RoR2.HealthComponent.orig_TakeDamage orig, RoR2.HealthComponent self, RoR2.DamageInfo info)
        {
            if (self && self.alive && info.attacker && info.procCoefficient > 0.0f)
            {
                var attacker_body = info.attacker.GetComponent<CharacterBody>();
                int count = GetCount(attacker_body);
                if (count > 0)
                {
                    OrbOfVenomBuff.Instance.ApplyTo(new BuffBase.ApplyParameters
                    {
                        victim = self.body,
                        attacker = attacker_body,
                        duration = PoisonDuration,
                        damage = (PoisonDamageBase + PoisonDamagePerStack * (count - 1)) / 100.0f / PoisonDuration,
                        max_stacks = 1
                    });
                }
            }

            orig(self, info);
        }
    }
}