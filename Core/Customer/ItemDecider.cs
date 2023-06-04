using SuspiciousGames.SellMe.Core.Items;
using System.Collections.Generic;
using UnityEngine;

namespace SuspiciousGames.SellMe.Core.Customer
{
    public static class ItemDecider
    {
        public static Item Decide(List<Item> items, Customer customer, int numToEvaluate)
        {
            if (customer.CustomerTraits.Count == 0)
            {
                return null;
            }

            List<KeyValuePair<Item, float>> sortedItems = new List<KeyValuePair<Item, float>>();
            foreach (var item in items)
            {
                float interest = 0;
                foreach (var customerTrait in customer.CustomerTraits)
                {
                    foreach (var wrapper in customerTrait.rarityInterests)
                    {
                        if (item.Rarity == wrapper.data)
                        {
                            interest += wrapper.weight;
                        }
                    }
                    foreach (var wrapper in customerTrait.categoryInterests)
                    {
                        if (item.Category == wrapper.data)
                        {
                            interest += wrapper.weight;
                        }

                    }
                    foreach (var wrapper in customerTrait.traitInterests)
                    {
                        if (item.TraitDatas.Exists(t => t.Trait == wrapper.data))
                        {
                            interest += wrapper.weight;
                        }
                    }
                }
                sortedItems.Add(new KeyValuePair<Item, float>(item, interest));
            }
            sortedItems.Sort((p1, p2) => p1.Value.CompareTo(p2.Value));
            sortedItems.Reverse();

            float maxInterest = GetPossibleMaxInterest(customer);

            float evaluations = numToEvaluate > sortedItems.Count ? sortedItems.Count : numToEvaluate;

            for (int i = 0; i < evaluations; i++)
            {
                if (sortedItems[i].Value >= Random.Range(0, maxInterest))
                {
                    return sortedItems[i].Key;
                }
            }
            return null;
        }

        private static float GetPossibleMaxInterest(Customer customer)
        {
            float maxInterest = 0;

            foreach (var trait in customer.CustomerTraits)
            {
                foreach (var wrapper in trait.rarityInterests)
                {
                    maxInterest += wrapper.weight;
                }
                foreach (var wrapper in trait.categoryInterests)
                {
                    maxInterest += wrapper.weight;
                }
                foreach (var wrapper in trait.traitInterests)
                {
                    maxInterest += wrapper.weight;
                }
            }
            return maxInterest;
        }
    }
}

