﻿using BepInEx.Configuration;
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
        public override string ItemTokenDesc => $"{Damage("Ignite")} enemies within {Damage($"{Radius}m")} for {Damage($"{IgniteBase}%")} {Stack($"(+{IgnitePerStack}% per stack)")} base damage. Additionally, enemies {Damage("burn")} for {Damage($"{BurnBase}%")} {Stack($"(+{BurnPerStack}% per stack)")} base damage and are {Utility("blinded")}, causing them to {Utility($"miss {MissChance}%")} of the time. {Utility("Unaffected by luck")}.";
        public override string ItemTokenLore => "A divine weapon that causes damage and a bright burning effect that lays waste to nearby enemies.";
        public override string ItemIconPath => "ROTA2.Icons.radiance.png";
        public override string ItemModelPath => "radiance.prefab";
        public override ItemTier Tier => ItemTier.Tier3;
        public override ItemTag[] ItemTags => [ItemTag.Damage];
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

        public float Radius;
        public float IgniteBase;
        public float IgnitePerStack;
        public float BurnBase;
        public float BurnPerStack;
        public float BurnDuration;
        public float MissChance;
        public float LingerDuration;
        private void CreateConfig(ConfigFile configuration)
        {
            Radius          = configuration.Bind("Item: " + ItemName, "Radius",                     30.0f,  "").Value;
            IgniteBase      = configuration.Bind("Item: " + ItemName, "Ignite Base Damage",         150.0f, "").Value;
            IgnitePerStack  = configuration.Bind("Item: " + ItemName, "Ignite Per Stack Damage",    150.0f, "").Value;
            BurnBase        = configuration.Bind("Item: " + ItemName, "Burn Base Damage",           100.0f, "").Value;
            BurnPerStack    = configuration.Bind("Item: " + ItemName, "Burn Per Stack Damage",      50.0f,  "").Value;
            BurnDuration    = configuration.Bind("Item: " + ItemName, "Burn Duration",              3.0f,   "").Value;
            MissChance      = configuration.Bind("Item: " + ItemName, "Miss Chance",                25.0f,  "").Value;
            LingerDuration  = configuration.Bind("Item: " + ItemName, "Miss Linger Duration",       1.0f,   "").Value;
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

                int count = Radiance.Instance.GetCount(body);
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
                        child.localScale = new Vector3(Instance.Radius * 2, Instance.Radius * 2, Instance.Radius * 2);
                    }
                    indicator.GetComponent<NetworkedBodyAttachment>().AttachToGameObjectAndSpawn(body.gameObject);
                }

                timer += Time.fixedDeltaTime;
                if (timer > 1.0f)
                {
                    timer -= 1.0f;
                    float radius2 = Radiance.Instance.Radius * Radiance.Instance.Radius;
                    
                    for (TeamIndex index = TeamIndex.Neutral; index < TeamIndex.Count; index++)
                    {
                        if (index != body.teamComponent.teamIndex)
                        {
                            foreach (var member in TeamComponent.GetTeamMembers(index))
                            {
                                CharacterBody enemy = member.GetComponent<CharacterBody>();
                                if (enemy && enemy.isActiveAndEnabled && enemy.healthComponent && (enemy.transform.position - body.transform.position).sqrMagnitude <= radius2)
                                {
                                    InflictDotInfo burn = new()
                                    {
                                        attackerObject = body.gameObject,
                                        victimObject = enemy.gameObject,
                                        dotIndex = DotController.DotIndex.Burn,
                                        duration = Radiance.Instance.BurnDuration,
                                        damageMultiplier = Radiance.Instance.BurnBase / 100.0f + Radiance.Instance.BurnPerStack / 100.0f * (count - 1)
                                    };
                                    StrengthenBurnUtils.CheckDotForUpgrade(body.inventory, ref burn);
                                    DotController.InflictDot(ref burn);

                                    RadianceBuff.Instance.ApplyTo(new BuffBase.ApplyParameters
                                    {
                                        victim = enemy,
                                        duration = 1.0f + Radiance.Instance.LingerDuration
                                    });

                                    DamageInfo ignite = new()
                                    {
                                        attacker = body.gameObject,
                                        damage = body.damage * (Radiance.Instance.IgniteBase / 100.0f + Radiance.Instance.IgnitePerStack / 100.0f * (count - 1)),
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