using SuspiciousGames.SellMe.Core.ShopSystem;
using SuspiciousGames.SellMe.GameEvents;
using System.Collections.Generic;
using UnityEngine;

namespace SuspiciousGames.SellMe.Core.Items
{
    [System.Serializable]
    public class Item
    {
        public static Item empty = new Item();

        #region Exposed private fields
        [SerializeField, HideInInspector]
        private string _name;
        [SerializeField, HideInInspector]
        private Category _category;
        [SerializeField, HideInInspector]
        private Rarity _rarity;
        [SerializeField, HideInInspector]
        private List<Trait> _traits;
        [SerializeField, HideInInspector]
        private int _value;
        [SerializeField, HideInInspector]
        private ItemContainer _container;
        #endregion

        #region Private fields
        private int _baseValue;
        private List<TraitData> _traitDatas;
        #endregion

        #region Public fields
        public ItemContainer Container
        {
            get
            {
                return _container;
            }
            set
            {
                if (value.GetType() == typeof(Showcase))
                    EventManager.Subscribe(GameEventID.ShopAppearanceChanged, OnShopAppearanceChanged);
                else
                    EventManager.Unsubscribe(GameEventID.ShopAppearanceChanged, OnShopAppearanceChanged);
                _container = value;
            }
        }

        public bool isRevealed = false;
        #endregion

        #region Properties
        public string Name { get => _name; set => _name = value; }
        public Category Category => _category;
        public Rarity Rarity => _rarity;
        public List<TraitData> TraitDatas => _traitDatas;
        public List<Trait> Traits => _traits;
        public int Value => _value;
        #endregion

        public Item()
        {
            _traitDatas = new List<TraitData>();
            if (_traits != null)
            {
                foreach (var trait in _traits)
                {
                    _traitDatas.Add(Resources.Load<TraitData>("Traits/" + trait));
                }
            }
        }

        public Item(Rarity r, Category c, List<Trait> traits = null, string name = "")
        {
            _rarity = r;
            _category = c;
            _traits = new List<Trait>();
            if (traits == null)
                _traits = new List<Trait>();
            else
                _traits = new List<Trait>(traits);
            _traitDatas = new List<TraitData>();
            foreach (var trait in _traits)
            {
                _traitDatas.Add(Resources.Load<TraitData>("Traits/" + trait));
            }
            if (name == "")
                GenerateName();
            CalcValue();
        }

        public Item(Item itemToCopy)
        {
            _name = itemToCopy._name;
            _rarity = itemToCopy._rarity;
            _category = itemToCopy._category;
            _traits = itemToCopy._traits;
            _traitDatas = itemToCopy._traitDatas;
            _baseValue = itemToCopy._baseValue;
            _container = itemToCopy._container;
            CalcValue();
        }

        #region Public methods
        public void Init(Category category, Rarity rarity, ICollection<Trait> traits, int baseValue = 0)
        {
            _category = category;
            _rarity = rarity;
            _traits = new List<Trait>(traits);
            foreach (var trait in _traits)
            {
                _traitDatas.Add(Resources.Load<TraitData>("Traits/" + trait));
            }
            _name = GenerateName();
            if (baseValue != 0)
                _baseValue = baseValue;
            else
                _baseValue = (int)rarity;
            CalcValue();
        }
        #endregion

        #region Private methods
        private string GenerateName()
        {
            string result = string.Empty;

            //result += _rarity.ToString() + ": ";

            //Get names from name database
            int numTraits = _traits.Count;
            for (int i = 0; i < numTraits; i++)
            {
                if (i < _traits.Count - 1)
                    result += _traitDatas[i].Name + " and ";
                else
                    result += _traitDatas[i].Name + " ";
            }
            result += _category.ToString();

            return result;
        }

        private void CalcValue()
        {
            float traitFactor = 1.0f;
            float specializeModifier = 1.0f;
            float rarityModifier = 1.0f;
            foreach (var traitData in _traitDatas)
                traitFactor *= traitData.PriceInfluenceFactor;

            if (_container != null && _container.GetType() == typeof(Showcase))
            {
                specializeModifier = ShopManager.Instance.SpecializedModifier(_category);
                rarityModifier = ((Showcase)_container).RarityModifier;
            }
            _value = Mathf.CeilToInt(_baseValue * traitFactor * specializeModifier * rarityModifier);
        }

        private void OnShopAppearanceChanged(GameEvent gameEvent)
        {
            CalcValue();
        }
        #endregion
    }
}