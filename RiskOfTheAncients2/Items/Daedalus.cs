using BepInEx.Configuration;
using R2API;
using RoR2;
using System;
using UnityEngine;

namespace ROTA2.Items
{
    public class Daedalus : ItemBase<Daedalus>
    {
        public override string ItemName => "Daedalus";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "DAEDALUS";
        public override string ItemTokenPickup => $"Your 'Critical Strikes' deal additional damage.";
        public override string ItemTokenDesc => $"Gain {Damage($"{CriticalChance}% critical chance")}. {Damage("Critical Strikes")} deal an additional {Damage($"{CriticalDamageBase}%")} {Stack($"(+{CriticalDamagePerStack}% per stack)")} {Damage("damage")}.";
        public override string ItemTokenLore => "A weapon of incredible power that is difficult for even the strongest of warriors to control.";
        public override string ItemIconPath => "ROTA2.Icons.daedalus.png";
        public override ItemTier Tier => ItemTier.Tier2;
        public override ItemTag[] ItemTags => [ItemTag.Damage];
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddCritChance;
            RecalculateStatsAPI.GetStatCoefficients += AddCritDamage;
            On.RoR2.HealthComponent.TakeDamage += OnTakeDamage;
        }


        public override void Init(ConfigFile configuration)
        {
            CreateConfig(configuration);
            CreateLanguageTokens();
            CreateItemDef();
            Hooks();
        }

        public float CriticalChance;
        public float CriticalDamageBase;
        public float CriticalDamagePerStack;
        private void CreateConfig(ConfigFile configuration)
        {
            CriticalChance         = configuration.Bind("Item: " + ItemName, "Critical Chance",           15.0f, "").Value;
            CriticalDamageBase     = configuration.Bind("Item: " + ItemName, "Critical Damage Base",      20.0f, "").Value;
            CriticalDamagePerStack = configuration.Bind("Item: " + ItemName, "Critical Damage Per Stack", 20.0f, "").Value;
        }

        private void AddCritChance(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            int count = GetCount(body);
            if (count > 0)
            {
                args.critAdd += CriticalChance;
            }
        }
        private void AddCritDamage(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            int count = GetCount(body);
            if (count > 0)
            {
                args.critDamageMultAdd += CriticalDamageBase / 100.0f + CriticalDamagePerStack / 100.0f * (count - 1);
            }
        }
        private void OnTakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo info)
        {
            orig(self, info);

            if (!info.rejected && info.damage > 0.0f && info.crit && info.attacker && GetCount(info.attacker.GetComponent<CharacterBody>()) > 0)
            {
                Util.PlaySound("Daedalus", self.body.gameObject);
            }
        }
    }
}