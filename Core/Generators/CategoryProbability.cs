using SuspiciousGames.SellMe.Core.Items;

namespace SuspiciousGames.SellMe.Core.Generators
{
    [System.Serializable]
    public class CategoryProbability : ProbabilityData<Category>
    {
        public CategoryProbability(ProbabilityData<Category> probData) : base(probData) { }

        public CategoryProbability(Category data, float probability) : base(data, probability) { }
    }
}