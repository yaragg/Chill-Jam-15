using System.Collections.Generic;
using System.Linq;
using deVoid.Utils;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class Hamster : MonoBehaviour
{
    public Tube Entrance { get; private set; }

    [Foldout("Internal Config")]
    public Transform spriteTransform;
    [Foldout("Internal Config")]
    public GameObject exclamationPrefab;
    [Foldout("Internal Config")]
    public float PathDurationPerTube = 0.4f;

    private Vector3 _prevPosition;
    private Vector3 _initialPosition;
    private Tween _pathTween;
    private FrameAnimator _frameAnimator;
    private Collider2D _collider;

    private void Start()
    {
        _initialPosition = transform.position;
        _frameAnimator = GetComponentInChildren<FrameAnimator>();
        _collider = GetComponent<Collider2D>();
        HamsterManager.Instance.RegisterHamster(this);
    }

    private void OnDestroy()
    {
        HamsterManager.Instance.UnregisterHamster(this);
    }

    public void SetEntrance(Tube entrance)
    {
        Entrance = entrance;
    }

    public void Reset()
    {
        _pathTween?.Kill();
        _collider.enabled = true;
        transform.position = _initialPosition;
        spriteTransform.rotation = Quaternion.identity;
        _frameAnimator.Play("idle");
    }

    public void AnimatePath(List<Tube> path)
    {
        _prevPosition = transform.position;

        void AdjustAngle()
        {
            if (_frameAnimator.CurrentlyPlaying == "jump") return;
            spriteTransform.right = transform.position - _prevPosition;
            _prevPosition = transform.position;
        }


        if (path.Count == 0)
        {
            _frameAnimator.Play("jump_fail");
            Utils.DelayCall(this, _frameAnimator.Duration, () => HandleEndOfPath(Entrance));
            return;
        }

        _frameAnimator.Play("jump");

        Utils.DelayCall(this, _frameAnimator.Duration, () => _frameAnimator.Play("float"));

        Vector3[] waypoints = path.Select(t => t.GetTargetPoint()).ToArray();
        _pathTween = transform
            .DOPath(waypoints, PathDurationPerTube * waypoints.Length, PathType.CatmullRom, PathMode.Sidescroller2D)
            .SetDelay(0.1f)
            .OnUpdate(AdjustAngle)
            .OnComplete(() => HandleEndOfPath(path.Last()));
    }

    private void HandleEndOfPath(Tube tube)
    {
        if (tube.IsExit)
        {
            _collider.enabled = false;
            float offset = Random.Range(-0.05f, 0.05f);
            transform.DOMoveX(transform.position.x + offset, 0.2f);
            _frameAnimator.Play("celebrate");
            spriteTransform.rotation = Quaternion.identity;
        }
        else
        {
            GameObject exclamation = Instantiate(exclamationPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
            Utils.DelayCall(this, 1f, () => Destroy(exclamation));
        }
        Signals.Get<HamsterArrivedSignal>().Dispatch(tube.IsExit);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Hamsters are on their own collision layer, so we know without checking that this is a valid collision with another hamster
        _pathTween.Kill();
        AudioManager.Instance.Play("SFX_Hamster_Collision", "sfx");
        _frameAnimator.Play("collision");
        Signals.Get<HamsterArrivedSignal>().Dispatch(false);
    }
}