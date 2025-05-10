using RoR2;
using ROTA2.Items;
using UnityEngine;

namespace ROTA2.Buffs
{
    public class NemesisCurseBuff : BuffBase<NemesisCurseBuff>
    {
        public override string BuffName => "Glassify";
        public override string BuffTokenName => "NEMESIS_CURSE_BUFF";
        public override bool BuffStacks => true;
        public override bool IsDebuff => true;
        public override Color BuffColor => Color.white;
        public override string BuffIconPath => "RiskOfTheAncients2.Icons.nemesis_curse.png";
        public override EliteDef BuffEliteDef => null;
        public override bool IsCooldown => false;
        public override bool IsHidden => false;
        public override NetworkSoundEventDef BuffStartSfx => null;
        public override void Hooks()
        {
            On.RoR2.HealthComponent.TakeDamage += OnTakeDamage;
        }

        private void OnTakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo info)
        {
            int count = GetBuffCount(self.body);
            if (count >= 1)
            {
                info.damage *= (1.0f + NemesisCurse.Instance.DamageBase / 100.0f) * Mathf.Pow(1.0f + NemesisCurse.Instance.DamagePerStack / 100.0f, count - 1);
                info.damageColorIndex = DamageColorIndex.DeathMark;
            }

            orig(self, info);
        }
    }
    public class NemesisCurseCooldown : BuffBase<NemesisCurseCooldown>
    {
        public override string BuffName => "Nemesis Curse Cooldown";
        public override string BuffTokenName => "NEMESIS_CURSE_COOLDOWN";
        public override bool BuffStacks => false;
        public override bool IsDebuff => false;
        public override Color BuffColor => Color.white;
        public override string BuffIconPath => "RiskOfTheAncients2.Icons.nemesis_curse_cooldown.png";
        public override EliteDef BuffEliteDef => null;
        public override bool IsCooldown => true;
        public override bool IsHidden => false;
        public override NetworkSoundEventDef BuffStartSfx => null;
    }
}