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

        public static BuffDef GetBuffDef()
        {
            return Instance.BuffDef;
        }
        public static bool HasThisBuff(CharacterBody body)
        {
            return body && body.HasBuff(GetBuffDef());
        }
        public static int GetBuffCount(CharacterBody body)
        {
            if (HasThisBuff(body))
            {
                return body.GetBuffCount(GetBuffDef());
            }
            else
            {
                return 0;
            }
        }
        public static void ApplyTo(CharacterBody body)
        {
            body.AddBuff(GetBuffDef());
        }
        public static void ApplyTo(CharacterBody body, float duration)
        {
            body.AddTimedBuff(GetBuffDef(), duration);
        }
        public static void ApplyTo(CharacterBody body, int stacks)
        {
            for (int i = 0; i < stacks; ++i)
            {
                body.AddBuff(GetBuffDef());
            }
        }
        public static void ApplyTo(CharacterBody body, float duration, int stacks)
        {
            for (int i = 0; i < stacks; ++i)
            {
                body.AddTimedBuff(GetBuffDef(), duration);
            }
        }
        public static void ApplyTo(CharacterBody body, int stacks, int max_stacks)
        {
            int count = Math.Min(max_stacks - body.GetBuffCount(GetBuffDef()), stacks);
            ApplyTo(body, count);
        }
        public static void ApplyTo(CharacterBody body, float duration, int stacks, int max_stacks)
        {
            for (int i = 0; i < stacks; ++i)
            {
                body.AddTimedBuff(GetBuffDef(), duration, max_stacks);
            }
        }
        public static void ApplyTo(CharacterBody victim, CharacterBody attacker, float damage)
        {
            InflictDotInfo inflict = new()
            {
                victimObject = victim.gameObject,
                attackerObject = attacker.gameObject,
                dotIndex = Instance.Index,
                duration = Mathf.Infinity,
                damageMultiplier = damage
            };
            InflictDot(ref inflict);
        }
        public static void ApplyTo(CharacterBody victim, CharacterBody attacker, float damage, float duration)
        {
            InflictDotInfo inflict = new()
            {
                victimObject = victim.gameObject,
                attackerObject = attacker.gameObject,
                dotIndex = Instance.Index,
                duration = duration,
                damageMultiplier = damage
            };
            InflictDot(ref inflict);
        }
        public static void ApplyTo(CharacterBody victim, CharacterBody attacker, float damage, int stacks)
        {
            InflictDotInfo inflict = new()
            {
                victimObject = victim.gameObject,
                attackerObject = attacker.gameObject,
                dotIndex = Instance.Index,
                duration = Mathf.Infinity,
                damageMultiplier = damage
            };
            for (int i = 0; i < stacks; ++i)
            {
                InflictDot(ref inflict);
            }
        }
        public static void ApplyTo(CharacterBody victim, CharacterBody attacker, float damage, float duration, int stacks)
        {
            InflictDotInfo inflict = new()
            {
                victimObject = victim.gameObject,
                attackerObject = attacker.gameObject,
                dotIndex = Instance.Index,
                duration = duration,
                damageMultiplier = damage
            };
            for (int i = 0; i < stacks; ++i)
            {
                InflictDot(ref inflict);
            }
        }
        public static void ApplyTo(CharacterBody victim, CharacterBody attacker, float damage, int stacks, int max_stacks)
        {
            InflictDotInfo inflict = new()
            {
                victimObject = victim.gameObject,
                attackerObject = attacker.gameObject,
                dotIndex = Instance.Index,
                duration = Mathf.Infinity,
                damageMultiplier = damage,
                maxStacksFromAttacker = (uint)max_stacks
            };
            for (int i = 0; i < stacks; ++i)
            {
                InflictDot(ref inflict);
            }
        }
        public static void ApplyTo(CharacterBody victim, CharacterBody attacker, float damage, float duration, int stacks, int max_stacks)
        {
            InflictDotInfo inflict = new()
            {
                victimObject = victim.gameObject,
                attackerObject = attacker.gameObject,
                dotIndex = Instance.Index,
                duration = duration,
                damageMultiplier = damage,
                maxStacksFromAttacker = (uint)max_stacks
            };
            for (int i = 0; i < stacks; ++i)
            {
                InflictDot(ref inflict);
            }
        }
    }

    public abstract class BuffBase
    {
        public abstract string BuffName { get; }
        public abstract string BuffTokenName { get; }
        public abstract string BuffDefGUID { get; }
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

        protected BuffDef BuffDef;

        protected void CreateLanguageTokens()
        {
            LanguageAPI.Add("BUFF_" + BuffTokenName + "_NAME", BuffName);
        }
        protected void CreateBuffDef()
        {
            BuffDef = Addressables.LoadAssetAsync<BuffDef>(BuffDefGUID).WaitForCompletion();
            BuffDef.name = BuffName;
            ContentAddition.AddBuffDef(BuffDef);
        }
    }
}