using UnityEngine;
using UnityEngine.UI;

public class UIButton : MonoBehaviour
{
    private Button _button;

    void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClicked);
    }

    private void OnClicked ()
    {
        AudioManager.Instance.Play("SFX_ButtonClick", "sfx");
    }
}