using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New piece")]
public class PieceData : ScriptableObject
{
    [SerializeField] private GameObject _tile;
    [SerializeField] private bool _isCentrified;
    [SerializeField] private Vector2Int[] _cells;

    public bool IsCentrified { get { return _isCentrified; } }
    public GameObject Tile { get => _tile; }
    public Vector2Int[] Cells { get => _cells; }

    public static List<Vector2Int> CalculateWallkicks(Vector2Int[] cells, int rotationIndex, int direction)
    {
        int height = CalculateHeight(cells);
        int width = CalculateWidth(cells);

        List<Vector2Int> result = new List<Vector2Int>();

        result.Add(new Vector2Int(0, 0));
        result.Add(new Vector2Int(0, -Mathf.RoundToInt(height / 2)));
        result.Add(new Vector2Int(0, Mathf.RoundToInt(height / 2)));
        result.Add(new Vector2Int(Mathf.RoundToInt(width / 2), 0));
        result.Add(new Vector2Int(-Mathf.RoundToInt(width / 2), 0));
        result.Add(new Vector2Int(Mathf.RoundToInt(width / 2), -Mathf.RoundToInt(height / 2)));
        result.Add(new Vector2Int(-Mathf.RoundToInt(width / 2), -Mathf.RoundToInt(height / 2)));
        result.Add(new Vector2Int(Mathf.RoundToInt(width / 2), Mathf.RoundToInt(height / 2)));
        result.Add(new Vector2Int(-Mathf.RoundToInt(width / 2), Mathf.RoundToInt(height / 2)));
        result.Add(new Vector2Int(0, -Mathf.RoundToInt(height / 2) - 1));
        result.Add(new Vector2Int(0, Mathf.RoundToInt(height / 2) + 1));
        result.Add(new Vector2Int(Mathf.RoundToInt(width / 2) + 1, 0));
        result.Add(new Vector2Int(-Mathf.RoundToInt(width / 2) - 1, 0));
        result.Add(new Vector2Int(Mathf.RoundToInt(width / 2) + 1, -Mathf.RoundToInt(height / 2) - 1));
        result.Add(new Vector2Int(-Mathf.RoundToInt(width / 2) - 1, -Mathf.RoundToInt(height / 2) + 1));
        result.Add(new Vector2Int(Mathf.RoundToInt(width / 2) + 1, Mathf.RoundToInt(height / 2) + 1));
        result.Add(new Vector2Int(-Mathf.RoundToInt(width / 2) - 1, Mathf.RoundToInt(height / 2) + 1));

        return result;
    }

    private static int CalculateWidth(Vector2Int[] cells)
    {
        int xMin = 0;
        int xMax = 0;

        foreach (var cell in cells)
        {
            if (cell.x < xMin)
            {
                xMin = cell.x;
            }
            else
            {
                xMax = cell.x;
            }
        }

        return xMax - xMin + 1;
    }

    private static int CalculateHeight(Vector2Int[] cells)
    {
        int yMin = 0;
        int yMax = 0;

        foreach (var cell in cells)
        {
            if (cell.y < yMin)
            {
                yMin = cell.y;
            }
            else
            {
                yMax = cell.y;
            }
        }

        return yMax - yMin + 1;
    }
}
