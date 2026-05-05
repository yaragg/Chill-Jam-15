using deVoid.Utils;
using UnityEngine;

public class GameAudioClip : MonoBehaviour
{
    public AudioClip clip;
    public string Tag;
    public AudioSource AudioSrc { get; private set; }
    public bool IsLooping { get => AudioSrc.loop; set => AudioSrc.loop = value; }

    private float _fadeOutSecs = 0f;
    private bool _hasStartedPlaying = false;
    private float _bgmVolume = 0.9f;

    public void Awake()
    {
        AudioSrc = gameObject.AddComponent<AudioSource>();
        AudioSrc.clip = clip;

        gameObject.name = clip.name;

        if (tag == "bgm")
        {
            // Ugly game jam haxx until we implement mixers
            AudioSrc.volume = _bgmVolume;
        }
    }

    public void Play()
    {
        AudioSrc.Play();
    }

    public void SetFadeoutSecs(float secs)
    {
        if (tag == "bgm" && _fadeOutSecs > 0 && secs == 0f)
        {
            AudioSrc.volume = _bgmVolume;
        }
        _fadeOutSecs = secs;
    }

    public void SetClip(AudioClip clip)
    {
        this.clip = clip;
        AudioSrc.clip = clip;
    }

    private void LateUpdate()
    {
        float timeLeft = clip.length - AudioSrc.time;
        if (_hasStartedPlaying && !AudioSrc.loop && timeLeft <= 0f)
        {
            HandleClipEnd();
        }

        if (AudioSrc.isPlaying) _hasStartedPlaying = true;

        if (_fadeOutSecs > 0f && timeLeft <= _fadeOutSecs)
        {
            AudioSrc.volume -= Time.deltaTime / _fadeOutSecs;
        }
    }

    private void HandleClipEnd()
    {
        Signals.Get<AudioEndedSignal>().Dispatch(clip.name, Tag);
        Destroy(gameObject);
    }
}