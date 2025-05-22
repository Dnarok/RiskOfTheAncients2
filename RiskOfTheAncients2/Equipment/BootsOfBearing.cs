using BepInEx.Configuration;
using R2API;
using RoR2;
using ROTA2.Buffs;
using System;
using UnityEngine;

namespace ROTA2.Equipment
{
    public class BootsOfBearing : EquipmentBase<BootsOfBearing>
    {
        public override string EquipmentName => "Boots of Bearing";
        public override string EquipmentTokenName => "BOOTS_OF_BEARING";
        public override string EquipmentTokenPickup => "Passively increase movement speed and health regneration. Activate to send you and all your allies into a frenzy.";
        public override string EquipmentTokenDesc => $"Passively increases {Utility("base movement speed")} by {Utility($"{PassiveMovementSpeed}")} and {Healing("base health regeneration")} by {Healing($"+{PassiveHealthRegeneration} hp/s")}. Activate to send all allies into a {Damage("frenzy")} for {Utility($"{FrenzyDuration} seconds")}, increasing {Utility("movement speed")} by {Utility($"{ActiveMovementSpeed}%")} and {Damage("attack speed")} by {Damage($"{ActiveAttackSpeed}%")}.";
        public override string EquipmentTokenLore => "Resplendent footwear fashioned for the ancient herald that first dared spread the glory of Stonehall beyond the original borders of its nascent claim.";
        public override float EquipmentCooldown => BootsOfBearingCooldown;
        public override string EquipmentIconPath => "ROTA2.Icons.boots_of_bearing.png";
        public override bool EquipmentCanDrop => false;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddMovementSpeed;
            RecalculateStatsAPI.GetStatCoefficients += AddHealthRegeneration;
        }
        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLanguageTokens();
            CreateEquipmentDef();
            Hooks();
        }

        public float PassiveMovementSpeed;
        public float PassiveHealthRegeneration;
        public float ActiveMovementSpeed;
        public float ActiveAttackSpeed;
        public float FrenzyDuration;
        public float BootsOfBearingCooldown;
        private void CreateConfig(ConfigFile config)
        {
            PassiveMovementSpeed        = config.Bind("Equipment: " + EquipmentName, "Passive Base Movement Speed",      1.2f,   "").Value;
            PassiveHealthRegeneration   = config.Bind("Equipment: " + EquipmentName, "Passive Base Health Regeneration", 5.0f,   "").Value;
            ActiveMovementSpeed         = config.Bind("Equipment: " + EquipmentName, "Active Movement Speed",            75.0f,  "").Value;
            ActiveAttackSpeed           = config.Bind("Equipment: " + EquipmentName, "Active Attack Speed",              150.0f, "").Value;
            FrenzyDuration              = config.Bind("Equipment: " + EquipmentName, "Frenzy Duration",                  7.0f,   "").Value;
            BootsOfBearingCooldown      = config.Bind("Equipment: " + EquipmentName, "Cooldown",                         45.0f,  "").Value;
        }

        protected override bool ActivateEquipment(EquipmentSlot slot)
        {
            if (slot && HasThisEquipment(slot.characterBody))
            {
                var allies = TeamComponent.GetTeamMembers(slot.characterBody.teamComponent.teamIndex);
                foreach (var member in allies)
                {
                    CharacterBody ally = member.GetComponent<CharacterBody>();
                    if (ally && ally.isActiveAndEnabled)
                    {
                        BootsOfBearingBuff.Instance.ApplyTo(new Buffs.BuffBase.ApplyParameters
                        {
                            victim = ally,
                            duration = FrenzyDuration
                        });
                    }
                }

                Vector3 position = slot.characterBody.corePosition;
                EffectData effect = new()
                {
                    origin = position
                };
                effect.SetNetworkedObjectReference(slot.characterBody.gameObject);
                EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/TeamWarCryActivation"), effect, true);

                Util.PlaySound("BootsOfBearing", slot.characterBody.gameObject);
            }

            return true;
        }

        private void AddMovementSpeed(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (HasThisEquipment(body))
            {
                args.baseMoveSpeedAdd += PassiveMovementSpeed;
            }
        }
        private void AddHealthRegeneration(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (HasThisEquipment(body))
            {
                args.baseRegenAdd += PassiveHealthRegeneration * (1.0f + 0.2f * (body.level - 1));
            }
        }
    }
}