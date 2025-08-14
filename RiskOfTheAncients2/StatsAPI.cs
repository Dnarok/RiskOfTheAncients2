using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ROTA2
{
    public class StatsAPI
    {
        public static void Init()
        {
            DamageTypes.BypassMagicResistance = DamageAPI.ReserveDamageType();
        }

        public static class DamageTypes
        {
            // to use, do R2API.DamageAPI.AddModdedDamageType(ref myDamageTypeCombo, ROTA2.StatsAPI.BypassMagicResistance).
            public static R2API.DamageAPI.ModdedDamageType BypassMagicResistance;
        }

        public class RecalculateArgs : EventArgs
        {
            #region magic resistance
            public List<float> MagicResistance = [];
            #endregion
        }
        public class CustomStats : MonoBehaviour, IOnIncomingDamageServerReceiver
        {
            CharacterBody body;
            public float MagicResistanceMultiplier = 1f;
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
                if (info.damageType.IsDamageSourceSkillBased && !info.HasModdedDamageType(DamageTypes.BypassMagicResistance))
                {
                    info.damage *= MagicResistanceMultiplier;
                }
            }
        }

        private static bool hooksEnabled = false;
        public delegate void RecalculateHandler(CharacterBody body, RecalculateArgs args);
        private static event RecalculateHandler _recalculate;
        public static event RecalculateHandler Recalculate
        {
            add
            {
                if (!hooksEnabled)
                {
                    On.RoR2.CharacterBody.RecalculateStats += OnRecalculateStats;
                    hooksEnabled = true;
                }

                _recalculate += value;
            }
            remove
            {
                _recalculate -= value;

                if (_recalculate == null || _recalculate.GetInvocationList().Length == 0)
                {
                    On.RoR2.CharacterBody.RecalculateStats -= OnRecalculateStats;
                    hooksEnabled = false;
                }
            }
        }
        private static void OnRecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);

            var behavior = self.GetComponent<CustomStats>();
            if (!behavior)
            {
                behavior = self.gameObject.AddComponent<CustomStats>();
            }

            var stats = new RecalculateArgs();
            if (_recalculate != null)
            {
                foreach (RecalculateHandler handler in _recalculate.GetInvocationList())
                {
                    try
                    {
                        handler(self, stats);
                    }
                    catch (Exception e)
                    {
                        Log.Error($"Exception thrown by {handler.Method.DeclaringType.Name}.{handler.Method.Name}:\n{e}");
                    }
                }
            }

            behavior.MagicResistanceMultiplier = 1f;
            foreach (float source in stats.MagicResistance)
            {
                behavior.MagicResistanceMultiplier *= 1 - source;
            }
        }
    }
}