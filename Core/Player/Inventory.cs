using SuspiciousGames.SellMe.Core.Items;
using System.Collections.Generic;
using UnityEngine;

namespace SuspiciousGames.SellMe.Core
{
    [System.Serializable]
    public class Inventory : ItemContainer
    {
        #region Protected variables
        [SerializeField, HideInInspector]
        protected new int maxCapacity;
        #endregion

        #region Public variables
        public int MaxCapacity => maxCapacity;
        public int NumOfItems => items.Count;
        public List<Item> Items => items;
        #endregion

        public Inventory()
        {
            maxCapacity = -1;
            //items.Capacity = maxCapacity;
        }
    }
}
