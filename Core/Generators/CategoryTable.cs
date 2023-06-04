using SuspiciousGames.SellMe.Core.Items;
using System.Collections.Generic;
using UnityEngine;

namespace SuspiciousGames.SellMe.Core.Generators
{
    [System.Serializable, CreateAssetMenu(fileName = "CategoryTable", menuName = "ScriptableObjects/ProbabilityTables/Category")]
    public class CategoryTable : ProbabilityTable<Category>
    {
        [SerializeField]
        private List<CategoryProbability> _probabilities = null;

        public override void Init()
        {
            baseProbabilities = new Dictionary<Category, ProbabilityData<Category>>();
            foreach (var item in _probabilities)
            {
                baseProbabilities.Add(item.Data, item);
            }
            Reset();
        }
        public override void Reset()
        {
            copiedProbabilities = new Dictionary<Category, ProbabilityData<Category>>();
            foreach (var item in _probabilities)
            {
                copiedProbabilities.Add(item.Data ,new CategoryProbability(item));
            }
        }
    }
}
