using BepInEx.Configuration;
using ROTA2.Buffs;
using RoR2;

namespace ROTA2.Equipment
{
    public class GhostScepter : EquipmentBase<GhostScepter>
    {
        public override string EquipmentName => "Ghost Scepter";
        public override string EquipmentTokenName => "GHOST_SCEPTER";
        public override string EquipmentTokenPickup => "Become immune to damage and unable to use your skills for a short time.";
        public override string EquipmentTokenDesc => $"Become {Utility("immune")} to {Damage("all incoming damage")} for {Utility($"{EtherealDuration} seconds")}. During this time, you {Health("cannot use your skills")}.";
        public override string EquipmentTokenLore => "Imbues the wielder with a ghostly presence, allowing them to evade physical damage.";
        public override string EquipmentIconPath => "RiskOfTheAncients2.Icons.ghost_scepter.png";
        public override float EquipmentCooldown => GhostScepterCooldown;
        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLanguageTokens();
            CreateEquipmentDef();
        }

        public float EtherealDuration;
        public float GhostScepterCooldown;
        private void CreateConfig(ConfigFile config)
        {
            EtherealDuration     = config.Bind("Equipment: " + EquipmentName, "Ethereal Duration", 6.0f,  "How long should the ethereal form last?").Value;
            GhostScepterCooldown = config.Bind("Equipment: " + EquipmentName, "Cooldown",          30.0f, "").Value;
        }

        protected override bool ActivateEquipment(EquipmentSlot slot)
        {
            var body = slot.characterBody;
            if (body)
            {
                GhostScepterBuff.Instance.ApplyTo(new BuffBase.ApplyParameters
                {
                    victim = body,
                    duration = EtherealDuration
                });
            }

            return true;
        }
    }
}