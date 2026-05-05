using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour 
{   
    public string bgmName = "HoliznaCC0 - Level 1";
    public void Start ()
    {
        if (!AudioManager.Instance.IsPlaying(bgmName)) AudioManager.Instance.Stop("bgm");
        GameAudioClip bgm = AudioManager.Instance.Play(bgmName, "bgm");
        bgm.IsLooping = true;
        bgm.SetFadeoutSecs(0f);
    }

    public void HandleMainMenuClicked ()
    {
        SceneManager.LoadScene("MainMenu");
    }
}