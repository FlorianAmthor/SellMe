using System.Collections.Generic;

public static class ListExtensions
{
    public static void RemoveNull<T>(this List<T> list)
    {
        var count = list.Count;
        for (var i = 0; i < count; i++)
        {
            if (list[i] == null)
            {
                int newCount = i++;
                for (; i < count; i++)
                {
                    if (list[i] != null)
                    {
                        list[newCount++] = list[i];
                    }
                }
                list.RemoveRange(newCount, count - newCount);
                break;
            }
        }
    }
}
