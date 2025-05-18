using BepInEx.Configuration;
using R2API;
using RoR2;
using System;
using UnityEngine;

namespace ROTA2.Items
{
    public class ShadowAmulet : ItemBase<ShadowAmulet>
    {
        public override string ItemName => "Shadow Amulet";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "SHADOW_AMULET";
        public override string ItemTokenPickup => $"Become invisible and gain attack speed after standing still for {StandingStillDuration} seconds.";
        public override string ItemTokenDesc => $"After standing still for {Utility($"{StandingStillDuration} seconds")}, become {Utility("invisible")} and increase {Damage("attack speed")} by {Damage($"{AttackSpeedBase}%")} {Stack($"(+{AttackSpeedPerStack}% per stack)")}.";
        public override string ItemTokenLore => "A small talisman that clouds the senses of one's enemies when held perfectly still.";
        public override ItemTier Tier => ItemTier.Tier2;
        public override string ItemIconPath => "ROTA2.Icons.shadow_amulet.png";
        public override void Hooks()
        {
            CharacterBody.onBodyInventoryChangedGlobal += OnInventoryChanged;
            RecalculateStatsAPI.GetStatCoefficients += AddAttackSpeed;
        }

        public override void Init(ConfigFile configuration)
        {
            CreateConfig(configuration);
            CreateLanguageTokens();
            CreateItemDef();
            Hooks();
        }

        public float StandingStillDuration;
        public float AttackSpeedBase;
        public float AttackSpeedPerStack;
        private void CreateConfig(ConfigFile configuration)
        {
            StandingStillDuration   = configuration.Bind("Item: " + ItemName, "Standing Still Duration", 2.0f, "").Value;
            AttackSpeedBase         = configuration.Bind("Item: " + ItemName, "Attack Speed Base", 30.0f, "").Value;
            AttackSpeedPerStack     = configuration.Bind("Item: " + ItemName, "Attack Speed Per Stack", 30.0f, "").Value;
        }

        private void OnInventoryChanged(CharacterBody body)
        {
            if (GetCount(body) > 0 && !body.GetComponent<ShadowAmuletBehavior>())
            {
                body.gameObject.AddComponent<ShadowAmuletBehavior>();
            }
        }
        private void AddAttackSpeed(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            int count = GetCount(body);
            if (count > 0)
            {
                ShadowAmuletBehavior behavior = body.GetComponent<ShadowAmuletBehavior>();
                if (behavior && behavior.invisible)
                {
                    args.attackSpeedMultAdd += AttackSpeedBase / 100.0f + AttackSpeedPerStack / 100.0f * (count - 1);
                }
            }
        }

        public class ShadowAmuletBehavior : MonoBehaviour
        {
            CharacterBody body;
            public bool invisible;
            public bool last_invisible;

            void Awake()
            {
                body = GetComponent<CharacterBody>();
                invisible = false;
                last_invisible = false;
            }
            void Update()
            {
                if (body.notMovingStopwatch >= ShadowAmulet.Instance.StandingStillDuration)
                {
                    invisible = true;
                }
                else
                {
                    invisible = false;
                }

                if (invisible && !last_invisible)
                {
                    body.AddBuff(RoR2Content.Buffs.Cloak);
                    Util.PlaySound("ShadowAmulet", body.gameObject);
                    body.MarkAllStatsDirty();
                }
                else if (!invisible && last_invisible)
                {
                    body.RemoveBuff(RoR2Content.Buffs.Cloak);
                    body.MarkAllStatsDirty();
                }

                last_invisible = invisible;
            }
        }
    }
}
