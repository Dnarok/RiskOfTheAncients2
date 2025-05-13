using RoR2;
using UnityEngine;

namespace ROTA2.Buffs
{
    public class GhostScepterBuff : BuffBase<GhostScepterBuff>
    {
        public override string BuffName => "Ethereal";
        public override string BuffTokenName => "GHOST_SCEPTER_BUFF";
        public override bool BuffStacks => false;
        public override bool IsDebuff => false;
        public override Color BuffColor => Color.white;
        public override string BuffIconPath => "ROTA2.Icons.ghost_scepter.png";
        public override EliteDef BuffEliteDef => null;
        public override bool IsCooldown => false;
        public override bool IsHidden => false;
        public override NetworkSoundEventDef BuffStartSfx => null;
        public override void Hooks()
        {
            On.RoR2.HealthComponent.TakeDamage += OnTakeDamage;
            On.RoR2.CharacterBody.OnBuffFirstStackGained += OnAdd;
            On.RoR2.CharacterBody.OnBuffFinalStackLost += OnRemove;
        }

        private void OnTakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo info)
        {
            if (self && self.body && HasThisBuff(self.body))
            {
                info.rejected = true;
            }

            orig(self, info);
        }
        private void OnAdd(On.RoR2.CharacterBody.orig_OnBuffFirstStackGained orig, CharacterBody self, BuffDef buffDef)
        {
            if (self && self.skillLocator && self.inventory && buffDef == BuffDef)
            {
                SkillSlot[] slots = [SkillSlot.Primary];
                foreach (SkillSlot slot in slots)
                {
                    GenericSkill skill = self.skillLocator.GetSkill(slot);
                    if (skill)
                    {
                        skill.SetSkillOverride(this, Plugin.disabledSkill, GenericSkill.SkillOverridePriority.Contextual);
                    }
                }

                if (self.inventory.GetItemCount(RoR2Content.Items.Ghost) <= 0)
                {
                    self.inventory.GiveItem(RoR2Content.Items.Ghost);
                }
            }

            orig(self, buffDef);
        }
        private void OnRemove(On.RoR2.CharacterBody.orig_OnBuffFinalStackLost orig, CharacterBody self, BuffDef buffDef)
        {
            if (self && self.skillLocator && self.inventory && buffDef == BuffDef)
            {
                SkillSlot[] slots = [SkillSlot.Primary];
                foreach (SkillSlot slot in slots)
                {
                    GenericSkill skill = self.skillLocator.GetSkill(slot);
                    if (skill)
                    {
                        skill.UnsetSkillOverride(this, Plugin.disabledSkill, GenericSkill.SkillOverridePriority.Contextual);
                    }
                }

                if (self.inventory.GetItemCount(RoR2Content.Items.Ghost) > 0)
                {
                    self.inventory.RemoveItem(RoR2Content.Items.Ghost);
                }
            }

            orig(self, buffDef);
        }
    }
}