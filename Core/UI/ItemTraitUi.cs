using SuspiciousGames.SellMe.Core.Items;
using TMPro;
using UnityEngine;

namespace SuspiciousGames.SellMe.Core.UI
{
    public class ItemTraitUi : MonoBehaviour
    {
        #region Exposed private fields
        [SerializeField]
        private TextMeshProUGUI _traitNamexText;
        [SerializeField]
        private GameObject _postiveTraitImage;
        [SerializeField]
        private GameObject _negativeTraitImage;
        #endregion

        #region Private fields
        private TraitData _tData;
        #endregion

        #region Public methods
        public void Init(TraitData tData)
        {
            _tData = tData;
            var itemcard = GetComponentInParent<ItemCard>();
            var parent = itemcard.transform.parent;
            bool zoomed;
            if (parent == null)
                zoomed = false;
            else
                zoomed = parent.name == "ZoomCard";
            if (_traitNamexText != null && zoomed)
            {
                _traitNamexText.gameObject.SetActive(true);
                _traitNamexText.text = _tData.Name;
            }

            bool isPositive = _tData.PriceInfluenceFactor > 1;
            _postiveTraitImage.SetActive(isPositive);
            _negativeTraitImage.SetActive(!isPositive);
        }
        #endregion
    }
}