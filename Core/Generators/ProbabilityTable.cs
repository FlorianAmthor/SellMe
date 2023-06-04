using System;
using System.Collections.Generic;
using UnityEngine;

namespace SuspiciousGames.SellMe.Core.Generators
{
    public abstract class ProbabilityTable<T> : ScriptableObject where T : Enum
    {
        #region Protected fields
        protected Dictionary<T, ProbabilityData<T>> copiedProbabilities = new Dictionary<T, ProbabilityData<T>>();
        protected Dictionary<T, ProbabilityData<T>> baseProbabilities = new Dictionary<T, ProbabilityData<T>>();
        //protected Dictionary<ModifierType, List<ProbabilityModifier<T>>> modifiers = new Dictionary<ModifierType, List<ProbabilityModifier<T>>>();
        #endregion

        #region Properties
        public IReadOnlyDictionary<T, ProbabilityData<T>> CurrentProbabilities => copiedProbabilities;
        #endregion

        #region Public methods
        public virtual bool Roll(out T result)
        {
            float sumOfProb = 0.0f;
            foreach (var pair in copiedProbabilities)
            {
                sumOfProb += pair.Value.Probability;
            }
            var rValue = UnityEngine.Random.Range(0.0f, sumOfProb);
            float accumulatedProb = 0.0f;
            result = default;

            foreach (var pair in copiedProbabilities)
            {
                accumulatedProb += pair.Value.Probability;
                if (rValue <= accumulatedProb)
                {
                    result = pair.Key;

                    if (pair.Key.Equals(Enum.GetValues(typeof(T)).GetValue(0)))
                        return false;
                    return true;
                }
            }
            return true;
        }

        public void SetProbabilityValue(T data, float newProbability)
        {
            copiedProbabilities[data].Probability = newProbability;
        }

        public float GetProbabilityValue(T data)
        {
            return copiedProbabilities[data].Probability;
        }

        //public void AddProbabilityModifier(ProbabilityModifier<T> probMod)
        //{
        //    if (modifiers.ContainsKey(probMod.modifierType))
        //        modifiers[probMod.modifierType].Add(probMod);
        //    else
        //        modifiers.Add(probMod.modifierType, new List<ProbabilityModifier<T>>() { probMod });
        //    RecalculateProbability();
        //}

        //public void UndoModifier(ProbabilityModifier<T> probMod)
        //{
        //    if (modifiers.ContainsKey(probMod.modifierType))
        //    {
        //        modifiers[probMod.modifierType].Remove(probMod);
        //        RecalculateProbability();
        //    }
        //}

        //public void UndoAllModifiers()
        //{
        //    modifiers.Clear();
        //    Reset();
        //}

        //private void RecalculateProbability()
        //{
        //    //Calc according to order of modifiertype
        //}

        public abstract void Init();

        public abstract void Reset();
        #endregion
    }
}
