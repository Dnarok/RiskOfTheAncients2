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
        public override string ItemTokenPickup => $"Become invisible and gain attack speed after standing still for {StandingStillDuration.Value} seconds.";
        public override string ItemTokenDesc => $"After standing still for {Utility($"{StandingStillDuration.Value} seconds")}, become {Utility("invisible")} and increase {Damage("attack speed")} by {Damage($"{AttackSpeedBase.Value}%")} {Stack($"(+{AttackSpeedPerStack.Value}% per stack)")}.";
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
        public ConfigEntry<float> LingerDuration;
        public ConfigEntry<bool> PlaySound;
        private void CreateConfig(ConfigFile configuration)
        {
            StandingStillDuration = configuration.Bind("Item: " + ItemName, "Standing Still Duration", 1.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(StandingStillDuration));
            AttackSpeedBase = configuration.Bind("Item: " + ItemName, "Attack Speed Base", 30.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(AttackSpeedBase));
            AttackSpeedPerStack = configuration.Bind("Item: " + ItemName, "Attack Speed Per Stack", 30.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(AttackSpeedPerStack));
            LingerDuration = configuration.Bind("Item: " + ItemName, "Linger Duration", 1.5f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(LingerDuration));
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
            public bool last_invisible = false;

            void Awake()
            {
                body = GetComponent<CharacterBody>();
            }
            void FixedUpdate()
            {
                if (GetCount(body) <= 0)
                {
                    return;
                }

                bool invisible = body.notMovingStopwatch >= Instance.StandingStillDuration.Value;
                if (invisible)
                {
                    body.AddTimedBuff(RoR2Content.Buffs.Cloak, Instance.LingerDuration.Value, 1);
                    if (!last_invisible)
                    {
                        body.AddBuff(ShadowAmuletBuff.GetBuffDef());
                        if (Instance.PlaySound.Value)
                        {
                            EffectManager.SimpleSoundEffect(Instance.sound.index, body.corePosition, true);
                        }
                    }
                }
                else if (!invisible && last_invisible)
                {
                    body.RemoveBuff(ShadowAmuletBuff.GetBuffDef());
                }
                last_invisible = invisible;
            }
        }
    }
}
