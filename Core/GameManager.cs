using SuspiciousGames.SellMe.Core.ShopSystem;
using SuspiciousGames.SellMe.GameEvents;
using SuspiciousGames.SellMe.Utility;
using SuspiciousGames.SellMe.Utility.Sensors;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SuspiciousGames.SellMe.Core
{

    public class GameManager : MonoBehaviour
    {
        #region Singleton
        private static GameManager _instance;
        public static GameManager Instance => _instance;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
                DontDestroyOnLoad(_instance);
                if(Application.platform == RuntimePlatform.Android)
                StartCoroutine(StepSensorReader.ConnectToSensor());
            }
        }
        #endregion

        #region Private fields
        private GameState _gameState;
        #endregion

        #region Properties
        public GameState GameState { get => _gameState; }
        public Player Player => Player.Instance;
        public string TimeTillNextRent = GameTime.TimeTillNextRent;
        #endregion

        #region MonoBehaviour
        void Start()
        {
            PlayServicesManager.InitiatePlayGames();
            PlayServicesManager.Login(LoginCallback, true);
        }

        private void OnApplicationQuit()
        {
        }

        private void OnApplicationPause(bool pause)
        {
            if (SceneManager.GetActiveScene().buildIndex == 1)
            {
                if (pause)
                    StepSensorReader.DisconnectFromSensor();
                else
                    StepSensorReader.ConnectToSensor();
            }
        }

        private void OnEnable()
        {
            EventManager.Subscribe(GameEventID.SaveGameLoaded, OnSaveGameLoaded);
            EventManager.Subscribe(GameEventID.TimePassed, OnTimePassed);
            EventManager.Subscribe(GameEventID.GoldLost, OnGoldLost);
        }

        private void OnDisable()
        {
            EventManager.Unsubscribe(GameEventID.SaveGameLoaded, OnSaveGameLoaded);
            EventManager.Unsubscribe(GameEventID.TimePassed, OnTimePassed);
            EventManager.Unsubscribe(GameEventID.GoldLost, OnGoldLost);
        }
        #endregion

        #region Public Methods
        public void SwitchToGameState(GameState newGameState)
        {
            if (_instance._gameState == newGameState)
                return;
            _instance._gameState = newGameState;
            SaveGame.SaveData(SaveId.GameState, (int)_instance._gameState);
        }

        public void SwitchToScene(int buildIndex)
        {
            if (buildIndex == 1)
            {
                StepSensorReader.ConnectToSensor();

            }
            else if (buildIndex == 0)
            {
                StepSensorReader.DisconnectFromSensor();
            }
            SceneManager.LoadSceneAsync(buildIndex, LoadSceneMode.Single);
        }
        #endregion

        #region Private methods
        private void GameOver()
        {
            //TODO: Save highscore(in game time played, gold earned) to google leaderboards
            SaveGame.SaveData(SaveId.GameOver, true);
            //TODO: Save all rewards the player may take into the next game
            SaveGame.Reset();
            //TODO: Apply all rewards the player may take into the next game
            SaveGame.Save();
            SwitchToScene(0);
        }

        private void InitGame()
        {
            SaveGame.SaveData(SaveId.NewGameCycle, false);
        }
        #endregion

        #region Method callbacks
        private void LoginCallback(bool success, string info)
        {
            StartCoroutine(LoadSaveGameRoutine());
        }

        private IEnumerator LoadSaveGameRoutine()
        {
            //TODO: Make game not interactable while savegame is loading, Show non interactable overlay
            SaveGame.Load();
            yield return new WaitUntil(() => SaveGame.Instance.WasLoaded);
            Debug.Log(SaveGame.Instance.ToString());
            //TODO: Make game interactable again
            if (SaveGame.LoadData(SaveId.GameOver, out string data))
                if (bool.Parse(data))
                    GameOver();
            //TODO: do we need this?
            if (SaveGame.LoadData(SaveId.NewGameCycle, out data))
                if (bool.Parse(data))
                    InitGame();
            if (SaveGame.LoadData(SaveId.GameState, out data))
                _gameState = (GameState)Enum.Parse(typeof(GameState), data);
            GameTime.LoadFromSaveGame();
            if (_gameState == GameState.Adventure)
            {
                SceneManager.LoadScene(1, LoadSceneMode.Single);
            }
        }

        private void OnSaveGameLoaded(GameEvent gameEvent)
        {
            //TODO: Remove non interactable overlay
        }

        private void OnGoldLost(GameEvent gameEvent)
        {
            if (Player.Instance.Gold <= 0)
            {
                GameOver();
            }
        }

        private void OnTimePassed(GameEvent gameEvent)
        {
            if ((bool)gameEvent.Data[1])
                ShopManager.Instance.Shop.PayRent();
        }
        #endregion
    }
}