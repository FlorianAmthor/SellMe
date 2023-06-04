using SuspiciousGames.SellMe.Core.Items;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuspiciousGames.SellMe.Core.UI
{
    public class ShowcaseSlotUi : MonoBehaviour
    {
        #region Exposed private fields
        [SerializeField]
        private TMPro.TextMeshProUGUI _itemNameText;
        [SerializeField]
        private TMPro.TextMeshProUGUI _itemValueText;
        [SerializeField]
        private Image _categoryImage;
        [SerializeField]
        private Image _rarityImage;
        //[SerializeField]
        //private GameObject _traitBackgroundObject;
        [SerializeField]
        private List<ItemTraitUi> _itemTraitUis;
        #endregion

        #region Private fields
        private Item _item = Item.empty;
        #endregion

        public void Init(Item item)
        {
            if(item == Item.empty)
            {
                //TODO: Handle empty item for ui
                return;
            }
            _item = item;
            _itemNameText.text = _item.Name;
            _itemValueText.text = _item.Value.ToString();
            _rarityImage.sprite = Resources.Load<Sprite>("Sprites/Rarity/" + _item.Rarity);
            _categoryImage.sprite = Resources.Load<Sprite>("Sprites/Category/" + _item.Category);
            if (_item.TraitDatas.Count == 0)
            {
                foreach (var traitUi in _itemTraitUis)
                    traitUi.gameObject.SetActive(false);
                //_traitBackgroundObject.SetActive(false);
            }
            else
            {
                //_traitBackgroundObject.SetActive(true);

                for (int i = 0; i < _item.TraitDatas.Count; i++)
                {
                    _itemTraitUis[i].gameObject.SetActive(true);
                    _itemTraitUis[i].Init(_item.TraitDatas[i]);
                }

                for (int i = 2; i > _item.TraitDatas.Count - 1; i--)
                    _itemTraitUis[i].gameObject.SetActive(false);
            }
        }

        public void UpdateValues()
        {
            _itemValueText.text = _item.Value.ToString();
        }
    }
}