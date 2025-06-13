using BepInEx.Configuration;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
using ROTA2.Buffs;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace ROTA2.Items
{
    public class Radiance : ItemBase<Radiance>
    {
        public override string ItemName => "Radiance";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "RADIANCE";
        public override string ItemTokenPickup => "Ignite and blind nearby enemies.";
        public override string ItemTokenDesc => $"{Damage("Ignite")} enemies within {Damage($"{Radius.Value}m")} for {Damage($"{IgniteBase.Value}%")} {Stack($"(+{IgnitePerStack.Value}% per stack)")} base damage. Additionally, enemies {Damage("burn")} for {Damage($"{BurnBase.Value}%")} {Stack($"(+{BurnPerStack.Value}% per stack)")} base damage and are {Utility("blinded")}, causing them to {Utility($"miss {MissChance.Value}%")} of the time. {Utility("Unaffected by luck")}.";
        public override string ItemTokenLore => "A divine weapon that causes damage and a bright burning effect that lays waste to nearby enemies.";
        public override string ItemDefGUID => Assets.Radiance.ItemDef;
        public override void Hooks()
        {
            CharacterBody.onBodyInventoryChangedGlobal += OnInventoryChanged;
        }
        public override void Init(ConfigFile configuration)
        {
            CreateConfig(configuration);
            CreateLanguageTokens();
            CreateItemDef();
            Hooks();
        }

        public ConfigEntry<float> Radius;
        public ConfigEntry<float> IgniteBase;
        public ConfigEntry<float> IgnitePerStack;
        public ConfigEntry<float> BurnBase;
        public ConfigEntry<float> BurnPerStack;
        public ConfigEntry<float> MissChance;
        public ConfigEntry<float> LingerDuration;
        private void CreateConfig(ConfigFile configuration)
        {
            Radius = configuration.Bind("Item: " + ItemName, "Radius", 30.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(Radius));
            IgniteBase = configuration.Bind("Item: " + ItemName, "Ignite Base Damage", 150.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(IgniteBase));
            IgnitePerStack = configuration.Bind("Item: " + ItemName, "Ignite Per Stack Damage", 150.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(IgnitePerStack));
            BurnBase = configuration.Bind("Item: " + ItemName, "Burn Base Damage", 200.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(BurnBase));
            BurnPerStack = configuration.Bind("Item: " + ItemName, "Burn Per Stack Damage", 200.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(BurnPerStack));
            MissChance = configuration.Bind("Item: " + ItemName, "Miss Chance", 25.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(MissChance));
            LingerDuration = configuration.Bind("Item: " + ItemName, "Miss Linger Duration", 1.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(LingerDuration));
        }

        private void OnInventoryChanged(CharacterBody body)
        {
            if (GetCount(body) > 0 && !body.GetComponent<RadianceBehavior>())
            {
                body.gameObject.AddComponent<RadianceBehavior>();
            }
        }

        private class RadianceBehavior : MonoBehaviour
        {
            static GameObject prefab;
            CharacterBody body;
            GameObject indicator;
            float timer = 0.0f;

            void Awake()
            {
                body = GetComponent<CharacterBody>();
                if (prefab == null)
                {
                    prefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/NearbyDamageBonus/NearbyDamageBonusIndicator.prefab").WaitForCompletion();
                }
            }
            void FixedUpdate()
            {
                if (!body || (body.healthComponent && !body.healthComponent.alive) || !NetworkServer.active)
                {
                    return;
                }

                int count = Radiance.GetCount(body);
                if (count <= 0)
                {
                    if (indicator != null)
                    {
                        Object.Destroy(indicator);
                        indicator = null;
                    }

                    return;
                }
                else if (count > 0 && indicator == null)
                {
                    indicator = Object.Instantiate(prefab, body.corePosition, Quaternion.identity);
                    foreach (var renderer in indicator.GetComponentsInChildren<Renderer>())
                    {
                        renderer.material.SetColor("_TintColor", new UnityEngine.Color()
                        {
                            r = 1.0f,
                            g = 0.6f,
                            b = 0.0f,
                            a = 0.5f
                        });
                    }
                    foreach (Transform child in indicator.transform)
                    {
                        child.localScale = new Vector3(Instance.Radius.Value * 2, Instance.Radius.Value * 2, Instance.Radius.Value * 2);
                    }
                    indicator.GetComponent<NetworkedBodyAttachment>().AttachToGameObjectAndSpawn(body.gameObject);
                }

                timer += Time.fixedDeltaTime;
                if (timer > 1.0f)
                {
                    timer -= 1.0f;
                    float radius2 = Instance.Radius.Value * Instance.Radius.Value;

                    for (TeamIndex index = TeamIndex.Neutral; index < TeamIndex.Count; index++)
                    {
                        if (index != body.teamComponent.teamIndex)
                        {
                            foreach (var member in TeamComponent.GetTeamMembers(index))
                            {
                                CharacterBody enemy = member.GetComponent<CharacterBody>();
                                if (enemy && enemy.isActiveAndEnabled && enemy.healthComponent && (enemy.transform.position - body.transform.position).sqrMagnitude <= radius2)
                                {
                                    float damage = body.damage * (Instance.BurnBase.Value / 100f + Instance.BurnPerStack.Value / 100f * (count - 1));
                                    var burn = default(InflictDotInfo);
                                    burn.attackerObject = body.gameObject;
                                    burn.victimObject = enemy.gameObject;
                                    burn.dotIndex = DotController.DotIndex.Burn;
                                    burn.totalDamage = damage;
                                    burn.damageMultiplier = 1f;
                                    StrengthenBurnUtils.CheckDotForUpgrade(body.inventory, ref burn);
                                    DotController.InflictDot(ref burn);

                                    RadianceBuff.ApplyTo(
                                        body: enemy,
                                        duration: 1.0f + Instance.LingerDuration.Value
                                    );

                                    DamageInfo ignite = new()
                                    {
                                        attacker = body.gameObject,
                                        damage = body.damage * (Instance.IgniteBase.Value / 100.0f + Instance.IgnitePerStack.Value / 100.0f * (count - 1)),
                                        position = enemy.transform.position,
                                        damageType = DamageType.AOE,
                                        procCoefficient = 0.0f
                                    };
                                    enemy.healthComponent.TakeDamage(ignite);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}