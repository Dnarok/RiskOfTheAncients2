using BepInEx.Configuration;
using R2API;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ROTA2.Items
{
    public class InfusedRaindrops : ItemBase<InfusedRaindrops>
    {
        public override string ItemName => "Infused Raindrops";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "INFUSED_RAINDROPS";
        public override string ItemTokenPickup => "Receive flat damage reduction from large attacks.";
        public override string ItemTokenDesc => $"Reduce all {Damage("incoming damage")} above {Damage($"{IncomingDamageMinimum.Value}")} by {Damage($"{DamageBlockBase.Value}")} {Stack($"(+{DamageBlockPerStack.Value} per stack)")}. Cannot be reduced below {Damage($"{DamageMinimum.Value}")}.";
        public override string ItemTokenLore => "Elemental protection from magical assaults.";
        public override string ItemDefGUID => Assets.InfusedRaindrops.ItemDef;
        public override ItemDef VoidFor => RoR2Content.Items.ArmorPlate;
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

        public ConfigEntry<float> IncomingDamageMinimum;
        public ConfigEntry<float> DamageBlockBase;
        public ConfigEntry<float> DamageBlockPerStack;
        public ConfigEntry<float> DamageMinimum;
        public ConfigEntry<bool> PlaySound;
        private void CreateConfig(ConfigFile configuration)
        {
            IncomingDamageMinimum = configuration.Bind("Item: " + ItemName, "Incoming Damage Minimum", 60.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(IncomingDamageMinimum));
            DamageBlockBase = configuration.Bind("Item: " + ItemName, "Flat Damage Block Base", 15.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(DamageBlockBase));
            DamageBlockPerStack = configuration.Bind("Item: " + ItemName, "Flat Damage Block Per Stack", 15.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(DamageBlockPerStack));
            DamageMinimum = configuration.Bind("Item: " + ItemName, "Damage Minimum After Block", 1.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(DamageMinimum));
            PlaySound = configuration.Bind("Item: " + ItemName, "Play Sound", true, "");
            ModSettingsManager.AddOption(new CheckBoxOption(PlaySound));
        }

        NetworkSoundEventDef sound = null;
        protected void CreateSounds()
        {
            Addressables.LoadAssetAsync<NetworkSoundEventDef>(Assets.InfusedRaindrops.NetworkSoundEventDef).Completed += (x) => { ContentAddition.AddNetworkSoundEventDef(x.Result); sound = x.Result; };
        }

        private void OnInventoryChanged(CharacterBody body)
        {
            int count = GetCount(body);
            var behavior = body.GetComponent<RaindropsBehavior>();
            if (count > 0 && !behavior)
            {
                body.gameObject.AddComponent<RaindropsBehavior>();
            }
            else if (count == 0 && behavior)
            {
                UnityEngine.Object.Destroy(behavior);
            }
        }

        public class RaindropsBehavior : MonoBehaviour, IOnIncomingDamageServerReceiver
        {
            CharacterBody body;

            void Awake()
            {
                body = GetComponent<CharacterBody>();
                if (body && body.healthComponent)
                {
                    HG.ArrayUtils.ArrayAppend(ref body.healthComponent.onIncomingDamageReceivers, this);
                }
            }
            void OnDestroy()
            {
                if (body && body.healthComponent)
                {
                    int i = Array.IndexOf(body.healthComponent.onIncomingDamageReceivers, this);
                    if (i != -1)
                    {
                        HG.ArrayUtils.ArrayRemoveAtAndResize(ref body.healthComponent.onIncomingDamageReceivers, body.healthComponent.onIncomingDamageReceivers.Length, i);
                    }
                }
            }
            public void OnIncomingDamageServer(DamageInfo info)
            {
                int count = GetCount(body);
                if (info.rejected || info.damageType.damageType.HasFlag(DamageType.BypassArmor) || info.damage < Instance.IncomingDamageMinimum.Value || count <= 0)
                {
                    return;
                }

                info.damage = Mathf.Max(Instance.DamageMinimum.Value, info.damage - (Instance.DamageBlockBase.Value + Instance.DamageBlockPerStack.Value * (count - 1)));
                if (Instance.PlaySound.Value)
                {
                    EffectManager.SimpleSoundEffect(Instance.sound.index, info.position, true);
                }
            }
        }
    }
}