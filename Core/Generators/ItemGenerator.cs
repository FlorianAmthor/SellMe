using SuspiciousGames.SellMe.Core.Items;
using System.Collections.Generic;
using UnityEngine;

namespace SuspiciousGames.SellMe.Core.Generators
{
    [CreateAssetMenu(fileName = "ItemGenerator", menuName = "ScriptableObjects/ItemGenerator")]
    public class ItemGenerator : ScriptableObject
    {
        #region Singleton
        private static ItemGenerator _instance;

        public static ItemGenerator Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<ItemGenerator>("ItemGenerator");
                    _instance.Init();
                }
                return _instance;
            }
        }
        #endregion

        #region Exposed private fields
        [SerializeField]
        private List<RarityTraitWrapper> _numOfTraits;
        #endregion

        #region Private fields
        private Dictionary<Rarity, int> _numOfTraitsDict;
        private Dictionary<Trait, TraitData> _traitData;
        #endregion

        #region Public fields
        public RarityTable rarityTable;
        public CategoryTable categoryTable;
        public TraitTable traitTable;
        #endregion

        #region Public methods
        public bool Generate(out Item result, Rarity rarity = Rarity.None, Category category = Category.None, int numOfPosiviteTraits = -1, int numOfNegativeTraits = -1, List<Trait> traitList = null)
        {
            if (rarity == Rarity.None)
            {
                if (category != Category.None || numOfPosiviteTraits != -1 || numOfNegativeTraits != -1 || traitList != null)
                    rarityTable.SetProbabilityValue(Rarity.None, 0);
                if (!rarityTable.Roll(out rarity))
                {
                    Debug.Log("No item generated");
                    result = null;
                    return false;
                }
            }

            result = new Item();
            if (category == Category.None)
                categoryTable.Roll(out category);
            List<Trait> traits = new List<Trait>();

            //Constraint value to max positive traits
            numOfPosiviteTraits = Mathf.Clamp(numOfPosiviteTraits, -1, 3);
            //Constraint value to max negative traits
            numOfNegativeTraits = Mathf.Clamp(numOfNegativeTraits, -1, 2);

            int maxTraits = -1;
            if (numOfPosiviteTraits > -1)
                maxTraits = numOfPosiviteTraits;
            if (numOfNegativeTraits > -1 && maxTraits == -1)
                maxTraits = numOfNegativeTraits;
            else if (numOfNegativeTraits > -1)
                maxTraits += numOfNegativeTraits;
            if (maxTraits > _numOfTraitsDict[rarity])
            {
                Debug.Log("No item generated. Invalid number of traits");
                return false;
            }

            if (maxTraits < 0 && traitList == null)
            {
                for (int i = 0; i < _numOfTraitsDict[rarity]; i++)
                {
                    traitTable.Roll(out Trait t);
                    if (t == Trait.None)
                        continue;
                    traitTable.SetProbabilityValue(t, 0);
                    float newNoneValue = 0;
                    foreach (var trait in traitTable.CurrentProbabilities)
                    {
                        newNoneValue += trait.Value.Probability;
                    }
                    traitTable.SetProbabilityValue(Trait.None, newNoneValue);
                    traits.Add(t);
                }
            }
            else
            {
                if (traitList != null)
                {
                    for (int i = 0; i < _numOfTraitsDict[rarity]; i++)
                    {
                        if (!traits.Contains(traitList[i]))
                            traits.Add(traitList[i]);
                    }
                }
                else
                {
                    for (int i = 0; i < numOfPosiviteTraits; i++)
                    {
                        int rTrait = Random.Range((int)Trait.Impressive, (int)Trait.Robust + 1);
                        traits.Add((Trait)rTrait);
                    }

                    for (int i = 0; i < numOfNegativeTraits; i++)
                    {
                        int rTrait = Random.Range((int)Trait.Heavy, (int)Trait.Used + 1);
                        traits.Add((Trait)rTrait);
                    }
                }
            }

            rarityTable.Reset();
            categoryTable.Reset();
            traitTable.Reset();
            result.Init(category, rarity, traits);
            Debug.Log(JsonUtility.ToJson(result, true));
            return true;
        }
        #endregion

        #region Private methods
        private void Init()
        {
            _numOfTraitsDict = new Dictionary<Rarity, int>();
            _traitData = new Dictionary<Trait, TraitData>();
            foreach (var wrapper in _numOfTraits)
            {
                _numOfTraitsDict.Add(wrapper.rarity, wrapper.numOfTraits);
            }

            foreach (Trait trait in System.Enum.GetValues(typeof(Trait)))
            {
                if (trait == Trait.None)
                    continue;
                _traitData.Add(trait, Resources.Load<TraitData>("Traits/" + trait));
            }

            rarityTable.Init();
            categoryTable.Init();
            traitTable.Init();
        }
        #endregion

    }
}