using BepInEx.Configuration;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
using ROTA2.Buffs;
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
@$"{Damage($"{OutProcChance.Value}%")} chance on hit to {Damage("curse")} an enemy, increasing {Damage("all damage")} they take by {Damage($"{DamageBase.Value}%")} {Stack($"(+{DamagePerStack.Value}% per stack, exponential)")} for {Damage($"{CurseDuration.Value} seconds")}. Recharges every {Damage($"{CurseCooldown.Value} seconds")}.
{Health($"{InProcChance.Value}%")} chance on {Health("being hit")} to be {Health("cursed")}, receiving {Death("permanent damage")}.";
        public override string ItemTokenLore => "The cursed amulet of a revenge-obsessed prince.";
        public override string ItemDefGUID => Assets.NemesisCurse.ItemDef;
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

        public ConfigEntry<float> OutProcChance;
        public ConfigEntry<float> InProcChance;
        public ConfigEntry<float> DamageBase;
        public ConfigEntry<float> DamagePerStack;
        public ConfigEntry<float> PermanentDamageCoefficientBase;
        public ConfigEntry<float> PermanentDamageCoefficientPerStack;
        public ConfigEntry<float> CurseDuration;
        public ConfigEntry<float> CurseCooldown;
        private void CreateConfig(ConfigFile configuration)
        {
            OutProcChance = configuration.Bind("Item: " + ItemName, "Enemy Proc Chance", 100.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(OutProcChance));
            InProcChance = configuration.Bind("Item: " + ItemName, "Self Proc Chance", 100.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(InProcChance));
            DamageBase = configuration.Bind("Item: " + ItemName, "Damage Increase Base", 100.0f, "Exponential");
            ModSettingsManager.AddOption(new FloatFieldOption(DamageBase));
            DamagePerStack = configuration.Bind("Item: " + ItemName, "Damage Increase Per Stack", 100.0f, "Exponential");
            ModSettingsManager.AddOption(new FloatFieldOption(DamagePerStack));
            PermanentDamageCoefficientBase = configuration.Bind("Item: " + ItemName, "Permanent Damage Coefficient Base", 80.0f, "Permanent damage curse stacks formula: ([this number] + [per stack number]) * Damage / Max Health. Maximum health is reduced by a factor of 1 + 0.01 * N, where N is the number of stacks total.");
            ModSettingsManager.AddOption(new FloatFieldOption(PermanentDamageCoefficientBase));
            PermanentDamageCoefficientPerStack = configuration.Bind("Item: " + ItemName, "Permanent Damage Coefficient Per Stack", 80.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(PermanentDamageCoefficientPerStack));
            CurseDuration = configuration.Bind("Item: " + ItemName, "Curse Duration", 5.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(CurseDuration));
            CurseCooldown = configuration.Bind("Item: " + ItemName, "Curse Cooldown", 7.5f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(CurseCooldown));
        }

        private void OnTakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, RoR2.HealthComponent self, RoR2.DamageInfo info)
        {
            if (self && info.attacker && info.procCoefficient > 0.0f)
            {
                CharacterBody attacker_body = info.attacker.GetComponent<CharacterBody>();
                if (self.body && !NemesisCurseCooldown.HasThisBuff(attacker_body))
                {
                    int count = GetCount(attacker_body);
                    if (count > 0 && Util.CheckRoll(OutProcChance.Value * info.procCoefficient, attacker_body.master))
                    {
                        NemesisCurseBuff.ApplyTo(
                            body: self.body,
                            duration: CurseDuration.Value,
                            stacks: count,
                            max_stacks: count
                        );
                        NemesisCurseCooldown.ApplyTo(
                            body: attacker_body,
                            duration: CurseCooldown.Value
                        );
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
                if (count > 0 && !Util.CheckRoll(100.0f - InProcChance.Value, self.body.master))
                {
                    float stacks = damageValue * (PermanentDamageCoefficientBase.Value + PermanentDamageCoefficientPerStack.Value * (count - 1)) / self.fullCombinedHealth;
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