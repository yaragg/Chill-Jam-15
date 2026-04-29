using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class Tube : MonoBehaviour
{
    [field: SerializeField, OnValueChanged(nameof(UpdateMechanic))]
    public MechanicType Mechanic { get; private set; } = MechanicType.None;
    public bool CanBeMoved = true;
    public bool CanBeRotated = true;
    public bool IsEntrance = false;
    public bool IsExit = false;


    [Foldout("Internal Config")]
    public Sprite AccelerateSprite;
    [Foldout("Internal Config")]
    public Sprite DecelerateSprite;
    [Foldout("Internal Config")]
    public SpriteRenderer TubeRenderer;
    [Foldout("Internal Config")]
    public SpriteRenderer MechanicRenderer;
    [Foldout("Internal Config")]
    public Transform targetOverride;

    public List<Tube> Connections { get; private set; } = new();

    private List<Collider2D> _exits = new();
    private ContactFilter2D _filter;

    public void Start()
    {
        _filter = new ContactFilter2D();
        _exits = GetComponentsInChildren<Collider2D>().ToList();
        UpdateMechanic();
        TubeManager.Instance.RegisterTube(this);
    }

    private void OnDestroy()
    {
        TubeManager.Instance.UnregisterTube(this);
    }

    private void UpdateMechanic()
    {
        switch (Mechanic)
        {
            case MechanicType.Accelerate:
                MechanicRenderer.sprite = AccelerateSprite;
                break;
            case MechanicType.Decelerate:
                MechanicRenderer.sprite = DecelerateSprite;
                break;
            default:
                MechanicRenderer.sprite = null;
                break;
        }
    }

    public void FindConnections()
    {
        Connections.Clear();
        // _filter.SetLayerMask(LayerMask.GetMask("Tube"));
        foreach (Collider2D exit in _exits)
        {
            List<Collider2D> overlapping = new List<Collider2D>();
            Physics2D.OverlapCollider(exit, _filter, overlapping);
            foreach (Collider2D col in overlapping)
            {
                Tube tube = col.GetComponentInParent<Tube>();
                if (tube != null && tube != this)
                {
                    Connections.Add(tube);
                }
            }
        }
    }

    public Vector3 GetTargetPoint()
    {
        return targetOverride != null ? targetOverride.position : transform.position;
    }

    public enum MechanicType
    {
        None,
        Accelerate,
        Decelerate
    }
}