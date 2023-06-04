using SuspiciousGames.SellMe.Core.Items;

namespace SuspiciousGames.SellMe.Core.Generators
{
    [System.Serializable]
    public class RarityProbability : ProbabilityData<Rarity>
    {
        public RarityProbability(ProbabilityData<Rarity> probData) : base(probData) { }

        public RarityProbability(Rarity data, float probability) : base(data, probability) { }
    }
}
