using BepInEx.Configuration;
using IL.RoR2.Achievements.Railgunner;
using RoR2;
using ROTA2.Buffs;
using System;
using UnityEngine;

namespace ROTA2.Items
{
    public class NemesisCurse : ItemBase<NemesisCurse>
    {
        public override string ItemName => "Nemesis Curse";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "NEMESIS_CURSE";
        public override string ItemTokenPickup => $"Curse on hit for increased damage... {Death("BUT receive permanent damage when hit.")}.";
        public override string ItemTokenDesc => 
@$"{Damage($"{OutProcChance}%")} chance on hit to {Damage("curse")} an enemy, increasing {Damage("all damage")} they take by {Damage($"{DamageBase}%")} {Stack($"(+{DamagePerStack}% per stack, exponential)")} for {Damage($"{CurseDuration} seconds")}. Recharges every {Damage($"{CurseCooldown} seconds")}.
{Health($"{InProcChance}%")} chance on {Health("being hit")} to be {Health("cursed")}, receiving {Death("permanent damage")}.";
        public override string ItemTokenLore => "The cursed amulet of a revenge-obsessed prince.";
        public override string ItemIconPath => "RiskOfTheAncients2.Icons.nemesis_curse.png";
        public override ItemTier Tier => ItemTier.Lunar;
        public override void Hooks()
        {
            On.RoR2.HealthComponent.TakeDamage += OnTakeDamage;
            On.RoR2.HealthComponent.UpdateLastHitTime += OnHit;
        }

        public override void Init(ConfigFile configuration)
        {
            CreateConfig(configuration);
            CreateLanguageTokens();
            CreateItemDef();
            Hooks();
        }

        public float OutProcChance;
        public float InProcChance;
        public float DamageBase;
        public float DamagePerStack;
        public float PermanentDamageCoefficientBase;
        public float PermanentDamageCoefficientPerStack;
        public float CurseDuration;
        public float CurseCooldown;
        private void CreateConfig(ConfigFile configuration)
        {
            OutProcChance                      = configuration.Bind("Item: " + ItemName, "Enemy Proc Chance",                      100.0f, "").Value;
            InProcChance                       = configuration.Bind("Item: " + ItemName, "Self Proc Chance",                       100.0f, "").Value;
            DamageBase                         = configuration.Bind("Item: " + ItemName, "Damage Increase Base",                   100.0f, "Exponential").Value;
            DamagePerStack                     = configuration.Bind("Item: " + ItemName, "Damage Increase Per Stack",              100.0f, "Exponential").Value;
            PermanentDamageCoefficientBase     = configuration.Bind("Item: " + ItemName, "Permanent Damage Coefficient Base",      40.0f, "Permanent damage curse stacks formula: ([this number] + [per stack number]) * Damage / Max Health. Maximum health is reduced by a factor of 1 + 0.01 * N, where N is the number of stacks total.").Value;
            PermanentDamageCoefficientPerStack = configuration.Bind("Item: " + ItemName, "Permanent Damage Coefficient Per Stack", 40.0f, "").Value;
            CurseDuration                      = configuration.Bind("Item: " + ItemName, "Curse Duration", 5.0f, "").Value;
            CurseCooldown                      = configuration.Bind("Item: " + ItemName, "Curse Cooldown", 5.0f, "").Value;
        }

        private void OnTakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, RoR2.HealthComponent self, RoR2.DamageInfo info)
        {
            if (self && info.attacker)
            {
                CharacterBody attacker_body = info.attacker.GetComponent<CharacterBody>();
                if (self.body && !NemesisCurseCooldown.Instance.HasThisBuff(attacker_body))
                {
                    int count = GetCount(attacker_body);
                    if (count > 0 && Util.CheckRoll(OutProcChance, attacker_body.master))
                    {
                        NemesisCurseBuff.Instance.ApplyTo(new BuffBase.ApplyParameters
                        {
                            victim = self.body,
                            duration = CurseDuration,
                            stacks = count,
                            max_stacks = count
                        });
                        NemesisCurseCooldown.Instance.ApplyTo(new BuffBase.ApplyParameters
                        {
                            victim = attacker_body,
                            duration = CurseCooldown
                        });
                    }
                }
            }

            orig(self, info);
        }
        private void OnHit(On.RoR2.HealthComponent.orig_UpdateLastHitTime orig, HealthComponent self, float damageValue, Vector3 damagePosition, bool damageIsSilent, GameObject attacker, bool delayedDamage, bool firstHitOfDelayedDamage)
        {
            if (self)
            {
                int count = GetCount(self.body);
                if (count > 0 && !Util.CheckRoll(100.0f - InProcChance, self.body.master))
                {
                    float stacks = damageValue * (PermanentDamageCoefficientBase + PermanentDamageCoefficientPerStack * (count - 1)) / self.fullCombinedHealth;
                    for (int i = 0; i < Mathf.FloorToInt(stacks); ++i)
                    {
                        self.body.AddBuff(RoR2Content.Buffs.PermanentCurse);
                    }
                }
            }

            orig(self, damageValue, damagePosition, damageIsSilent, attacker, delayedDamage, firstHitOfDelayedDamage);
        }
    }
}