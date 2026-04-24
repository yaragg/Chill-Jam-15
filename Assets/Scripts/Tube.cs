using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tube : MonoBehaviour
{
    private List<Collider2D> _exits = new();

    public void Start ()
    {
        _exits = GetComponentsInChildren<Collider2D>().ToList();
        TubeManager.Instance.RegisterTube(this);
    }

    private void OnDestroy ()
    {
        TubeManager.Instance.UnregisterTube(this);
    }
}