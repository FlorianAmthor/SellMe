using System.Collections.Generic;

namespace SuspiciousGames.SellMe.Utility
{
    public static class MathExtensions
    {
        public static float GetMedian(this IList<float> list)
        {
            int length = list.Count;
            if (length % 2 == 0)
                return (list[length / 2 - 1] + list[length / 2]) / 2;
            else
                return list[(length / 2)];
        }

        public static float GetMedian(this IList<int> list)
        {
            int length = list.Count;
            if (length % 2 == 0)
                return (list[length / 2 - 1] + list[length / 2]) / 2;
            else
                return list[(length / 2)];
        }

        public static float GetMean(this IList<float> list)
        {
            float sum = 0;
            foreach (var num in list)
                sum += num;
            return sum / list.Count;
        }

        public static float GetMean(this IList<int> list)
        {
            float sum = 0;
            foreach (var num in list)
                sum += num;
            return sum / list.Count;
        }
    }
}

