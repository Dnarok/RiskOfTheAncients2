using BepInEx.Configuration;
using R2API;
using RiskOfOptions;
using RiskOfOptions.Options;
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
        public override string ItemTokenDesc => $"{Damage($"{ProcChance.Value}%")} chance on hit to {Damage("hit again")} for {Damage($"{DamageBase.Value}%")} {Stack($"(+{DamagePerStack.Value}% per stack)")} TOTAL damage.";
        public override string ItemTokenLore => "A rather typical spear that can sometimes pierce through an enemy's armor when used to attack.";
        public override string ItemDefGUID => Assets.Javelin.ItemDef;
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

        public ConfigEntry<float> ProcChance;
        public ConfigEntry<float> DamageBase;
        public ConfigEntry<float> DamagePerStack;
        public ConfigEntry<float> ProcCoefficient;
        public void CreateConfig(ConfigFile configuration)
        {
            ProcChance = configuration.Bind("Item: " + ItemName, "Proc Chance", 20.0f, "What is the chance on hit to proc?");
            ModSettingsManager.AddOption(new FloatFieldOption(ProcChance));
            DamageBase = configuration.Bind("Item: " + ItemName, "Damage Base", 20.0f, "How much total damage should the bonus hit do on the first stack?");
            ModSettingsManager.AddOption(new FloatFieldOption(DamageBase));
            DamagePerStack = configuration.Bind("Item: " + ItemName, "Damage Per Stack", 20.0f, "How much extra total damage should the bonus hit do for subsequent stacks?");
            ModSettingsManager.AddOption(new FloatFieldOption(DamagePerStack));
            ProcCoefficient = configuration.Bind("Item: " + ItemName, "Proc Coefficient", 0.3f, "What is the proc coefficient of the bonus hit?");
            ModSettingsManager.AddOption(new FloatFieldOption(ProcCoefficient));
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
                    if (count > 0 && Util.CheckRoll(ProcChance.Value * info.procCoefficient, attacker_body.master))
                    {
                        ProcChainMask new_mask = info.procChainMask;
                        new_mask.AddModdedProc(proc);
                        DamageInfo bonus_hit = new()
                        {
                            attacker = attacker,
                            damage = info.damage * (DamageBase.Value / 100.0f + DamagePerStack.Value / 100.0f * (count - 1)),
                            position = info.position,
                            damageColorIndex = DamageColorIndex.Void,
                            damageType = info.damageType,
                            procCoefficient = ProcCoefficient.Value,
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