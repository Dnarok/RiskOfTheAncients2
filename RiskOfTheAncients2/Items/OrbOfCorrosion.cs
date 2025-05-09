using BepInEx.Configuration;
using ROTA2.Buffs;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace ROTA2.Items
{
    public class OrbOfCorrosion : ItemBase<OrbOfCorrosion>
    {
        public override string ItemName => "Orb of Corrosion";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "ORB_OF_CORROSION";
        public override string ItemTokenPickup => "Reduces armor, movement speed, and attack speed, and poisons on hit.";
        public override string ItemTokenDesc => 
@$"On hit:
    Reduce {Damage("armor")} by {Damage($"{ArmorReduction}")} for {Damage($"{ArmorReductionDuration} seconds")}, up to {Damage($"{MaxStacksBase}")} {Stack($"(+{MaxStacksPerStack} per stack)")} times.
    {Utility("Slow")} enemies for {Utility($"-{MovementSpeedSlowBase}%")} {Stack($"(-{MovementSpeedSlowPerStack}% per stack)")} {Utility("movement speed")} and {Utility($"-{AttackSpeedSlowBase}%")} {Stack($"(-{AttackSpeedSlowPerStack}% per stack)")} {Utility("attack speed")} for {Utility($"{SlowDuration} seconds")}.
    {Healing("Poison")} enemies for {Damage($"{PoisonDamageBase}%")} {Stack($"(+{PoisonDamagePerStack}% per stack)")} base damage over {Damage($"{PoisonDuration} seconds")}.
Effects stack with component items.";
        public override string ItemTokenLore => "Seepage from the wounds of a warrior deity, sealed in an arcanist's orb following a campaign of vicious slaughter.";
        public override ItemTier Tier => ItemTier.Tier2;
        public override ItemTag[] ItemTags => [ItemTag.WorldUnique];
        public override string ItemModelPath => "RoR2/Base/Mystery/PickupMystery.prefab";
        public override string ItemIconPath => "RiskOfTheAncients2.Icons.orb_of_corrosion.png";
        public override void Hooks()
        {
            On.RoR2.HealthComponent.TakeDamage += OnHit;
            CharacterBody.onBodyInventoryChangedGlobal += OnInventoryChanged;
        }
        public override void Init(ConfigFile configuration)
        {
            CreateConfig(configuration);
            CreateLanguageTokens();
            CreateItemDef();
            Hooks();
        }

        public float ArmorReduction;
        public int MaxStacksBase;
        public int MaxStacksPerStack;
        public float ArmorReductionDuration;
        public float MovementSpeedSlowBase;
        public float MovementSpeedSlowPerStack;
        public float AttackSpeedSlowBase;
        public float AttackSpeedSlowPerStack;
        public float SlowDuration;
        public float PoisonDamageBase;
        public float PoisonDamagePerStack;
        public float PoisonDuration;
        public void CreateConfig(ConfigFile configuration)
        {
            ArmorReduction = configuration.Bind("Item: " + ItemName, "Armor Reduction Per Stack", 7.5f, "How much armor should be removed per stack of the debuff?").Value;
            MaxStacksBase = configuration.Bind("Item: " + ItemName, "Base Max Stacks", 3, "How many debuff stacks can be applied by the first stack?").Value;
            MaxStacksPerStack = configuration.Bind("Item: " + ItemName, "Stacking Max Stacks", 2, "How many debuff stacks can be applied by subsequent stack?").Value;
            ArmorReductionDuration = configuration.Bind("Item: " + ItemName, "Armor Reduction Duration", 3.0f, "How long should the armor reduction last?").Value;

            MovementSpeedSlowBase = configuration.Bind("Item: " + ItemName, "Movement Speed Reduction Base", 15.0f, "How much should movement speed be reduced initially?").Value;
            MovementSpeedSlowPerStack = configuration.Bind("Item: " + ItemName, "Movement Speed Reduction Per Stack", 15.0f, "How much should movement speed be reduced per stack?").Value;
            AttackSpeedSlowBase = configuration.Bind("Item: " + ItemName, "Attack Speed Reduction Base", 15.0f, "How much should attack speed be reduced initially?").Value;
            AttackSpeedSlowPerStack = configuration.Bind("Item: " + ItemName, "Attack Speed Reduction Per Stack", 15.0f, "How much should attack speed be reduced per stack?").Value;
            SlowDuration = configuration.Bind("Item: " + ItemName, "Slow Duration", 3.0f, "How long should the slows last?").Value;

            PoisonDamageBase = configuration.Bind("Item: " + ItemName, "Poison Damage Base", 300.0f, "How much base damage should the poison do with the first stack?").Value;
            PoisonDamagePerStack = configuration.Bind("Item: " + ItemName, "Poison Damage Per Stack", 300.0f, "How much base damage should the poison do with subsequent stacks?").Value;
            PoisonDuration = configuration.Bind("Item: " + ItemName, "Poison Duration", 3.0f, "How long should the poison last?").Value;
        }

        private void OnHit(On.RoR2.HealthComponent.orig_TakeDamage orig, RoR2.HealthComponent self, RoR2.DamageInfo info)
        {
            if (self && self.alive && info.attacker && info.procCoefficient > 0.0f)
            {
                var attacker_body = info.attacker.GetComponent<CharacterBody>();
                int count = GetCount(attacker_body);
                if (count > 0)
                {
                    OrbOfCorrosionArmor.Instance.ApplyTo(new BuffBase.ApplyParameters
                    {
                        victim = self.body,
                        duration = ArmorReductionDuration,
                        max_stacks = MaxStacksBase + MaxStacksPerStack * (count - 1)
                    });
                    OrbOfCorrosionSlow.Instance.ApplyTo(new BuffBase.ApplyParameters
                    {
                        victim = self.body,
                        duration = SlowDuration,
                        stacks = count,
                        max_stacks = count
                    });
                    OrbOfCorrosionPoison.Instance.ApplyTo(new BuffBase.ApplyParameters
                    {
                        victim = self.body,
                        attacker = attacker_body,
                        duration = PoisonDuration,
                        damage = (PoisonDamageBase + PoisonDamagePerStack * (count - 1)) / 100.0f / PoisonDuration,
                        max_stacks = 1
                    });
                }
            }

            orig(self, info);
        }
        private void OnInventoryChanged(CharacterBody body)
        {
            if (!body.GetComponent<OrbOfCorrosionInventoryBehavior>())
            {
                body.gameObject.AddComponent<OrbOfCorrosionInventoryBehavior>();
            }
        }

        public class OrbOfCorrosionInventoryBehavior : MonoBehaviour
        {
            public CharacterBody body;

            public void Awake()
            {
                body = GetComponent<CharacterBody>();
            }
            public void Update()
            {
                if (NetworkServer.active && body && body.inventory)
                {
                    int blights = body.inventory.GetItemCount(OrbOfBlight.Instance.ItemDef);
                    int frosts = body.inventory.GetItemCount(OrbOfFrost.Instance.ItemDef);
                    int venoms = body.inventory.GetItemCount(OrbOfVenom.Instance.ItemDef);

                    if (blights > 0 && frosts > 0 && venoms > 0)
                    {
                        int transform = Math.Min(blights, Math.Min(frosts, venoms));
                        Log.Info($"Converting {transform} blights, frosts, and venoms.");
                        body.inventory.RemoveItem(OrbOfBlight.Instance.ItemDef, transform);
                        body.inventory.RemoveItem(OrbOfFrost.Instance.ItemDef, transform);
                        body.inventory.RemoveItem(OrbOfVenom.Instance.ItemDef, transform);
                        body.inventory.GiveItem(OrbOfCorrosion.Instance.ItemDef, transform);


                        CharacterMasterNotificationQueue.PushItemTransformNotification(body.master, OrbOfBlight.Instance.ItemDef.itemIndex, OrbOfCorrosion.Instance.ItemDef.itemIndex, CharacterMasterNotificationQueue.TransformationType.Default);
                        CharacterMasterNotificationQueue.PushItemTransformNotification(body.master, OrbOfFrost.Instance.ItemDef.itemIndex, OrbOfCorrosion.Instance.ItemDef.itemIndex, CharacterMasterNotificationQueue.TransformationType.Default);
                        CharacterMasterNotificationQueue.PushItemTransformNotification(body.master, OrbOfVenom.Instance.ItemDef.itemIndex, OrbOfCorrosion.Instance.ItemDef.itemIndex, CharacterMasterNotificationQueue.TransformationType.Default);
                    }
                }
            }
        }
    }
}