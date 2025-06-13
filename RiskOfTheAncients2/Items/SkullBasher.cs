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
    public class SkullBasher : ItemBase<SkullBasher>
    {
        public override string ItemName => "Skull Basher";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "SKULL_BASHER";
        public override string ItemTokenPickup => "Chance on hit to strike for extra damage and stun. Recharges over time.";
        public override string ItemTokenDesc => $"{Damage($"{ProcChance.Value}%")} chance on hit to {Damage("bash")} for {Damage($"{DamageBase.Value}%")} {Stack($"(+{DamagePerStack.Value}% per stack)")} TOTAL damage and {Utility("stun")} enemies for {Utility($"{BashDuration.Value} seconds")}. Recharges every {Utility($"{BashCooldown.Value} seconds")}.";
        public override string ItemTokenLore => "A rather typical spear that can sometimes pierce through an enemy's armor when used to attack.";
        public override string ItemDefGUID => Assets.SkullBasher.ItemDef;
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

        public ConfigEntry<float> ProcChance;
        public ConfigEntry<float> DamageBase;
        public ConfigEntry<float> DamagePerStack;
        public ConfigEntry<float> BashDuration;
        public ConfigEntry<float> BashCooldown;
        public ConfigEntry<bool> PlaySound;
        public void CreateConfig(ConfigFile configuration)
        {
            ProcChance = configuration.Bind("Item: " + ItemName, "Proc Chance", 15.0f, "What is the chance on hit to proc?");
            ModSettingsManager.AddOption(new FloatFieldOption(ProcChance));
            DamageBase = configuration.Bind("Item: " + ItemName, "Damage Base", 250.0f, "How much total damage should the bash do with the first stack?");
            ModSettingsManager.AddOption(new FloatFieldOption(DamageBase));
            DamagePerStack = configuration.Bind("Item: " + ItemName, "Damage Per Stack", 250.0f, "How much extra total damage should the bash do for subsequent stacks?");
            ModSettingsManager.AddOption(new FloatFieldOption(DamagePerStack));
            BashDuration = configuration.Bind("Item: " + ItemName, "Bash Duration", 2.5f, "How long should the bash last?");
            ModSettingsManager.AddOption(new FloatFieldOption(BashDuration));
            BashCooldown = configuration.Bind("Item: " + ItemName, "Bash Cooldown", 5.0f, "How long should it take to recharge before another bash?");
            ModSettingsManager.AddOption(new FloatFieldOption(BashCooldown));
            PlaySound = configuration.Bind("Item: " + ItemName, "Play Sound", true, "");
            ModSettingsManager.AddOption(new CheckBoxOption(PlaySound));
        }

        NetworkSoundEventDef sound = null;
        protected void CreateSounds()
        {
            Addressables.LoadAssetAsync<NetworkSoundEventDef>(Assets.SkullBasher.NetworkSoundEventDef).Completed += (x) => { ContentAddition.AddNetworkSoundEventDef(x.Result); sound = x.Result; };
        }

        private void OnHit(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo info, GameObject victim)
        {
            if (info.rejected || info.procCoefficient <= 0)
            {
                orig(self, info, victim);
                return;
            }

            GameObject attacker = info.attacker;
            if (attacker)
            {
                CharacterBody attacker_body = attacker.GetComponent<CharacterBody>();
                HealthComponent victim_health = victim.GetComponent<HealthComponent>();
                if (attacker_body && victim_health && !SkullBasherCooldown.HasThisBuff(attacker_body))
                {
                    int count = GetCount(attacker_body);
                    if (count > 0 && Util.CheckRoll(ProcChance.Value * info.procCoefficient, attacker_body.master))
                    {
                        DamageInfo bonus_hit = new()
                        {
                            attacker = attacker,
                            damage = info.damage * (DamageBase.Value / 100.0f + DamagePerStack.Value / 100.0f * (count - 1)),
                            position = info.position,
                            damageColorIndex = DamageColorIndex.Sniper,
                            damageType = info.damageType,
                            procCoefficient = 0.0f,
                            crit = info.crit,
                            procChainMask = info.procChainMask
                        };
                        victim_health.TakeDamage(bonus_hit);

                        SetStateOnHurt.SetStunOnObject(victim, BashDuration.Value);

                        SkullBasherCooldown.ApplyTo(
                            body: attacker_body,
                            duration: BashCooldown.Value
                        );

                        if (PlaySound.Value)
                        {
                            EffectManager.SimpleSoundEffect(sound.index, info.position, true);
                        }
                    }
                }
            }

            orig(self, info, victim);
        }
    }
}