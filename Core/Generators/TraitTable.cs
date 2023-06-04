using SuspiciousGames.SellMe.Core.Items;
using System.Collections.Generic;
using UnityEngine;

namespace SuspiciousGames.SellMe.Core.Generators
{
    [System.Serializable, CreateAssetMenu(fileName = "TraitTable", menuName = "ScriptableObjects/ProbabilityTables/Traits")]
    public class TraitTable : ProbabilityTable<Trait>
    {
        #region Exposed private Fields
        [SerializeField]
        private List<TraitProbability> _probabilities = null;
        #endregion

        public override void Init()
        {
            baseProbabilities = new Dictionary<Trait, ProbabilityData<Trait>>();
            foreach (var item in _probabilities)
            {
                baseProbabilities.Add(item.Data, item);
            }
            Reset();
        }

        public override void Reset()
        {
            copiedProbabilities = new Dictionary<Trait, ProbabilityData<Trait>>();
            foreach (var item in _probabilities)
            {
                copiedProbabilities.Add(item.Data, new TraitProbability(item));
            }
        }
    }
}
