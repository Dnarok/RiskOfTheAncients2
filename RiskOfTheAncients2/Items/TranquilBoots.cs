using BepInEx.Configuration;
using R2API;
using RoR2;
using ROTA2.Buffs;
using System;
using UnityEngine;

namespace ROTA2.Items
{
    public class TranquilBoots : ItemBase<TranquilBoots>
    {
        public override string ItemName => "Tranquil Boots";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "TRANQUIL_BOOTS";
        public override string ItemTokenPickup => "Increase base movement speed and rapidly heal outside of danger.";
        public override string ItemTokenDesc => $"Increases {Utility("base movement speed")} by {Utility($"{MovementSpeedBase}")} {Stack($"(+{MovementSpeedPerStack} per stack)")}. Outside of danger, increases {Healing("base health regeneration")} by {Healing($"+{OODHealthRegenerationBase} hp/s")} {Stack($"(+{OODHealthRegenerationPerStack} hp/s per stack)")} and {Utility("base movement speed")} by a further {Utility($"{OODMovementSpeedBase}")} {Stack($"(+{OODMovementSpeedPerStack} per stack)")}.";
        public override string ItemTokenLore => "While they increase the longevity of the wearer, this boot is not particularly reliable.";
        public override ItemTier Tier => ItemTier.Tier2;
        public override ItemTag[] ItemTags => [ItemTag.WorldUnique];
        public override string ItemIconPath => "ROTA2.Icons.tranquil_boots.png";
        public override void Hooks()
        {
            CharacterBody.onBodyInventoryChangedGlobal += OnInventoryChanged;
            RecalculateStatsAPI.GetStatCoefficients += AddMovementSpeed;
        }

        public override void Init(ConfigFile configuration)
        {
            CreateConfig(configuration);
            CreateLanguageTokens();
            CreateItemDef();
            Hooks();
        }

        public float MovementSpeedBase;
        public float MovementSpeedPerStack;
        public float OODHealthRegenerationBase;
        public float OODHealthRegenerationPerStack;
        public float OODMovementSpeedBase;
        public float OODMovementSpeedPerStack;
        public float OODDelay;
        private void CreateConfig(ConfigFile configuration)
        {
            MovementSpeedBase               = configuration.Bind("Item: " + ItemName, "Passive Base Movement Speed Base", 0.5f, "").Value;
            MovementSpeedPerStack           = configuration.Bind("Item: " + ItemName, "Passive Base Movement Speed Per Stack", 0.5f, "").Value;
            OODHealthRegenerationBase       = configuration.Bind("Item: " + ItemName, "Out Of Danger Health Regeneration Base", 4.5f, "").Value;
            OODHealthRegenerationPerStack   = configuration.Bind("Item: " + ItemName, "Out Of Danger Health Regeneration Per Stack", 4.5f, "").Value;
            OODMovementSpeedBase            = configuration.Bind("Item: " + ItemName, "Out Of Danger Base Movement Speed Base", 0.5f, "").Value;
            OODMovementSpeedPerStack        = configuration.Bind("Item: " + ItemName, "Out Of Danger Base Movement Speed Per Stack", 0.5f, "").Value;
            OODDelay                        = configuration.Bind("Item: " + ItemName, "Out Of Danger Delay", 5.0f, "").Value;
        }

        private void OnInventoryChanged(CharacterBody body)
        {
            if (GetCount(body) > 0 && !body.GetComponent<TranquilBootsBehavior>())
            {
                body.gameObject.AddComponent<TranquilBootsBehavior>();
            }
        }
        private void AddMovementSpeed(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            int count = GetCount(body);
            if (count > 0)
            {
                args.baseMoveSpeedAdd += MovementSpeedBase + MovementSpeedPerStack * (count - 1);
            }
        }

        public class TranquilBootsBehavior : MonoBehaviour
        {
            CharacterBody body;
            public bool last_out_of_danger;
            public bool out_of_danger;

            void Awake()
            {
                body = GetComponent<CharacterBody>();
                last_out_of_danger = false;
                out_of_danger = false;
            }
            void FixedUpdate()
            {
                int count = Instance.GetCount(body);
                if (count == 0)
                {
                    if (out_of_danger)
                    {
                        body.SetBuffCount(TranquilBootsBuff.Instance.BuffDef.buffIndex, 0);
                        body.MarkAllStatsDirty();
                        out_of_danger = false;
                        last_out_of_danger = false;
                    }
                    return;
                }

                if (body.healthComponent.timeSinceLastHit >= TranquilBoots.Instance.OODDelay)
                {
                    out_of_danger = true;
                }
                else
                {
                    out_of_danger = false;
                }

                if (out_of_danger && !last_out_of_danger)
                {
                    TranquilBootsBuff.Instance.ApplyTo(new BuffBase.ApplyParameters
                    {
                        victim = body,
                        stacks = count,
                        max_stacks = count
                    });
                    Util.PlaySound("Play_item_proc_slug_emerge", body.gameObject);
                    body.MarkAllStatsDirty();
                }
                else if (!out_of_danger && last_out_of_danger)
                {
                    body.SetBuffCount(TranquilBootsBuff.Instance.BuffDef.buffIndex, 0);
                    Util.PlaySound("Play_item_proc_slug_hide", body.gameObject);
                    body.MarkAllStatsDirty();
                }

                last_out_of_danger = out_of_danger;
            }
        }
    }
}
