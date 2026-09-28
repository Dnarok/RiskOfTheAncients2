using BepInEx.Configuration;
using R2API;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
using ROTA2.Buffs;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ROTA2.Items
{
    public class ShadowAmulet : ItemBase<ShadowAmulet>
    {
        public override string ItemName => "Shadow Amulet";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "SHADOW_AMULET";
        public override string ItemTokenPickup => $"Increased attack speed while invisible, and becoming invisible after standing still.";
        public override string ItemTokenDesc => $"Increase {Damage("attack speed")} while {Utility("invisible")} by {Damage($"{AttackSpeedBase.Value}")} {Stack($"(+{AttackSpeedPerStack.Value}% per stack)")}. After standing still for {Utility($"{StandingStillDuration.Value}")} seconds, become {Utility("invisible")}, which {Utility("lingers")} for {Utility($"{LingerDurationBase.Value}")} {Stack($"(+{LingerDurationPerStack.Value} per stack)")} seconds after moving.";
        public override string ItemTokenLore => "A small talisman that clouds the senses of one's enemies when held perfectly still.";
        public override string ItemDefGUID => Assets.ShadowAmulet.ItemDef;
        public override void Hooks()
        {
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

        public ConfigEntry<float> StandingStillDuration;
        public ConfigEntry<float> AttackSpeedBase;
        public ConfigEntry<float> AttackSpeedPerStack;
        public ConfigEntry<float> LingerDurationBase;
        public ConfigEntry<float> LingerDurationPerStack;
        public ConfigEntry<bool> PlaySound;
        private void CreateConfig(ConfigFile configuration)
        {
            StandingStillDuration = configuration.Bind("Item: " + ItemName, "Standing Still Duration", 1.25f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(StandingStillDuration));
            AttackSpeedBase = configuration.Bind("Item: " + ItemName, "Attack Speed Base", 25f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(AttackSpeedBase));
            AttackSpeedPerStack = configuration.Bind("Item: " + ItemName, "Attack Speed Per Stack", 25f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(AttackSpeedPerStack));
            LingerDurationBase = configuration.Bind("Item: " + ItemName, "Linger Duration Base", 2f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(LingerDurationBase));
            LingerDurationPerStack = configuration.Bind("Item: " + ItemName, "Linger Duration Per Stack", 0.5f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(LingerDurationPerStack));
            PlaySound = configuration.Bind("Item: " + ItemName, "Play Sound", true, "");
            ModSettingsManager.AddOption(new CheckBoxOption(PlaySound));
        }

        NetworkSoundEventDef sound = null;
        protected void CreateSounds()
        {
            Addressables.LoadAssetAsync<NetworkSoundEventDef>(Assets.ShadowAmulet.NetworkSoundEventDef).Completed += (x) => { ContentAddition.AddNetworkSoundEventDef(x.Result); sound = x.Result; };
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
            public bool last_immobile = false;

            void Awake()
            {
                body = GetComponent<CharacterBody>();
            }
            void FixedUpdate()
            {
                int count = GetCount(body);
                if (count <= 0)
                {
                    return;
                }

                bool immobile = body.notMovingStopwatch >= Instance.StandingStillDuration.Value;
                if (immobile)
                {
                    float duration = Instance.LingerDurationBase.Value + Instance.LingerDurationPerStack.Value * (count - 1);
                    body.AddTimedBuff(RoR2Content.Buffs.Cloak, duration, 1);
                    body.AddTimedBuff(RoR2Content.Buffs.CloakSpeed, duration, 1);
                    if (!last_immobile)
                    {
                        if (Instance.PlaySound.Value)
                        {
                            EffectManager.SimpleSoundEffect(Instance.sound.index, body.corePosition, true);
                        }
                    }
                }
                last_immobile = immobile;

                bool has_fade = body.HasBuff(ShadowAmuletBuff.GetBuffDef());
                if (body.HasBuff(RoR2Content.Buffs.Cloak))
                {
                    if (!has_fade)
                    {
                        body.AddBuff(ShadowAmuletBuff.GetBuffDef());
                    }
                }
                else if (has_fade)
                {
                    body.RemoveBuff(ShadowAmuletBuff.GetBuffDef());
                }
            }
        }
    }
}