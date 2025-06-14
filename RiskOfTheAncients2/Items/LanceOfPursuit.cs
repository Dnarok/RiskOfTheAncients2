﻿using BepInEx.Configuration;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
using UnityEngine;

namespace ROTA2.Items
{
    public class LanceOfPursuit : ItemBase<LanceOfPursuit>
    {
        public override string ItemName => "Lance of Pursuit";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "LANCE_OF_PURSUIT";
        public override string ItemTokenPickup => "Increase damage dealt when hitting an enemy from behind.";
        public override string ItemTokenDesc => $"Deal an additional {Damage($"{DamageBase.Value}%")} {Stack($"(+{DamagePerStack.Value}% per stack)")} damage when hitting an enemy from {Damage("behind")}.";
        public override string ItemTokenLore => "The gleaming weapon of a tarnished knight haunted by his duties to an unworthy king.";
        public override string ItemDefGUID => Assets.LanceOfPursuit.ItemDef;
        public override void Hooks()
        {
            On.RoR2.HealthComponent.TakeDamage += OnTakeDamage;
        }
        public override void Init(ConfigFile configuration)
        {
            CreateConfig(configuration);
            CreateLanguageTokens();
            CreateItemDef();
            Hooks();
        }

        public ConfigEntry<float> DamageBase;
        public ConfigEntry<float> DamagePerStack;
        public void CreateConfig(ConfigFile configuration)
        {
            DamageBase = configuration.Bind("Item: " + ItemName, "Damage Base", 20.0f, "How much damage should the first stack provide?");
            ModSettingsManager.AddOption(new FloatFieldOption(DamageBase));
            DamagePerStack = configuration.Bind("Item: " + ItemName, "Damage Per Stack", 20.0f, "How much damage should subsequent stacks provide?");
            ModSettingsManager.AddOption(new FloatFieldOption(DamagePerStack));
        }

        private void OnTakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo info)
        {
            if (info.rejected || info.procCoefficient <= 0)
            {
                orig(self, info);
                return;
            }

            GameObject attacker = info.attacker;
            if (attacker)
            {
                CharacterBody attacker_body = attacker.GetComponent<CharacterBody>();
                if (attacker_body)
                {
                    Vector3 vector = attacker_body.corePosition - info.position;
                    int count = GetCount(attacker_body);
                    if (count > 0 && BackstabManager.IsBackstab(-vector, self.body))
                    {
                        info.damage *= 1.0f + DamageBase.Value / 100.0f + DamagePerStack.Value / 100.0f * (count - 1);
                        info.damageColorIndex = DamageColorIndex.WeakPoint;

                        if ((bool)BackstabManager.backstabImpactEffectPrefab)
                        {
                            EffectManager.SimpleImpactEffect(BackstabManager.backstabImpactEffectPrefab, info.position, -info.force, transmit: true);
                        }
                    }
                }
            }

            orig(self, info);
        }
    }
}