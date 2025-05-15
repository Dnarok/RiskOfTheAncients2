using BepInEx.Configuration;
using ROTA2.Buffs;
using RoR2;

namespace ROTA2.Items
{
    public class OrbOfBlight : ItemBase<OrbOfBlight>
    {
        public override string ItemName => "Orb of Blight";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "ORB_OF_BLIGHT";
        public override string ItemTokenPickup => "Reduces armor on hit, up to a max.";
        public override string ItemTokenDesc => $"On hit reduce {Damage("armor")} by {Damage($"{ArmorReduction}")} for {Damage($"{ArmorReductionDuration} seconds")}, up to {Damage($"{MaxStacksBase} times")} {Stack($"(+{MaxStacksPerStack} per stack)")}.";
        public override string ItemTokenLore => "An unnerving stone unearthed beneath the Fields of Endless Carnage.";
        public override ItemTier Tier => ItemTier.Tier1;
        public override string ItemIconPath => "ROTA2.Icons.orb_of_blight.png";
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

        public float ArmorReduction;
        public int MaxStacksBase;
        public int MaxStacksPerStack;
        public float ArmorReductionDuration;
        public void CreateConfig(ConfigFile configuration)
        {
            ArmorReduction = configuration.Bind("Item: " + ItemName, "Armor Reduction Per Stack", 5.0f, "How much armor should be removed per stack of the debuff?").Value;
            MaxStacksBase = configuration.Bind("Item: " + ItemName, "Base Max Stacks", 3, "How many debuff stacks can be applied by the first stack?").Value;
            MaxStacksPerStack = configuration.Bind("Item: " + ItemName, "Stacking Max Stacks", 2, "How many debuff stacks can be applied by subsequent stack?").Value;
            ArmorReductionDuration = configuration.Bind("Item: " + ItemName, "Armor Reduction Duration", 3.0f, "How long should the armor reduction last?").Value;
        }

        private void OnHit(On.RoR2.HealthComponent.orig_TakeDamage orig, RoR2.HealthComponent self, RoR2.DamageInfo info)
        {
            if (self && self.alive && info.attacker && info.procCoefficient > 0.0f)
            {
                var attacker_body = info.attacker.GetComponent<CharacterBody>();
                int count = GetCount(attacker_body);
                if (count > 0)
                {
                    OrbOfBlightBuff.Instance.ApplyTo(new BuffBase.ApplyParameters
                    {
                        victim = self.body,
                        duration = ArmorReduction,
                        max_stacks = MaxStacksBase + MaxStacksPerStack * (count - 1)
                    });
                }
            }

            orig(self, info);
        }
    }
}