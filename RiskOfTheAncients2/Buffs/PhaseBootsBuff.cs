using KinematicCharacterController;
using RoR2;
using UnityEngine;

namespace ROTA2.Buffs
{
    public class PhaseBootsBuff : BuffBase<PhaseBootsBuff>
    {
        public override string BuffName => "Phase";
        public override string BuffTokenName => "PHASE_BOOTS_BUFF";
        public override bool BuffStacks => false;
        public override bool IsDebuff => false;
        public override Color BuffColor => Color.white;
        public override string BuffIconPath => "ROTA2.Icons.phase_boots.png";
        public override EliteDef BuffEliteDef => null;
        public override bool IsCooldown => false;
        public override bool IsHidden => false;
        public override NetworkSoundEventDef BuffStartSfx => null;
        public override void Hooks()
        {
            On.RoR2.CharacterBody.OnBuffFirstStackGained += OnAdd;
            On.RoR2.CharacterBody.OnBuffFinalStackLost += OnRemove;
        }

        private void OnAdd(On.RoR2.CharacterBody.orig_OnBuffFirstStackGained orig, CharacterBody self, BuffDef buffDef)
        {
            if (buffDef == BuffDef)
            {
                self.gameObject.layer = LayerIndex.GetAppropriateFakeLayerForTeam(self.teamComponent.teamIndex).intVal;
                self.characterMotor.Motor.RebuildCollidableLayers();
            }
        }
        private void OnRemove(On.RoR2.CharacterBody.orig_OnBuffFinalStackLost orig, CharacterBody self, BuffDef buffDef)
        {
            if (buffDef == BuffDef)
            {
                self.gameObject.layer = LayerIndex.GetAppropriateLayerForTeam(self.teamComponent.teamIndex);
                self.characterMotor.Motor.RebuildCollidableLayers();
            }
        }
    }
}