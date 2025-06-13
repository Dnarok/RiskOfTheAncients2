using BepInEx.Configuration;
using R2API;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
using RoR2.UI;
using ROTA2.Buffs;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace ROTA2.Equipment
{
    public class Bottle : EquipmentBase<Bottle>
    {
        public override string EquipmentName => "Bottle";
        public override string EquipmentTokenName => "BOTTLE";
        public override string EquipmentTokenPickup => "Drink to gain different temporary rune buffs.";
        public override string EquipmentTokenDesc =>
$@"Drink to gain one of the following rune effects:
    {Utility("Amplify Damage")}: Increases {Damage("damage")} by {Damage($"{AmplifyDamageBonus.Value}%")} for {Utility($"{AmplifyDamageDuration.Value} seconds")}.
    {Color("Arcane", "#FF007F")}: Reduces {Utility("skill cooldowns")} by {Utility($"{ArcaneReduction.Value}%")} for {Utility($"{ArcaneDuration.Value} seconds")}.
    {Color("Bounty", "#D3AF37")}: Gain {Gold($"{BountyGold.Value} gold")}. {Utility("Scales over time")}.
    {Health("Haste")}: Increases {Utility("movement speed")} by {Utility($"{HasteBonus.Value}%")} for {Utility($"{HasteDuration.Value} seconds")}.
    {Color("Illusion", "#FFFF00")}: Creates {Damage($"{IllusionCount.Value} ghosts of yourself")} that last for {Damage($"{IllusionDuration.Value} seconds")}.
    {Color("Invisibility", "#6C3082")}: Gain {Utility("invisibility")} for {Utility($"{InvisibilityDuration.Value} seconds")}.
    {Healing("Regeneration")}: Increases {Healing("base health regeneration")} by {Healing($"{RegenerationMaximumHealthPercentage.Value}% of your maximum health")} per second for {Utility($"{RegenerationDuration.Value} seconds")}.
    {Color("Shield", "#30D5C8")}: Gain a {Healing("temporary barrier")} for {Healing($"{ShieldBarrier.Value}% of your maximum health")}.
    {Color("Water", "#43EBFF")}: Instantly {Healing("heal")} for {Healing($"{WaterHeal.Value}% of your maximum health")} and {Utility($"restore {WaterStocksRestored.Value} stocks of all skills")}.
    {Color("Wisdom", "#6C3BAA")}: Instantly {Utility("gain experience")} equal to {Utility($"{WisdomExperience.Value}% of the current level requirement")}.
Runes will be removed from the pool until all effects have been seen.";
        public override string EquipmentTokenLore => "An old bottle that survived the ages, the contents placed inside become enchanted.";
        public override float EquipmentCooldown => BottleCooldown.Value;
        public override string EquipmentDefGUID => Assets.Bottle.EquipmentDef;
        public override void Hooks()
        {
            On.RoR2.UI.EquipmentIcon.SetDisplayData += ModifyDisplayData;
        }
        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateSounds();
            CreateTextures();
            CreateLanguageTokens();
            CreateEquipmentDef();
            Hooks();
        }

        public ConfigEntry<float> AmplifyDamageBonus;
        public ConfigEntry<float> AmplifyDamageDuration;
        public ConfigEntry<float> ArcaneReduction;
        public ConfigEntry<float> ArcaneDuration;
        public ConfigEntry<int> BountyGold;
        public ConfigEntry<float> HasteBonus;
        public ConfigEntry<float> HasteDuration;
        public ConfigEntry<float> InvisibilityDuration;
        public ConfigEntry<int> IllusionCount;
        public ConfigEntry<float> IllusionDuration;
        public ConfigEntry<float> RegenerationMaximumHealthPercentage;
        public ConfigEntry<float> RegenerationDuration;
        public ConfigEntry<float> ShieldBarrier;
        public ConfigEntry<float> WaterHeal;
        public ConfigEntry<float> WaterStocksRestored;
        public ConfigEntry<float> WisdomExperience;
        public ConfigEntry<float> BottleCooldown;
        private void CreateConfig(ConfigFile config)
        {
            AmplifyDamageBonus = config.Bind("Equipment: " + EquipmentName, "Amplify Damage Bonus Damage", 50.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(AmplifyDamageBonus));
            AmplifyDamageDuration = config.Bind("Equipment: " + EquipmentName, "Amplify Damage Duration", 10.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(AmplifyDamageDuration));
            ArcaneReduction = config.Bind("Equipment: " + EquipmentName, "Arcane Skill Cooldown Reduction", 50.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(ArcaneReduction));
            ArcaneDuration = config.Bind("Equipment: " + EquipmentName, "Arcane Duration", 10.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(ArcaneDuration));
            BountyGold = config.Bind("Equipment: " + EquipmentName, "Bounty Gold", 50, "");
            ModSettingsManager.AddOption(new IntFieldOption(BountyGold));
            HasteBonus = config.Bind("Equipment: " + EquipmentName, "Haste Movement Speed Bonus", 50.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(HasteBonus));
            HasteDuration = config.Bind("Equipment: " + EquipmentName, "Haste Duration", 10.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(HasteDuration));
            InvisibilityDuration = config.Bind("Equipment: " + EquipmentName, "Invisibility Duration", 10.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(InvisibilityDuration));
            IllusionCount = config.Bind("Equipment: " + EquipmentName, "Illusion Count", 2, "");
            ModSettingsManager.AddOption(new IntFieldOption(IllusionCount));
            IllusionDuration = config.Bind("Equipment: " + EquipmentName, "Illusion Duration", 25.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(IllusionDuration));
            RegenerationMaximumHealthPercentage = config.Bind("Equipment: " + EquipmentName, "Regeneration Maximum Health Percentage", 5.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(RegenerationMaximumHealthPercentage));
            RegenerationDuration = config.Bind("Equipment: " + EquipmentName, "Regeneration Duration", 10.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(RegenerationDuration));
            ShieldBarrier = config.Bind("Equipment: " + EquipmentName, "Shield Maximum Health Percentage", 100.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(ShieldBarrier));
            WaterHeal = config.Bind("Equipment: " + EquipmentName, "Water Maximum Health Percentage", 33.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(WaterHeal));
            WaterStocksRestored = config.Bind("Equipment: " + EquipmentName, "Water Stocks Restored", 1.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(WaterStocksRestored));
            WisdomExperience = config.Bind("Equipment: " + EquipmentName, "Wisdom Percent Of Level Requirement", 50.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(WisdomExperience));
            BottleCooldown = config.Bind("Equipment: " + EquipmentName, "Cooldown", 40.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(BottleCooldown));
        }

        NetworkSoundEventDef amplifyDamageSound = null;
        NetworkSoundEventDef arcaneSound = null;
        NetworkSoundEventDef bountySound = null;
        NetworkSoundEventDef hasteSound = null;
        NetworkSoundEventDef illusionSound = null;
        NetworkSoundEventDef invisibilitySound = null;
        NetworkSoundEventDef regenerationSound = null;
        NetworkSoundEventDef shieldSound = null;
        NetworkSoundEventDef waterSound = null;
        NetworkSoundEventDef wisdomSound = null;
        protected void CreateSounds()
        {
            Addressables.LoadAssetAsync<NetworkSoundEventDef>(Assets.Bottle.AmplifyDamage.NetworkSoundEventDef).Completed += (x) => { ContentAddition.AddNetworkSoundEventDef(x.Result); amplifyDamageSound = x.Result; };
            Addressables.LoadAssetAsync<NetworkSoundEventDef>(Assets.Bottle.Arcane.NetworkSoundEventDef).Completed += (x) => { ContentAddition.AddNetworkSoundEventDef(x.Result); arcaneSound = x.Result; };
            Addressables.LoadAssetAsync<NetworkSoundEventDef>(Assets.Bottle.Bounty.NetworkSoundEventDef).Completed += (x) => { ContentAddition.AddNetworkSoundEventDef(x.Result); bountySound = x.Result; };
            Addressables.LoadAssetAsync<NetworkSoundEventDef>(Assets.Bottle.Haste.NetworkSoundEventDef).Completed += (x) => { ContentAddition.AddNetworkSoundEventDef(x.Result); hasteSound = x.Result; };
            Addressables.LoadAssetAsync<NetworkSoundEventDef>(Assets.Bottle.Illusion.NetworkSoundEventDef).Completed += (x) => { ContentAddition.AddNetworkSoundEventDef(x.Result); illusionSound = x.Result; };
            Addressables.LoadAssetAsync<NetworkSoundEventDef>(Assets.Bottle.Invisibility.NetworkSoundEventDef).Completed += (x) => { ContentAddition.AddNetworkSoundEventDef(x.Result); invisibilitySound = x.Result; };
            Addressables.LoadAssetAsync<NetworkSoundEventDef>(Assets.Bottle.Regeneration.NetworkSoundEventDef).Completed += (x) => { ContentAddition.AddNetworkSoundEventDef(x.Result); regenerationSound = x.Result; };
            Addressables.LoadAssetAsync<NetworkSoundEventDef>(Assets.Bottle.Shield.NetworkSoundEventDef).Completed += (x) => { ContentAddition.AddNetworkSoundEventDef(x.Result); shieldSound = x.Result; };
            Addressables.LoadAssetAsync<NetworkSoundEventDef>(Assets.Bottle.Water.NetworkSoundEventDef).Completed += (x) => { ContentAddition.AddNetworkSoundEventDef(x.Result); waterSound = x.Result; };
            Addressables.LoadAssetAsync<NetworkSoundEventDef>(Assets.Bottle.Wisdom.NetworkSoundEventDef).Completed += (x) => { ContentAddition.AddNetworkSoundEventDef(x.Result); wisdomSound = x.Result; };
        }

        Texture2D AmplifyDamageIcon = null;
        Texture2D ArcaneIcon = null;
        Texture2D BountyIcon = null;
        Texture2D HasteIcon = null;
        Texture2D IllusionIcon = null;
        Texture2D InvisibilityIcon = null;
        Texture2D RegenerationIcon = null;
        Texture2D ShieldIcon = null;
        Texture2D WaterIcon = null;
        Texture2D WisdomIcon = null;
        protected void CreateTextures()
        {
            Addressables.LoadAssetAsync<Sprite>(Assets.Bottle.AmplifyDamage.Icon).Completed += (x) => { AmplifyDamageIcon = x.Result.texture; };
            Addressables.LoadAssetAsync<Sprite>(Assets.Bottle.Arcane.Icon).Completed += (x) => { ArcaneIcon = x.Result.texture; };
            Addressables.LoadAssetAsync<Sprite>(Assets.Bottle.Bounty.Icon).Completed += (x) => { BountyIcon = x.Result.texture; };
            Addressables.LoadAssetAsync<Sprite>(Assets.Bottle.Haste.Icon).Completed += (x) => { HasteIcon = x.Result.texture; };
            Addressables.LoadAssetAsync<Sprite>(Assets.Bottle.Illusion.Icon).Completed += (x) => { IllusionIcon = x.Result.texture; };
            Addressables.LoadAssetAsync<Sprite>(Assets.Bottle.Invisibility.Icon).Completed += (x) => { InvisibilityIcon = x.Result.texture; };
            Addressables.LoadAssetAsync<Sprite>(Assets.Bottle.Regeneration.Icon).Completed += (x) => { RegenerationIcon = x.Result.texture; };
            Addressables.LoadAssetAsync<Sprite>(Assets.Bottle.Shield.Icon).Completed += (x) => { ShieldIcon = x.Result.texture; };
            Addressables.LoadAssetAsync<Sprite>(Assets.Bottle.Water.Icon).Completed += (x) => { WaterIcon = x.Result.texture; };
            Addressables.LoadAssetAsync<Sprite>(Assets.Bottle.Wisdom.Icon).Completed += (x) => { WisdomIcon = x.Result.texture; };
        }
        private void ModifyDisplayData(On.RoR2.UI.EquipmentIcon.orig_SetDisplayData orig, EquipmentIcon self, EquipmentIcon.DisplayData data)
        {
            orig(self, data);
            if (self && self.targetEquipmentSlot && self.targetEquipmentSlot.characterBody && self.currentDisplayData.equipmentDef == EquipmentDef)
            {
                BottleBehavior behavior = self.targetEquipmentSlot.characterBody.GetComponent<BottleBehavior>();
                if (!behavior)
                {
                    behavior = self.targetEquipmentSlot.characterBody.gameObject.AddComponent<BottleBehavior>();
                }
                if (behavior)
                {
                    Texture new_texture = null;
                    switch (behavior.PeekNext())
                    {
                        // amplify damage
                        case 0:
                            new_texture = AmplifyDamageIcon;
                            break;
                        // arcane
                        case 1:
                            new_texture = ArcaneIcon;
                            break;
                        // bounty
                        case 2:
                            new_texture = BountyIcon;
                            break;
                        // haste
                        case 3:
                            new_texture = HasteIcon;
                            break;
                        // invisibility
                        case 4:
                            new_texture = InvisibilityIcon;
                            break;
                        // illusion
                        case 5:
                            new_texture = IllusionIcon;
                            break;
                        // regeneration
                        case 6:
                            new_texture = RegenerationIcon;
                            break;
                        // shield
                        case 7:
                            new_texture = ShieldIcon;
                            break;
                        // water
                        case 8:
                            new_texture = WaterIcon;
                            break;
                        // wisdom
                        case 9:
                            new_texture = WisdomIcon;
                            break;
                    }

                    if (new_texture)
                    {
                        self.iconImage.texture = new_texture;
                    }
                }
            }
        }

        protected override bool ActivateEquipment(EquipmentSlot slot)
        {
            var body = slot.characterBody;
            if (HasThisEquipment(body))
            {
                var behavior = body.GetComponent<BottleBehavior>();
                if (!behavior)
                {
                    behavior = body.gameObject.AddComponent<BottleBehavior>();
                }

                int rune = behavior.NextRune();
                switch (rune)
                {
                    // amplify damage
                    case 0:
                        DoAmplifyDamage(body);
                        break;
                    // arcane
                    case 1:
                        DoArcane(body);
                        break;
                    // bounty
                    case 2:
                        DoBounty(body);
                        break;
                    // haste
                    case 3:
                        DoHaste(body);
                        break;
                    // invisibility
                    case 4:
                        DoInvisibility(body);
                        break;
                    // illusion
                    case 5:
                        DoIllusion(body);
                        break;
                    // regeneration
                    case 6:
                        DoRegeneration(body);
                        break;
                    // shield
                    case 7:
                        DoShield(body);
                        break;
                    // water
                    case 8:
                        DoWater(body);
                        break;
                    // wisdom
                    case 9:
                        DoWisdom(body);
                        break;
                }
            }

            return true;
        }

        void DoAmplifyDamage(CharacterBody body)
        {
            AmplifyDamageBuff.ApplyTo(
                body: body,
                duration: AmplifyDamageDuration.Value
            );

            EffectManager.SimpleSoundEffect(amplifyDamageSound.index, body.corePosition, true);
        }
        void DoArcane(CharacterBody body)
        {
            ArcaneBuff.ApplyTo(
                body: body,
                duration: ArcaneDuration.Value
            );

            EffectManager.SimpleSoundEffect(arcaneSound.index, body.corePosition, true);
        }
        void DoBounty(CharacterBody body)
        {
            float cost = Run.instance.GetDifficultyScaledCost(BountyGold.Value, Run.instance.difficultyCoefficient);
            body.master.GiveMoney((uint)cost);

            EffectManager.SimpleSoundEffect(bountySound.index, body.corePosition, true);
        }
        void DoHaste(CharacterBody body)
        {
            HasteBuff.ApplyTo(
                body: body,
                duration: HasteDuration.Value
            );

            EffectManager.SimpleSoundEffect(hasteSound.index, body.corePosition, true);
        }
        void DoInvisibility(CharacterBody body)
        {
            body.AddTimedBuffAuthority(RoR2Content.Buffs.Cloak.buffIndex, InvisibilityDuration.Value);

            EffectManager.SimpleSoundEffect(invisibilitySound.index, body.corePosition, true);
        }
        void DoIllusion(CharacterBody body)
        {
            if (body.inventory.GetItemCount(RoR2Content.Items.Ghost) <= 0)
            {
                for (int i = 0; i < IllusionCount.Value; ++i)
                {
                    SpawnIllusion(body);
                }

                EffectManager.SimpleSoundEffect(illusionSound.index, body.corePosition, true);
            }
        }
        void DoRegeneration(CharacterBody body)
        {
            RegenerationBuff.ApplyTo(
                body: body,
                duration: RegenerationDuration.Value
            );

            EffectManager.SimpleSoundEffect(regenerationSound.index, body.corePosition, true);
        }
        void DoShield(CharacterBody body)
        {
            if (body.healthComponent)
            {
                body.healthComponent.AddBarrier(body.healthComponent.fullCombinedHealth * ShieldBarrier.Value / 100.0f);
            }

            EffectManager.SimpleSoundEffect(shieldSound.index, body.corePosition, true);
        }
        void DoWater(CharacterBody body)
        {
            if (body.healthComponent)
            {
                body.healthComponent.Heal(body.healthComponent.fullCombinedHealth * WaterHeal.Value / 100.0f, default);
            }
            var skills = body.skillLocator.allSkills;
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

            EffectManager.SimpleSoundEffect(waterSound.index, body.corePosition, true);
        }
        void DoWisdom(CharacterBody body)
        {
            ulong required_next = TeamManager.GetExperienceForLevel(TeamManager.instance.GetTeamLevel(body.teamComponent.teamIndex) + 1);
            ulong required_now = TeamManager.instance.GetTeamCurrentLevelExperience(body.teamComponent.teamIndex);
            ExperienceManager.instance.AwardExperience(body.transform.position, body, (ulong)((required_next - required_now) * WisdomExperience.Value / 100.0f));

            EffectManager.SimpleSoundEffect(wisdomSound.index, body.corePosition, true);
        }

        private class BottleBehavior : MonoBehaviour
        {
            // ad arc b h inv ill r s wat wis
            public List<int> runes = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];
            public CharacterBody body;

            public void Awake()
            {
                body = GetComponent<CharacterBody>();
                Util.ShuffleList(runes);
            }
            public int NextRune()
            {
                if (runes.Count == 0)
                {
                    runes = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];
                    Util.ShuffleList(runes);
                }

                int result = runes[^1];
                runes.RemoveAt(runes.Count - 1);
                return result;
            }
            public int PeekNext()
            {
                if (runes.Count == 0)
                {
                    runes = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];
                    Util.ShuffleList(runes);
                }

                return runes[^1];
            }
        }

        private static GameObject teleportHelperPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/DirectorSpawnProbeHelperPrefab.prefab").WaitForCompletion();
        private void SpawnIllusion(CharacterBody body)
        {
            if (!NetworkServer.active)
            {
                return;
            }

            if (!body)
            {
                return;
            }

            GameObject prefab = BodyCatalog.FindBodyPrefab(body);
            if (!prefab)
            {
                return;
            }

            CharacterMaster master = MasterCatalog.allAiMasters.FirstOrDefault((CharacterMaster master) => master.bodyPrefab == prefab);
            if (!master)
            {
                return;
            }

            CharacterDirection direction = body.GetComponent<CharacterDirection>();
            MasterSummon summon = new()
            {
                masterPrefab = master.gameObject,
                ignoreTeamMemberLimit = true,
                rotation = direction ? Quaternion.Euler(0f, direction.yaw, 0f) : body.transform.rotation,
                summonerBodyObject = body.gameObject,
                inventoryToCopy = body.inventory,
                useAmbientLevel = null
            };

            {
                Vector3 position = body.corePosition;
                SpawnCard card = ScriptableObject.CreateInstance<SpawnCard>();
                card.hullSize = body.hullClassification;
                card.nodeGraphType = RoR2.Navigation.MapNodeGroup.GraphType.Ground;
                card.prefab = teleportHelperPrefab;
                float minimum = 4f;
                float maximum = 15f;

                GameObject gameObject = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(card, new DirectorPlacementRule
                {
                    placementMode = DirectorPlacementRule.PlacementMode.Approximate,
                    position = position,
                    minDistance = minimum,
                    maxDistance = maximum
                }, RoR2Application.rng));
                if (gameObject)
                {
                    position = gameObject.transform.position;
                    UnityEngine.Object.Destroy(gameObject);
                }
                UnityEngine.Object.Destroy(card);

                summon.position = position;
            }

            CharacterMaster master1 = summon.Perform();
            if (!master1)
            {
                return;
            }

            Inventory inventory = master1.inventory;
            if (inventory)
            {
                if (inventory.GetItemCount(RoR2Content.Items.Ghost) <= 0)
                {
                    inventory.GiveItem(RoR2Content.Items.Ghost);
                }

                MasterSuicideOnTimer suicide = master1.gameObject.AddComponent<MasterSuicideOnTimer>();
                suicide.lifeTimer = IllusionDuration.Value;
            }

            CharacterBody body1 = master1.GetBody();
            if (body1)
            {
                foreach (EntityStateMachine machine in body.GetComponents<EntityStateMachine>())
                {
                    machine.initialStateType = machine.mainStateType;
                }
            }
        }
    }
}