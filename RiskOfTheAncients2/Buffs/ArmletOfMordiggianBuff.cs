using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using ROTA2.Equipment;
using System;
using UnityEngine;

namespace ROTA2.Buffs
{
    public class ArmletOfMordiggianBuff : BuffBase<ArmletOfMordiggianBuff>
    {
        public override string BuffName => "Unholy Strength";
        public override string BuffTokenName => "ARMLET_OF_MORDIGGIAN_BUFF";
        public override bool BuffStacks => false;
        public override bool IsDebuff => false;
        public override Color BuffColor => Color.white;
        public override string BuffIconPath => "ROTA2.Icons.armlet_of_mordiggian_toggled.png";
        public override EliteDef BuffEliteDef => null;
        public override bool IsCooldown => false;
        public override bool IsHidden => false;
        public override NetworkSoundEventDef BuffStartSfx => null;
        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += (il) =>
            {
                ILCursor cursor = new(il);
                cursor.GotoNext(
                    x => x.MatchLdarg(0),
                    x => x.MatchCall<CharacterBody>("get_maxHealth"),
                    x => x.MatchCall<CharacterBody>("set_maxBonusHealth")
                );
                cursor.Emit(OpCodes.Ldarg_0);
                cursor.EmitDelegate<Action<CharacterBody>>((body) =>
                {
                    if (HasThisBuff(body))
                    {
                        body.damage *= 1.0f + ArmletOfMordiggianToggled.Instance.DamageBonus / 100.0f;
                        body.attackSpeed *= 1.0f + ArmletOfMordiggianToggled.Instance.AttackSpeedBonus / 100.0f;
                        body.armor += ArmletOfMordiggianToggled.Instance.ArmorBonus;
                    }
                });
            };
        }
    }
}