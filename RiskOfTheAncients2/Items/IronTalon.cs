using BepInEx.Configuration;
using R2API;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
namespace ROTA2.Items
{
    public class IronTalon : ItemBase<IronTalon>
    {
        public override string ItemName => "Iron Talon";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "IRON_TALON";
        public override string ItemTokenPickup => "Deal large damage to Boss monsters on your first hit.";
        public override string ItemTokenDesc => $"{Damage("Damage")} boss monsters on {Damage("first hit")} for {Health($"{HealthDamageBase.Value}%")} {Stack($"(+{HealthDamagePerStack.Value} per stack)")} of their {Health("current health")}.";
        public override string ItemTokenLore => "A simple but effective weapon devised to quell a great Hellbear uprising.";
        public override string ItemDefGUID => Assets.IronTalon.ItemDef;
        public override void Hooks()
        {
            On.RoR2.GlobalEventManager.OnHitEnemy += OnHit;
        }
        public override void Init(ConfigFile configuration)
        {
            CreateConfig(configuration);
            CreateSounds();
            CreateLanguageTokens();
            CreateItemDef();
            Hooks();
        }

        public ConfigEntry<float> HealthDamageBase;
        public ConfigEntry<float> HealthDamagePerStack;
        public ConfigEntry<bool> PlaySound;
        private void CreateConfig(ConfigFile configuration)
        {
            HealthDamageBase = configuration.Bind("Item: " + ItemName, "Current Health Damage Base", 10.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(HealthDamageBase));
            HealthDamagePerStack = configuration.Bind("Item: " + ItemName, "Current Health Damage Per Stack", 10.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(HealthDamagePerStack));
            PlaySound = configuration.Bind("Item: " + ItemName, "Play Sound", true, "");
            ModSettingsManager.AddOption(new CheckBoxOption(PlaySound));
        }

        NetworkSoundEventDef sound = null;
        protected void CreateSounds()
        {
            Addressables.LoadAssetAsync<NetworkSoundEventDef>(Assets.IronTalon.NetworkSoundEventDef).Completed += (x) => { ContentAddition.AddNetworkSoundEventDef(x.Result); sound = x.Result; };
        }

        private void OnHit(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo info, GameObject victim)
        {
            if (!NetworkServer.active || info.rejected || info.procCoefficient <= 0)
            {
                orig(self, info, victim);
                return;
            }

            GameObject attacker = info.attacker;
            if (attacker && victim)
            {
                var attacker_body = attacker.GetComponent<CharacterBody>();
                var victim_body = victim.GetComponent<CharacterBody>();
                if (attacker_body && victim_body && victim_body.healthComponent && victim_body.isChampion)
                {
                    int count = GetCount(attacker_body);
                    if (count > 0 && !victim.GetComponent<IronTalonBehavior>())
                    {
                        DamageInfo damage = new()
                        {
                            damage = victim_body.healthComponent.combinedHealth * Util.ConvertAmplificationPercentageIntoReductionNormalized(HealthDamageBase.Value / 100.0f + HealthDamagePerStack.Value / 100.0f * (count - 1)),
                            crit = false,
                            position = info.position,
                            damageColorIndex = DamageColorIndex.WeakPoint,
                            procCoefficient = 0.0f,
                            damageType = new DamageTypeCombo
                            {
                                damageType = DamageType.BypassArmor
                            }
                        };
                        victim_body.healthComponent.TakeDamage(damage);

                        victim.AddComponent<IronTalonBehavior>();

                        if (PlaySound.Value)
                        {
                            EffectManager.SimpleSoundEffect(Instance.sound.index, victim_body.corePosition, true);
                        }
                    }
                }
            }

            orig(self, info, victim);
        }

        // this currently serves only to tag enemies that we have already hit.
        public class IronTalonBehavior : MonoBehaviour
        { }
    }
}