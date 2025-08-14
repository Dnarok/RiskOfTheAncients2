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
        }

        private void AddHealthRegeneration(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs arguments)
        {
            if (HasThisBuff(body))
            {
                HealthComponent health = body.GetComponent<HealthComponent>();
                int count = HealingSalve.GetCount(body);
                if (health && count > 0)
                {
                    arguments.baseRegenAdd += (HealingSalve.Instance.RegenerationBase.Value + HealingSalve.Instance.RegenerationPerStack.Value * (count - 1)) * (1 + 0.2f * body.level);
                }
            }
        }
        private void OnTakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (damageInfo.rejected || damageInfo.damage == 0f)
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
    }
}