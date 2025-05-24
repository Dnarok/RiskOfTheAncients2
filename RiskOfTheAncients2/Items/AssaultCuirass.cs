using BepInEx.Configuration;
using RoR2;
using ROTA2.Buffs;
using UnityEngine;
using UnityEngine.Networking;

namespace ROTA2.Items
{
    public class AssaultCuirass : ItemBase<AssaultCuirass>
    {
        public override string ItemName => "Assault Cuirass";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "ASSAULT_CUIRASS";
        public override string ItemTokenPickup => "Your allies attack faster and have additional armor.";
        public override string ItemTokenDesc => $"Increase {Damage("attack speed")} by {Damage($"{AttackSpeedBase}%")} {Stack($"(+{AttackSpeedPerStack}% per stack)")} and {Damage("armor")} by {Damage($"{ArmorBase}")} {Stack($"(+{ArmorPerStack} per stack)")} for {Healing("all allies")}.";
        public override string ItemTokenLore => "Forged in the depths of the nether reaches, this hellish mail provides an army with increased armor and attack speed.";
        public override ItemTier Tier => ItemTier.Tier3;
        public override ItemTag[] ItemTags => [ItemTag.Utility, ItemTag.Damage];
        public override string ItemIconPath => "ROTA2.Icons.assault_cuirass.png";
        public override void Hooks()
        {
            CharacterBody.onBodyInventoryChangedGlobal += OnInventoryChanged;
        }
        public override void Init(ConfigFile configuration)
        {
            CreateConfig(configuration);
            CreateLanguageTokens();
            CreateItemDef();
            Hooks();
        }

        public float AttackSpeedBase;
        public float AttackSpeedPerStack;
        public float ArmorBase;
        public float ArmorPerStack;
        private void CreateConfig(ConfigFile configuration)
        {
            AttackSpeedBase     = configuration.Bind("Item: " + ItemName, "Attack Speed Base", 40.0f, "").Value;
            AttackSpeedPerStack = configuration.Bind("Item: " + ItemName, "Attack Speed Per Stack", 40.0f, "").Value;
            ArmorBase           = configuration.Bind("Item: " + ItemName, "Armor Base", 30.0f, "").Value;
            ArmorPerStack       = configuration.Bind("Item: " + ItemName, "Armor Per Stack", 30.0f, "").Value;
        }

        private void OnInventoryChanged(CharacterBody body)
        {
            if (GetCount(body) > 0 && !body.GetComponent<AssaultCuirassBehavior>())
            {
                body.gameObject.AddComponent<AssaultCuirassBehavior>();
            }
        }

        private class AssaultCuirassBehavior : MonoBehaviour
        {
            CharacterBody body;
            float timer = 0.0f;
            void Awake()
            {
                body = GetComponent<CharacterBody>();
            }
            void FixedUpdate()
            {
                if (!NetworkServer.active || Instance.GetCount(body) <= 0 || (body.healthComponent && !body.healthComponent.alive))
                {
                    return;
                }

                timer += Time.fixedDeltaTime;
                if (timer >= 1.0f)
                {
                    timer -= 1.0f;
                    int count = AssaultCuirass.Instance.GetCount(body);
                    foreach (var member in TeamComponent.GetTeamMembers(body.teamComponent.teamIndex))
                    {
                        CharacterBody ally = member.GetComponent<CharacterBody>();
                        if (ally && ally.isActiveAndEnabled)
                        {
                            AssaultCuirassBuff.Instance.ApplyTo(new BuffBase.ApplyParameters
                            {
                                victim = ally,
                                duration = 1.1f,
                                stacks = count,
                                max_stacks = count
                            });
                        }
                    }
                }
            }
        }
    }
}