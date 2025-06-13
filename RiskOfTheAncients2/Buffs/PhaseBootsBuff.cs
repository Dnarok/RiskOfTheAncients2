using RoR2;

namespace ROTA2.Buffs
{
    public class PhaseBootsBuff : BuffBase<PhaseBootsBuff>
    {
        public override string BuffName => "Phase";
        public override string BuffTokenName => "PHASE_BOOTS_BUFF";
        public override string BuffDefGUID => Assets.PhaseBoots.BuffDef;
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

            orig(self, buffDef);
        }
        private void OnRemove(On.RoR2.CharacterBody.orig_OnBuffFinalStackLost orig, CharacterBody self, BuffDef buffDef)
        {
            if (buffDef == BuffDef)
            {
                self.gameObject.layer = LayerIndex.GetAppropriateLayerForTeam(self.teamComponent.teamIndex);
                self.characterMotor.Motor.RebuildCollidableLayers();
            }

            orig(self, buffDef);
        }
    }
}