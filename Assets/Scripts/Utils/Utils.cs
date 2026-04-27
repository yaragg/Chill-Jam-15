using System.Collections.Generic;

public class Utils
{
    public static T GetRandomItem<T>(List<T> list)
    {
        if (list.Count == 0) return default;
        int index = UnityEngine.Random.Range(0, list.Count);
        return list[index];
    }
}