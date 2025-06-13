using RoR2;
using ROTA2.Items;

namespace ROTA2.Buffs
{
    public class RadianceBuff : BuffBase<RadianceBuff>
    {
        public override string BuffName => "Radiance Burn";
        public override string BuffTokenName => "RADIANCE_BUFF";
        public override string BuffDefGUID => Assets.Radiance.BuffDef;
        public override void Hooks()
        {
            On.RoR2.HealthComponent.TakeDamage += OnHit;
        }

        private void OnHit(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (damageInfo.attacker)
            {
                CharacterBody body = damageInfo.attacker.GetComponent<CharacterBody>();
                if (HasThisBuff(body) && Util.CheckRoll(Radiance.Instance.MissChance.Value))
                {
                    damageInfo.rejected = true;
                }
            }

            orig(self, damageInfo);
        }
    }
}