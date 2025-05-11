using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;

namespace ROTA2.Items
{
    public class DragonScale : ItemBase<DragonScale>
    {
        public override string ItemName => "Dragon Scale";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "DRAGON_SCALE";
        public override string ItemTokenPickup => "Chance to burn enemies on hit.";
        public override string ItemTokenDesc => $"{Damage($"{ProcChance}%")} chance on hit to {Damage("burn")} enemies for {Damage($"{DamageBase}%")} {Stack($"(+{DamagePerStack}% per stack)")} base damage.";
        public override string ItemTokenLore => "The remains of a dragon always outvalue the wealth of even the most prodigious hoard.";
        public override ItemTier Tier => ItemTier.Tier1;
        public override string ItemIconPath => "ROTA2.Icons.dragon_scale.png";
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

        public float ProcChance;
        public float DamageBase;
        public float DamagePerStack;
        public float BurnDuration;
        public void CreateConfig(ConfigFile configuration)
        {
            ProcChance = configuration.Bind("Item: " + ItemName, "Proc Chance", 20.0f, "What is the chance on hit to proc?").Value;
            DamageBase = configuration.Bind("Item: " + ItemName, "Damage Base", 60.0f, "How much base damage should the burn do with the first stack?").Value;
            DamagePerStack = configuration.Bind("Item: " + ItemName, "Damage Per Stack", 60.0f, "How much extra base damage should the burn do with subsequent stacks?").Value;
            BurnDuration = configuration.Bind("Item: " + ItemName, "Burn Duration", 3.0f, "How long should each stack of burn last?").Value;
        }

        private void OnHit(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo info, GameObject victim)
        {
            if (info.rejected || info.procCoefficient <= 0)
            {
                orig(self, info, victim);
                return;
            }

            GameObject attacker = info.attacker;
            if (attacker)
            {
                CharacterBody attacker_body = attacker.GetComponent<CharacterBody>();
                if (attacker_body)
                {
                    int count = GetCount(attacker_body);
                    if (count > 0 && Util.CheckRoll(ProcChance * info.procCoefficient, attacker_body.master))
                    {
                        InflictDotInfo burn = new()
                        {
                            attackerObject = attacker,
                            victimObject = victim,
                            dotIndex = DotController.DotIndex.Burn,
                            duration = BurnDuration,
                            damageMultiplier = DamageBase / 100.0f + DamagePerStack / 100.0f * (count - 1)
                        };
                        StrengthenBurnUtils.CheckDotForUpgrade(attacker_body.inventory, ref burn);
                        DotController.InflictDot(ref burn);
                    }
                }
            }

            orig(self, info, victim);
        }
    }
}