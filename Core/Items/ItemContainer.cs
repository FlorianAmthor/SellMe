using SuspiciousGames.SellMe.Core.Items;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SuspiciousGames.SellMe.Core
{
    public abstract class ItemContainer
    {
        #region Protected exposed fields
        [SerializeField]
        protected int maxCapacity = 1;
        [SerializeField, HideInInspector] protected List<Item> items;
        #endregion

        #region Public fields
        public virtual event Action<Item> onItemAdded;
        public virtual event Action<Item> onItemRemoved;
        #endregion

        #region Public Methods
        /// <summary>
        /// Initializes the container with the preset capacity
        /// </summary>
        public ItemContainer()
        {
            items = new List<Item>();
        }

        /// <summary>
        /// Initializes the container with the given <paramref name="capacity"/>
        /// </summary>
        /// <param name="capacity">Capacity of the container</param>
        public ItemContainer(int capacity)
        {
            items = new List<Item>(capacity);
        }

        /// <summary>
        /// Adds an item to the inventory if the maximum capacity isn't yet reached
        /// </summary>
        /// <param name="itemToAdd">The item to add</param>
        /// <returns>If the item was added successfully</returns>
        public virtual bool AddItem(Item itemToAdd)
        {
            if (items.Count < maxCapacity || maxCapacity == -1)
            {
                items.Add(itemToAdd);
                itemToAdd.Container = this;
                onItemAdded?.Invoke(itemToAdd);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Removes an item from the inventory if it exists
        /// </summary>
        /// <param name="itemToRemove">The item to remove</param>
        /// <returns>If the item was removed successfully</returns>
        public virtual bool RemoveItem(Item itemToRemove)
        {
            if (items.Remove(itemToRemove))
            {
                itemToRemove.Container = null;
                onItemRemoved?.Invoke(itemToRemove);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes the item at index <paramref name="index"/> from the item container
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual bool RemoveItem(int index)
        {
            if (index >= items.Count)
                return false;
            var item = items[index];
            items.RemoveAt(index);
            onItemRemoved?.Invoke(item);
            return true;
        }

        public Item GetItemAt(int index)
        {
            if (index < 0 || index >= items.Count)
                return null;
            return items[index];
        }
        #endregion
    }
}
