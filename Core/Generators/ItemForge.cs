using SuspiciousGames.SellMe.Core.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SuspiciousGames.SellMe.Core.Generators
{
    public class ItemForge : MonoBehaviour
    {
        public void ForgeItem(IReadOnlyCollection<Item> usedItems, out Item result)
        {
            Dictionary<Category, byte> numCategory = new Dictionary<Category, byte>();
            List<TraitData> usedTraits = new List<TraitData>();
            foreach (var item in usedItems)
                item.TraitDatas.ForEach(traitData => usedTraits.Add(traitData));
            byte sumOfRarityValues = 0;

            foreach (var item in usedItems)
            {
                sumOfRarityValues += (byte)item.Rarity;
                if (!numCategory.ContainsKey(item.Category))
                    numCategory.Add(item.Category, 1);
                else
                    numCategory[item.Category]++;
            }

            Category cResult = CalcForgingCategory(numCategory);
            Rarity rResult = CalcForgingRarity(sumOfRarityValues);

            List<Trait> traitResults = CalcForgingTraits(sumOfRarityValues, rResult, usedTraits);
            int value = 0;

            foreach (var item in usedItems)
                value += item.Value;

            result = new Item();
            result.Init(cResult, rResult, traitResults, value);
            Debug.Log(JsonUtility.ToJson(result, true));
        }

        private List<Trait> CalcForgingTraits(byte sumOfRarityValues, Rarity rarity, List<TraitData> usedTraits)
        {
            List<Trait> result = new List<Trait>();
            //TODO: What do we do with surplus value?
            //byte surplusRarityValue = (byte)(sumOfRarityValues - (byte)rarity);
            return result;
        }

        private Rarity CalcForgingRarity(byte sumOfRarityValues)
        {
            if (sumOfRarityValues >= (byte)Rarity.Legendary)
                return Rarity.Legendary;

            var rarityValues = Enum.GetValues(typeof(Rarity));
            for (int i = 1; i < rarityValues.Length; i++)
            {
                if (sumOfRarityValues >= (byte)rarityValues.GetValue(i + 1))
                    continue;
                else
                    return (Rarity)rarityValues.GetValue(i);
            }
            return Rarity.None;
        }

        private Category CalcForgingCategory(Dictionary<Category, byte> numCategory)
        {
            HashSet<Category> maxCategories = new HashSet<Category>();
            byte maxCategory = 0;

            maxCategory = numCategory.Values.Max();

            foreach (var pair in numCategory)
            {
                if (pair.Value == maxCategory)
                {
                    if (maxCategories.Contains(pair.Key))
                        continue;
                    maxCategories.Add(pair.Key);
                }
            }

            if (maxCategories.Count > 1)
            {
                int rValue = UnityEngine.Random.Range(0, 3);
                return maxCategories.ElementAt(rValue);
            }
            else
            {
                return maxCategories.First();
            }
        }
    }
}