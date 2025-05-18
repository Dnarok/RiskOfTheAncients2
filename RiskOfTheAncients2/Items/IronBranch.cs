using BepInEx.Configuration;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace ROTA2.Items
{
    public class IronBranch : ItemBase<IronBranch>
    {
        public override string ItemName => "Iron Branch";
        public override string ConfigItemName => ItemName;
        public override string ItemTokenName => "IRON_BRANCH";
        public override string ItemTokenPickup => "Increases all stats by a small amount.";
        public override string ItemTokenDesc => $"Grants {Utility($"{StatIncreaseBase}%")} {Stack($"(+{StatIncreasePerStack}% per stack)")} increase to {Utility("ALL stats")}.";
        public override string ItemTokenLore => "A seemingly ordinary branch, its ironlike qualities are bestowed upon the bearer.";
        public override string ItemModelPath => "iron_branch.prefab";
        public override string ItemIconPath => "ROTA2.Icons.iron_branch.png";
        public override ItemTier Tier => ItemTier.Tier1;
        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += (il) =>
            {
                ILCursor cursor = new(il);
                cursor.GotoNext(
                    x => x.MatchLdarg(0),
                    x => x.MatchCall<CharacterBody>("get_maxHealth"),
                    x => x.MatchCall<CharacterBody>("set_maxBonusHealth")
                );
                cursor.Emit(OpCodes.Ldarg_0);
                cursor.EmitDelegate<Action<CharacterBody>>((body) =>
                {
                    int count = GetCount(body);
                    if (count > 0)
                    {
                        float multiplier = 1.0f + (StatIncreaseBase / 100.0f + StatIncreasePerStack / 100.0f * (count - 1));
                        body.maxHealth *= multiplier;
                        body.maxShield *= multiplier;
                        body.moveSpeed *= multiplier;
                        body.damage *= multiplier;
                        body.attackSpeed *= multiplier;
                        body.crit *= multiplier;
                        body.regen *= multiplier;
                        body.armor *= multiplier;
                    }
                });
            };
        }

        public override void Init(ConfigFile configuration)
        {
            CreateConfig(configuration);
            CreateLanguageTokens();
            CreateItemDef();
            Hooks();
        }

        public float StatIncreaseBase;
        public float StatIncreasePerStack;
        private void CreateConfig(ConfigFile configuration)
        {
            StatIncreaseBase = configuration.Bind("Item: " + ItemName, "All Stats Increase Base", 1.5f, "").Value;
            StatIncreasePerStack = configuration.Bind("Item: " + ItemName, "All Stats Increase Per Stack", 1.5f, "").Value;
        }
    }
}