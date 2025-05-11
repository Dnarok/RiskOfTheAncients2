using ROTA2.Items;
using R2API;
using RoR2;
using System;
using UnityEngine;

namespace ROTA2.Buffs
{
    public class RadianceBuff : BuffBase<RadianceBuff>
    {
        public override string BuffName => "Radiance Burn";
        public override string BuffTokenName => "RADIANCE_BUFF";
        public override bool BuffStacks => false;
        public override bool IsDebuff => true;
        public override Color BuffColor => Color.white;
        public override string BuffIconPath => "ROTA2.Icons.radiance.png";
        public override EliteDef BuffEliteDef => null;
        public override bool IsCooldown => false;
        public override bool IsHidden => false;
        public override NetworkSoundEventDef BuffStartSfx => null;
        public override void Hooks()
        {
            On.RoR2.HealthComponent.TakeDamage += OnHit;
        }

        private void OnHit(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (damageInfo.attacker)
            {
                CharacterBody body = damageInfo.attacker.GetComponent<CharacterBody>();
                if (HasThisBuff(body) && Util.CheckRoll(Radiance.Instance.MissChance))
                {
                    Log.Debug($"Blocked {damageInfo.damage} because of Radiance.");
                    damageInfo.rejected = true;
                }
            }

            orig(self, damageInfo);
        }
    }
}