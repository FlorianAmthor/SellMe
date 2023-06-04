using SuspiciousGames.SellMe.Core.Customer;
using SuspiciousGames.SellMe.Core.Items;
using SuspiciousGames.SellMe.Core.UI;
using SuspiciousGames.SellMe.GameEvents;
using SuspiciousGames.SellMe.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SuspiciousGames.SellMe.Core.ShopSystem
{
    public class ShopManager : MonoBehaviour
    {
        #region Singleton
        private static ShopManager _instance;
        public static ShopManager Instance => _instance;

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                _instance = this;
        }
        #endregion

        #region Exposed private fields
        [Header("Prefabs")]
        [SerializeField]
        private GameObject _customerManagerPrefab;
        [Header("Showcase Prices")]
        [SerializeField]
        private int _smallShowcasePrice = 10;
        [SerializeField]
        private int _mediumShowCasePrice = 20;
        [SerializeField]
        private int _largeShowCasePrice = 30;
        [Header("Thresholds")]
        [SerializeField]
        private int _varietyModifierThreshold = 8;
        [SerializeField]
        private float _specialThreshold = 0.5f;
        [SerializeField]
        private int _diverseThreshold = 5;
        [Header("Diverse function parameters")]
        [SerializeField]
        private float _diverseWeight = 1.0f;
        [SerializeField]
        private float _diverseBaseMod = 15.0f;
        [SerializeField]
        private float _diverseMaxModifier = 0.666f;
        [Header("Specialize function parameters")]
        [SerializeField]
        private float _specializedWeight = 1.0f;
        [SerializeField]
        private float _specializedBaseMod = 20.0f;
        [SerializeField]
        private float _specializedMaxModifier = 2.0f;
        [Header("Rarity Modifiers")]
        [SerializeField]
        private float _commonModifier = 1.1f;
        [SerializeField]
        private float _rareModifier = 1.15f;
        [SerializeField]
        private float _epicModifier = 1.2f;
        [SerializeField]
        private float _legendaryModifier = 1.25f;
        [Header("ShowcaseUis")]
        [SerializeField]
        private List<ShowcaseUi> _showcaseUis;
        [SerializeField]
        private List<ShowCaseDataContainer> _showcaseDataContainers;
        #endregion

        #region Private fields
        private Shop _shop;
        private List<int> _sortedCategories;
        private Dictionary<Category, float> _specializedModifierDict;
        private Dictionary<Rarity, float> _rarityModifierDict;
        private HashSet<Category> _specializedCategories;
        private bool _isDiverse;
        private float _diverseBonus = 0.0f;
        #endregion

        #region Properties
        public Item SelectedItem { private get; set; }
        public float SpecializedModifier(Category c) => _specializedModifierDict[c];
        public float DiverseBonus => _diverseBonus;
        public float RarityModifer(Rarity r) => r != Rarity.None ? _rarityModifierDict[r] : 1.0f;
        public int ShowcasePrice(ShowcaseType s) => s == ShowcaseType.Small ? _smallShowcasePrice : s == ShowcaseType.Medium ? _mediumShowCasePrice : _largeShowCasePrice;
        public Shop Shop { get => _shop; }
        public List<ShowcaseUi> ShowcaseUis => _showcaseUis;
        #endregion

        #region MonoBehaviour
        private void Start()
        {
            if (SaveGame.Instance.WasLoaded)
                if (SaveGame.LoadData(SaveId.Shop, out string data))
                {
                    if (_shop == null)
                        _shop = new Shop();
                    _shop.Init();
                    Init();
                }
        }

        private void OnEnable()
        {
            EventManager.Subscribe(GameEventID.SaveGameLoaded, OnSaveGameLoaded);
            if (_shop?.Showcases != null)
                foreach (var showcase in _shop.Showcases)
                {
                    showcase.onItemAdded += OnItemAddedToShop;
                    showcase.onItemAdded += OnItemRemovedFromShop;
                }
        }

        private void OnDisable()
        {
            SaveGame.SaveData(SaveId.Shop, _shop);
            EventManager.Unsubscribe(GameEventID.SaveGameLoaded, OnSaveGameLoaded);
            foreach (var showcase in _shop?.Showcases)
            {
                showcase.onItemAdded += OnItemAddedToShop;
                showcase.onItemAdded += OnItemRemovedFromShop;
            }
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
                SaveGame.SaveData(SaveId.Shop, _shop);
        }
        #endregion

        #region Public methods
        public void SwitchShopMode()
        {
            _shop.SwitchShopMode();
            if (GameManager.Instance.GameState == GameState.Default)
            {
                if (CustomerManager.Instance == null)
                    Instantiate(_customerManagerPrefab);
            }
            else
                CustomerManager.Instance.Finalize(OnCustomerManagerFinalized);
        }

        public bool UnlockShowcaseTier(int indexOfSpot, ShowcaseType showcaseType)
        {
            int showcasePrice = ShowcasePrice(showcaseType);
            bool canUnlock = false;

            if (showcaseType != ShowcaseType.Small)
                if ((int)showcaseType - 1 == (int)_shop.Showcases[indexOfSpot].HighestUnlockedTier)
                    canUnlock = true;
            if (canUnlock)
            {
                var player = Player.Instance;
                if (player.Gold > showcasePrice)
                {
                    player.RemoveGold(showcasePrice);
                    _shop.Showcases[indexOfSpot].UpgradePossibleTier();
                    SaveGame.SaveData(SaveId.Shop, _shop);
                    return true;
                }
                Debug.Log($"Not enough money to unlock a {showcaseType.ToString()} Showcase");
                return false;
            }
            return false;
        }

        public bool UnlockShowcaseTier(Showcase showcase, ShowcaseType showcaseType)
        {
            int indexOfShowcase = -1;
            for (int i = 0; i < _shop.Showcases.Count; i++)
            {
                if (_shop.Showcases[i] == showcase)
                {
                    indexOfShowcase = i;
                    break;
                }
            }
            if (indexOfShowcase != -1)
                return UnlockShowcaseTier(indexOfShowcase, showcaseType);
            return false;
        }

        public bool UnlockSpot()
        {
            int spotsUnlocked = _shop.Showcases.Count;
            if (spotsUnlocked < Shop.MAXSHOWCASES)
            {
                var player = Player.Instance;
                int spotPrice = spotsUnlocked * _smallShowcasePrice + _largeShowCasePrice;
                if (player.Gold > spotPrice)
                {
                    player.RemoveGold(spotPrice);
                    _shop.UnlockSpot(out Showcase showcase);
                    showcase.onItemAdded += OnItemAddedToShop;
                    showcase.onItemRemoved += OnItemRemovedFromShop;
                    SaveGame.SaveData(SaveId.Shop, _shop);
                    return true;
                }
                Debug.Log("Not enough money!");
                return false;
            }
            Debug.Log("Max spots reached!");
            return false;
        }

        public void PlaceSelectedItemInShowcase(int showcaseIndex, int showcaseSlotIndex)
        {
            _shop.Showcases[showcaseIndex].AddItemAt(showcaseSlotIndex, SelectedItem);
        }

        public void ResetSelectedItem()
        {
            SelectedItem = null;
        }
        #endregion

        #region Callbacks
        private void OnCustomerManagerFinalized()
        {
            //TODO: Show sell summary?
        }

        private void OnItemRemovedFromShop(Item item)
        {
            _shop.PresentedItems.Remove(item);
            _shop.NumberOfCategories[item.Category]--;
            _sortedCategories = new List<int>(_shop.NumberOfCategories.Values);
            _sortedCategories.Sort();
            CalculateShopAppearance();
            SaveGame.SaveData(SaveId.Shop, _shop);
        }

        private void OnItemAddedToShop(Item item)
        {
            _shop.PresentedItems.Add(item);
            if (_shop.NumberOfCategories.ContainsKey(item.Category))
                _shop.NumberOfCategories[item.Category]++;
            else
                _shop.NumberOfCategories.Add(item.Category, 1);
            _sortedCategories = new List<int>(_shop.NumberOfCategories.Values);
            _sortedCategories.Sort();
            CalculateShopAppearance();
            SaveGame.SaveData(SaveId.Shop, _shop);
        }

        private void OnSaveGameLoaded(GameEvent gameEvent)
        {
            if (SaveGame.LoadData(SaveId.Shop, out string data))
            {
                if (_shop == null)
                    _shop = new Shop();
                _shop.InitFromData(data);
                Init();
            }
        }
        #endregion

        #region Private methods
        internal void Init()
        {
            _rarityModifierDict = new Dictionary<Rarity, float>()
            {
                { Rarity.Common, _commonModifier },
                { Rarity.Rare, _rareModifier },
                { Rarity.Epic, _epicModifier},
                { Rarity.Legendary, _legendaryModifier}
            };
            _specializedModifierDict = new Dictionary<Category, float>();

            _specializedCategories = new HashSet<Category>();
            _sortedCategories = new List<int>(_shop.NumberOfCategories.Values);
            _sortedCategories.Sort();

            foreach (var showcase in _shop.Showcases)
            {
                showcase.onItemAdded += OnItemAddedToShop;
                showcase.onItemRemoved += OnItemRemovedFromShop;
                showcase.CalcRarityModifier();
            }

            foreach (Category key in Enum.GetValues(typeof(Category)))
            {
                if (key == Category.None)
                    continue;
                if (!_specializedModifierDict.ContainsKey(key))
                    _specializedModifierDict.Add(key, 1.0f);
                else
                    _specializedModifierDict[key] = 1.0f;
            }
            CalculateShopAppearance();
            if (GameManager.Instance.GameState == GameState.Selling)
            {
                if (CustomerManager.Instance == null)
                    Instantiate(_customerManagerPrefab);
                CustomerManager.Instance.Init();
            }
        }

        private void ApplyDiverseModifier()
        {
            Debug.Log("Diverse: " + _isDiverse);
            if (!_isDiverse && _diverseBonus == 0.0f)
            {
                return;
            }
            else if (!_isDiverse)
            {
                _diverseBonus = 0.0f;
                return;
            }
            _specializedCategories.Clear();
            int x = _shop.NumberOfCategories.Count - _diverseThreshold;
            _diverseBonus = Mathf.Min((_diverseWeight * (x * x) + _diverseBaseMod) / 100, _diverseMaxModifier);
        }

        private void ApplySpecializeModifier()
        {
            if (_specializedModifierDict == null)
            {
                _specializedModifierDict = new Dictionary<Category, float>();

                foreach (Category key in Enum.GetValues(typeof(Category)))
                {
                    if (key == Category.None)
                        continue;
                    if (!_specializedModifierDict.ContainsKey(key))
                        _specializedModifierDict.Add(key, 1.0f);
                    else
                        _specializedModifierDict[key] = 1.0f;
                }
            }

            foreach (Category key in Enum.GetValues(typeof(Category)))
            {
                if (key == Category.None)
                    continue;
                _specializedModifierDict[key] = 1.0f;
            }

            if (_specializedCategories.Count == 0)
                return;
            _isDiverse = false;

            int x = 0;
            float mod = 0.0f;

            foreach (Category category in _specializedCategories)
            {
                x = _shop.NumberOfCategories[category] - Mathf.CeilToInt(_shop.NumOfItems * _specialThreshold);

                mod = Mathf.Min(((_specializedWeight * (x * x) + _specializedBaseMod) / 100) + 1.0f, _specializedMaxModifier);
                _specializedModifierDict[category] = mod;
                Debug.Log("Specialized: " + category.ToString());
            }
        }
        #endregion

        #region Internal methods
        private void CalculateShopAppearance()
        {
            if (_shop.NumOfItems < _varietyModifierThreshold)
                return;

            float median = _sortedCategories.GetMedian();
            float mean = _sortedCategories.GetMean();
            float minRange = median - mean < 0 ? 0 : median - mean;
            float maxRange = median + mean;

            _specializedCategories.Clear();
            _isDiverse = _shop.NumberOfCategories.Count >= _diverseThreshold;

            foreach (Category c in Enum.GetValues(typeof(Category)))
            {
                if (!_shop.NumberOfCategories.ContainsKey(c))
                    continue;

                if (_shop.NumberOfCategories[c] / _shop.NumOfItems >= _specialThreshold)
                    _specializedCategories.Add(c);

                if (_specializedCategories.Count > 0 || !_isDiverse)
                    continue;

                if (_shop.NumberOfCategories[c] < minRange || _shop.NumberOfCategories[c] > maxRange)
                    _isDiverse = false;
            }

            ApplySpecializeModifier();
            ApplyDiverseModifier();

            EventManager.TriggerEvent(GameEventID.ShopAppearanceChanged);
        }

        internal Vector3 GetViewingPointForShowcase(Showcase showcase)
        {
            int index = _shop.Showcases.IndexOf(showcase);
            var viewPoints = _showcaseDataContainers[index].ViewPoints;
            return viewPoints[UnityEngine.Random.Range(0, viewPoints.Count - 1)].position;
        }
        #endregion
    }
}
