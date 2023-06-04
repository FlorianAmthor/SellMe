using SuspiciousGames.SellMe.Core.Items;
using UnityEngine;
using UnityEngine.UI;

namespace SuspiciousGames.SellMe.Core.UI
{
    public class SummaryItem : MonoBehaviour, IZoomable
    {
        #region Exposed private fields
        [SerializeField]
        private Image _rarityImage;
        [SerializeField]
        private Image _categoryImage;
        [SerializeField]
        private TMPro.TextMeshProUGUI _itemNameText;
        [SerializeField]
        private GameObject _zoomCardPrefab;
        #endregion

        #region Private fields
        private Item _item;
        private static GameObject _zoomCard;
        #endregion

        #region Properties
        public Item Item { get => _item; }

        private GameObject ZoomCard
        {
            get
            {
                if (_zoomCard == null)
                {
                    _zoomCard = Instantiate(_zoomCardPrefab);
                    _zoomCard.transform.SetParent(FindObjectOfType<Canvas>().transform);
                    var rectTransform = _zoomCard.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = Vector2.zero;
                    rectTransform.anchoredPosition3D = Vector3.zero;
                    rectTransform.offsetMax = Vector2.zero;
                    rectTransform.offsetMin = Vector2.zero;
                    rectTransform.pivot = new Vector2(0.5f, 0.5f);
                }
                return _zoomCard;
            }
        }
        #endregion

        #region Public methods
        public void Init(Item item)
        {
            _item = item;
            _itemNameText.text = _item.Name;
            _rarityImage.sprite = Resources.Load<Sprite>("Sprites/Rarity/" + _item.Rarity);
            _categoryImage.sprite = Resources.Load<Sprite>("Sprites/Category/" + _item.Category);
        }

        public void Zoom()
        {
            ZoomCard.SetActive(true);
            ZoomCard.GetComponentInChildren<ItemCard>().Init(_item);
        }
        #endregion
    }
}
