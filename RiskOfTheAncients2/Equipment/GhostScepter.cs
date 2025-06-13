using BepInEx.Configuration;
using R2API;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
using ROTA2.Buffs;
using UnityEngine.AddressableAssets;

namespace ROTA2.Equipment
{
    public class GhostScepter : EquipmentBase<GhostScepter>
    {
        public override string EquipmentName => "Ghost Scepter";
        public override string EquipmentTokenName => "GHOST_SCEPTER";
        public override string EquipmentTokenPickup => "Become immune to damage and unable to use your skills for a short time.";
        public override string EquipmentTokenDesc => $"Become {Utility("immune")} to {Damage("all incoming damage")} for {Utility($"{EtherealDuration.Value} seconds")}. During this time, you {Health("cannot use your Primary skill")}.";
        public override string EquipmentTokenLore => "Imbues the wielder with a ghostly presence, allowing them to evade physical damage.";
        public override string EquipmentDefGUID => Assets.GhostScepter.EquipmentDef;
        public override float EquipmentCooldown => GhostScepterCooldown.Value;
        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateSounds();
            CreateLanguageTokens();
            CreateEquipmentDef();
        }

        public ConfigEntry<float> EtherealDuration;
        public ConfigEntry<float> GhostScepterCooldown;
        private void CreateConfig(ConfigFile config)
        {
            EtherealDuration = config.Bind("Equipment: " + EquipmentName, "Ethereal Duration", 6.0f, "How long should the ethereal form last?");
            ModSettingsManager.AddOption(new FloatFieldOption(EtherealDuration));
            GhostScepterCooldown = config.Bind("Equipment: " + EquipmentName, "Cooldown", 60.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(GhostScepterCooldown));
        }

        NetworkSoundEventDef sound = null;
        protected void CreateSounds()
        {
            Addressables.LoadAssetAsync<NetworkSoundEventDef>(Assets.GhostScepter.NetworkSoundEventDef).Completed += (x) => { ContentAddition.AddNetworkSoundEventDef(x.Result); sound = x.Result; };
        }

        protected override bool ActivateEquipment(EquipmentSlot slot)
        {
            if (slot && HasThisEquipment(slot.characterBody))
            {
                GhostScepterBuff.ApplyTo(
                    body: slot.characterBody,
                    duration: EtherealDuration.Value
                );

                EffectManager.SimpleSoundEffect(sound.index, slot.characterBody.corePosition, true);
            }

            return true;
        }
    }
}