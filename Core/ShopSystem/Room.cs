using UnityEngine;

namespace SuspiciousGames.SellMe.Core.ShopSystem
{
    public abstract class Room : ScriptableObject
    {
        #region Private exposed fields
        [SerializeField]
        protected new string name;
        [SerializeField]
        protected string description;
        [SerializeField]
        protected int rent;
        #endregion

        #region Properties
        public string Name => name;
        public string Description => description;
        public int Rent => rent;
        #endregion
    }
}