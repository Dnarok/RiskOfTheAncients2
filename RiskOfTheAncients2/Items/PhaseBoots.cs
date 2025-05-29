using BepInEx.Configuration;
using R2API;
using RoR2;
using ROTA2.Buffs;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace ROTA2.Items
{
    public class PhaseBoots : ItemBase<PhaseBoots>
    {
        public override string ItemName => "Phase Boots";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "PHASE_BOOTS";
        public override string ItemTokenPickup => "Increases damage and sprint speed, and allows you to phase through others while sprinting.";
        public override string ItemTokenDesc => $"Increases {Damage("damage")} by {Damage($"{DamageBase}%")} {Stack($"(+{DamagePerStack}% per stack)")}, and {Utility("sprint speed")} by {Utility($"{SprintSpeedBase}%")} {Stack($"(+{SprintSpeedPerStack}% per stack)")}. While sprinting, {Utility("phase through")} enemies and allies.";
        public override string ItemTokenLore => "Boots that allow the wearer to travel between the ether.";
        public override ItemTier Tier => ItemTier.Tier2;
        public override ItemTag[] ItemTags => [ItemTag.WorldUnique];
        public override string ItemIconPath => "ROTA2.Icons.phase_boots.png";
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddDamage;
            RecalculateStatsAPI.GetStatCoefficients += AddSprintSpeed;
            CharacterBody.onBodyInventoryChangedGlobal += OnInventoryChanged;
        }

        public override void Init(ConfigFile configuration)
        {
            CreateConfig(configuration);
            CreateLanguageTokens();
            CreateItemDef();
            Hooks();
        }

        public float DamageBase;
        public float DamagePerStack;
        public float SprintSpeedBase;
        public float SprintSpeedPerStack;
        private void CreateConfig(ConfigFile configuration)
        {
            DamageBase          = configuration.Bind("Item: " + ItemName, "Damage Base",            15.0f, "").Value;
            DamagePerStack      = configuration.Bind("Item: " + ItemName, "Damage Per Stack",       15.0f, "").Value;
            SprintSpeedBase     = configuration.Bind("Item: " + ItemName, "Sprint Speed Base",      35.0f, "").Value;
            SprintSpeedPerStack = configuration.Bind("Item: " + ItemName, "Sprint Speed Per Stack", 35.0f, "").Value;
        }

        private void AddDamage(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            int count = GetCount(body);
            if (count > 0)
            {
                args.damageMultAdd += DamageBase / 100.0f + DamagePerStack / 100.0f * (count - 1);
            }
        }
        private void AddSprintSpeed(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            int count = GetCount(body);
            if (count > 0 && body.isSprinting)
            {
                args.moveSpeedMultAdd += (SprintSpeedBase / 100.0f + SprintSpeedPerStack / 100.0f * (count - 1)) / body.sprintingSpeedMultiplier;
            }
        }
        private void OnInventoryChanged(CharacterBody body)
        {
            if (GetCount(body) > 0 && !body.GetComponent<PhaseBootsBehavior>())
            {
                body.gameObject.AddComponent<PhaseBootsBehavior>();
            }
        }

        public class PhaseBootsBehavior : MonoBehaviour
        {
            CharacterBody body;
            bool was_sprinting;
            bool is_sprinting;

            void Awake()
            {
                body = GetComponent<CharacterBody>();
                was_sprinting = false;
                is_sprinting = body.isSprinting;
            }
            void FixedUpdate()
            {
                if (!NetworkServer.active)
                {
                    return;
                }
                
                if (Instance.GetCount(body) <= 0)
                {
                    if (body.HasBuff(PhaseBootsBuff.Instance.BuffDef))
                    {
                        body.RemoveBuff(PhaseBootsBuff.Instance.BuffDef);
                    }

                    return;
                }

                is_sprinting = body.isSprinting;
                if (!was_sprinting && is_sprinting)
                {
                    body.AddBuff(PhaseBootsBuff.Instance.BuffDef);
                    Util.PlaySound("PhaseBoots", body.gameObject);
                }
                else if (was_sprinting && !is_sprinting)
                {
                    body.RemoveBuff(PhaseBootsBuff.Instance.BuffDef);
                }
                was_sprinting = is_sprinting;
            }
        }
    }
}