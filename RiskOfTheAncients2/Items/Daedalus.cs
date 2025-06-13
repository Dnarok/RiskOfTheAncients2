using BepInEx.Configuration;
using R2API;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
using UnityEngine.AddressableAssets;

namespace ROTA2.Items
{
    public class Daedalus : ItemBase<Daedalus>
    {
        public override string ItemName => "Daedalus";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "DAEDALUS";
        public override string ItemTokenPickup => $"Your 'Critical Strikes' deal additional damage.";
        public override string ItemTokenDesc => $"Gain {Damage($"{CriticalChance.Value}% critical chance")}. {Damage("Critical Strikes")} deal an additional {Damage($"{CriticalDamageBase.Value}%")} {Stack($"(+{CriticalDamagePerStack.Value}% per stack)")} {Damage("damage")}.";
        public override string ItemTokenLore => "A weapon of incredible power that is difficult for even the strongest of warriors to control.";
        public override string ItemDefGUID => Assets.Daedalus.ItemDef;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddCritChance;
            RecalculateStatsAPI.GetStatCoefficients += AddCritDamage;
            On.RoR2.HealthComponent.TakeDamage += OnTakeDamage;
        }


        public override void Init(ConfigFile configuration)
        {
            CreateConfig(configuration);
            CreateSounds();
            CreateLanguageTokens();
            CreateItemDef();
            Hooks();
        }

        public ConfigEntry<float> CriticalChance;
        public ConfigEntry<float> CriticalDamageBase;
        public ConfigEntry<float> CriticalDamagePerStack;
        public ConfigEntry<bool> PlaySound;
        private void CreateConfig(ConfigFile configuration)
        {
            CriticalChance = configuration.Bind("Item: " + ItemName, "Critical Chance", 15.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(CriticalChance));
            CriticalDamageBase = configuration.Bind("Item: " + ItemName, "Critical Damage Base", 20.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(CriticalDamageBase));
            CriticalDamagePerStack = configuration.Bind("Item: " + ItemName, "Critical Damage Per Stack", 20.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(CriticalDamagePerStack));
            PlaySound = configuration.Bind("Item: " + ItemName, "Play Sound", true, "");
            ModSettingsManager.AddOption(new CheckBoxOption(PlaySound));
        }

        NetworkSoundEventDef sound = null;
        protected void CreateSounds()
        {
            Addressables.LoadAssetAsync<NetworkSoundEventDef>(Assets.Daedalus.NetworkSoundEventDef).Completed += (x) => { ContentAddition.AddNetworkSoundEventDef(x.Result); sound = x.Result; };
        }

        private void AddCritChance(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            int count = GetCount(body);
            if (count > 0)
            {
                args.critAdd += CriticalChance.Value;
            }
        }
        private void AddCritDamage(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            int count = GetCount(body);
            if (count > 0)
            {
                args.critDamageMultAdd += CriticalDamageBase.Value / 100.0f + CriticalDamagePerStack.Value / 100.0f * (count - 1);
            }
        }
        private void OnTakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo info)
        {
            orig(self, info);

            if (PlaySound.Value && !info.rejected && info.damage > 0.0f && info.crit && info.attacker && GetCount(info.attacker.GetComponent<CharacterBody>()) > 0)
            {
                EffectManager.SimpleSoundEffect(sound.index, self.body.corePosition, true);
            }
        }
    }
}