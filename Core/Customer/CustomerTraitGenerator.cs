using System.Collections.Generic;
using UnityEngine;

namespace SuspiciousGames.SellMe.Core.Customer
{
    public static class CustomerTraitGenerator
    {
        public static List<CustomerTrait> GenerateRandom(List<CustomerTrait> traitPool, int minTraits, int maxTraits)
        {
            int rndTraitNumber = Random.Range(minTraits, maxTraits);

            var traitList = new List<CustomerTrait>();
            for (int i = 0; i < rndTraitNumber; i++)
            {
                int rnd = Random.Range(0, traitPool.Count);
                traitList.Add(traitPool[rnd]);
            }
            return traitList;
        }
    }
}

