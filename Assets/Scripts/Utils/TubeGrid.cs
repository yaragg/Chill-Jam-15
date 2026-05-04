using NaughtyAttributes;
using UnityEngine;

public class TubeGrid : MonoBehaviour
{
    [SerializeField]
    private int Rows = 1;
    [SerializeField]
    private int Columns = 1;
    [SerializeField]
    private Vector2 CellSize = Vector2.one;
    public Vector2 Overlap = Vector2.zero;
    public Vector3 Offset = new Vector3(0.5f, 0.5f, 0f);

    void Start()
    {
        TubeManager.Instance.SetGrid(this);
    }

    [Button("Snap To Grid")]
    public void SnapObjectsToGrid()
    {
        foreach (Transform child in transform)
        {
            Vector2? snappedPosition = SnapPositionToGrid(child.position);
            if (snappedPosition != null) child.position = (Vector2) snappedPosition;
        }
    }

    public float GetWidth ()
    {
        return Columns * (CellSize.x - Overlap.x);
    }

    public Vector2? SnapPositionToGrid(Vector2 worldPosition)
    {
        Vector2 localPosition = transform.InverseTransformPoint(worldPosition);
        Vector2 totalCellSize = CellSize - Overlap;
        int cellX = Mathf.RoundToInt(localPosition.x / totalCellSize.x);
        int cellY = Mathf.RoundToInt(localPosition.y / totalCellSize.y);

        if (cellX < 0 || cellX >= Columns || cellY < 0 || cellY >= Rows)
        {
            Debug.LogWarning($"Position {worldPosition} is outside the grid bounds on '{name}'.", this);
            return null;
        }

        return transform.TransformPoint(new Vector2(cellX * totalCellSize.x, cellY * totalCellSize.y));
    }

    private void OnDrawGizmos ()
    {
        Gizmos.color = Color.green;
        Vector2 totalCellSize = CellSize - Overlap;
        for (int x = 0; x <= Columns; x++)
        {
            Vector3 start = transform.TransformPoint(new Vector2(x * totalCellSize.x, 0)) -Offset;
            Vector3 end = transform.TransformPoint(new Vector2(x * totalCellSize.x, Rows * totalCellSize.y)) -Offset;
            Gizmos.DrawLine(start, end);
        }
        for (int y = 0; y <= Rows; y++)
        {
            Vector3 start = transform.TransformPoint(new Vector2(0, y * totalCellSize.y)) -Offset;
            Vector3 end = transform.TransformPoint(new Vector2(Columns * totalCellSize.x, y * totalCellSize.y)) -Offset;
            Gizmos.DrawLine(start, end);
        }
    }
}
