using BepInEx.Configuration;
using R2API;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
using UnityEngine;

namespace ROTA2.Items
{
    public class BootsOfTravel : ItemBase<BootsOfTravel>
    {
        public override string ItemName => "Boots of Travel";
        public override string ItemTokenName => "BOOTS_OF_TRAVEL";
        public override string ItemTokenPickup => "Increases base movement speed. Damage dealt increases the faster you're moving.";
        public override string ItemTokenDesc => $"Increases {Utility("base movement speed")} by {Utility($"{MovementSpeedBonus.Value}%")}. {Damage("Outgoing damage")} is increased the faster you move, up to {Damage($"{DamageBonusBase.Value}%")} {Stack($"(+{DamageBonusPerStack.Value}% per stack)")}.";
        public override string ItemTokenLore => "Winged boots that grant omnipresence.";
        public override string ItemDefGUID => Assets.BootsOfTravel.ItemDef;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddMovementSpeed;
            On.RoR2.HealthComponent.TakeDamage += OnTakeDamage;
        }

        public override void Init(ConfigFile configuration)
        {
            CreateConfig(configuration);
            CreateLanguageTokens();
            CreateItemDef();
            Hooks();
        }

        public ConfigEntry<float> MovementSpeedBonus;
        public ConfigEntry<float> DamageBonusBase;
        public ConfigEntry<float> DamageBonusPerStack;
        private void CreateConfig(ConfigFile configuration)
        {
            MovementSpeedBonus = configuration.Bind("Item: " + ItemName, "Base Movement Speed Bonus", 50.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(MovementSpeedBonus));
            DamageBonusBase = configuration.Bind("Item: " + ItemName, "Speed Damage Bonus Base", 100.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(DamageBonusBase));
            DamageBonusPerStack = configuration.Bind("Item: " + ItemName, "Speed Damage Bonus Per Stack", 50.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(DamageBonusPerStack));
        }

        private void AddMovementSpeed(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (GetCount(body) > 0)
            {
                args.moveSpeedTotalMult *= 1.0f + MovementSpeedBonus.Value / 100.0f;
            }
        }
        private void OnTakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo info)
        {
            if (info.rejected)
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
                    int count = GetCount(attacker_body);
                    if (count > 0)
                    {
                        float multiplier = (1 - 1 / (attacker_body.moveSpeed / (attacker_body.baseMoveSpeed * (1.0f + MovementSpeedBonus.Value / 100.0f)))) * (DamageBonusBase.Value / 100.0f + DamageBonusPerStack.Value / 100.0f * (count - 1));
                        info.damage *= 1.0f + multiplier;
                    }
                }
            }

            orig(self, info);
        }
    }
}