using System.Collections.Generic;
using UnityEngine;

public class ObjectIdentifier : MonoBehaviour 
{
    public string id;
    public List<string> groups = new(){"Default"};

    private void Awake ()
    {
        ObjectManager.Instance.AddObject(this);
    }

    private void OnDestroy ()
    {
        ObjectManager.Instance.RemoveObject(this);
    }
}