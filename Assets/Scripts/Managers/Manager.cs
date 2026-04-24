using System.Collections;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public bool IsInitialized {get; private set;} = false;

    protected IEnumerator InitializeOnce()
    {
        if (!IsInitialized)
        {
            yield return Initialize();
            IsInitialized = true;
        }
        yield return null;
    }

    protected virtual IEnumerator Initialize()
    {
        yield return null;
    }
}

public class Manager<T> : Manager where T : Manager
{
    protected static T _instance;
    public static T Instance => _instance;

    protected void Awake ()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this as T;
        DontDestroyOnLoad(gameObject);
        StartCoroutine(InitializeOnce());
    }
}
