using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SuspiciousGames.SellMe.GameEvents
{
    public class EventManager : MonoBehaviour
    {
        #region Singleton
        private static EventManager _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
                _instance._eventDictionary = new Dictionary<GameEventID, UnityGameEvent>();
                DontDestroyOnLoad(_instance);
            }
        }
        #endregion

        #region Private fields
        private Dictionary<GameEventID, UnityGameEvent> _eventDictionary;
        #endregion

        #region Public methods
        public static void Subscribe(GameEventID gameEventID, UnityAction<GameEvent> action)
        {
            if (_instance._eventDictionary.TryGetValue(gameEventID, out UnityGameEvent gameEvent))
            {
                gameEvent.AddListener(action);
            }
            else
            {
                gameEvent = new UnityGameEvent();
                gameEvent.AddListener(action);
                _instance._eventDictionary.Add(gameEventID, gameEvent);
            }
        }

        public static void Unsubscribe(GameEventID gameEventID, UnityAction<GameEvent> action)
        {
            if (_instance._eventDictionary.TryGetValue(gameEventID, out UnityGameEvent gameEvent))
            {
                gameEvent.RemoveListener(action);
            }
        }

        public static void TriggerEvent(GameEventID gameEventID, params object[] data)
        {
            if (_instance._eventDictionary.TryGetValue(gameEventID, out UnityGameEvent gameEvent))
            {
                gameEvent?.Invoke(new GameEvent(data));
            }
        }
        #endregion
    }
}
