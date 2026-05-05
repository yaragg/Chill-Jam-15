using deVoid.Utils;
using UnityEngine;

public class GameAudioClip : MonoBehaviour
{
    public AudioClip clip;
    public string Tag;
    public float fadeOutSecs = 0f;
    public AudioSource AudioSrc {get; private set;}
    public bool IsLooping {get => AudioSrc.loop; set => AudioSrc.loop = value;}

    private bool _hasStartedPlaying = false;

    public void Awake ()
    {
        AudioSrc = gameObject.AddComponent<AudioSource>();
        AudioSrc.clip = clip;
    }

    public void Play ()
    {
        AudioSrc.Play();
    }

    public void SetClip (AudioClip clip)
    {
        this.clip = clip;
        AudioSrc.clip = clip;
    }

    private void LateUpdate ()
    {
        float timeLeft = clip.length - AudioSrc.time;
        if (_hasStartedPlaying && !AudioSrc.loop && timeLeft <= 0f)
        {
            HandleClipEnd();
        }

        if (AudioSrc.isPlaying) _hasStartedPlaying = true;

        if (fadeOutSecs > 0f && timeLeft <= fadeOutSecs)
        {
            AudioSrc.volume -= Time.deltaTime / fadeOutSecs;
        }
    }

    private void HandleClipEnd ()
    {
        Signals.Get<AudioEndedSignal>().Dispatch(clip.name, Tag);
        Destroy(gameObject);
    }
}