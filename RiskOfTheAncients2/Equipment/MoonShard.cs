using BepInEx.Configuration;
using R2API;
using RoR2;
using ROTA2.Items;

namespace ROTA2.Equipment
{
    public class MoonShard : EquipmentBase<MoonShard>
    {
        public override string EquipmentName => "Moon Shard";
        public override string EquipmentTokenName => "MOON_SHARD";
        public override string EquipmentTokenPickup => "Passively increases attack speed. Consume to gain permanent attack speed.";
        public override string EquipmentTokenDesc => $"Passively increases {Damage("attack speed")} by {Damage($"{AttackSpeed}%")} while held. {Utility("Consume")} on use to {Damage("permanently")} increase {Damage("attack speed")} by {Damage($"{ConsumedMoonShard.Instance.AttackSpeedBase}%")} {Stack($"(+{ConsumedMoonShard.Instance.AttackSpeedPerStack}% per stack)")}.";
        public override string EquipmentTokenLore => "Said to be a tear from the lunar goddess Selemene.";
        public override string EquipmentIconPath => "ROTA2.Icons.moon_shard.png";
        public override float EquipmentCooldown => 0.5f;
        public override bool EquipmentCanBeRandomlyTriggered => false;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddAttackSpeed;
        }
        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLanguageTokens();
            CreateEquipmentDef();
            Hooks();
        }

        public float AttackSpeed;
        private void CreateConfig(ConfigFile config)
        {
            AttackSpeed = config.Bind("Equipment: " + EquipmentName, "Held Attack Speed", 60.0f, "").Value;
        }

        protected override bool ActivateEquipment(EquipmentSlot slot)
        {
            if (slot && slot.characterBody && slot.characterBody.inventory)
            {
                slot.characterBody.inventory.SetEquipmentIndexForSlot(EquipmentIndex.None, slot.activeEquipmentSlot);
                slot.characterBody.inventory.GiveItem(ConsumedMoonShard.Instance.ItemDef);
            }

            return true;
        }

        private void AddAttackSpeed(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (HasThisEquipment(body))
            {
                args.attackSpeedMultAdd += AttackSpeed / 100.0f;
            }
        }
    }

    public class ConsumedMoonShard : ItemBase<ConsumedMoonShard>
    {
        public override string ItemName => "Consumed Moon Shard";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "CONSUMED_MOON_SHARD";
        public override string ItemTokenPickup => "Increase attack speed.";
        public override string ItemTokenDesc => $"Increases {Damage("attack speed")} by {Damage($"{AttackSpeedBase}%")} {Stack($"(+{AttackSpeedPerStack} per stack)")}.";
        public override string ItemTokenLore => "The power has waned.";
        public override ItemTier Tier => ItemTier.NoTier;
        public override ItemTag[] ItemTags => [ItemTag.WorldUnique];
        public override string ItemIconPath => "ROTA2.Icons.consumed_moon_shard.png";
        public override bool Removable => false;
        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddAttackSpeed;
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
        public void CreateConfig(ConfigFile configuration)
        {
            AttackSpeedBase = configuration.Bind("Item: " + ItemName, "Attack Speed Base", 20.0f, "").Value;
            AttackSpeedPerStack = configuration.Bind("Item: " + ItemName, "Attack Speed Per Stack", 20.0f, "").Value;
        }

        private void AddAttackSpeed(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            int count = GetCount(body);
            if (count > 0)
            {
                args.attackSpeedMultAdd += AttackSpeedBase / 100.0f + AttackSpeedPerStack / 100.0f * (count - 1);
            }
        }
    }
}