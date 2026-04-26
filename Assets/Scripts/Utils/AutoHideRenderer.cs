using UnityEngine;

public class AutoHideRenderer : MonoBehaviour
{
    private void Awake()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null) renderer.enabled = false;
    }
}
