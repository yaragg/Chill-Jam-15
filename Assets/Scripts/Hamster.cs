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
        transform.position = _initialPosition;
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
        transform
            .DOPath(waypoints, defaultDuration * waypoints.Length, PathType.CatmullRom, PathMode.Sidescroller2D)
            .OnUpdate(AdjustAngle)
            .OnComplete(() => HandleHamsterArrived(path.Last()));
    }

    private void HandleHamsterArrived (Tube tube)
    {
        Signals.Get<HamsterArrivedSignal>().Dispatch(tube.IsExit);
    }
}