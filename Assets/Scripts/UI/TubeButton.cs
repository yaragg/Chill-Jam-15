using TMPro;
using UnityEngine;

public class TubeButton : MonoBehaviour 
{
    public Tube.TubeType Type;

    private TMP_Text _counterText;
    private int _usesLeft;

    void Start()
    {
        switch (Type)
        {
            case Tube.TubeType.ITube:
                _usesLeft = GameManager.Instance.GetCurrentLevelDef().numITubes;
                break;
            case Tube.TubeType.LTube:
                _usesLeft = GameManager.Instance.GetCurrentLevelDef().numLTubes;
                break;
            case Tube.TubeType.TTube:
                _usesLeft = GameManager.Instance.GetCurrentLevelDef().numTTubes;
                break;
            case Tube.TubeType.CrossTube:
                _usesLeft = GameManager.Instance.GetCurrentLevelDef().numCrossTubes;
                break;
        }

        _counterText = GetComponentInChildren<TMP_Text>();
        UpdateCounterText();
    }

    public void HandleClick ()
    {
        if (_usesLeft > 0 && !GameManager.Instance.IsSimulationRunning)
        {
            SpawnTubeAtMouse();
            _usesLeft--;
            UpdateCounterText();
        }
    }

    public void SpawnTubeAtMouse ()
    {
        Vector2 position = InputManager.Instance.GetMouseWorldPosition();
        Tube tube = TubeManager.Instance.SpawnTubeAt(Type, position);
        tube.StartMove();
    }

    private void UpdateCounterText()
    {
        if (_counterText != null)
        {
            _counterText.text = _usesLeft.ToString();
        }
    }
}