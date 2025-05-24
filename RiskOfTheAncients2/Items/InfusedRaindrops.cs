using BepInEx.Configuration;
using RoR2;
using System;
using UnityEngine;

namespace ROTA2.Items
{
    public class InfusedRaindrops : ItemBase<InfusedRaindrops>
    {
        public override string ItemName => "Infused Raindrops";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "INFUSED_RAINDROPS";
        public override string ItemTokenPickup => "Receive flat damage reduction from large attacks.";
        public override string ItemTokenDesc => $"Reduce all {Damage("incoming damage")} above {Damage($"{IncomingDamageMinimum}")} by {Damage($"{DamageBlockBase}")} {Stack($"(+{DamageBlockPerStack} per stack)")}. Cannot be reduced below {Damage($"{DamageMinimum}")}.";
        public override string ItemTokenLore => "Elemental protection from magical assaults.";
        public override ItemTier Tier => ItemTier.VoidTier1;
        public override ItemDef VoidFor => RoR2Content.Items.ArmorPlate;
        public override string ItemIconPath => "ROTA2.Icons.infused_raindrops.png";
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

        public float IncomingDamageMinimum;
        public float DamageBlockBase;
        public float DamageBlockPerStack;
        public float DamageMinimum;
        private void CreateConfig(ConfigFile configuration)
        {
            IncomingDamageMinimum   = configuration.Bind("Item: " + ItemName, "Incoming Damage Minimum",     60.0f, "").Value;
            DamageBlockBase         = configuration.Bind("Item: " + ItemName, "Flat Damage Block Base",      15.0f, "").Value;
            DamageBlockPerStack     = configuration.Bind("Item: " + ItemName, "Flat Damage Block Per Stack", 15.0f, "").Value;
            DamageMinimum           = configuration.Bind("Item: " + ItemName, "Damage Minimum After Block",  1.0f,  "").Value;
        }

        private void OnInventoryChanged(CharacterBody body)
        {
            int count = GetCount(body);
            var behavior = body.GetComponent<RaindropsBehavior>();
            if (count > 0 && !behavior)
            {
                body.gameObject.AddComponent<RaindropsBehavior>();
            }
            else if (count == 0 && behavior)
            {
                UnityEngine.Object.Destroy(behavior);
            }
        }

        public class RaindropsBehavior : MonoBehaviour, IOnIncomingDamageServerReceiver
        {
            CharacterBody body;

            void Awake()
            {
                body = GetComponent<CharacterBody>();
                if (body && body.healthComponent)
                {
                    HG.ArrayUtils.ArrayAppend(ref body.healthComponent.onIncomingDamageReceivers, this);
                }
            }
            void OnDestroy()
            {
                if (body && body.healthComponent)
                {
                    int i = Array.IndexOf(body.healthComponent.onIncomingDamageReceivers, this);
                    if (i != -1)
                    {
                        HG.ArrayUtils.ArrayRemoveAtAndResize(ref body.healthComponent.onIncomingDamageReceivers, body.healthComponent.onIncomingDamageReceivers.Length, i);
                    }
                }
            }
            public void OnIncomingDamageServer(DamageInfo info)
            {
                int count = Instance.GetCount(body);
                if (info.rejected || info.damageType.damageType.HasFlag(DamageType.BypassArmor) || info.damage < Instance.IncomingDamageMinimum || count <= 0)
                {
                    return;
                }

                info.damage = Mathf.Max(Instance.DamageMinimum, info.damage - (Instance.DamageBlockBase + Instance.DamageBlockPerStack * (count - 1)));
                Util.PlaySound("InfusedRaindrops", body.gameObject);
            }
        }
    }
}