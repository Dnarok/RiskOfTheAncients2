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
    public class HealingSalve : ItemBase<HealingSalve>
    {
        public override string ItemName => "Healing Salve";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "HEALING_SALVE";
        public override string ItemTokenPickup => "Receive interruptible healing after using your Special skill.";
        public override string ItemTokenDesc => $"Activating your {Utility("Special skill")} {Healing("heals")} you for {Healing($"{MaximumHealthRegenerationBase.Value}%")} {Stack($"(+{MaximumHealthRegenerationPerStack.Value}% per stack)")} {Healing("of your maximum health")} per second for {Utility($"{BuffDuration.Value} seconds")}. Healing is {Health("interrupted by taking damage.")}";
        public override string ItemTokenLore => "A magical salve that can quickly mend even the deepest of wounds.";
        public override string ItemDefGUID => Assets.HealingSalve.ItemDef;
        public override void Hooks()
        {
            On.RoR2.CharacterBody.OnSkillActivated += OnSkill;
        }
        public static GameObject effectPrefab;
        public override void Init(ConfigFile configuration)
        {
            CreateConfig(configuration);
            CreateSounds();
            CreateLanguageTokens();
            CreateItemDef();
            Hooks();

            effectPrefab = Addressables.LoadAssetAsync<GameObject>(Assets.HealingSalve.Effect).WaitForCompletion();
            var component = effectPrefab.AddComponent<NetworkedBodyAttachment>();
            component.shouldParentToAttachedBody = true;
            component.forceHostAuthority = false;
        }

        public ConfigEntry<float> MaximumHealthRegenerationBase;
        public ConfigEntry<float> MaximumHealthRegenerationPerStack;
        public ConfigEntry<float> BuffDuration;
        public ConfigEntry<bool> PlaySound;
        public void CreateConfig(ConfigFile configuration)
        {
            MaximumHealthRegenerationBase = configuration.Bind("Item: " + ItemName, "Maximum Health Regeneration Base", 2.0f, "How much maximum health percentage regeneration should the first stack provide?");
            ModSettingsManager.AddOption(new FloatFieldOption(MaximumHealthRegenerationBase));
            MaximumHealthRegenerationPerStack = configuration.Bind("Item: " + ItemName, "Maximum Health Regeneration Per Stack", 2.0f, "How much maximum health percentage regeneration should subsequent stacks provide?");
            ModSettingsManager.AddOption(new FloatFieldOption(MaximumHealthRegenerationPerStack));
            BuffDuration = configuration.Bind("Item: " + ItemName, "Healing Duration", 5.0f, "How long should the regeneration last?");
            ModSettingsManager.AddOption(new FloatFieldOption(BuffDuration));
            PlaySound = configuration.Bind("Item: " + ItemName, "Play Sound", true, "");
            ModSettingsManager.AddOption(new CheckBoxOption(PlaySound));
        }

        NetworkSoundEventDef sound = null;
        protected void CreateSounds()
        {
            Addressables.LoadAssetAsync<NetworkSoundEventDef>(Assets.HealingSalve.NetworkSoundEventDef).Completed += (x) => { ContentAddition.AddNetworkSoundEventDef(x.Result); sound = x.Result; };
        }

        private void OnSkill(On.RoR2.CharacterBody.orig_OnSkillActivated orig, CharacterBody body, GenericSkill skill)
        {
            int count = GetCount(body);
            if (count > 0 && skill == body.skillLocator.special)
            {
                HealingSalveBuff.ApplyTo(
                    body: body,
                    duration: BuffDuration.Value
                );

                if (PlaySound.Value)
                {
                    EffectManager.SimpleSoundEffect(sound.index, body.corePosition, true);
                }
            }

            orig(body, skill);
        }
    }
}