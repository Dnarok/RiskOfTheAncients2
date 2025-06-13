using R2API;
using RoR2;
using ROTA2.Items;
using UnityEngine;
using UnityEngine.Networking;

namespace ROTA2.Buffs
{
    public class HealingSalveBuff : BuffBase<HealingSalveBuff>
    {
        public override string BuffName => "Salve";
        public override string BuffTokenName => "SALVE";
        public override string BuffDefGUID => Assets.HealingSalve.BuffDef;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddHealthRegeneration;
            On.RoR2.HealthComponent.TakeDamage += OnTakeDamage;
            On.RoR2.CharacterBody.OnBuffFirstStackGained += OnAdd;
            On.RoR2.CharacterBody.OnBuffFinalStackLost += OnRemove;
        }

        private void AddHealthRegeneration(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs arguments)
        {
            if (HasThisBuff(body))
            {
                HealthComponent health = body.GetComponent<HealthComponent>();
                int count = HealingSalve.GetCount(body);
                if (health && count > 0)
                {
                    arguments.baseRegenAdd += health.fullHealth * (HealingSalve.Instance.MaximumHealthRegenerationBase.Value / 100.0f + HealingSalve.Instance.MaximumHealthRegenerationPerStack.Value / 100.0f * (count - 1));
                }
            }
        }
        private void OnTakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (damageInfo.rejected || damageInfo.damage == 0.0f)
            {
                orig(self, damageInfo);
                return;
            }

            if (HasThisBuff(self.body))
            {
                self.body.RemoveOldestTimedBuff(BuffDef);
            }

            orig(self, damageInfo);
        }
        private void OnAdd(On.RoR2.CharacterBody.orig_OnBuffFirstStackGained orig, CharacterBody self, BuffDef buffDef)
        {
            if (buffDef == BuffDef)
            {
                var behavior = self.GetComponent<HealingSalveBehavior>();
                if (behavior)
                {
                    behavior.enabled = true;
                }
                else
                {
                    behavior = self.gameObject.AddComponent<HealingSalveBehavior>();
                    behavior.enabled = true;
                }
            }

            orig(self, buffDef);
        }
        private void OnRemove(On.RoR2.CharacterBody.orig_OnBuffFinalStackLost orig, CharacterBody self, BuffDef buffDef)
        {
            if (buffDef == BuffDef)
            {
                var behavior = self.GetComponent<HealingSalveBehavior>();
                if (behavior)
                {
                    Object.Destroy(behavior);
                }
            }

            orig(self, buffDef);
        }

        public class HealingSalveBehavior : MonoBehaviour
        {
            CharacterBody body;
            GameObject effect;

            void Awake()
            {
                body = GetComponent<CharacterBody>();
            }
            void OnEnabled()
            {
                if (NetworkServer.active)
                {
                    effect = Instantiate(HealingSalve.effectPrefab, body.coreTransform);
                    effect.GetComponent<NetworkedBodyAttachment>().AttachToGameObjectAndSpawn(body.gameObject);
                }
            }
            void OnDisabled()
            {
                if (effect)
                {
                    Destroy(effect);
                    effect = null;
                }
            }
        }
    }
}