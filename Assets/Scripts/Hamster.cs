using System.Collections.Generic;
using System.Linq;
using deVoid.Utils;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class Hamster : MonoBehaviour
{
    public Tube Entrance;

    [Foldout("Internal Config")]
    public Transform spriteTransform;

    private Vector3 _prevPosition;
    private Vector3 _initialPosition;
    private Tween _pathTween;

    private void Start ()
    {
        _initialPosition = transform.position;
        HamsterManager.Instance.RegisterHamster(this);
    }

    private void OnDestroy ()
    {
        HamsterManager.Instance.UnregisterHamster(this);
    }

    public void Reset ()
    {
        _pathTween?.Kill();
        transform.position = _initialPosition;
        transform.rotation = Quaternion.identity;
    }

    public void AnimatePath (List<Tube> path)
    {
        float defaultDuration = 0.5f;
        _prevPosition = transform.position;

        void AdjustAngle ()
        {
            spriteTransform.right = transform.position - _prevPosition;
            _prevPosition = transform.position;
        }

        Vector3[] waypoints = path.Select(t => t.GetTargetPoint()).ToArray();
        _pathTween = transform
            .DOPath(waypoints, defaultDuration * waypoints.Length, PathType.CatmullRom, PathMode.Sidescroller2D)
            .OnUpdate(AdjustAngle)
            .OnComplete(() => HandleEndOfPath(path.Last()));
    }

    private void HandleEndOfPath (Tube tube)
    {
        Signals.Get<HamsterArrivedSignal>().Dispatch(tube.IsExit);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Hamsters are on their own collision layer, so we know without checking that this is a valid collision with another hamster
        _pathTween.Kill();
        Signals.Get<HamsterArrivedSignal>().Dispatch(false);

        Utils.LogMessage(this, $"collided with {collision.gameObject.name}");
    }
}