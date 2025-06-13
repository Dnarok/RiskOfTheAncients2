using BepInEx.Configuration;
using R2API;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
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
        public override string ItemTokenDesc => $"When a monster is killed it has a {Utility($"{DropChanceBase.Value}%")} {Stack($"(+{DropChancePerStack.Value}% per stack)")} chance of dropping scrap.";
        public override string ItemTokenLore => "A salty skulltopper cursed with endless good fortune.";
        public override string ItemDefGUID => Assets.PirateHat.ItemDef;
        public override ItemDef VoidFor => DLC2Content.Items.ItemDropChanceOnKill;
        public override void Hooks()
        {
            GlobalEventManager.onCharacterDeathGlobal += OnKill;
        }
        public override void Init(ConfigFile configuration)
        {
            CreateConfig(configuration);
            CreateSounds();
            CreateLanguageTokens();
            CreateItemDef();
            Hooks();
        }

        public ConfigEntry<float> DropChanceBase;
        public ConfigEntry<float> DropChancePerStack;
        public ConfigEntry<float> GreenScrapChance;
        public ConfigEntry<float> RedScrapChance;
        public ConfigEntry<bool> BossesDropYellowScrap;
        public ConfigEntry<bool> PlaySound;
        private void CreateConfig(ConfigFile configuration)
        {
            DropChanceBase = configuration.Bind("Item: " + ItemName, "Drop Chance Base", 3.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(DropChanceBase));
            DropChancePerStack = configuration.Bind("Item: " + ItemName, "Drop Chance Per Stack", 1.5f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(DropChancePerStack));
            GreenScrapChance = configuration.Bind("Item: " + ItemName, "Green Scrap Chance", 20.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(GreenScrapChance));
            RedScrapChance = configuration.Bind("Item: " + ItemName, "Red Scrap Chance", 1.0f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(RedScrapChance));
            BossesDropYellowScrap = configuration.Bind("Item: " + ItemName, "Bosses Drop Yellow Scrap Instead", true, "When the drop chance roll succeeds, that is.");
            ModSettingsManager.AddOption(new CheckBoxOption(BossesDropYellowScrap));
            PlaySound = configuration.Bind("Item: " + ItemName, "Play Sound", true, "");
            ModSettingsManager.AddOption(new CheckBoxOption(PlaySound));
        }

        NetworkSoundEventDef sound = null;
        protected void CreateSounds()
        {
            Addressables.LoadAssetAsync<NetworkSoundEventDef>(Assets.PirateHat.NetworkSoundEventDef).Completed += (x) => { ContentAddition.AddNetworkSoundEventDef(x.Result); sound = x.Result; };
        }

        private void OnKill(DamageReport report)
        {
            int count = GetCount(report.attackerBody);
            if (count > 0 && report.victim)
            {
                if (Util.CheckRoll(DropChanceBase.Value + DropChancePerStack.Value * (count - 1), report.attackerMaster))
                {
                    PickupIndex index = PickupCatalog.FindPickupIndex(RoR2Content.Items.ScrapWhite.itemIndex);
                    if (BossesDropYellowScrap.Value && report.victimIsChampion)
                    {
                        index = PickupCatalog.FindPickupIndex(RoR2Content.Items.ScrapYellow.itemIndex);
                    }
                    else if (Util.CheckRoll(RedScrapChance.Value, report.attackerMaster))
                    {
                        index = PickupCatalog.FindPickupIndex(RoR2Content.Items.ScrapRed.itemIndex);
                    }
                    else if (Util.CheckRoll(GreenScrapChance.Value, report.attackerMaster))
                    {
                        index = PickupCatalog.FindPickupIndex(RoR2Content.Items.ScrapGreen.itemIndex);
                    }
                    PickupDropletController.CreatePickupDroplet(index, report.victim.transform.position + Vector3.up * 1.5f, Vector3.up * 20f);

                    if (PlaySound.Value)
                    {
                        EffectManager.SimpleSoundEffect(Instance.sound.index, report.victim.transform.position + Vector3.up * 1.5f, true);
                    }
                }
            }
        }
    }
}