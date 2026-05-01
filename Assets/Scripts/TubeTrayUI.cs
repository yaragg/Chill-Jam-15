using UnityEngine;

public class TubeTrayUI : MonoBehaviour
{
    public void HandleITubeButtonPressed ()
    {
        SpawnTubeAtMouse(Tube.TubeType.ITube);
    }

    public void HandleLTubeButtonPressed ()
    {
        SpawnTubeAtMouse(Tube.TubeType.LTube);
    }

    public void HandleTTubeButtonPressed ()
    {
        SpawnTubeAtMouse(Tube.TubeType.TTube);
    }

    public void HandleCrossTubeButtonPressed ()
    {
        SpawnTubeAtMouse(Tube.TubeType.CrossTube);
    }

    private void SpawnTubeAtMouse (Tube.TubeType type)
    {
        Vector2 position = InputManager.Instance.GetMouseWorldPosition();
        Tube tube = TubeManager.Instance.SpawnTubeAt(type, position);
        tube.StartMove();
    }
}