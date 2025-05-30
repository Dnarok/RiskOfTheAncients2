using BepInEx.Configuration;
using ROTA2.Buffs;
using RoR2;
using UnityEngine;

namespace ROTA2.Items
{
    public class SkullBasher : ItemBase<SkullBasher>
    {
        public override string ItemName => "Skull Basher";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "SKULL_BASHER";
        public override string ItemTokenPickup => "Chance on hit to strike for extra damage and stun. Recharges over time.";
        public override string ItemTokenDesc => $"{Damage($"{ProcChance}%")} chance on hit to {Damage("bash")} for {Damage($"{DamageBase}%")} {Stack($"(+{DamagePerStack}% per stack)")} TOTAL damage and {Utility("stun")} enemies for {Utility($"{BashDuration} seconds")}. Recharges every {Utility($"{BashCooldown} seconds")}.";
        public override string ItemTokenLore => "A rather typical spear that can sometimes pierce through an enemy's armor when used to attack.";
        public override ItemTier Tier => ItemTier.Tier2;
        public override ItemTag[] ItemTags => [ItemTag.Damage];
        public override string ItemIconPath => "ROTA2.Icons.skull_basher.png";
        public override string ItemModelPath => "skull_basher.prefab";
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
        public float BashDuration;
        public float BashCooldown;
        public void CreateConfig(ConfigFile configuration)
        {
            ProcChance     = configuration.Bind("Item: " + ItemName, "Proc Chance",      15.0f,  "What is the chance on hit to proc?").Value;
            DamageBase     = configuration.Bind("Item: " + ItemName, "Damage Base",      250.0f, "How much total damage should the bash do with the first stack?").Value;
            DamagePerStack = configuration.Bind("Item: " + ItemName, "Damage Per Stack", 250.0f, "How much extra total damage should the bash do for subsequent stacks?").Value;
            BashDuration   = configuration.Bind("Item: " + ItemName, "Bash Duration",    2.5f,   "How long should the bash last?").Value;
            BashCooldown   = configuration.Bind("Item: " + ItemName, "Bash Cooldown",    5.0f,  "How long should it take to recharge before another bash?").Value;
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
                HealthComponent victim_health = victim.GetComponent<HealthComponent>();
                if (attacker_body && victim_health && !SkullBasherCooldown.Instance.HasThisBuff(attacker_body))
                {
                    int count = GetCount(attacker_body);
                    if (count > 0 && Util.CheckRoll(ProcChance * info.procCoefficient, attacker_body.master))
                    {
                        DamageInfo bonus_hit = new()
                        {
                            attacker = attacker,
                            damage = info.damage * (DamageBase / 100.0f + DamagePerStack / 100.0f * (count - 1)),
                            position = info.position,
                            damageColorIndex = DamageColorIndex.Sniper,
                            damageType = info.damageType,
                            procCoefficient = 0.0f,
                            crit = info.crit,
                            procChainMask = info.procChainMask
                        };
                        victim_health.TakeDamage(bonus_hit);

                        SetStateOnHurt.SetStunOnObject(victim, BashDuration);

                        SkullBasherCooldown.Instance.ApplyTo(new BuffBase.ApplyParameters
                        {
                            victim = attacker_body,
                            duration = BashCooldown
                        });

                        Util.PlaySound("SkullBasher", victim);
                    }
                }
            }

            orig(self, info, victim);
        }
    }
}