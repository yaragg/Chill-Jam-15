using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class FrameAnimator : MonoBehaviour
{
    public string StartingAnimation;
    [ReorderableList, AllowNesting]
    public List<FrameAnimationDef> Animations = new();
    private SpriteRenderer _spriteRenderer;
    private Coroutine _animationCoroutine;
    private FrameAnimationDef _currentAnimation;
    public string CurrentlyPlaying => _currentAnimation?.name;
    public float Duration {get; private set;}

    private void Awake ()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        if (!string.IsNullOrEmpty(StartingAnimation)) Play(StartingAnimation);
    }

    public Coroutine Play (string animationName)
    {
        Stop();

        FrameAnimationDef animationDef = Animations.Find(a => a.name == animationName);
        if (animationDef == null)
        {
            Utils.LogWarning(this, $"Animation '{animationName}' could not be found.");
            return null;
        }

        _currentAnimation = animationDef;

        Duration = (animationDef.frameDuration / 1000f) * animationDef.frames.Length;
        _animationCoroutine = StartCoroutine(Animate(animationDef));
        return _animationCoroutine;
    }

    public void Stop ()
    {
        if (_animationCoroutine != null) StopCoroutine(_animationCoroutine);
    }

    private IEnumerator Animate (FrameAnimationDef animationDef)
    {
        while (true)
        {
            foreach (Sprite frame in animationDef.frames)
            {
                _spriteRenderer.sprite = frame;
                yield return new WaitForSeconds(animationDef.frameDuration / 1000f);
            }
            
            if (!animationDef.shouldLoop)
                break;
        }
    }

    [Serializable]
    public class FrameAnimationDef
    {
        public string name;
        public float frameDuration;
        public bool shouldLoop = false;
        [ReorderableList]
        public Sprite[] frames;
    }
}