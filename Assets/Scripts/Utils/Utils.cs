using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public static T GetRandomItem<T>(List<T> list)
    {
        if (list.Count == 0) return default;
        int index = UnityEngine.Random.Range(0, list.Count);
        return list[index];
    }

    public static void LogMessage(MonoBehaviour component, string message)
    {
        Debug.Log($"[{component.name}]: {message}", component.gameObject);
    }
}