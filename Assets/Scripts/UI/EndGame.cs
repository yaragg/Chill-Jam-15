using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour 
{   
    public void Start ()
    {
        AudioManager.Instance.Stop("bgm");
        GameAudioClip bgm = AudioManager.Instance.Play("HoliznaCC0 - Level 1", "bgm");
        bgm.IsLooping = true;
    }

    public void HandleMainMenuClicked ()
    {
        SceneManager.LoadScene("MainMenu");
    }
}