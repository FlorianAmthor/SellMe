using SuspiciousGames.SellMe.Core.Items;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SuspiciousGames.SellMe.Core.ShopSystem
{
    [Serializable]
    public class Showcase : ItemContainer
    {
        #region Events
        public override event Action<Item> onItemAdded;
        public override event Action<Item> onItemRemoved;
        #endregion

        #region Private exposed fields
        [SerializeField]
        private ShowcaseType _type;
        #endregion

        #region Private fields
        private float _rarityModifier = 1.0f;
        [SerializeField, HideInInspector]
        private ShowcaseType _highestUnlockedTier = ShowcaseType.Small;
        #endregion

        #region Properties
        public List<Item> Items { get => items; }
        public ShowcaseType Type { get => _type; }
        public int Size { get => ((int)_type) + 1; }
        public float RarityModifier { get => _rarityModifier; }
        public ShowcaseType HighestUnlockedTier { get => _highestUnlockedTier; }
        #endregion

        public Showcase() : base()
        {
            _highestUnlockedTier = ShowcaseType.Small;
            _type = _highestUnlockedTier;
            maxCapacity = (int)_type + 1;
            items = new List<Item>();
            for (int i = 0; i < maxCapacity; i++)
            {
                items.Add(Item.empty);
            }
            onItemAdded += OnItemAdded;
            onItemRemoved += OnItemRemoved;
        }

        /// <summary>
        /// Initializes the object according to <paramref name="type"/>
        /// </summary>
        /// <param name="type">Size of the showcase</param>
        public Showcase(ShowcaseType type)
        {
            _type = type;
            maxCapacity = (int)_type + 1;
            items = new List<Item>();
            for (int i = 0; i < maxCapacity; i++)
            {
                items.Add(Item.empty);
            }
            onItemAdded += OnItemAdded;
            onItemRemoved += OnItemRemoved;
        }

        private Showcase(int capacity) : base(capacity)
        {
            if (capacity > (int)ShowcaseType.Large + 1)
                _type = ShowcaseType.Large;
            else if (capacity < (int)ShowcaseType.Small + 1)
                _type = ShowcaseType.Small;
            else
                _type = (ShowcaseType)(capacity - 1);
            maxCapacity = (int)_type + 1;
            items = new List<Item>();
            for (int i = 0; i < maxCapacity; i++)
            {
                items.Add(Item.empty);
            }
            onItemAdded += OnItemAdded;
            onItemRemoved += OnItemRemoved;
        }

        ~Showcase()
        {
            onItemAdded -= OnItemAdded;
            onItemRemoved -= OnItemRemoved;
        }

        #region Public methods
        public bool AddItemAt(int index, Item itemToAdd)
        {
            if (index <= maxCapacity - 1)
            {
                var player = Player.Instance;
                if (items[index].Rarity != Rarity.None)
                {
                    var item = items[index];
                    item.Container = player.Inventory;
                    onItemRemoved?.Invoke(item);
                }
                itemToAdd.Container = this;
                items[index] = itemToAdd;
                int i = ShopManager.Instance.Shop.Showcases.IndexOf(this);
                ShopManager.Instance.ShowcaseUis[i].InitSlot(index, items[index]);
                ShopManager.Instance.ShowcaseUis[i].OnSlotContentChange();
                player.Save();
                onItemAdded?.Invoke(itemToAdd);
                return true;
            }
            else
                return false;
        }

        public override bool AddItem(Item itemToAdd)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Rarity == Rarity.None)
                {
                    var player = Player.Instance;
                    itemToAdd.Container = this;
                    items[i] = itemToAdd;
                    int index = ShopManager.Instance.Shop.Showcases.IndexOf(this);
                    ShopManager.Instance.ShowcaseUis[index].InitSlot(i, items[i]);
                    ShopManager.Instance.ShowcaseUis[index].OnSlotContentChange();
                    player.Save();
                    onItemAdded?.Invoke(itemToAdd);
                    return true;
                }
            }
            return false;
        }

        public override bool RemoveItem(int index)
        {
            if (items[index] != null)
            {
                var item = items[index];
                items[index] = null;
                var player = Player.Instance;
                item.Container = player.Inventory;
                player.Save();
                int i = ShopManager.Instance.Shop.Showcases.IndexOf(this);
                ShopManager.Instance.ShowcaseUis[i].InitSlot(index, Item.empty);
                ShopManager.Instance.ShowcaseUis[i].OnSlotContentChange();
                onItemRemoved?.Invoke(item);
                return true;
            }
            return false;
        }

        public override bool RemoveItem(Item item)
        {
            if (items.Contains(item))
            {
                items[items.IndexOf(item)] = null;
                var player = Player.Instance;
                item.Container = player.Inventory;
                player.Save();
                int i = ShopManager.Instance.Shop.Showcases.IndexOf(this);
                ShopManager.Instance.ShowcaseUis[i].InitSlot(items.IndexOf(item), Item.empty);
                ShopManager.Instance.ShowcaseUis[i].OnSlotContentChange();
                onItemRemoved?.Invoke(item);
                return true;
            }
            return false;
        }
        #endregion

        #region Internal methods
        /// <summary>
        /// Switches the showcase to another size and returns surplus items that were removed if the showcase was made smaller
        /// </summary>
        /// <param name="type">Type to change into</param>
        /// <param name="surplusItems">Surplus items if new showcase type is smaller than current showcase</param>
        /// <returns></returns>
        internal bool SwitchToType(ShowcaseType type, out List<Item> surplusItems)
        {
            surplusItems = new List<Item>();
            if (type > _highestUnlockedTier || type == _type)
                return false;
            _type = type;
            maxCapacity = (int)_type + 1;
            int newCapacity = maxCapacity;

            if (newCapacity < items.Count)
            {
                surplusItems = new List<Item>(items.GetRange(newCapacity, items.Count - newCapacity));
                foreach (var item in surplusItems)
                    RemoveItem(item);
                items.RemoveRange(newCapacity, items.Count - newCapacity);
            }
            else
            {
                for (int i = items.Count; i < newCapacity; i++)
                    items.Add(null);
            }
            return true;
        }

        internal void UpgradePossibleTier()
        {
            if (_highestUnlockedTier == ShowcaseType.Large)
                return;
            _highestUnlockedTier++;
            SwitchToType(_highestUnlockedTier, out _);
        }

        /// <summary>
        /// Calculates the rarity modifier of the showcase by calculting the average of all modifiers
        /// </summary>
        internal void CalcRarityModifier()
        {
            float modifierSum = 0;
            int numOfItems = 0;
            foreach (var item in items)
            {
                if (item.Rarity == Rarity.None)
                    continue;
                modifierSum += ShopManager.Instance.RarityModifer(item.Rarity);
                numOfItems++;
            }
            if (numOfItems <= 1)
                _rarityModifier = 1.0f;
            else
                _rarityModifier = modifierSum / numOfItems;
        }
        #endregion

        #region Private Methods
        private void OnItemAdded(Item item)
        {
            CalcRarityModifier();
        }

        private void OnItemRemoved(Item item)
        {
            CalcRarityModifier();
        }
        #endregion
    }
}