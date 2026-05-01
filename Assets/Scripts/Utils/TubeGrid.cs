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
    [SerializeField]
    private Vector2 Overlap = Vector2.zero;

    void Start()
    {
        TubeManager.Instance.SetGrid(this);
    }

    [Button("Snap To Grid")]
    public void SnapObjectsToGrid()
    {
        foreach (Transform child in transform)
        {
            child.position = SnapPositionToGrid(child.position);
        }
    }

    public Vector2 SnapPositionToGrid(Vector2 worldPosition)
    {
        Vector2 localPosition = transform.InverseTransformPoint(worldPosition);
        Vector2 totalCellSize = CellSize - Overlap;
        int cellX = Mathf.RoundToInt(localPosition.x / totalCellSize.x);
        int cellY = Mathf.RoundToInt(localPosition.y / totalCellSize.y);

        if (cellX < 0 || cellX >= Columns || cellY < 0 || cellY >= Rows)
        {
            Debug.LogWarning($"Position {worldPosition} is outside the grid bounds on '{name}'.", this);
            return worldPosition;
        }

        return transform.TransformPoint(new Vector2(cellX * totalCellSize.x, cellY * totalCellSize.y));
    }
}
