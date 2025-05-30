﻿using BepInEx.Configuration;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using ROTA2.Buffs;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using System.Linq;
using RoR2.UI;
using MonoMod.RuntimeDetour;
using System;

namespace ROTA2.Equipment
{
    public class Bottle : EquipmentBase<Bottle>
    {
        public override string EquipmentName => "Bottle";
        public override string EquipmentTokenName => "BOTTLE";
        public override string EquipmentTokenPickup => "Drink to gain different temporary rune buffs.";
        public override string EquipmentTokenDesc =>
$@"Drink to gain one of the following rune effects:
    {Utility("Amplify Damage")}: Increases {Damage("damage")} by {Damage($"{AmplifyDamageBonus}%")} for {Utility($"{AmplifyDamageDuration} seconds")}.
    {Color("Arcane", "#FF007F")}: Reduces {Utility("skill cooldowns")} by {Utility($"{ArcaneReduction}%")} for {Utility($"{ArcaneDuration} seconds")}.
    {Color("Bounty", "#D3AF37")}: Gain {Gold($"{BountyGold} gold")}. {Utility("Scales over time")}.
    {Health("Haste")}: Increases {Utility("movement speed")} by {Utility($"{HasteBonus}%")} for {Utility($"{HasteDuration} seconds")}.
    {Color("Illusion", "#FFFF00")}: Creates {Damage($"{IllusionCount} ghosts of yourself")} that last for {Damage($"{IllusionDuration} seconds")}.
    {Color("Invisibility", "#6C3082")}: Gain {Utility("invisibility")} for {Utility($"{InvisibilityDuration} seconds")}.
    {Healing("Regeneration")}: Increases {Healing("base health regeneration")} by {Healing($"{RegenerationMaximumHealthPercentage}% of your maximum health")} per second for {Utility($"{RegenerationDuration} seconds")}.
    {Color("Shield", "#30D5C8")}: Gain a {Healing("temporary barrier")} for {Healing($"{ShieldBarrier}% of your maximum health")}.
    {Color("Water", "#43EBFF")}: Instantly {Healing("heal")} for {Healing($"{WaterHeal}% of your maximum health")} and {Utility($"restore {WaterStocksRestored} stocks of all skills")}.
    {Color("Wisdom", "#6C3BAA")}: Instantly {Utility("gain experience")} equal to {Utility($"{WisdomExperience}% of the current level requirement")}.
Runes will be removed from the pool until all effects have been seen.";
        public override string EquipmentTokenLore => "An old bottle that survived the ages, the contents placed inside become enchanted.";
        public override float EquipmentCooldown => BottleCooldown;
        public override string EquipmentIconPath => "ROTA2.Icons.bottle.png";
        public override string EquipmentModelPath => "bottle.prefab";
        public override void Hooks()
        {
            var target = typeof(EquipmentIcon).GetMethod(nameof(EquipmentIcon.SetDisplayData), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var destination = typeof(Bottle).GetMethod(nameof(ModifyDisplayData), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            new Hook(target, destination, this);
        }

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLanguageTokens();
            CreateEquipmentDef();
            Hooks();
        }

        public float AmplifyDamageBonus;
        public float AmplifyDamageDuration;
        public float ArcaneReduction;
        public float ArcaneDuration;
        public int BountyGold;
        public float HasteBonus;
        public float HasteDuration;
        public float InvisibilityDuration;
        public int IllusionCount;
        public float IllusionDuration;
        public float RegenerationMaximumHealthPercentage;
        public float RegenerationDuration;
        public float ShieldBarrier;
        public float WaterHeal;
        public float WaterStocksRestored;
        public float WisdomExperience;
        public float BottleCooldown;
        private void CreateConfig(ConfigFile config)
        {
            AmplifyDamageBonus                  = config.Bind("Equipment: " + EquipmentName, "Amplify Damage Bonus Damage",             50.0f,  "").Value;
            AmplifyDamageDuration               = config.Bind("Equipment: " + EquipmentName, "Amplify Damage Duration",                 10.0f,  "").Value;
            ArcaneReduction                     = config.Bind("Equipment: " + EquipmentName, "Arcane Skill Cooldown Reduction",         50.0f,  "").Value;
            ArcaneDuration                      = config.Bind("Equipment: " + EquipmentName, "Arcane Duration",                         10.0f,  "").Value;
            BountyGold                          = config.Bind("Equipment: " + EquipmentName, "Bounty Gold",                             50,     "").Value;
            HasteBonus                          = config.Bind("Equipment: " + EquipmentName, "Haste Movement Speed Bonus",              50.0f,  "").Value;
            HasteDuration                       = config.Bind("Equipment: " + EquipmentName, "Haste Duration",                          10.0f,  "").Value;
            InvisibilityDuration                = config.Bind("Equipment: " + EquipmentName, "Invisibility Duration",                   10.0f,  "").Value;
            IllusionCount                       = config.Bind("Equipment: " + EquipmentName, "Illusion Count",                          2,      "").Value;
            IllusionDuration                    = config.Bind("Equipment: " + EquipmentName, "Illusion Duration",                       25.0f,  "").Value;
            RegenerationMaximumHealthPercentage = config.Bind("Equipment: " + EquipmentName, "Regeneration Maximum Health Percentage",  5.0f,   "").Value;
            RegenerationDuration                = config.Bind("Equipment: " + EquipmentName, "Regeneration Duration",                   10.0f,  "").Value;
            ShieldBarrier                       = config.Bind("Equipment: " + EquipmentName, "Shield Maximum Health Percentage",        100.0f, "").Value;
            WaterHeal                           = config.Bind("Equipment: " + EquipmentName, "Water Maximum Health Percentage",         33.0f,  "").Value;
            WaterStocksRestored                 = config.Bind("Equipment: " + EquipmentName, "Water Stocks Restored",                   1.0f,   "").Value;
            WisdomExperience                    = config.Bind("Equipment: " + EquipmentName, "Wisdom Percent Of Level Requirement",     50.0f,  "").Value;
            BottleCooldown                      = config.Bind("Equipment: " + EquipmentName, "Cooldown",                                40.0f,  "").Value;
        }

        private string AmplifyDamageIconPath    = "ROTA2.Icons.bottle_amplify_damage.png";
        private string ArcaneIconPath           = "ROTA2.Icons.bottle_arcane.png";
        private string BountyIconPath           = "ROTA2.Icons.bottle_bounty.png";
        private string HasteIconPath            = "ROTA2.Icons.bottle_haste.png";
        private string IllusionIconPath         = "ROTA2.Icons.bottle_illusion.png";
        private string InvisibilityIconPath     = "ROTA2.Icons.bottle_invisibility.png";
        private string RegenerationIconPath     = "ROTA2.Icons.bottle_regeneration.png";
        private string ShieldIconPath           = "ROTA2.Icons.bottle_shield.png";
        private string WaterIconPath            = "ROTA2.Icons.bottle_water.png";
        private string WisdomIconPath           = "ROTA2.Icons.bottle_wisdom.png";
        private void ModifyDisplayData(Action<EquipmentIcon, EquipmentIcon.DisplayData> orig, EquipmentIcon self, EquipmentIcon.DisplayData data)
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
                            new_texture = Plugin.ExtractSprite(AmplifyDamageIconPath).texture;
                            break;
                        // arcane
                        case 1:
                            new_texture = Plugin.ExtractSprite(ArcaneIconPath).texture;
                            break;
                        // bounty
                        case 2:
                            new_texture = Plugin.ExtractSprite(BountyIconPath).texture;
                            break;
                        // haste
                        case 3:
                            new_texture = Plugin.ExtractSprite(HasteIconPath).texture;
                            break;
                        // invisibility
                        case 4:
                            new_texture = Plugin.ExtractSprite(InvisibilityIconPath).texture;
                            break;
                        // illusion
                        case 5:
                            new_texture = Plugin.ExtractSprite(IllusionIconPath).texture;
                            break;
                        // regeneration
                        case 6:
                            new_texture = Plugin.ExtractSprite(RegenerationIconPath).texture;
                            break;
                        // shield
                        case 7:
                            new_texture = Plugin.ExtractSprite(ShieldIconPath).texture;
                            break;
                        // water
                        case 8:
                            new_texture = Plugin.ExtractSprite(WaterIconPath).texture;
                            break;
                        // wisdom
                        case 9:
                            new_texture = Plugin.ExtractSprite(WisdomIconPath).texture;
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
            AmplifyDamageBuff.Instance.ApplyTo(new BuffBase.ApplyParameters
            {
                victim = body,
                duration = AmplifyDamageDuration
            });

            Util.PlaySound("AmplifyDamage", body.gameObject);
        }
        void DoArcane(CharacterBody body)
        {
            ArcaneBuff.Instance.ApplyTo(new BuffBase.ApplyParameters
            {
                victim = body,
                duration = ArcaneDuration
            });

            Util.PlaySound("Arcane", body.gameObject);
        }
        void DoBounty(CharacterBody body)
        {
            float cost = Run.instance.GetDifficultyScaledCost(BountyGold, Run.instance.difficultyCoefficient);
            body.master.GiveMoney((uint)cost);

            Util.PlaySound("Bounty", body.gameObject);
        }
        void DoHaste(CharacterBody body)
        {
            HasteBuff.Instance.ApplyTo(new BuffBase.ApplyParameters
            {
                victim = body,
                duration = HasteDuration
            });

            Util.PlaySound("Haste", body.gameObject);
        }
        void DoInvisibility(CharacterBody body)
        {
            body.AddTimedBuffAuthority(RoR2Content.Buffs.Cloak.buffIndex, InvisibilityDuration);

            Util.PlaySound("Invisibility", body.gameObject);
        }
        void DoIllusion(CharacterBody body)
        {
            if (body.inventory.GetItemCount(RoR2Content.Items.Ghost) <= 0)
            {
                for (int i = 0; i < IllusionCount; ++i)
                {
                    SpawnIllusion(body);
                }

                Util.PlaySound("Illusion", body.gameObject);
            }
        }
        void DoRegeneration(CharacterBody body)
        {
            RegenerationBuff.Instance.ApplyTo(new BuffBase.ApplyParameters
            {
                victim = body,
                duration = RegenerationDuration
            });

            Util.PlaySound("Regeneration", body.gameObject);
        }
        void DoShield(CharacterBody body)
        {
            if (body.healthComponent)
            {
                body.healthComponent.AddBarrier(body.healthComponent.fullCombinedHealth * ShieldBarrier / 100.0f);
            }

            Util.PlaySound("Shield", body.gameObject);
        }
        void DoWater(CharacterBody body)
        {
            if (body.healthComponent)
            {
                body.healthComponent.Heal(body.healthComponent.fullCombinedHealth * WaterHeal / 100.0f, default);
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

            Util.PlaySound("Water", body.gameObject);
        }
        void DoWisdom(CharacterBody body)
        {
            ulong required_next = TeamManager.GetExperienceForLevel(TeamManager.instance.GetTeamLevel(body.teamComponent.teamIndex) + 1);
            ulong required_now = TeamManager.instance.GetTeamCurrentLevelExperience(body.teamComponent.teamIndex);
            ExperienceManager.instance.AwardExperience(body.transform.position, body, (ulong)((required_next - required_now) * WisdomExperience / 100.0f));

            Util.PlaySound("Wisdom", body.gameObject);
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
                suicide.lifeTimer = IllusionDuration;
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