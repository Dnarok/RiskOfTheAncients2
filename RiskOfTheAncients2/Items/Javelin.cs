using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;

namespace ROTA2.Items
{
    public class Javelin : ItemBase<Javelin>
    {
        public override string ItemName => "Javelin";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "JAVELIN";
        public override string ItemTokenPickup => "Chance on hit to strike a second time for small damage.";
        public override string ItemTokenDesc => $"{Damage($"{ProcChance}%")} chance on hit to {Damage("hit again")} for {Damage($"{DamageBase}%")} {Stack($"(+{DamagePerStack}% per stack)")} TOTAL damage.";
        public override string ItemTokenLore => "A rather typical spear that can sometimes pierce through an enemy's armor when used to attack.";
        public override ItemTier Tier => ItemTier.Tier1;
        public override string ItemIconPath => "RiskOfTheAncients2.Icons.javelin.png";
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

            proc = ProcTypeAPI.ReserveProcType();
        }

        public float ProcChance;
        public float DamageBase;
        public float DamagePerStack;
        public float ProcCoefficient;
        public void CreateConfig(ConfigFile configuration)
        {
            ProcChance       = configuration.Bind("Item: " + ItemName, "Proc Chance",       20.0f,  "What is the chance on hit to proc?").Value;
            DamageBase       = configuration.Bind("Item: " + ItemName, "Damage Base",       25.0f,  "How much total damage should the bonus hit do on the first stack?").Value;
            DamagePerStack   = configuration.Bind("Item: " + ItemName, "Damage Per Stack",  15.0f,  "How much extra total damage should the bonus hit do for subsequent stacks?").Value;
            ProcCoefficient  = configuration.Bind("Item: " + ItemName, "Proc Coefficient",  0.2f,   "What is the proc coefficient of the bonus hit?").Value;
        }

        private ModdedProcType proc;
        private void OnHit(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo info, GameObject victim)
        {
            if (info.rejected || info.procCoefficient <= 0 || info.procChainMask.HasModdedProc(proc))
            {
                orig(self, info, victim);
                return;
            }

            GameObject attacker = info.attacker;
            if (attacker)
            {
                CharacterBody attacker_body = attacker.GetComponent<CharacterBody>();
                HealthComponent victim_health = victim.GetComponent<HealthComponent>();
                if (attacker_body && victim_health)
                {
                    int count = GetCount(attacker_body);
                    if (count > 0 && Util.CheckRoll(ProcChance * info.procCoefficient, attacker_body.master))
                    {
                        ProcChainMask new_mask = info.procChainMask;
                        new_mask.AddModdedProc(proc);
                        DamageInfo bonus_hit = new()
                        {
                            attacker = attacker,
                            damage = info.damage * (DamageBase / 100.0f + DamagePerStack / 100.0f * (count - 1)),
                            position = info.position,
                            damageColorIndex = DamageColorIndex.Void,
                            damageType = info.damageType,
                            procCoefficient = ProcCoefficient,
                            crit = info.crit,
                            procChainMask = new_mask
                        };

                        victim_health.TakeDamage(bonus_hit);
                    }
                }
            }

            orig(self, info, victim);
        }
    }
}