using System.Collections.Generic;
using System.Linq;
using deVoid.Utils;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class Tube : MonoBehaviour
{
    [field: SerializeField, OnValueChanged(nameof(UpdateMechanic))]
    public MechanicType Mechanic { get; private set; } = MechanicType.None;
    public bool CanBeMoved = true;
    public bool CanBeRotated = true;
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
    [Foldout("Internal Config")]
    public TubeType Type;

    public List<Tube> Connections { get; private set; } = new();

    private List<Collider2D> _exits = new();
    private ContactFilter2D _filter;
    private bool _isBeingMoved = false;
    private bool _isRotating = false;
    private Collider2D _tileCollider;

    public void Start()
    {
        _filter = new ContactFilter2D();
        _exits = GetComponentsInChildren<Collider2D>().ToList();
        _tileCollider = GetComponent<Collider2D>();
        UpdateMechanic();
        TubeManager.Instance.RegisterTube(this);

        Signals.Get<TapSignal>().AddListener(HandleTap);
        Signals.Get<DragSignal>().AddListener(HandleDrag);
        Signals.Get<RotateSignal>().AddListener(HandleRotate);
    }

    private void OnDestroy()
    {
        Signals.Get<TapSignal>().RemoveListener(HandleTap);
        Signals.Get<DragSignal>().RemoveListener(HandleDrag);
        Signals.Get<RotateSignal>().RemoveListener(HandleRotate);
        TubeManager.Instance.UnregisterTube(this);
    }

    void Update()
    {
        if (_isBeingMoved)
        {
            Vector2 mousePos = InputManager.Instance.GetMouseWorldPosition();
            transform.position = mousePos;
        }
    }

    private void HandleTap (Vector2 clickWorldPosition)
    {
        if (_tileCollider.OverlapPoint(clickWorldPosition))
        {
            if (_isBeingMoved) EndMove();
            else StartMove();
        }
    }

    private void HandleDrag (Vector2 clickWorldPosition, bool isStarting)
    {
        if (isStarting)
        {
            if (_tileCollider.OverlapPoint(clickWorldPosition))
            {
                StartMove();
            }
        }
        else
        {
            if (_isBeingMoved) EndMove();
        }
    }

    private void HandleRotate (Vector2 clickWorldPosition)
    {
        if (!_isRotating && CanBeRotated && _tileCollider.OverlapPoint(clickWorldPosition))
        {
            _isRotating = true;
            transform.DORotate(new Vector3(0f, 0f, 90f), 0.3f, RotateMode.LocalAxisAdd).SetEase(Ease.InOutQuad).OnComplete(() => _isRotating = false);
        }
    }

    public void StartMove ()
    {
        _isBeingMoved = true;
        transform.position = new Vector3(transform.position.x, transform.position.y, -1f);
    }

    public void EndMove ()
    {
        _isBeingMoved = false;
        Vector3 snappedPosition = TubeManager.Instance.TubeGrid.SnapPositionToGrid(transform.position);
        transform.position = new Vector3(snappedPosition.x, snappedPosition.y, 0f);
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

    public enum TubeType
    {
        None,
        ITube,
        LTube,
        TTube,
        CrossTube
    }
}