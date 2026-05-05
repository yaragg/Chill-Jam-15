using UnityEngine;

public class PlayAudioOnStart : MonoBehaviour
{
    public string clipName;
    public string clipTag;

    void Start()
    {
        AudioManager.Instance.Play(clipName, clipTag);
    }
}