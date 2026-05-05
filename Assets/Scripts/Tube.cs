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

    [ShowIf("IsRandom")]
    public List<TubeType> RandomTypes = new List<TubeType>() { TubeType.ITube, TubeType.LTube, TubeType.TTube, TubeType.CrossTube };

    [Foldout("Internal Config")]
    public Sprite AccelerateSprite;
    [Foldout("Internal Config")]
    public Sprite DecelerateSprite;
    [Foldout("Internal Config")]
    public SpriteRenderer TubeRenderer;
    [Foldout("Internal Config")]
    public GameObject poofFX;
    [Foldout("Internal Config")]
    public SpriteRenderer MechanicRenderer;
    [Foldout("Internal Config")]
    public Transform targetOverride;
    [Foldout("Internal Config")]
    public TubeType Type;

    public List<Tube> Connections { get; private set; } = new();
    public bool IsExit => Type == TubeType.Exit;
    public bool IsEntrance => Type == TubeType.Entrance;
    public bool IsRandom => Type == TubeType.Random;

    private List<Collider2D> _exits = new();
    private ContactFilter2D _exitsFilter;
    private bool _isBeingMoved = false;
    private bool _isRotating = false;
    private bool _wasSpawnedByPlayer = false;
    private Collider2D _tileCollider;
    private List<float> _randomAngles = new List<float>() { 0f, 90f, 180f, 270f };

    public void Start()
    {
        if (Type == TubeType.Random)
        {
            RandomizeTile();
            return;
        }

        if (!_wasSpawnedByPlayer && CanBeMoved)
        {
            Destroy(gameObject);
            return;
        }

        if (CanBeRotated)
        {
            float randomAngle = Utils.GetRandomItem(_randomAngles);
            transform.rotation = Quaternion.Euler(0f, 0f, randomAngle);
        }

        _exitsFilter = new ContactFilter2D();
        _exitsFilter.SetLayerMask(LayerMask.GetMask("Tube Exits"));
        _exitsFilter.useLayerMask = true;

        _exits = GetComponentsInChildren<Collider2D>().Where(c => c.gameObject.layer == LayerMask.NameToLayer("Tube Exits")).ToList();
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

    public void SetWasSpawned(bool value)
    {
        _wasSpawnedByPlayer = value;

        // Keep default sprite for spawned tubes so the player can keep track of them
        if (_wasSpawnedByPlayer)
        {
            SpriteRandomizer randomizer = GetComponentInChildren<SpriteRandomizer>();
            randomizer.RestoreOriginalSprite();
        }
    }

    private void RandomizeTile()
    {
        TubeType type = Utils.GetRandomItem(RandomTypes);
        Tube newTube = TubeManager.Instance.SpawnTubeAt(type, transform.position);
        newTube.CanBeMoved = CanBeMoved;
        newTube.CanBeRotated = CanBeRotated;

        Destroy(gameObject);
    }

    private void HandleTap(Vector2 clickWorldPosition)
    {
        if (_tileCollider.OverlapPoint(clickWorldPosition))
        {
            if (!CanBeMoved) return;
            if (_isBeingMoved) EndMove();
            else StartMove();
        }
    }

    private void HandleDrag(Vector2 clickWorldPosition, bool isStarting)
    {
        if (isStarting)
        {
            if (_tileCollider.OverlapPoint(clickWorldPosition))
            {
                if (!CanBeMoved) return;
                StartMove();
            }
        }
        else
        {
            if (_isBeingMoved) EndMove();
        }
    }

    private void HandleRotate(Vector2 clickWorldPosition)
    {
        if (TubeManager.Instance.CurrentTube != null && TubeManager.Instance.CurrentTube != this) return;

        if (!_isRotating && CanBeRotated && _tileCollider.OverlapPoint(clickWorldPosition))
        {
            AudioManager.Instance.Play("SFX_RotatePiece", "sfx");
            _isRotating = true;
            transform.DORotate(new Vector3(0f, 0f, 90f), 0.3f, RotateMode.LocalAxisAdd).SetEase(Ease.InOutQuad).OnComplete(() => _isRotating = false);
        }
    }

    public void StartMove()
    {
        AudioManager.Instance.Play("SFX_RemovePiece", "sfx");
        TubeManager.Instance.SetCurrentTube(this);
        _isBeingMoved = true;
        TubeRenderer.sortingOrder = 2;
    }

    public void EndMove()
    {
        TubeManager.Instance.SetCurrentTube(null);
        _isBeingMoved = false;
        Vector2? snappedPosition = TubeManager.Instance.TubeGrid.SnapPositionToGrid(transform.position);
        TubeRenderer.sortingOrder = -1;

        if (snappedPosition != null)
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll((Vector2)snappedPosition, Vector2.up, 0.1f);
            if (hits.Any(c => c.collider != _tileCollider && c.collider.GetComponent<Tube>() != null))
            {
                // There's already a tube in the target spot, we can't add it
                PoofAway();
                return;
            }
            AudioManager.Instance.Play("SFX_PlacePiece", "sfx");
            transform.position = (Vector2)snappedPosition;
        }
        else PoofAway();
    }

    private void PoofAway()
    {
        // TODO add sound
        Signals.Get<RecoverTubeSignal>().Dispatch(Type);
        Instantiate(poofFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
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
            Physics2D.OverlapCollider(exit, _exitsFilter, overlapping);

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
        CrossTube,
        Entrance,
        Exit,
        Random
    }
}