using BepInEx.Configuration;
using RoR2;
using ROTA2.Buffs;

namespace ROTA2.Equipment
{
    public class Mekansm : EquipmentBase<Mekansm>
    {
        public override string EquipmentName => "Mekansm";
        public override string EquipmentTokenName => "MEKANSM";
        public override string EquipmentTokenPickup => "Heal all allies and give them armor for a short time.";
        public override string EquipmentTokenDesc => $"{Healing("Heal")} all allies for {Healing($"{MaximumHealthHeal}% of their maximum health")}, and increase their {Damage("armor")} by {Damage($"{ArmorBonus}")} for {Utility($"{ArmorBonusDuration} seconds")}.";
        public override string EquipmentTokenLore => "A glowing jewel formed out of assorted parts that somehow fit together perfectly.";
        public override float EquipmentCooldown => MekansmCooldown;
        public override string EquipmentIconPath => "ROTA2.Icons.mekansm.png";
        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLanguageTokens();
            CreateEquipmentDef();
        }

        public float MaximumHealthHeal;
        public float ArmorBonus;
        public float ArmorBonusDuration;
        public float MekansmCooldown;
        private void CreateConfig(ConfigFile config)
        {
            MaximumHealthHeal   = config.Bind("Equipment: " + EquipmentName, "Maximum Health Heal", 25.0f, "").Value;
            ArmorBonus          = config.Bind("Equipment: " + EquipmentName, "Armor Bonus",         50.0f, "").Value;
            ArmorBonusDuration  = config.Bind("Equipment: " + EquipmentName, "Armor Duration",      8.0f, "").Value;
            MekansmCooldown     = config.Bind("Equipment: " + EquipmentName, "Cooldown",            30.0f, "").Value;
        }

        protected override bool ActivateEquipment(EquipmentSlot slot)
        {
            if (slot && HasThisEquipment(slot.characterBody) && slot.characterBody.teamComponent)
            {
                var allies = TeamComponent.GetTeamMembers(slot.characterBody.teamComponent.teamIndex);
                foreach (var member in allies)
                {
                    HealthComponent ally = member.GetComponent<HealthComponent>();
                    if (ally && ally.isActiveAndEnabled)
                    {
                        ally.HealFraction(MaximumHealthHeal / 100.0f, default);
                        MekansmBuff.Instance.ApplyTo(new Buffs.BuffBase.ApplyParameters
                        {
                            victim = ally.body,
                            duration = ArmorBonusDuration
                        });
                    }
                }

                Util.PlaySound("Mekansm", slot.characterBody.gameObject);
            }

            return true;
        }
    }
}