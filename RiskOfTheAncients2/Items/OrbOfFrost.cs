using BepInEx.Configuration;
using ROTA2.Buffs;
using RoR2;

namespace ROTA2.Items
{
    public class OrbOfFrost : ItemBase<OrbOfFrost>
    {
        public override string ItemName => "Orb of Frost";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "ORB_OF_FROST";
        public override string ItemTokenPickup => "Slightly slow enemies on hit.";
        public override string ItemTokenDesc => $"{Utility("Slow")} enemies on hit for {Utility($"{MovementSpeedSlowBase}% movement speed")} {Stack($"(-{MovementSpeedSlowPerStack}% per stack)")} and {Utility($"-{AttackSpeedSlowBase}% attack speed")} {Stack($"-{AttackSpeedSlowPerStack}% per stack)")} for {Utility($"{DebuffDuration} seconds")}.";
        public override string ItemTokenLore => "Freezes your foes with a frosty force.";
        public override ItemTier Tier => ItemTier.Tier1;
        public override string ItemIconPath => "ROTA2.Icons.orb_of_frost.png";
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

        public float MovementSpeedSlowBase;
        public float MovementSpeedSlowPerStack;
        public float AttackSpeedSlowBase;
        public float AttackSpeedSlowPerStack;
        public float DebuffDuration;
        public void CreateConfig(ConfigFile configuration)
        {
            MovementSpeedSlowBase     = configuration.Bind("Item: " + ItemName, "Movement Speed Reduction Base",      10.0f, "How much should movement speed be reduced initially?").Value;
            MovementSpeedSlowPerStack = configuration.Bind("Item: " + ItemName, "Movement Speed Reduction Per Stack", 10.0f, "How much should movement speed be reduced per stack?").Value;
            AttackSpeedSlowBase       = configuration.Bind("Item: " + ItemName, "Attack Speed Reduction Base",        10.0f, "How much should attack speed be reduced initially?").Value;
            AttackSpeedSlowPerStack   = configuration.Bind("Item: " + ItemName, "Attack Speed Reduction Per Stack",   10.0f, "How much should attack speed be reduced per stack?").Value;
            DebuffDuration            = configuration.Bind("Item: " + ItemName, "Slow Duration",                      3.0f,  "How long should the slows last?").Value;
        }

        private void OnHit(On.RoR2.HealthComponent.orig_TakeDamage orig, RoR2.HealthComponent self, RoR2.DamageInfo info)
        {
            if (self && self.alive && info.attacker && info.procCoefficient > 0.0f)
            {
                var attacker_body = info.attacker.GetComponent<CharacterBody>();
                int count = GetCount(attacker_body);
                if (count > 0)
                {
                    OrbOfFrostBuff.Instance.ApplyTo(new BuffBase.ApplyParameters
                    {
                        victim = self.body,
                        duration = DebuffDuration,
                        stacks = count,
                        max_stacks = count
                    });
                }
            }

            orig(self, info);
        }
    }
}