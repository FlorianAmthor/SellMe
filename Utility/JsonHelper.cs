using System.Collections.Generic;
using UnityEngine;

namespace SuspiciousGames.SellMe.Utility
{
    public static class JsonHelper
    {
        public static ICollection<T> FromJsonArray<T>(string json)
        {
            T[] t = JsonUtility.FromJson<EnumerableWrapper<T>>(json).items;
            return t;
        }

        public static string ToJsonArray<T>(ICollection<T> collection)
        {
            EnumerableWrapper<T> wrapper = new EnumerableWrapper<T>(collection);
            return JsonUtility.ToJson(wrapper);
        }

        public static string ToJsonArray<T>(ICollection<T> collection, bool prettyPrint)
        {
            EnumerableWrapper<T> wrapper = new EnumerableWrapper<T>(collection);
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        [System.Serializable]
        private class EnumerableWrapper<T>
        {
            public T[] items;

            public EnumerableWrapper(ICollection<T> collection)
            {
                items = new T[collection.Count];
                int i = 0;
                foreach (var item in collection)
                {
                    items[i] = item;
                    i++;
                }
            }
        }
    }
}
