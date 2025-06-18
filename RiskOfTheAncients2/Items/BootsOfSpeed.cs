using BepInEx.Configuration;
using R2API;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ROTA2.Items
{
    public class BootsOfSpeed : ItemBase<BootsOfSpeed>
    {
        public override string ItemName => "Boots of Speed";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "BOOTS_OF_SPEED";
        public override string ItemTokenPickup => "Slightly increases base movement speed.";
        public override string ItemTokenDesc => $"Increases {Utility("base movement speed")} by {Utility($"{MovementSpeedBase.Value}")} {Stack($"(+{MovementSpeedPerStack.Value} per stack)")}.";
        public override string ItemTokenLore => "Fleet footwear, increasing movement.";
        public override string ItemDefGUID => Assets.BootsOfSpeed.ItemDef;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddMovementSpeed;
        }
        public override void Init(ConfigFile configuration)
        {
            CreateConfig(configuration);
            CreateLanguageTokens();
            CreateItemDef();
            Hooks();
        }
        protected override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            var prefabL = Addressables.LoadAssetAsync<GameObject>(Assets.BootsOfSpeed.DisplayL).WaitForCompletion();
            var prefabR = Addressables.LoadAssetAsync<GameObject>(Assets.BootsOfSpeed.DisplayR).WaitForCompletion();
            ItemDisplayRuleDict rules = new();
            rules.Add("mdlCommandoDualies",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefabL,
                    childName = "CalfL",
                    localPos = new Vector3(0.00837F, 0.41651F, -0.03094F),
                    localAngles = new Vector3(1.96493F, 97.75288F, 171.1686F),
                    localScale = new Vector3(0.25112F, 0.25221F, 0.25617F)

                },
                new()
                {
                    ruleType = default,
                    followerPrefab = prefabR,
                    childName = "CalfR",
                    localPos = new Vector3(-0.01933F, 0.41689F, -0.024F),
                    localAngles = new Vector3(1.96493F, 97.75287F, 171.1686F),
                    localScale = new Vector3(0.25112F, 0.25221F, 0.25617F)
                }
            ]);
            rules.Add("mdlHuntress",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefabL,
                    childName = "CalfL",
                    localPos = new Vector3(-0.01728F, 0.52116F, -0.02965F),
                    localAngles = new Vector3(1.73936F, 111.1081F, 171.4584F),
                    localScale = new Vector3(0.25112F, 0.25221F, 0.25617F)

                },
                new()
                {
                    ruleType = default,
                    followerPrefab = prefabR,
                    childName = "CalfR",
                    localPos = new Vector3(0.02037F, 0.50538F, -0.04194F),
                    localAngles = new Vector3(353.889F, 57.96633F, 175.0929F),
                    localScale = new Vector3(0.25112F, 0.25221F, 0.25617F)
                }
            ]);
            rules.Add("mdlToolbot",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefabL,
                    childName = "ExtraCalfL",
                    localPos = new Vector3(0.03432F, 2.93575F, -0.42975F),
                    localAngles = new Vector3(357.1212F, 98.83696F, 173.5472F),
                    localScale = new Vector3(2F, 2F, 2F)
                },
                new()
                {
                    ruleType = default,
                    followerPrefab = prefabR,
                    childName = "ExtraCalfR",
                    localPos = new Vector3(0.03F, 2.93F, -0.4F),
                    localAngles = new Vector3(2.06686F, 79.94244F, 174.6651F),
                    localScale = new Vector3(2F, 2F, 2F)
                }
            ]);
            rules.Add("mdlEngi",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefabL,
                    childName = "CalfL",
                    localPos = new Vector3(0.02067F, 0.31017F, -0.01674F),
                    localAngles = new Vector3(3.96606F, 104.5332F, 170.5488F),
                    localScale = new Vector3(0.31403F, 0.35458F, 0.49237F)
                },
                new()
                {
                    ruleType = default,
                    followerPrefab = prefabR,
                    childName = "CalfR",
                    localPos = new Vector3(-0.00905F, 0.31566F, 0.00298F),
                    localAngles = new Vector3(354.1474F, 68.63204F, 174.3465F),
                    localScale = new Vector3(0.314F, 0.354F, 0.492F)
                }
            ]);
            rules.Add("mdlEngiTurret",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefabL,
                    childName = "Base",
                    localPos = new Vector3(0f, 0f, 0f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = new Vector3(0f, 0f, 0f),
                },
                new()
                {
                    ruleType = default,
                    followerPrefab = prefabR,
                    childName = "Base",
                    localPos = new Vector3(0f, 0f, 0f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = new Vector3(0f, 0f, 0f),
                },
                new()
                {
                    ruleType = default,
                    followerPrefab = prefabL,
                    childName = "Base",
                    localPos = new Vector3(0f, 0f, 0f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = new Vector3(0f, 0f, 0f),
                }
            ]);
            rules.Add("mdlEngiWalkerTurret",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefabL,
                    childName = "Base",
                    localPos = new Vector3(0f, 0f, 0f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = new Vector3(0f, 0f, 0f),
                },
                new()
                {
                    ruleType = default,
                    followerPrefab = prefabR,
                    childName = "Base",
                    localPos = new Vector3(0f, 0f, 0f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = new Vector3(0f, 0f, 0f),
                },
                new()
                {
                    ruleType = default,
                    followerPrefab = prefabL,
                    childName = "Base",
                    localPos = new Vector3(0f, 0f, 0f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = new Vector3(0f, 0f, 0f),
                }
            ]);
            rules.Add("mdlMage",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefabL,
                    childName = "CalfL",
                    localPos = new Vector3(-0.01027F, 0.51617F, -0.01628F),
                    localAngles = new Vector3(351.9848F, 106.1374F, 177.5483F),
                    localScale = new Vector3(0.25112F, 0.25221F, 0.25617F)
                },
                new()
                {
                    ruleType = default,
                    followerPrefab = prefabR,
                    childName = "FootR",
                    localPos = new Vector3(-0.02144F, 0.04171F, -0.00649F),
                    localAngles = new Vector3(356.4482F, 96.6318F, 210.4114F),
                    localScale = new Vector3(0.25112F, 0.25221F, 0.25617F)
                }
            ]);
            rules.Add("mdlMerc",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefabL,
                    childName = "CalfL",
                    localPos = new Vector3(0.00652F, 0.39558F, -0.02215F),
                    localAngles = new Vector3(2.39462F, 97.92603F, 167.9142F),
                    localScale = new Vector3(0.26006F, 0.26119F, 0.26529F)
                },
                new()
                {
                    ruleType = default,
                    followerPrefab = prefabR,
                    childName = "CalfR",
                    localPos = new Vector3(-0.00507F, 0.39663F, -0.05856F),
                    localAngles = new Vector3(359.7427F, 75.89891F, 158.6672F),
                    localScale = new Vector3(0.26F, 0.26F, 0.265F)
                }
            ]);
            rules.Add("mdlTreebot",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefabL,
                    childName = "FootBackLEnd",
                    localPos = new Vector3(-0.00087F, -0.04162F, -0.02922F),
                    localAngles = new Vector3(1.96493F, 97.75288F, 171.1686F),
                    localScale = new Vector3(0.25112F, 0.25221F, 0.25617F)
                },
                new()
                {
                    ruleType = default,
                    followerPrefab = prefabL,
                    childName = "FootFrontLEnd",
                    localPos = new Vector3(0.01381F, -0.03949F, -0.03488F),
                    localAngles = new Vector3(1.96493F, 97.75288F, 171.1686F),
                    localScale = new Vector3(0.25112F, 0.25221F, 0.25617F)
                },
                new()
                {
                    ruleType = default,
                    followerPrefab = prefabR,
                    childName = "FootBackREnd",
                    localPos = new Vector3(-0.01933F, -0.03F, -0.024F),
                    localAngles = new Vector3(1.96493F, 97.75287F, 171.1686F),
                    localScale = new Vector3(0.25112F, 0.25221F, 0.25617F)
                },
                new()
                {
                    ruleType = default,
                    followerPrefab = prefabR,
                    childName = "FootFrontREnd",
                    localPos = new Vector3(-0.01933F, -0.03F, -0.024F),
                    localAngles = new Vector3(1.96493F, 97.75287F, 171.1686F),
                    localScale = new Vector3(0.25112F, 0.25221F, 0.25617F)
                }
            ]);
            rules.Add("mdlLoader",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefabL,
                    childName = "CalfL",
                    localPos = new Vector3(-0.00133F, 0.52593F, -0.05163F),
                    localAngles = new Vector3(356.2478F, 104.1765F, 167.9954F),
                    localScale = new Vector3(0.28562F, 0.28686F, 0.29137F)
                },
                new()
                {
                    ruleType = default,
                    followerPrefab = prefabR,
                    childName = "CalfR",
                    localPos = new Vector3(0.01278F, 0.52474F, -0.03142F),
                    localAngles = new Vector3(1.56704F, 80.89141F, 168.5808F),
                    localScale = new Vector3(0.285F, 0.285F, 0.29F)
                }
            ]);
            rules.Add("mdlCroco",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefabL,
                    childName = "Head",
                    localPos = new Vector3(0.00596F, 1.40418F, 2.56174F),
                    localAngles = new Vector3(351.2359F, 90.58022F, 276.6091F),
                    localScale = new Vector3(2.83673F, 2.83673F, 2.83673F)
                }
            ]);
            rules.Add("mdlCaptain",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefabL,
                    childName = "CalfL",
                    localPos = new Vector3(-0.00066F, 0.46759F, -0.03169F),
                    localAngles = new Vector3(4.55247F, 113.4016F, 187.7969F),
                    localScale = new Vector3(0.27198F, 0.27316F, 0.27745F)
                },
                new()
                {
                    ruleType = default,
                    followerPrefab = prefabR,
                    childName = "CalfR",
                    localPos = new Vector3(0.00093F, 0.47128F, -0.05297F),
                    localAngles = new Vector3(7.12267F, 74.13599F, 174.8727F),
                    localScale = new Vector3(0.27F, 0.27F, 0.27F)
                }
            ]);
            rules.Add("mdlBandit2",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefabL,
                    childName = "CalfL",
                    localPos = new Vector3(0.01961F, 0.48245F, -0.03734F),
                    localAngles = new Vector3(0.84245F, 110.3629F, 171.7344F),
                    localScale = new Vector3(0.29F, 0.29F, 0.29F)
                },
                new()
                {
                    ruleType = default,
                    followerPrefab = prefabR,
                    childName = "CalfR",
                    localPos = new Vector3(0.00634F, 0.44193F, -0.05449F),
                    localAngles = new Vector3(355.5555F, 72.46548F, 174.0455F),
                    localScale = new Vector3(0.29633F, 0.29633F, 0.29633F)
                }
            ]);
            rules.Add("mdlChef",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefabL,
                    childName = "Wheel",
                    localPos = new Vector3(-0.7291F, -0.02883F, 0.00085F),
                    localAngles = new Vector3(356.5978F, 185.5335F, 88.63451F),
                    localScale = new Vector3(0.25112F, 0.25221F, 0.25617F)
                },
                new()
                {
                    ruleType = default,
                    followerPrefab = prefabR,
                    childName = "Wheel",
                    localPos = new Vector3(0.72078F, -0.03413F, -0.00672F),
                    localAngles = new Vector3(2.54936F, 0.88284F, 87.0245F),
                    localScale = new Vector3(0.25112F, 0.25221F, 0.25617F)
                }
            ]);
            rules.Add("mdlRailGunner",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefabL,
                    childName = "FootL",
                    localPos = new Vector3(0F, 0.20731F, -0.28581F),
                    localAngles = new Vector3(359.8374F, 89.82021F, 93.16787F),
                    localScale = new Vector3(0.25F, 0.25F, 0.25F)
                },
                new()
                {
                    ruleType = default,
                    followerPrefab = prefabR,
                    childName = "FootR",
                    localPos = new Vector3(0.0199F, 0.16109F, -0.28418F),
                    localAngles = new Vector3(355.2117F, 76.25698F, 74.35645F),
                    localScale = new Vector3(0.25F, 0.25F, 0.25F)
                }
            ]);
            rules.Add("mdlVoidSurvivor",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefabL,
                    childName = "CalfL",
                    localPos = new Vector3(0.03696F, 0.47852F, -0.01563F),
                    localAngles = new Vector3(6.98862F, 26.58779F, 184.4657F),
                    localScale = new Vector3(0.25112F, 0.25221F, 0.25617F)
                },
                new()
                {
                    ruleType = default,
                    followerPrefab = prefabR,
                    childName = "CalfR",
                    localPos = new Vector3(-0.05866F, 0.4596F, 0.00571F),
                    localAngles = new Vector3(5.8545F, 174.0633F, 167.8919F),
                    localScale = new Vector3(0.25112F, 0.25221F, 0.25617F)
                }
            ]);
            rules.Add("mdlSeeker",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefabL,
                    childName = "CalfL",
                    localPos = new Vector3(0.01572F, 0.51297F, -0.01407F),
                    localAngles = new Vector3(358.6348F, 4.38497F, 173.2584F),
                    localScale = new Vector3(0.25112F, 0.25221F, 0.25617F)
                },
                new()
                {
                    ruleType = default,
                    followerPrefab = prefabR,
                    childName = "CalfR",
                    localPos = new Vector3(0.04749F, 0.51379F, -0.00294F),
                    localAngles = new Vector3(352.3821F, 342.6278F, 170.6911F),
                    localScale = new Vector3(0.25112F, 0.25221F, 0.25617F)
                }
            ]);
            rules.Add("mdlFalseSon",
            [
                new()
                {
                    ruleType = default,
                    followerPrefab = prefabL,
                    childName = "ClubExplosionPoint",
                    localPos = new Vector3(-0.03661F, 0.40164F, -0.49127F),
                    localAngles = new Vector3(12.71907F, 118.8509F, 131.6088F),
                    localScale = new Vector3(0.25112F, 0.25221F, 0.25617F)
                },
                new()
                {
                    ruleType = default,
                    followerPrefab = prefabR,
                    childName = "ClubExplosionPoint",
                    localPos = new Vector3(0.01463F, -0.57003F, -0.49865F),
                    localAngles = new Vector3(342.1849F, 93.99335F, 38.93793F),
                    localScale = new Vector3(0.25112F, 0.25221F, 0.25617F)
                }
            ]);
            return rules;
        }

        public ConfigEntry<float> MovementSpeedBase;
        public ConfigEntry<float> MovementSpeedPerStack;
        public void CreateConfig(ConfigFile configuration)
        {
            MovementSpeedBase = configuration.Bind("Item: " + ItemName, "Initial Base Movement Speed Bonus", 0.7f, "How much base movement speed should be provided by the first stack?");
            ModSettingsManager.AddOption(new FloatFieldOption(MovementSpeedBase));
            MovementSpeedPerStack = configuration.Bind("Item: " + ItemName, "Stacking Base Movement Speed Bonus", 0.7f, "How much base movement speed should be provided by subsequent stacks?");
            ModSettingsManager.AddOption(new FloatFieldOption(MovementSpeedPerStack));
        }

        private void AddMovementSpeed(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs arguments)
        {
            int count = GetCount(body);
            if (count > 0)
            {
                arguments.baseMoveSpeedAdd += MovementSpeedBase.Value + MovementSpeedPerStack.Value * (count - 1);
            }
        }
    }
}