using BepInEx.Configuration;
using RoR2;
using System;

namespace ROTA2.Equipment
{
    public class ArcaneBoots : EquipmentBase<ArcaneBoots>
    {
        public override string EquipmentName => "Arcane Boots";
        public override string EquipmentTokenName => "ARCANE_BOOTS";
        public override string EquipmentTokenPickup => "Reset all skill cooldowns of all allies.";
        public override string EquipmentTokenDesc => $"{Utility("Reset all skill cooldowns")} of all allies.";
        public override string EquipmentTokenLore => "Magi equipped with these boots are valued in battle.";
        public override float EquipmentCooldown => ArcaneBootsCooldown;
        public override string EquipmentIconPath => "ROTA2.Icons.arcane_boots.png";
        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLanguageTokens();
            CreateEquipmentDef();
        }

        public float ArcaneBootsCooldown;
        private void CreateConfig(ConfigFile config)
        {
            ArcaneBootsCooldown = config.Bind("Equipment: " + EquipmentName, "Cooldown", 30.0f, "").Value;
        }

        protected override bool ActivateEquipment(EquipmentSlot slot)
        {
            if (slot && HasThisEquipment(slot.characterBody) && slot.characterBody.teamComponent)
            {
                var allies = TeamComponent.GetTeamMembers(slot.characterBody.teamComponent.teamIndex);
                foreach (var member in allies)
                {
                    CharacterBody ally = member.GetComponent<CharacterBody>();
                    if (ally && ally.isActiveAndEnabled && ally.skillLocator)
                    {
                        var skills = ally.skillLocator.allSkills;
                        if (skills != null)
                        {
                            foreach (var skill in skills)
                            {
                                if (skill && skill.CanApplyAmmoPack() && skill.cooldownRemaining > 0.0f)
                                {
                                    skill.ApplyAmmoPack();
                                }
                            }
                        }
                    }
                }

                Util.PlaySound("ArcaneBoots", slot.characterBody.gameObject);
            }

            return true;
        }
    }
}