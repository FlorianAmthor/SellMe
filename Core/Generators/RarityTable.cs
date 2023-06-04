using SuspiciousGames.SellMe.Core.Items;
using System.Collections.Generic;
using UnityEngine;

namespace SuspiciousGames.SellMe.Core.Generators
{
    [System.Serializable, CreateAssetMenu(fileName = "RarityTable", menuName = "ScriptableObjects/ProbabilityTables/Rarity")]
    public class RarityTable : ProbabilityTable<Rarity>
    {
        #region Exposed private Fields
        [SerializeField]
        private List<RarityProbability> _probabilities = null;
        #endregion

        public override void Init()
        {
            baseProbabilities = new Dictionary<Rarity, ProbabilityData<Rarity>>();
            foreach (var item in _probabilities)
            {
                baseProbabilities.Add(item.Data, item);
            }
            Reset();
        }

        public override void Reset()
        {
            copiedProbabilities = new Dictionary<Rarity, ProbabilityData<Rarity>>();
            foreach (var item in _probabilities)
            {
                copiedProbabilities.Add(item.Data, new RarityProbability(item));
            }
        }
    }
}
