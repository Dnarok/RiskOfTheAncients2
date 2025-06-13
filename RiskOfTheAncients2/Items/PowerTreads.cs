using BepInEx.Configuration;
using R2API;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
using ROTA2.Buffs;

namespace ROTA2.Items
{
    public class PowerTreads : ItemBase<PowerTreads>
    {
        public override string ItemName => "Power Treads";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "POWER_TREADS";
        public override string ItemTokenPickup => "Increase movement speed and attack speed. Cycle through stats on utility use.";
        public override string ItemTokenDesc =>
$@"Increases {Utility("movement speed")} by {Utility($"{MovementSpeedBase.Value}%")} {Stack($"(+{MovementSpeedPerStack.Value}% per stack)")} and {Damage("attack speed")} by {Damage($"{AttackSpeedBase.Value}%")} {Stack($"(+{AttackSpeedPerStack.Value}% per stack)")}. Activating your {Utility("Utility skill")} cycles through the following {Utility("bonus stats")}:
    {Color("Strength", "#EC3D06")}: Increases {Healing("base health regeneration")} by {Healing($"+{StrengthRegen.Value} hp/s")}.
    {Color("Agility", "#26E030")}: Increases {Damage("armor")} by {Damage($"{AgilityArmor.Value}")}.
    {Color("Intelligence", "#00A6BA")}: Increases {Damage("damage")} by {Damage($"{IntelligenceDamage.Value}%")}.";
        public override string ItemTokenLore => "A pair of tough-skinned boots that change to meet the demands of the wearer. ";
        public override string ItemDefGUID => Assets.PowerTreads.ItemDef;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddStats;
            On.RoR2.CharacterBody.OnSkillActivated += OnSkill;
        }

        public override void Init(ConfigFile configuration)
        {
            CreateConfig(configuration);
            CreateLanguageTokens();
            CreateItemDef();
            Hooks();
        }

        public ConfigEntry<float> MovementSpeedBase;
        public ConfigEntry<float> MovementSpeedPerStack;
        public ConfigEntry<float> AttackSpeedBase;
        public ConfigEntry<float> AttackSpeedPerStack;
        public ConfigEntry<float> StrengthRegen;
        public ConfigEntry<float> AgilityArmor;
        public ConfigEntry<float> IntelligenceDamage;
        private void CreateConfig(ConfigFile configuration)
        {
            MovementSpeedBase = configuration.Bind("Item: " + ItemName, "Movement Speed Base", 21.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(MovementSpeedBase));
            MovementSpeedPerStack = configuration.Bind("Item: " + ItemName, "Movement Speed Per Stack", 21.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(MovementSpeedPerStack));
            AttackSpeedBase = configuration.Bind("Item: " + ItemName, "Attack Speed Base", 22.5f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(AttackSpeedBase));
            AttackSpeedPerStack = configuration.Bind("Item: " + ItemName, "Attack Speed Per Stack", 22.5f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(AttackSpeedPerStack));
            StrengthRegen = configuration.Bind("Item: " + ItemName, "Strength Base Health Regen", 3.2f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(StrengthRegen));
            AgilityArmor = configuration.Bind("Item: " + ItemName, "Agility Armor", 25.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(AgilityArmor));
            IntelligenceDamage = configuration.Bind("Item: " + ItemName, "Intelligence Damage", 20.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(IntelligenceDamage));
        }

        private void AddStats(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            int count = GetCount(body);
            if (count > 0)
            {
                args.moveSpeedMultAdd += MovementSpeedBase.Value / 100f + MovementSpeedPerStack.Value / 100f * (count - 1);
                args.attackSpeedMultAdd += AttackSpeedBase.Value / 100f + AttackSpeedPerStack.Value / 100f * (count - 1);
            }
        }
        private void OnSkill(On.RoR2.CharacterBody.orig_OnSkillActivated orig, CharacterBody body, GenericSkill skill)
        {
            int count = GetCount(body);
            if (count > 0 && skill == body.skillLocator.utility)
            {
                if (PowerTreadsStrength.HasThisBuff(body))
                {
                    body.RemoveBuff(PowerTreadsStrength.GetBuffDef());
                    body.AddBuff(PowerTreadsAgility.GetBuffDef());
                }
                else if (PowerTreadsAgility.HasThisBuff(body))
                {
                    body.RemoveBuff(PowerTreadsAgility.GetBuffDef());
                    body.AddBuff(PowerTreadsIntelligence.GetBuffDef());
                }
                else if (PowerTreadsIntelligence.HasThisBuff(body))
                {
                    body.RemoveBuff(PowerTreadsIntelligence.GetBuffDef());
                    body.AddBuff(PowerTreadsStrength.GetBuffDef());
                }
                else
                {
                    body.AddBuff(PowerTreadsStrength.GetBuffDef());
                }

                body.MarkAllStatsDirty();
            }

            orig(body, skill);
        }
    }
}