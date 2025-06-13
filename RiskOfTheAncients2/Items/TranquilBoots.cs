using BepInEx.Configuration;
using R2API;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
using ROTA2.Buffs;
using UnityEngine;

namespace ROTA2.Items
{
    public class TranquilBoots : ItemBase<TranquilBoots>
    {
        public override string ItemName => "Tranquil Boots";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "TRANQUIL_BOOTS";
        public override string ItemTokenPickup => "Increase base movement speed and rapidly heal outside of danger.";
        public override string ItemTokenDesc => $"Increases {Utility("base movement speed")} by {Utility($"{MovementSpeedBase.Value}")} {Stack($"(+{MovementSpeedPerStack.Value} per stack)")}. Outside of danger, increases {Healing("base health regeneration")} by {Healing($"+{OODHealthRegenerationBase.Value} hp/s")} {Stack($"(+{OODHealthRegenerationPerStack.Value} hp/s per stack)")} and {Utility("base movement speed")} by a further {Utility($"{OODMovementSpeedBase.Value}")} {Stack($"(+{OODMovementSpeedPerStack.Value} per stack)")}.";
        public override string ItemTokenLore => "While they increase the longevity of the wearer, this boot is not particularly reliable.";
        public override string ItemDefGUID => Assets.TranquilBoots.ItemDef;
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

        public ConfigEntry<float> MovementSpeedBase;
        public ConfigEntry<float> MovementSpeedPerStack;
        public ConfigEntry<float> OODHealthRegenerationBase;
        public ConfigEntry<float> OODHealthRegenerationPerStack;
        public ConfigEntry<float> OODMovementSpeedBase;
        public ConfigEntry<float> OODMovementSpeedPerStack;
        public ConfigEntry<float> OODDelay;
        private void CreateConfig(ConfigFile configuration)
        {
            MovementSpeedBase = configuration.Bind("Item: " + ItemName, "Passive Base Movement Speed Base", 0.5f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(MovementSpeedBase));
            MovementSpeedPerStack = configuration.Bind("Item: " + ItemName, "Passive Base Movement Speed Per Stack", 0.5f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(MovementSpeedPerStack));
            OODHealthRegenerationBase = configuration.Bind("Item: " + ItemName, "Out Of Danger Health Regeneration Base", 4.5f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(OODHealthRegenerationBase));
            OODHealthRegenerationPerStack = configuration.Bind("Item: " + ItemName, "Out Of Danger Health Regeneration Per Stack", 4.5f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(OODHealthRegenerationPerStack));
            OODMovementSpeedBase = configuration.Bind("Item: " + ItemName, "Out Of Danger Base Movement Speed Base", 0.5f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(OODMovementSpeedBase));
            OODMovementSpeedPerStack = configuration.Bind("Item: " + ItemName, "Out Of Danger Base Movement Speed Per Stack", 0.5f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(OODMovementSpeedPerStack));
            OODDelay = configuration.Bind("Item: " + ItemName, "Out Of Danger Delay", 5.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(OODDelay));
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
                args.baseMoveSpeedAdd += MovementSpeedBase.Value + MovementSpeedPerStack.Value * (count - 1);
            }
        }

        public class TranquilBootsBehavior : MonoBehaviour
        {
            CharacterBody body;
            bool cleaned = false;
            public bool last_out_of_danger = false;

            void Awake()
            {
                body = GetComponent<CharacterBody>();
            }
            void Start()
            {
                if (GetCount(body) <= 0)
                {
                    return;
                }

                bool out_of_danger = body.healthComponent.timeSinceLastHit >= Instance.OODDelay.Value;
                if (out_of_danger)
                {
                    body.AddBuff(TranqbuilBootsOn.GetBuffDef());
                    Util.PlaySound("Play_item_proc_slug_emerge", body.gameObject);
                }
                else
                {
                    body.AddBuff(TranquilBootsOff.GetBuffDef());
                    Util.PlaySound("Play_item_proc_slug_hide", body.gameObject);
                }
                last_out_of_danger = out_of_danger;
            }
            void FixedUpdate()
            {
                if (GetCount(body) <= 0)
                {
                    if (!cleaned)
                    {
                        body.RemoveBuff(TranqbuilBootsOn.GetBuffDef());
                        body.RemoveBuff(TranquilBootsOff.GetBuffDef());
                        cleaned = true;
                    }
                    return;
                }

                bool out_of_danger = body.healthComponent.timeSinceLastHit >= Instance.OODDelay.Value;
                if (out_of_danger && !last_out_of_danger)
                {
                    body.AddBuff(TranqbuilBootsOn.GetBuffDef());
                    body.RemoveBuff(TranquilBootsOff.GetBuffDef());
                    Util.PlaySound("Play_item_proc_slug_emerge", body.gameObject);
                }
                else if (!out_of_danger && last_out_of_danger)
                {
                    body.AddBuff(TranquilBootsOff.GetBuffDef());
                    body.RemoveBuff(TranqbuilBootsOn.GetBuffDef());
                    Util.PlaySound("Play_item_proc_slug_hide", body.gameObject);
                }
                last_out_of_danger = out_of_danger;
            }
        }
    }
}