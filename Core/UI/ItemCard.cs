using SuspiciousGames.SellMe.Core.Items;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SuspiciousGames.SellMe.Core.UI
{
    public class ItemCard : MonoBehaviour, IZoomable
    {
        #region Exposed private fields
        [SerializeField]
        private TextMeshProUGUI _itemNameText;
        [SerializeField]
        private Image _categoryImage;
        [SerializeField]
        private Image _rarityImage;
        [SerializeField]
        private GameObject _traitsBackground;
        [SerializeField]
        private List<ItemTraitUi> _itemTraitUis;
        [SerializeField]
        private GameObject _zoomCardPrefab;
        #endregion

        #region Private fields
        private static GameObject _zoomCard;
        private Item _item;
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
                    _zoomCard.name = "ZoomCard";
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
            if (_item.TraitDatas.Count == 0)
            {
                foreach (var traitUi in _itemTraitUis)
                    traitUi.gameObject.SetActive(false);
                _traitsBackground.SetActive(false);
            }
            else
            {
                _traitsBackground.SetActive(true);

                for (int i = 0; i < _item.TraitDatas.Count; i++)
                {
                    _itemTraitUis[i].gameObject.SetActive(true);
                    _itemTraitUis[i].Init(_item.TraitDatas[i]);
                }

                for (int i = 2; i > _item.TraitDatas.Count - 1; i--)
                    _itemTraitUis[i].gameObject.SetActive(false);
            }
        }

        public void Zoom()
        {
            ZoomCard.SetActive(true);
            ZoomCard.GetComponentInChildren<ItemCard>().Init(_item);
        }

        public void Reveal()
        {
            _item.isRevealed = true;
        }

        public void PreSelect()
        {
            ShopSystem.ShopManager.Instance.SelectedItem = _item;
        }
        #endregion
    }
}