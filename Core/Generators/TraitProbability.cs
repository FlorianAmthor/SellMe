using SuspiciousGames.SellMe.Core.Items;

namespace SuspiciousGames.SellMe.Core.Generators
{
    [System.Serializable]
    public class TraitProbability : ProbabilityData<Trait>
    {
        public TraitProbability(ProbabilityData<Trait> probData) : base(probData) { }

        public TraitProbability(Trait data, float probability) : base(data, probability) { }
    }
}
