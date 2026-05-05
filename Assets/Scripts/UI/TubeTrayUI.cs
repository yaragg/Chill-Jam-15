using deVoid.Utils;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class TubeTrayUI : MonoBehaviour
{
    [Foldout("Internal Config")]
    public Button playButton;
    [Foldout("Internal Config")]
    public Sprite playButtonSprite;
    [Foldout("Internal Config")]
    public Sprite stopButtonSprite;

    private bool _isShowingPlayButton = true;

    private void Start()
    {
        Signals.Get<SimulationResetSignal>().AddListener(Reset);
    }

    private void OnDestroy()
    {
        Signals.Get<SimulationResetSignal>().RemoveListener(Reset);
    }

    private void Reset ()
    {
        playButton.image.sprite = playButtonSprite;
        _isShowingPlayButton = true;
    }

    public void HandleSimulationPressed()
    {
        if (_isShowingPlayButton)
        {
            Signals.Get<SimulationStartSignal>().Dispatch();
            playButton.image.sprite = stopButtonSprite;
            _isShowingPlayButton = false;
        }
        else
        {
            Signals.Get<SimulationResetSignal>().Dispatch();
        }
    }
}