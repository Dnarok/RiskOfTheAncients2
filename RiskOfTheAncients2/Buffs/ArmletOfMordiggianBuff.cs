﻿using Mono.Cecil.Cil;
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
                var index = 0;
                ILLabel label = null;
                if (!cursor.TryGotoNext
                    (
                        x => x.MatchLdsfld(typeof(DLC2Content.Items), nameof(DLC2Content.Items.BoostAllStats)),
                        x => x.MatchCallOrCallvirt<Inventory>(nameof(Inventory.GetItemCount)),
                        x => x.MatchStloc(out index)
                    ) ||
                    !cursor.TryGotoNext
                    (
                        x => x.MatchLdloc(index),
                        x => x.MatchLdcI4(0),
                        x => x.MatchBle(out label)
                    )
                ){
                    Log.Error("Failed to match IL for Unholy Strength, it won't do anything!");
                    return;
                }
                cursor.GotoLabel(label);
                cursor.Emit(OpCodes.Ldarg_0);
                cursor.EmitDelegate<Action<CharacterBody>>((body) =>
                {
                    if (HasThisBuff(body))
                    {
                        body.damage *= 1.0f + ArmletOfMordiggian.Instance.DamageBonus / 100.0f;
                        body.attackSpeed *= 1.0f + ArmletOfMordiggian.Instance.AttackSpeedBonus / 100.0f;
                        body.armor += ArmletOfMordiggian.Instance.ArmorBonus;
                    }
                });
            };
        }
    }
}