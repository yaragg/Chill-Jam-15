using UnityEditor;

public static class SceneAutoloaderData
{
    private static string PREF_SHOULD_AUTOLOAD_KEY = "SceneAutoloader.ShouldAutoload";
    private static string PREF_AUTOLOADED_SCENE_KEY = "SceneAutoloader.AutoLoadedScene";
    private static string PREF_SAVED_SCENE_KEY = "SceneAutoloader.SavedScene";

    public static bool ShouldAutoload
    {
        #if UNITY_EDITOR
        get => EditorPrefs.GetBool(PREF_SHOULD_AUTOLOAD_KEY, false);
        set => EditorPrefs.SetBool(PREF_SHOULD_AUTOLOAD_KEY, value);
        #else
        get => false;
        set => {};
        #endif
    }

    public static string AutoloadedScene
    {
        #if UNITY_EDITOR
        get => EditorPrefs.GetString(PREF_AUTOLOADED_SCENE_KEY, "Initialization.unity");
        set => EditorPrefs.SetString(PREF_AUTOLOADED_SCENE_KEY, value);
        #else
        get => "";
        set => {};
        #endif
    }

    public static string SavedScene
    {
        #if UNITY_EDITOR
        get => EditorPrefs.GetString(PREF_SAVED_SCENE_KEY, "Initialization.unity");
        set => EditorPrefs.SetString(PREF_SAVED_SCENE_KEY, value);
        #else
        get => "";
        set => {};
        #endif
    }
}