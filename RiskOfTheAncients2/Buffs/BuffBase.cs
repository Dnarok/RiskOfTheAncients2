using R2API;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static RoR2.DotController;

namespace ROTA2.Buffs
{
    public abstract class BuffBase<T> : BuffBase where T : BuffBase<T>
    {
        public static T Instance { get; private set; }

        public BuffBase()
        {
            if (Instance != null)
            {
                throw new InvalidOperationException($"Singleton class \"{typeof(T).Name}\" inheriting BuffBase was instantiated twice.");
            }
            else
            {
                Instance = this as T;
            }
        }
    }

    public abstract class BuffBase
    {
        public abstract string BuffName { get; }
        public abstract string BuffTokenName { get; }
        public abstract bool BuffStacks { get; }
        public abstract bool IsDebuff { get; }
        public abstract Color BuffColor { get; }
        public abstract string BuffIconPath { get; }
        public abstract EliteDef BuffEliteDef { get; }
        public abstract bool IsCooldown { get; }
        public abstract bool IsHidden { get; }
        public abstract NetworkSoundEventDef BuffStartSfx { get; }
        public virtual DotIndex Index { get; set; } = default;
        public virtual void Init()
        {
            CreateLanguageTokens();
            CreateBuffDef();
            Hooks();
        }
        public virtual void Hooks()
        {

        }

        public struct ApplyParameters
        {
            public CharacterBody victim;
            public float? duration;
            public int? stacks;
            public int? max_stacks;
            public CharacterBody? attacker;
            public float? damage;
        }
        public virtual void ApplyTo(ApplyParameters parameters)
        {
            if (parameters.victim)
            {
                if (parameters.attacker != null && parameters.damage != null)
                {
                    InflictDotInfo inflict = new()
                    {
                        victimObject = parameters.victim.gameObject,
                        attackerObject = parameters.attacker.gameObject,
                        dotIndex = Index,
                        duration = parameters.duration ?? Mathf.Infinity,
                        damageMultiplier = (float)parameters.damage,
                        maxStacksFromAttacker = (uint?)parameters.max_stacks
                    };

                    for (int i = 0; i < (parameters.stacks ?? 1); ++i)
                    {
                        InflictDot(ref inflict);
                    }
                }
                else if (parameters.duration == null)
                {
                    for (int i = 0; i < (parameters.stacks ?? 1); ++i)
                    {
                        parameters.victim.AddBuff(BuffDef);
                    }
                }
                else
                {
                    for (int i = 0; i < (parameters.stacks ?? 1); ++i)
                    {
                        parameters.victim.AddTimedBuff(BuffDef, (float)parameters.duration, parameters.max_stacks ?? int.MaxValue);
                    }
                }

                parameters.victim.RecalculateStats();
            }
        }

        public BuffDef BuffDef;

        protected void CreateLanguageTokens()
        {
            LanguageAPI.Add("BUFF_" + BuffTokenName + "_NAME", BuffName);
        }
        protected void CreateBuffDef()
        {
            BuffDef = ScriptableObject.CreateInstance<BuffDef>();

            BuffDef.name = BuffName;
            BuffDef.canStack = BuffStacks;
            BuffDef.isDebuff = IsDebuff;
            BuffDef.isCooldown = IsCooldown;
            BuffDef.buffColor = BuffColor;
            BuffDef.eliteDef = BuffEliteDef;
            BuffDef.startSfx = BuffStartSfx;

            if (BuffIconPath == "")
            {
                BuffDef.iconSprite = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/MiscIcons/texMysteryIcon.png").WaitForCompletion();
            }
            else
            {
                BuffDef.iconSprite = Plugin.ExtractSprite(BuffIconPath);
            }

            ContentAddition.AddBuffDef(BuffDef);
        }

        public bool HasThisBuff(CharacterBody body)
        {
            return body && body.HasBuff(BuffDef);
        }
        public int GetBuffCount(CharacterBody body)
        {
            if (HasThisBuff(body))
            {
                return body.GetBuffCount(BuffDef);
            }
            else
            {
                return 0;
            }
        }
    }
}