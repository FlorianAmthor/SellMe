namespace SuspiciousGames.SellMe.GameEvents
{
    public class GameEvent
    {
        private object[] _data;
        public object[] Data => _data;
        public GameEvent(params object[] data)
        {
            _data = data;
        }
    }

    [System.Serializable]
    public class UnityGameEvent : UnityEngine.Events.UnityEvent<GameEvent> { }
}
