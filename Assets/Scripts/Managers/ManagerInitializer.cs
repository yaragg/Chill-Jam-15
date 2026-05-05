using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManagerInitializer : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _managerPrefabs = new();

    private void Awake ()
    {
        StartCoroutine(InitializeManagers());
    }

    private IEnumerator InitializeManagers ()
    {
        foreach (GameObject managerPrefab in _managerPrefabs)
        {
            Manager manager = Instantiate(managerPrefab).GetComponent<Manager>();
            manager.gameObject.name = managerPrefab.name;
            while (!manager.IsInitialized)
            {
                yield return null;
            }
        }

        #if UNITY_EDITOR
        if (SceneAutoloaderData.ShouldAutoload)
        {
            SceneManager.LoadScene(SceneAutoloaderData.SavedScene);
            yield break;
        }
        #endif

        SceneManager.LoadScene("MainMenu");
    }
}