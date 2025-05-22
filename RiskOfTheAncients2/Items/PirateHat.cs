using BepInEx.Configuration;
using RoR2;
using RoR2.ExpansionManagement;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ROTA2.Items
{
    public class PirateHat : ItemBase<PirateHat>
    {
        public override string ItemName => "Pirate Hat";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "PIRATE_HAT";
        public override string ItemTokenPickup => "Upon monster kill chance to drop scrap.";
        public override string ItemTokenDesc => $"When a monster is killed it has a {Utility($"{DropChanceBase}%")} {Stack($"(+{DropChancePerStack}% per stack)")} chance of dropping scrap.";
        public override string ItemTokenLore => "A salty skulltopper cursed with endless good fortune.";
        public override ItemTier Tier => ItemTier.VoidTier3;
        public override string ItemIconPath => "ROTA2.Icons.pirate_hat.png";
        public override ItemDef VoidFor => DLC2Content.Items.ItemDropChanceOnKill;
        public override void Hooks()
        {
            GlobalEventManager.onCharacterDeathGlobal += OnKill;
        }
        public override void Init(ConfigFile configuration)
        {
            CreateConfig(configuration);
            CreateLanguageTokens();
            CreateItemDef();
            Hooks();
        }

        public float DropChanceBase;
        public float DropChancePerStack;
        public float GreenScrapChance;
        public float RedScrapChance;
        public bool  BossesDropYellowScrap;
        private void CreateConfig(ConfigFile configuration)
        {
            DropChanceBase = configuration.Bind("Item: " + ItemName, "Drop Chance Base", 3.0f, "").Value;
            DropChancePerStack = configuration.Bind("Item: " + ItemName, "Drop Chance Per Stack", 1.5f, "").Value;
            GreenScrapChance = configuration.Bind("Item: " + ItemName, "Green Scrap Chance", 20.0f, "").Value;
            RedScrapChance = configuration.Bind("Item: " + ItemName, "Red Scrap Chance", 1.0f, "").Value;
            BossesDropYellowScrap = configuration.Bind("Item: " + ItemName, "Bosses Drop Yellow Scrap Instead", true, "When the drop chance roll succeeds, that is.").Value;
        }

        private void OnKill(DamageReport report)
        {
            int count = GetCount(report.attackerBody);
            if (count > 0 && report.victim)
            {
                if (Util.CheckRoll(DropChanceBase + DropChancePerStack * (count - 1), report.attackerMaster))
                {
                    PickupIndex index = PickupCatalog.FindPickupIndex(RoR2Content.Items.ScrapWhite.itemIndex);
                    if (BossesDropYellowScrap && report.victimIsChampion)
                    {
                        index = PickupCatalog.FindPickupIndex(RoR2Content.Items.ScrapYellow.itemIndex);
                    }
                    else if (Util.CheckRoll(RedScrapChance, report.attackerMaster))
                    {
                        index = PickupCatalog.FindPickupIndex(RoR2Content.Items.ScrapRed.itemIndex);
                    }
                    else if (Util.CheckRoll(GreenScrapChance, report.attackerMaster))
                    {
                        index = PickupCatalog.FindPickupIndex(RoR2Content.Items.ScrapGreen.itemIndex);
                    }
                    PickupDropletController.CreatePickupDroplet(index, report.victim.transform.position + Vector3.up * 1.5f, Vector3.up * 20f);
                    Util.PlaySound("PirateHat", report.victimBody.gameObject);
                }
            }
        }
    }
}