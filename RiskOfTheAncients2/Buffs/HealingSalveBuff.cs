using ROTA2.Items;
using R2API;
using RoR2;
using UnityEngine;

namespace ROTA2.Buffs
{
    public class HealingSalveBuff : BuffBase<HealingSalveBuff>
    {
        public override string BuffName => "Salve";
        public override string BuffTokenName => "SALVE";
        public override bool BuffStacks => false;
        public override bool IsDebuff => false;
        public override Color BuffColor => Color.white;
        public override string BuffIconPath => "RiskOfTheAncients2.Icons.healing_salve.png";
        public override EliteDef BuffEliteDef => null;
        public override bool IsCooldown => false;
        public override bool IsHidden => false;
        public override NetworkSoundEventDef BuffStartSfx => null;
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
                int count = HealingSalve.Instance.GetCount(body);
                if (health && count > 0)
                {
                    arguments.baseRegenAdd += health.fullHealth * (HealingSalve.Instance.MaximumHealthRegenerationBase / 100.0f + HealingSalve.Instance.MaximumHealthRegenerationPerStack / 100.0f * (count - 1));
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
    }
}