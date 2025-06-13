using BepInEx.Configuration;
using RiskOfOptions;
using RiskOfOptions.Options;
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
        public override string ItemTokenDesc => $"Increase {Damage("attack speed")} by {Damage($"{AttackSpeedBase.Value}%")} {Stack($"(+{AttackSpeedPerStack.Value}% per stack)")} and {Damage("armor")} by {Damage($"{ArmorBase.Value}")} {Stack($"(+{ArmorPerStack.Value} per stack)")} for {Healing("all allies")}.";
        public override string ItemTokenLore => "Forged in the depths of the nether reaches, this hellish mail provides an army with increased armor and attack speed.";
        public override string ItemDefGUID => Assets.AssaultCuirass.ItemDef;
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

        public ConfigEntry<float> AttackSpeedBase;
        public ConfigEntry<float> AttackSpeedPerStack;
        public ConfigEntry<float> ArmorBase;
        public ConfigEntry<float> ArmorPerStack;
        private void CreateConfig(ConfigFile configuration)
        {
            AttackSpeedBase = configuration.Bind("Item: " + ItemName, "Attack Speed Base", 40.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(AttackSpeedBase));
            AttackSpeedPerStack = configuration.Bind("Item: " + ItemName, "Attack Speed Per Stack", 20.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(AttackSpeedPerStack));
            ArmorBase = configuration.Bind("Item: " + ItemName, "Armor Base", 40.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(ArmorBase));
            ArmorPerStack = configuration.Bind("Item: " + ItemName, "Armor Per Stack", 20.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(ArmorPerStack));
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
                if (!NetworkServer.active || GetCount(body) <= 0 || (body.healthComponent && !body.healthComponent.alive))
                {
                    return;
                }

                timer += Time.fixedDeltaTime;
                if (timer >= 1.0f)
                {
                    timer -= 1.0f;
                    int count = AssaultCuirass.GetCount(body);
                    foreach (var member in TeamComponent.GetTeamMembers(body.teamComponent.teamIndex))
                    {
                        CharacterBody ally = member.GetComponent<CharacterBody>();
                        if (ally && ally.isActiveAndEnabled)
                        {
                            AssaultCuirassBuff.ApplyTo(
                                body: ally,
                                duration: 1.1f,
                                stacks: count,
                                max_stacks: count
                            );
                        }
                    }
                }
            }
        }
    }
}