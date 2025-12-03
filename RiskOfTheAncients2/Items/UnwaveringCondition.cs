using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
using System;

namespace ROTA2.Items
{
    public class UnwaveringCondition : ItemBase<UnwaveringCondition>
    {
        public override string ItemName => "Unwavering Condition";
        public override string ItemTokenName => "UNWAVERING_CONDITION";
        public override string ItemTokenPickup => $"Increase magic resistance... {Death("BUT cap your maximum health.")}";
        public override string ItemTokenDesc =>
@$"Increase {Utility("magic resistance")} by {Utility($"{MagicResistanceBase.Value}%")} {Stack($"(+{MagicResistancePerStack.Value}% per stack)")}.
{Health("Combined maximum health")} cannot go above {Death($"{CombinedHealthLimit.Value}")} {Stack($"(-{LimitReductionPerStack.Value}% per stack)")}.";
        public override string ItemTokenLore => "A spiteful demon's skull, it is still imbued with dark energy.";
        public override string ItemDefGUID => Assets.UnwaveringCondition.ItemDef;
        public override void Hooks()
        {
            StatsAPI.Recalculate += (body, args) =>
            {
                int count = GetCount(body);
                if (count > 0)
                {
                    args.MagicResistance.Add(MagicResistanceBase.Value / 100f);
                    args.MagicResistance.AddRange(System.Linq.Enumerable.Repeat(MagicResistancePerStack.Value / 100f, count - 1));
                }
            };
            IL.RoR2.CharacterBody.RecalculateStats += OnRecalculateStats;
        }

        public override void Init(ConfigFile configuration)
        {
            CreateConfig(configuration);
            CreateLanguageTokens();
            CreateItemDef();
            Hooks();
        }

        public ConfigEntry<float> MagicResistanceBase;
        public ConfigEntry<float> MagicResistancePerStack;
        public ConfigEntry<float> CombinedHealthLimit;
        public ConfigEntry<float> LimitReductionPerStack;
        private void CreateConfig(ConfigFile configuration)
        {
            MagicResistanceBase = configuration.Bind("Item: " + ItemName, "Magic Resistance Base", 95f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(MagicResistanceBase));
            MagicResistancePerStack = configuration.Bind("Item: " + ItemName, "Magic Resistance Increase Per Stack", 50f, "");
            ModSettingsManager.AddOption(new FloatFieldOption(MagicResistancePerStack));
            CombinedHealthLimit = configuration.Bind("Item: " + ItemName, "Combined Health Limit", 100f, "Combined health is max health + max shields.");
            ModSettingsManager.AddOption(new FloatFieldOption(CombinedHealthLimit));
            LimitReductionPerStack = configuration.Bind("Item: " + ItemName, "Limit Reduction Per Stack", 50f, "Exponential (100 -> 50 -> 25...)");
            ModSettingsManager.AddOption(new FloatFieldOption(LimitReductionPerStack));
        }

        private void OnRecalculateStats(ILContext il)
        {
            ILCursor cursor = new(il);
            var index = 0;
            ILLabel label = null;
            if (!cursor.TryGotoNext
                (
                    x => x.MatchLdsfld(typeof(DLC2Content.Items), nameof(DLC2Content.Items.BoostAllStats)),
                    x => x.MatchCallOrCallvirt<Inventory>(nameof(Inventory.GetItemCountEffective)),
                    x => x.MatchStloc(out index)
                ) ||
                !cursor.TryGotoNext
                (
                    x => x.MatchLdloc(index),
                    x => x.MatchLdcI4(0),
                    x => x.MatchBle(out label)
                )
            )
            {
                Log.Error("Failed to match IL for Unwavering Condition, it won't limit combined maximum health!");
                return;
            }

            cursor.GotoLabel(label);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.EmitDelegate<Action<CharacterBody>>((body =>
            {
                int count = GetCount(body);
                if (count > 0)
                {
                    float combinedHealthLimit = CombinedHealthLimit.Value * MathF.Pow(LimitReductionPerStack.Value / 100f, count - 1);
                    float ratio = body.maxHealth / (body.maxHealth + body.maxShield);
                    body.maxHealth = MathF.Max(1f, combinedHealthLimit * ratio);
                    body.maxShield = MathF.Min(99f, combinedHealthLimit * (1 - ratio));
                }
            }));
        }
    }
}