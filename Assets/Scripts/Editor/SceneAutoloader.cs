#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
static class SceneAutoloader
{
    private const string AUTOLOADER_PATH = "File/Scene Autoloader/";
    private const string SHOULD_AUTOLOAD_PATH = AUTOLOADER_PATH + "Autoload Scene";
    private const string SELECT_SCENE_PATH = AUTOLOADER_PATH + "Select Autoloaded Scene";


    static SceneAutoloader ()
    {
        EditorApplication.delayCall += () =>
        {
            UpdateShouldAutoload(SceneAutoloaderData.ShouldAutoload);
        };
        EditorApplication.playModeStateChanged += HandlePlayModeChanged;
    }

    [MenuItem(SELECT_SCENE_PATH)]
    private static void SelectAutoloadedScene ()
    {
        string scene = EditorUtility.OpenFilePanel("Select Autoloaded Scene", Application.dataPath, "unity");
        scene = scene.Replace(Application.dataPath, "Assets");  // AssetDatabase requires a relative path
        if (!string.IsNullOrEmpty(scene))
        {
            SceneAutoloaderData.AutoloadedScene = scene;
            UpdateShouldAutoload(true);
        }
    }

    [MenuItem(SHOULD_AUTOLOAD_PATH)]
    private static void ToggleShouldAutoload()
    {
        UpdateShouldAutoload(!SceneAutoloaderData.ShouldAutoload);
    }

    private static void UpdateShouldAutoload (bool value)
    {
        SceneAutoloaderData.ShouldAutoload = value;
        Menu.SetChecked(SHOULD_AUTOLOAD_PATH, SceneAutoloaderData.ShouldAutoload);

        if (value)
        {
            SceneAsset scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(SceneAutoloaderData.AutoloadedScene);
            if (scene != null)
            {
                EditorSceneManager.playModeStartScene = scene;
            }
        }
        else
        {
            EditorSceneManager.playModeStartScene = default;
        }
    }

    private static void HandlePlayModeChanged (PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            if (SceneAutoloaderData.ShouldAutoload)
            {
                SceneAutoloaderData.SavedScene = EditorSceneManager.GetActiveScene().path;
                SceneAutoloaderData.ShouldAutoload = true;
            } 
            else
            {
                SceneAutoloaderData.SavedScene = null;
                SceneAutoloaderData.ShouldAutoload = false;
            }
        }
    }
}
#endif