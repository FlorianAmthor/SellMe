using SuspiciousGames.SellMe.Core.Items;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SuspiciousGames.SellMe.Core.Customer
{
    [System.Serializable, CreateAssetMenu(fileName = "CustomerTrait", menuName = "ScriptableObjects/Customer/CustomerTrait")]
    public class CustomerTrait : ScriptableObject
    {
        public string traitName;
        public List<RarityWeightWrapper> rarityInterests;
        public List<CategoryWeightWrapper> categoryInterests;
        public List<TraitWeightWrapper> traitInterests;
        [HideInInspector]
        public bool init = false;

        public void OnEnable()
        {
            if (!init)
            {
                Init();
            }
        }

        public void Init()
        {
            rarityInterests = new List<RarityWeightWrapper>();
            foreach (Rarity value in Enum.GetValues(typeof(Rarity)))
            {
                if (value != Rarity.None)
                {
                    rarityInterests.Add(new RarityWeightWrapper() { data = value, weight = 0 });
                }                
            }

            categoryInterests = new List<CategoryWeightWrapper>();
            foreach (Category value in Enum.GetValues(typeof(Category)))
            {
                if (value != Category.None)
                {
                    categoryInterests.Add(new CategoryWeightWrapper() { data = value, weight = 0 });
                }
                
            }

            traitInterests = new List<TraitWeightWrapper>();
            foreach (Trait value in Enum.GetValues(typeof(Trait)))
            {
                if (value != Trait.None)
                {
                    traitInterests.Add(new TraitWeightWrapper() { data = value, weight = 0 });
                }
                
            }

            init = true;
        }
    }

    [System.Serializable]
    public class RarityWeightWrapper : WeightWrapper<Rarity>
    {
    }

    [System.Serializable]
    public class CategoryWeightWrapper : WeightWrapper<Category>
    {
    }

    [System.Serializable]
    public class TraitWeightWrapper : WeightWrapper<Trait>
    {
    }

    [System.Serializable]
    public abstract class WeightWrapper<T>
    {
        [SerializeField]
        public T data;
        [SerializeField, Range(-1, 1)]
        public float weight;
    }
}
