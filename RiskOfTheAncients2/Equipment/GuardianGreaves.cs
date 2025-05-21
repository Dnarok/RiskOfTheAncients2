using BepInEx.Configuration;
using RoR2;
using ROTA2.Buffs;
using System;

namespace ROTA2.Equipment
{
    public class GuardianGreaves : EquipmentBase<GuardianGreaves>
    {
        public override string EquipmentName => "Guardian Greaves";
        public override string EquipmentTokenName => "GUARDIAN_GREAVES";
        public override string EquipmentTokenPickup => "Heal all allies, reset all of their skill cooldowns, and give them armor for a short time.";
        public override string EquipmentTokenDesc => $"{Healing("Heal")} all allies for {Healing($"{MaximumHealthHeal}% of their maximum health")}, {Utility("reset all of their skill cooldowns")}, and increase their {Damage("armor")} by {Damage($"{ArmorBonus}")} for {Utility($"{ArmorBonusDuration} seconds")}.";
        public override string EquipmentTokenLore => "One of many holy instruments constructed to honor the Omniscience.";
        public override float EquipmentCooldown => GuardianGreavesCooldown;
        public override string EquipmentIconPath => "ROTA2.Icons.guardian_greaves.png";
        public override bool EquipmentCanDrop => false;
        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLanguageTokens();
            CreateEquipmentDef();
        }

        public float MaximumHealthHeal;
        public float ArmorBonus;
        public float ArmorBonusDuration;
        public float GuardianGreavesCooldown;
        private void CreateConfig(ConfigFile config)
        {
            MaximumHealthHeal       = config.Bind("Equipment: " + EquipmentName, "Maximum Health Heal", 50.0f, "").Value;
            ArmorBonus              = config.Bind("Equipment: " + EquipmentName, "Armor Bonus",         100.0f, "").Value;
            ArmorBonusDuration      = config.Bind("Equipment: " + EquipmentName, "Armor Duration",      8.0f, "").Value;
            GuardianGreavesCooldown = config.Bind("Equipment: " + EquipmentName, "Cooldown",            30.0f, "").Value;
        }

        protected override bool ActivateEquipment(EquipmentSlot slot)
        {
            if (slot && HasThisEquipment(slot.characterBody))
            {
                var allies = TeamComponent.GetTeamMembers(slot.characterBody.teamComponent.teamIndex);
                foreach (var member in allies)
                {
                    CharacterBody ally = member.GetComponent<CharacterBody>();
                    if (ally && ally.isActiveAndEnabled && ally.healthComponent && ally.skillLocator)
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

                        ally.healthComponent.HealFraction(MaximumHealthHeal / 100.0f, default);
                        GuardianGreavesBuff.Instance.ApplyTo(new Buffs.BuffBase.ApplyParameters
                        {
                            victim = ally,
                            duration = ArmorBonusDuration
                        });
                    }
                }

                Util.PlaySound("GuardianGreaves", slot.characterBody.gameObject);
            }

            return true;
        }
    }
}