using SuspiciousGames.SellMe.Core;
using SuspiciousGames.SellMe.Core.Generators;
using SuspiciousGames.SellMe.Core.Items;
using SuspiciousGames.SellMe.Core.UI;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField]
    private GameObject _itemCardPrefab;
    [SerializeField]
    private Transform _listTransform;

    private List<GameObject> _itemCards;
    private List<Item> _items = new List<Item>();
    private Rarity _activeRarityFilter = Rarity.None;
    private Category _activeCategoryFilter = Category.None;

    private void OnEnable()
    {
        var player = Player.Instance;
        if (_itemCards == null)
            _itemCards = new List<GameObject>();
        ResetFilter();
        if (_items != player.Inventory.Items)
        {
            while (_itemCards.Count != 0)
            {
                var itemCard = _itemCards[0];
                _itemCards.RemoveAt(0);
                Destroy(itemCard);
            }
            _items = new List<Item>(player.Inventory.Items);
            foreach (var item in _items)
            {
                var itemCard = Instantiate(_itemCardPrefab).GetComponent<ItemCard>();
                itemCard.Init(item);
                itemCard.transform.SetParent(_listTransform, false);
                _itemCards.Add(itemCard.gameObject);
            }
            SortItems();
        }
    }

    public void SetCategoryFilter(int enumValue)
    {
        if (_activeCategoryFilter == Category.None || _activeCategoryFilter != (Category)enumValue)
            _activeCategoryFilter = (Category)enumValue;
        else if (_activeCategoryFilter == (Category)enumValue)
            _activeCategoryFilter = Category.None;
        ApplyFilter();
    }

    public void SetRarityFilter(int enumValue)
    {
        if (_activeRarityFilter == Rarity.None || _activeRarityFilter != (Rarity)enumValue)
            _activeRarityFilter = (Rarity)enumValue;
        else if (_activeRarityFilter == (Rarity)enumValue)
            _activeRarityFilter = Rarity.None;
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        bool rarityFilter = _activeRarityFilter != Rarity.None;
        bool categoryFilter = _activeCategoryFilter != Category.None;
        foreach (var gObj in _itemCards)
        {
            var item = gObj.GetComponent<ItemCard>().Item;
            if (rarityFilter && categoryFilter)
                gObj.SetActive(item.Category == _activeCategoryFilter && item.Rarity == _activeRarityFilter);
            else if (rarityFilter && !categoryFilter)
                gObj.SetActive(item.Rarity == _activeRarityFilter);
            else if (!rarityFilter && categoryFilter)
                gObj.SetActive(item.Category == _activeCategoryFilter);
            else
                ResetFilter();
        }
    }

    public void ResetFilter()
    {
        _activeCategoryFilter = Category.None;
        _activeRarityFilter = Rarity.None;
        foreach (var gObj in _itemCards)
            gObj.SetActive(true);
    }

    public void SortItems()
    {
        _itemCards.Sort((gObj1, gObj2) =>
        {
            var item1 = gObj1.GetComponent<ItemCard>().Item;
            var item2 = gObj2.GetComponent<ItemCard>().Item;
            return item1.Rarity.CompareTo(item2.Rarity);
        });
    }
}
