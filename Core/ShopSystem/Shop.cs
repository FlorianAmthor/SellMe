using SuspiciousGames.SellMe.Core.Items;
using SuspiciousGames.SellMe.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SuspiciousGames.SellMe.Core.ShopSystem
{
    [Serializable]
    public class Shop
    {
        public const int MAXSHOWCASES = 6;

        #region Exposed private fields
        [SerializeField]
        private int _rentPerSpot;
        [SerializeField, HideInInspector]
        private int _startSpots = 1;
        [SerializeField, HideInInspector]
        private List<Room> _unlockedRooms = new List<Room>();
        [SerializeField, HideInInspector]
        private List<Showcase> _showcases;
        #endregion

        #region Private fields
        private Dictionary<Category, int> _numberOfCategories;
        private List<Item> _presentedItems;
        private int _spotRent;
        private int _roomRent;
        private int _currentRent;
        #endregion

        #region Properties
        public List<Showcase> Showcases => _showcases;
        public int UnlockedSpots => _showcases.Count;
        public List<Room> UnlockedRooms => _unlockedRooms;
        public Dictionary<Category, int> NumberOfCategories => _numberOfCategories;
        public int NumOfItems { get => _presentedItems.Count; }
        public List<Item> PresentedItems => _presentedItems;
        public int CurrentRent => _currentRent;
        #endregion

        public Shop()
        {
            _showcases = new List<Showcase>();
            _numberOfCategories = new Dictionary<Category, int>();
            _presentedItems = new List<Item>();
            foreach (var showcase in _showcases)
            {
                foreach (var item in showcase.Items)
                {
                    _presentedItems.Add(item);
                }
            }
            CalcRent();
        }

        #region Public methods
        public void Init()
        {
            SaveGame.LoadData(SaveId.Shop, out string data);
            InitFromData(data);
        }

        public void InitFromData(string jsonData)
        {
            JsonUtility.FromJsonOverwrite(jsonData, this);

            if (_showcases.Count < _showcases.Capacity)
            {
                for (int i = _showcases.Count; i < _showcases.Capacity; i++)
                {
                    var showcase = new Showcase();
                    _showcases.Add(showcase);
                    SaveGame.SaveData(SaveId.Shop, this);
                }
            }

            _presentedItems = new List<Item>();
            foreach (var showcase in _showcases)
            {
                foreach (var item in showcase.Items)
                {
                    _presentedItems.Add(item);
                    if (_numberOfCategories.ContainsKey(item.Category))
                        _numberOfCategories[item.Category]++;
                    else
                        _numberOfCategories.Add(item.Category, 1);
                }
            }
            CalcRent();
        }

        public void PayRent()
        {
            var player = Player.Instance;
            player.RemoveGold(_currentRent);
        }
        #endregion

        #region Internal methods
        internal void BuyItem(Item itemToBuy)
        {
            if(itemToBuy != null)
            {
                var player = Player.Instance;
                player.AddGold(itemToBuy.Value);
                itemToBuy.Container.RemoveItem(itemToBuy);
                SaveGame.SaveData(SaveId.Shop, this);
            }
                GameTime.AddTime(hours: 2);
        }

        internal bool UnlockSpot(out Showcase showcase)
        {
            showcase = null;
            if (_showcases.Count >= MAXSHOWCASES)
                return false;
            showcase = new Showcase();
            _showcases.Add(showcase);
            return true;
        }

        internal void SwitchShopMode()
        {
            if (GameManager.Instance.GameState == GameState.Default)
                GameManager.Instance.SwitchToGameState(GameState.Selling);
            else
                GameManager.Instance.SwitchToGameState(GameState.Default);
        }
        #endregion

        #region Private methods
        private void CalcRent()
        {
            _roomRent = 0;

            foreach (var room in _unlockedRooms)
                _roomRent += room.Rent;

            _spotRent = _showcases.Count * _rentPerSpot;

            _currentRent = _spotRent + _roomRent;
        }
        #endregion
    }
}
