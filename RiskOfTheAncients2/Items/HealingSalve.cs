using BepInEx.Configuration;
using ROTA2.Buffs;
using RoR2;

namespace ROTA2.Items
{
    public class HealingSalve : ItemBase<HealingSalve>
    {
        public override string ItemName => "Healing Salve";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "HEALING_SALVE";
        public override string ItemTokenPickup => "Receive interruptible healing after using your Special skill.";
        public override string ItemTokenDesc => $"Activating your {Utility("Special skill")} {Healing("heals")} you for {Healing($"{MaximumHealthRegenerationBase}%")} {Stack($"(+{MaximumHealthRegenerationPerStack}% per stack)")} {Healing("of your maximum health")} per second for {Utility($"{BuffDuration} seconds")}. Healing is {Health("interrupted by taking damage.")}";
        public override string ItemTokenLore => "A magical salve that can quickly mend even the deepest of wounds.";
        public override ItemTier Tier => ItemTier.Tier1;
        public override string ItemIconPath => "ROTA2.Icons.healing_salve.png";
        public override void Hooks()
        {
            On.RoR2.CharacterBody.OnSkillActivated += OnSkill;
        }

        public override void Init(ConfigFile configuration)
        {
            CreateConfig(configuration);
            CreateLanguageTokens();
            CreateItemDef();
            Hooks();
        }

        public float MaximumHealthRegenerationBase;
        public float MaximumHealthRegenerationPerStack;
        public float BuffDuration;
        public void CreateConfig(ConfigFile configuration)
        {
            MaximumHealthRegenerationBase     = configuration.Bind("Item: " + ItemName, "Maximum Health Regeneration Base",      2.0f, "How much maximum health percentage regeneration should the first stack provide?").Value;
            MaximumHealthRegenerationPerStack = configuration.Bind("Item: " + ItemName, "Maximum Health Regeneration Per Stack", 2.0f, "How much maximum health percentage regeneration should subsequent stacks provide?").Value;
            BuffDuration                      = configuration.Bind("Item: " + ItemName, "Healing Duration",                      5.0f, "How long should the regeneration last?").Value;
        }

        private void OnSkill(On.RoR2.CharacterBody.orig_OnSkillActivated orig, CharacterBody body, GenericSkill skill)
        {
            int count = GetCount(body);
            if (count > 0 && skill == body.skillLocator.special)
            {
                HealingSalveBuff.Instance.ApplyTo(new BuffBase.ApplyParameters
                {
                    victim = body,
                    duration = BuffDuration
                });
            }

            orig(body, skill);
        }
    }
}