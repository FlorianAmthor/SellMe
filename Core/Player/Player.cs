using SuspiciousGames.SellMe.Core.Items;
using SuspiciousGames.SellMe.GameEvents;
using SuspiciousGames.SellMe.Utility;
using UnityEngine;

namespace SuspiciousGames.SellMe.Core
{
    [System.Serializable]
    public class Player
    {
        #region Singleton
        private static Player _instance;

        public static Player Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Player();
                _instance.Load();
                return _instance;
            }
        }
        #endregion

        #region Exposed private fields
        [SerializeField]
        private int _startGold;
        #endregion

        #region Private fields
        [SerializeField, HideInInspector] private Inventory _inventory = new Inventory();
        [SerializeField, HideInInspector] private int _gold;
        #endregion

        #region Properties
        public string Name { get; set; }
        public int Gold => _gold;
        public Inventory Inventory => _inventory;
        #endregion

        #region Public methods
        public void AddGold(int amount)
        {
            _gold += amount;
            Save();
            EventManager.TriggerEvent(GameEventID.GoldReceived);
        }

        public void RemoveGold(int amount)
        {
            _gold -= amount;
            if (_gold < 0)
                _gold = 0;
            Save();
            EventManager.TriggerEvent(GameEventID.GoldLost);
        }

        public void Load()
        {
            SaveGame.LoadData(SaveId.Player, out string json);
            JsonUtility.FromJsonOverwrite(json, _instance);
            foreach (var item in _inventory.Items)
            {
                if (item.Traits.Count == 0)
                    continue;
                if(item.Traits.Count != item.TraitDatas.Count)
                    foreach (var trait in item.Traits)
                        item.TraitDatas.Add(Resources.Load<TraitData>("Traits/" + trait));
            }
        }

        public void Save()
        {
            if (_instance != null)
                SaveGame.SaveData(SaveId.Player, _instance);
        }
        #endregion
    }
}
