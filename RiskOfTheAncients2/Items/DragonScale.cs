using BepInEx.Configuration;
using RiskOfOptions;
using RiskOfOptions.Options;
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
        public override string ItemTokenDesc => $"{Damage($"{ProcChance.Value}%")} chance on hit to {Damage("burn")} enemies for {Damage($"{DamageBase.Value}%")} {Stack($"(+{DamagePerStack.Value}% per stack)")} base damage.";
        public override string ItemTokenLore => "The remains of a dragon always outvalue the wealth of even the most prodigious hoard.";
        public override string ItemDefGUID => Assets.DragonScale.ItemDef;
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

        public ConfigEntry<float> ProcChance;
        public ConfigEntry<float> DamageBase;
        public ConfigEntry<float> DamagePerStack;
        public void CreateConfig(ConfigFile configuration)
        {
            ProcChance = configuration.Bind("Item: " + ItemName, "Proc Chance", 20.0f, "What is the chance on hit to proc?");
            ModSettingsManager.AddOption(new FloatFieldOption(ProcChance));
            DamageBase = configuration.Bind("Item: " + ItemName, "Damage Base", 210.0f, "How much base damage should the burn do with the first stack?");
            ModSettingsManager.AddOption(new FloatFieldOption(DamageBase));
            DamagePerStack = configuration.Bind("Item: " + ItemName, "Damage Per Stack", 210.0f, "How much extra base damage should the burn do with subsequent stacks?");
            ModSettingsManager.AddOption(new FloatFieldOption(DamagePerStack));
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
                    if (count > 0 && Util.CheckRoll(ProcChance.Value * info.procCoefficient, attacker_body.master))
                    {
                        float damage = attacker_body.damage * (DamageBase.Value / 100f + DamagePerStack.Value / 100f * (count - 1));
                        var burn = default(InflictDotInfo);
                        burn.attackerObject = attacker;
                        burn.victimObject = victim;
                        burn.dotIndex = DotController.DotIndex.Burn;
                        burn.totalDamage = damage;
                        burn.damageMultiplier = 1f;
                        StrengthenBurnUtils.CheckDotForUpgrade(attacker_body.inventory, ref burn);
                        DotController.InflictDot(ref burn);
                    }
                }
            }

            orig(self, info, victim);
        }
    }
}