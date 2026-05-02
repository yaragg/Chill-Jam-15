using System.Collections;
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

    public static void LogWarning(MonoBehaviour component, string message)
    {
        Debug.LogWarning($"[{component.name}]: {message}", component.gameObject);
    }

    public static void LogError(MonoBehaviour component, string message)
    {
        Debug.LogError($"[{component.name}]: {message}", component.gameObject);
    }

    public static void DelayCall(MonoBehaviour component, float delay, System.Action action)
    {
        component.StartCoroutine(DelayCoroutine(delay, action));
    }

    private static IEnumerator DelayCoroutine(float delay, System.Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
}