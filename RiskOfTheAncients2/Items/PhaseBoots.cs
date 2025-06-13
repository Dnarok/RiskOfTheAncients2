using BepInEx.Configuration;
using R2API;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
using ROTA2.Buffs;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace ROTA2.Items
{
    public class PhaseBoots : ItemBase<PhaseBoots>
    {
        public override string ItemName => "Phase Boots";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "PHASE_BOOTS";
        public override string ItemTokenPickup => "Increases damage and sprint speed, and allows you to phase through others while sprinting.";
        public override string ItemTokenDesc => $"Increases {Damage("damage")} by {Damage($"{DamageBase.Value}%")} {Stack($"(+{DamagePerStack.Value}% per stack)")}, and {Utility("sprint speed")} by {Utility($"{SprintSpeedBase.Value}%")} {Stack($"(+{SprintSpeedPerStack.Value}% per stack)")}. While sprinting, {Utility("phase through")} enemies and allies.";
        public override string ItemTokenLore => "Boots that allow the wearer to travel between the ether.";
        public override string ItemDefGUID => Assets.PhaseBoots.ItemDef;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddDamage;
            RecalculateStatsAPI.GetStatCoefficients += AddSprintSpeed;
            CharacterBody.onBodyInventoryChangedGlobal += OnInventoryChanged;
        }

        public override void Init(ConfigFile configuration)
        {
            CreateConfig(configuration);
            CreateSounds();
            CreateLanguageTokens();
            CreateItemDef();
            Hooks();
        }

        public ConfigEntry<float> DamageBase;
        public ConfigEntry<float> DamagePerStack;
        public ConfigEntry<float> SprintSpeedBase;
        public ConfigEntry<float> SprintSpeedPerStack;
        public ConfigEntry<bool> PlaySound;
        private void CreateConfig(ConfigFile configuration)
        {
            DamageBase = configuration.Bind("Item: " + ItemName, "Damage Base", 15.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(DamageBase));
            DamagePerStack = configuration.Bind("Item: " + ItemName, "Damage Per Stack", 15.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(DamagePerStack));
            SprintSpeedBase = configuration.Bind("Item: " + ItemName, "Sprint Speed Base", 35.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(SprintSpeedBase));
            SprintSpeedPerStack = configuration.Bind("Item: " + ItemName, "Sprint Speed Per Stack", 35.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(SprintSpeedPerStack));
            PlaySound = configuration.Bind("Item: " + ItemName, "Play Sound", true, "");
            ModSettingsManager.AddOption(new CheckBoxOption(PlaySound));
        }

        NetworkSoundEventDef sound = null;
        protected void CreateSounds()
        {
            Addressables.LoadAssetAsync<NetworkSoundEventDef>(Assets.PhaseBoots.NetworkSoundEventDef).Completed += (x) => { ContentAddition.AddNetworkSoundEventDef(x.Result); sound = x.Result; };
        }

        private void AddDamage(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            int count = GetCount(body);
            if (count > 0)
            {
                args.damageMultAdd += DamageBase.Value / 100.0f + DamagePerStack.Value / 100.0f * (count - 1);
            }
        }
        private void AddSprintSpeed(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            int count = GetCount(body);
            if (count > 0 && body.isSprinting)
            {
                args.moveSpeedMultAdd += (SprintSpeedBase.Value / 100.0f + SprintSpeedPerStack.Value / 100.0f * (count - 1)) / body.sprintingSpeedMultiplier;
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
            NetworkSoundEventDef sound = null;

            void Awake()
            {
                body = GetComponent<CharacterBody>();
                was_sprinting = false;
                is_sprinting = body.isSprinting;
                sound = Addressables.LoadAssetAsync<NetworkSoundEventDef>(Assets.PhaseBoots.NetworkSoundEventDef).WaitForCompletion();
            }
            void FixedUpdate()
            {
                if (!NetworkServer.active)
                {
                    return;
                }

                if (GetCount(body) <= 0)
                {
                    if (body.HasBuff(PhaseBootsBuff.GetBuffDef()))
                    {
                        body.RemoveBuff(PhaseBootsBuff.GetBuffDef());
                    }

                    return;
                }

                is_sprinting = body.isSprinting;
                if (!was_sprinting && is_sprinting)
                {
                    body.AddBuff(PhaseBootsBuff.GetBuffDef());
                    if (Instance.PlaySound.Value)
                    {
                        EffectManager.SimpleSoundEffect(Instance.sound.index, body.corePosition, true);
                    }
                }
                else if (was_sprinting && !is_sprinting)
                {
                    body.RemoveBuff(PhaseBootsBuff.GetBuffDef());
                }
                was_sprinting = is_sprinting;
            }
        }
    }
}