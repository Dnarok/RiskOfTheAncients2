using BepInEx.Configuration;
using RoR2;
using ROTA2.Buffs;
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
        public override ItemTag[] ItemTags => [ItemTag.Utility, ItemTag.Damage];
        public override string ItemIconPath => "ROTA2.Icons.shadow_amulet.png";
        public override void Hooks()
        {
            CharacterBody.onBodyInventoryChangedGlobal += OnInventoryChanged;
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
        public float LingerDuration;
        private void CreateConfig(ConfigFile configuration)
        {
            StandingStillDuration   = configuration.Bind("Item: " + ItemName, "Standing Still Duration", 1.5f, "").Value;
            AttackSpeedBase         = configuration.Bind("Item: " + ItemName, "Attack Speed Base", 30.0f, "").Value;
            AttackSpeedPerStack     = configuration.Bind("Item: " + ItemName, "Attack Speed Per Stack", 30.0f, "").Value;
            LingerDuration          = configuration.Bind("Item: " + ItemName, "Linger Duration", 1.0f, "").Value;
        }

        private void OnInventoryChanged(CharacterBody body)
        {
            if (GetCount(body) > 0 && !body.GetComponent<ShadowAmuletBehavior>())
            {
                body.gameObject.AddComponent<ShadowAmuletBehavior>();
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
            void FixedUpdate()
            {
                int count = Instance.GetCount(body);
                if (count == 0)
                {
                    if (invisible)
                    {
                        body.RemoveBuff(RoR2Content.Buffs.Cloak);
                        body.SetBuffCount(ShadowAmuletBuff.Instance.BuffDef.buffIndex, 0);
                        body.MarkAllStatsDirty();
                        invisible = false;
                        last_invisible = false;
                    }
                    return;
                }

                if (body.notMovingStopwatch >= ShadowAmulet.Instance.StandingStillDuration)
                {
                    invisible = true;
                }
                else
                {
                    invisible = false;
                }

                if (invisible)
                {
                    body.AddTimedBuff(RoR2Content.Buffs.Cloak, Instance.LingerDuration, 1);
                    if (!last_invisible)
                    {
                        ShadowAmuletBuff.Instance.ApplyTo(new BuffBase.ApplyParameters
                        {
                            victim = body,
                            stacks = count,
                            max_stacks = count
                        });
                        Util.PlaySound("ShadowAmulet", body.gameObject);
                        body.MarkAllStatsDirty();
                    }
                }
                else if (!invisible && last_invisible)
                {
                    body.SetBuffCount(ShadowAmuletBuff.Instance.BuffDef.buffIndex, 0);
                    body.MarkAllStatsDirty();
                }

                last_invisible = invisible;
            }
        }
    }
}
