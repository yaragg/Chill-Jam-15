using System.Collections;
using System.Collections.Generic;
using System.Linq;
using deVoid.Utils;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;


public class AudioManager : Manager<AudioManager>
{
    private Dictionary<string, AudioClip> audioClips = new();
    private List<GameAudioClip> currentClips = new();
    protected override IEnumerator Initialize()
    {
        List<AudioClip> clips = Resources.LoadAll<AudioClip>("Audio").ToList();
        clips.ForEach(c => audioClips.Add(c.name, c));

        Signals.Get<AudioEndedSignal>().AddListener(HandleAudioEnded);

        return base.Initialize();
    }

    public GameAudioClip Play(string clipName, string tag = "", bool forceStart = false)
    {
        GameAudioClip previousClip = GetCurrentClipByName(clipName);
        if (!forceStart && previousClip != null) return previousClip;

        if (audioClips.TryGetValue(clipName, out AudioClip clip))
        {
            GameObject go = new();
            go.transform.parent = transform;
            GameAudioClip gAudioClip = go.AddComponent<GameAudioClip>();
            gAudioClip.SetClip(clip);
            gAudioClip.Tag = tag;
            gAudioClip.Play();
            currentClips.Add(gAudioClip);
            return gAudioClip;
        }
        else
        {
            Utils.LogError(this, $"Could not find clip '{clipName}' in Resources/Audio folder.");
            return null;
        }
    }

    public void Stop(string tag)
    {
        GetCurrentClipsByTag(tag).ForEach(c => c.AudioSrc.Stop());
    }

    public bool IsPlaying(string clipName)
    {
        return currentClips.Any(c => c.clip.name == clipName && c.AudioSrc.isPlaying);
    }

    public List<GameAudioClip> GetCurrentClipsByTag(string tag = "")
    {
        return currentClips.Where(c => c.Tag == tag).ToList();
    }

    public GameAudioClip GetCurrentClipByName(string clipName)
    {
        return currentClips.Find(c => c.clip.name == clipName);
    }

    private void HandleAudioEnded(string clipName, string tag)
    {
        currentClips = currentClips.Where(c => !c.IsUnityNull() && !c.IsDestroyed()).ToList();
    }


}