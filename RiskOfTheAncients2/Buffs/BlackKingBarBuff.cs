using RoR2;
using ROTA2.Equipment;
using UnityEngine;

namespace ROTA2.Buffs
{
    public class BlackKingBarBuff : BuffBase<BlackKingBarBuff>
    {
        public override string BuffName => "Avatar";
        public override string BuffTokenName => "BLACK_KING_BAR_BUFF";
        public override string BuffDefGUID => Assets.BlackKingBar.BuffDef;
        public override void Hooks()
        {
            On.RoR2.HealthComponent.TakeDamage += OnTakeDamage;
            On.RoR2.DotController.AddDot += OnAddDot;
            On.RoR2.CharacterBody.AddBuff_BuffDef += OnAddBuff_BuffDef;
            On.RoR2.CharacterBody.AddBuff_BuffIndex += OnAddBuff_BuffIndex;
            On.RoR2.CharacterBody.AddTimedBuffAuthority += OnAddTimedBuffAuthority;
            On.RoR2.CharacterBody.AddTimedBuff_BuffDef_float += OnAddTimedBuff_BuffDef_float;
            On.RoR2.CharacterBody.AddTimedBuff_BuffDef_float_int += OnAddTimedBuff_BuffDef_float_int;
        }

        private void OnTakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo info)
        {
            if (self && HasThisBuff(self.body))
            {
                info.damage *= 1.0f - (BlackKingBar.Instance.DamageReduction.Value / 100.0f);
            }

            orig(self, info);
        }
        private void OnAddDot(On.RoR2.DotController.orig_AddDot orig, DotController self, GameObject attackerObject, float duration, DotController.DotIndex dotIndex, float damageMultiplier, uint? maxStacksFromAttacker, float? totalDamage, DotController.DotIndex? preUpgradeDotIndex)
        {
            if (self && HasThisBuff(self.victimBody))
            {
                // don't add it.
                return;
            }
            else
            {
                orig(self, attackerObject, duration, dotIndex, damageMultiplier, maxStacksFromAttacker, totalDamage, preUpgradeDotIndex);
            }
        }
        private void OnAddBuff_BuffDef(On.RoR2.CharacterBody.orig_AddBuff_BuffDef orig, CharacterBody self, BuffDef buffDef)
        {
            if (self && buffDef != null && buffDef.isDebuff && !buffDef.isCooldown && HasThisBuff(self))
            {
                // don't add it.
                return;
            }
            else
            {
                orig(self, buffDef);
            }
        }
        private void OnAddBuff_BuffIndex(On.RoR2.CharacterBody.orig_AddBuff_BuffIndex orig, CharacterBody self, BuffIndex buffType)
        {
            var buffDef = BuffCatalog.GetBuffDef(buffType);
            if (self && buffDef != null && buffDef.isDebuff && !buffDef.isCooldown && HasThisBuff(self))
            {
                // don't add it.
                return;
            }
            else
            {
                orig(self, buffType);
            }
        }
        private void OnAddTimedBuffAuthority(On.RoR2.CharacterBody.orig_AddTimedBuffAuthority orig, CharacterBody self, BuffIndex buffType, float duration)
        {
            var buffDef = BuffCatalog.GetBuffDef(buffType);
            if (self && buffDef != null && buffDef.isDebuff && !buffDef.isCooldown && HasThisBuff(self))
            {
                // don't add it.
                return;
            }
            else
            {
                orig(self, buffType, duration);
            }
        }
        private void OnAddTimedBuff_BuffDef_float(On.RoR2.CharacterBody.orig_AddTimedBuff_BuffDef_float orig, CharacterBody self, BuffDef buffDef, float duration)
        {
            if (self && buffDef != null && buffDef.isDebuff && !buffDef.isCooldown && HasThisBuff(self))
            {
                // don't add it.
                return;
            }
            else
            {
                orig(self, buffDef, duration);
            }
        }
        private void OnAddTimedBuff_BuffDef_float_int(On.RoR2.CharacterBody.orig_AddTimedBuff_BuffDef_float_int orig, CharacterBody self, BuffDef buffDef, float duration, int maxStacks)
        {
            if (self && buffDef != null && buffDef.isDebuff && !buffDef.isCooldown && HasThisBuff(self))
            {
                // don't add it.
                return;
            }
            else
            {
                orig(self, buffDef, duration, maxStacks);
            }
        }
    }
}