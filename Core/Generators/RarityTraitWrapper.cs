using SuspiciousGames.SellMe.Core.Items;

namespace SuspiciousGames.SellMe.Core.Generators
{
    [System.Serializable]
    public struct RarityTraitWrapper
    {
        public Rarity rarity;
        public int numOfTraits;

        public RarityTraitWrapper(Rarity r, int num)
        {
            rarity = r;
            numOfTraits = num;
        }
    }

    [System.Serializable]
    public struct TraitDataWrapper
    {
        public Trait trait;
        public TraitData traitData;

        public TraitDataWrapper(Trait t, TraitData tData)
        {
            trait = t;
            traitData = tData;
        }
    }
}