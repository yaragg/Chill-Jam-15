using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class Hamster : MonoBehaviour
{
    [Foldout("Internal Config")]
    public Transform spriteTransform;

    private Vector3 _prevPosition;

    private void Start ()
    {
        HamsterManager.Instance.RegisterHamster(this);
    }

    private void OnDestroy ()
    {
        HamsterManager.Instance.UnregisterHamster(this);
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
        transform.DOPath(waypoints, defaultDuration * waypoints.Length, PathType.CatmullRom, PathMode.Sidescroller2D).OnUpdate(AdjustAngle);
    }
}