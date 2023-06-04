using SuspiciousGames.SellMe.GameEvents;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SuspiciousGames.SellMe.Utility
{
    public class SaveGame
    {
        #region Singleton
        private static SaveGame _instance;

        public static SaveGame Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SaveGame();
                    _instance._wasLoaded = false;
                    _instance._savedData = new Dictionary<SaveId, string>(_baseSaveGame);
                }
                return _instance;
            }
        }
        #endregion

        #region BaseSaveGame
        private static Dictionary<SaveId, string> _baseSaveGame = new Dictionary<SaveId, string>()
        {
            {SaveId.ActiveAdventure, ""},
            {SaveId.AdventureList, "{\"items\":[]}"},
            {SaveId.GameCycleAdventureSteps, "0"},
            {SaveId.GameOver, "false"},
            {SaveId.GameState, "0" },
            {SaveId.GameTime, "0"},
            {SaveId.NewGameCycle, "true"},
            {SaveId.Player,"{\"_startGold\":10,\"_inventory\":{\"maxCapacity\":100,\"items\":[{\"_name\":\"Chest\",\"_category\":6,\"_rarity\":1,\"_traits\":[],\"_value\":1},{\"_name\":\"Used Legs\",\"_category\":7,\"_rarity\":3,\"_traits\":[5],\"_value\":3},{\"_name\":\"Sword\",\"_category\":1,\"_rarity\":3,\"_traits\":[],\"_value\":3},{\"_name\":\"Axe\",\"_category\":3,\"_rarity\":1,\"_traits\":[],\"_value\":1},{\"_name\":\"Sword\",\"_category\":1,\"_rarity\":3,\"_traits\":[],\"_value\":3},{\"_name\":\"Legs\",\"_category\":7,\"_rarity\":1,\"_traits\":[],\"_value\":1},{\"_name\":\"Impressive Sword\",\"_category\":1,\"_rarity\":8,\"_traits\":[1],\"_value\":10},{\"_name\":\"Bow\",\"_category\":2,\"_rarity\":1,\"_traits\":[],\"_value\":1},{\"_name\":\"Mace\",\"_category\":4,\"_rarity\":8,\"_traits\":[],\"_value\":8},{\"_name\":\"Robust Legs\",\"_category\":7,\"_rarity\":3,\"_traits\":[3],\"_value\":4},{\"_name\":\"Head\",\"_category\":5,\"_rarity\":1,\"_traits\":[],\"_value\":1},{\"_name\":\"Mace\",\"_category\":4,\"_rarity\":1,\"_traits\":[],\"_value\":1},{\"_name\":\"Heavy Bow\",\"_category\":2,\"_rarity\":3,\"_traits\":[4],\"_value\":3},{\"_name\":\"Robust Chest\",\"_category\":6,\"_rarity\":3,\"_traits\":[3],\"_value\":4},{\"_name\":\"Axe\",\"_category\":3,\"_rarity\":1,\"_traits\":[],\"_value\":1},{\"_name\":\"Mace\",\"_category\":4,\"_rarity\":1,\"_traits\":[],\"_value\":1},{\"_name\":\"Robust Feet\",\"_category\":8,\"_rarity\":3,\"_traits\":[3],\"_value\":4},{\"_name\":\"Axe\",\"_category\":3,\"_rarity\":1,\"_traits\":[],\"_value\":1},{\"_name\":\"Impressive and Robust Feet\",\"_category\":8,\"_rarity\":8,\"_traits\":[1,3],\"_value\":11},{\"_name\":\"Sword\",\"_category\":1,\"_rarity\":3,\"_traits\":[],\"_value\":3},{\"_name\":\"Head\",\"_category\":5,\"_rarity\":1,\"_traits\":[],\"_value\":1},{\"_name\":\"Feet\",\"_category\":8,\"_rarity\":8,\"_traits\":[],\"_value\":8},{\"_name\":\"Axe\",\"_category\":3,\"_rarity\":1,\"_traits\":[],\"_value\":1}],\"maxCapacity\":100},\"_gold\":10000}" },
            //{SaveId.Player, "{\"_startGold\":10,\"_inventory\":{\"maxCapacity\":100,\"items\":[]},\"_gold\":10000}"},
            {SaveId.Shop, "{\"_rentPerSpot\":0,\"_startSpots\":1,\"_unlockedRooms\":[],\"_showcases\":[{\"maxCapacity\":3,\"items\":[{\"_name\":\"\",\"_category\":0,\"_rarity\":0,\"_traits\":[],\"_value\":0},{\"_name\":\"\",\"_category\":0,\"_rarity\":0,\"_traits\":[],\"_value\":0},{\"_name\":\"\",\"_category\":0,\"_rarity\":0,\"_traits\":[],\"_value\":0}],\"_type\":2,\"_highestUnlockedTier\":2}]}" },
            {SaveId.ShowTutorial, "true"},
            {SaveId.TotalSteps, "0"},
        };
        #endregion

        #region Private fields
        private Dictionary<SaveId, string> _savedData;
        private bool _wasLoaded = false;
        #endregion

        #region Properties
        public static bool HasData => Instance._savedData.Count > 0;
        public bool WasLoaded => _wasLoaded;
        #endregion

        public SaveGame()
        {
            _savedData = new Dictionary<SaveId, string>();
        }

        #region Public methods
        /// <summary>
        /// Save the <paramref name="data"/> as string under the <paramref name="key"/>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static void SaveData(SaveId key, object data)
        {
            string json = JsonUtility.ToJson(data);

            if (json == "{}")
                json = data.ToString();

            if (Instance._savedData.ContainsKey(key))
                Instance._savedData[key] = json;
            else
                Instance._savedData.Add(key, json);
            Save();
        }

        /// <summary>
        /// Save the <paramref name="data"/> as string under the <paramref name="key"/>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static void SaveData<T>(SaveId key, ICollection<T> data)
        {
            string json = JsonHelper.ToJsonArray(data);

            if (json == "{}")
                json = data.ToString();

            if (Instance._savedData.ContainsKey(key))
                Instance._savedData[key] = json;
            else
                Instance._savedData.Add(key, json);

            Save();
        }

        /// <summary>
        /// Loads the <paramref name="data"/> as string with the <paramref name="key"/>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool LoadData(SaveId key, out string data)
        {
            if (Instance._savedData.TryGetValue(key, out data))
            {
                return true;
            }
            else
            {
                Debug.LogError($"No entry with key: {key.ToString()}");
                return false;
            }
        }

        /// <summary>
        /// Loads the savegame either from disk or cloud
        /// </summary>
        public static void Load()
        {
            if (Application.platform == RuntimePlatform.Android)
                CloudSaveManager.Instance.LoadFromCloud(Instance.InitFromData);
            else
                Instance.InitFromData("");
        }

        /// <summary>
        /// Resets the game to standard variables
        /// </summary>
        public static void Reset()
        {
            Instance._savedData = new Dictionary<SaveId, string>(_baseSaveGame);
        }

        /// <summary>
        /// Saves the savegame to the disk and cloud
        /// </summary>
        public static void Save()
        {
            if (Application.platform == RuntimePlatform.Android)
                CloudSaveManager.Instance.SaveToCloud(Instance.ToString());
        }

        public override string ToString()
        {
            string fileContent = "";
            foreach (var pair in Instance._savedData)
                fileContent += pair.Key + "=" + pair.Value + Environment.NewLine;
            return fileContent;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Inits the dictionary from the string loaded from a savegame
        /// </summary>
        /// <param name="loadedData"></param>
        private void InitFromData(string loadedData)
        {
            if (loadedData == string.Empty)
            {
                Instance._savedData = new Dictionary<SaveId, string>(_baseSaveGame);
                _wasLoaded = true;
                EventManager.TriggerEvent(GameEventID.SaveGameLoaded);
                return;
            }
            if (Instance._savedData == null)
                Instance._savedData = new Dictionary<SaveId, string>();
            foreach (var line in loadedData.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {
                var lineData = line.Split('=');
                var saveId = (SaveId)Enum.Parse(typeof(SaveId), lineData[0]);
                if (Instance._savedData.ContainsKey(saveId))
                    Instance._savedData[saveId] = lineData[1];
                else
                    Instance._savedData.Add((SaveId)byte.Parse(lineData[0]), lineData[1]);
            }
            _wasLoaded = true;
            EventManager.TriggerEvent(GameEventID.SaveGameLoaded);
        }
        #endregion
    }
}