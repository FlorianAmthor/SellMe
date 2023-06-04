using UnityEngine;

namespace SuspiciousGames.SellMe.Core.Items
{
    [System.Serializable, CreateAssetMenu(fileName = "newTrait", menuName = "ScriptableObjects/Trait")]
    public class TraitData : ScriptableObject
    {
        #region Exposed private fields
        [SerializeField] private string _name = null;
        [SerializeField] private Trait _trait = Trait.None;
        [SerializeField] private float _priceInfluenceFactor = 0.0f;
        #endregion

        #region Properties
        public string Name => _name;
        public Trait Trait => _trait;
        public float PriceInfluenceFactor => _priceInfluenceFactor;
        #endregion
    }
}