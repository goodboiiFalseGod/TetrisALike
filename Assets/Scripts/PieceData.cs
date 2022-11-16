using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New piece")]
public class PieceData : ScriptableObject
{
    [SerializeField] private GameObject _tile;
    [SerializeField] private bool _isCentrified;
    [SerializeField] private Vector2Int[] _cells;
    [SerializeField] private Vector2Int _spawnPositionOffset;
    [SerializeField] private Vector2Int[] _wallkicks;

    public Vector2Int SpawnPositionOffset { get => _spawnPositionOffset; }
    public bool IsCentrified { get { return _isCentrified; } }
    public GameObject Tile { get => _tile; }
    public Vector2Int[] Cells { get => _cells; }
    public Vector2Int[] Wallkicks { get => _wallkicks; }

    public void UpdateData(GameObject tile, Vector2Int[] cells)
    {
        _cells = Centralize(cells);
        _tile = tile;
        _isCentrified = CheckIsCentrified(cells);
        _wallkicks = CalculateWallkicks(cells).ToArray();
        _spawnPositionOffset = CalculateSpawnPositionOffset(cells);
    }

    private static List<Vector2Int> CalculateWallkicks(Vector2Int[] cells)
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

    public static Vector2Int[] Centralize(Vector2Int[] cells)
    {
        Vector2Int[] result = new Vector2Int[cells.Length];

        float totalMass = 0;
        float totalX = 0;
        float totalY = 0;

        foreach(var c in cells)
        {
            totalMass++;

            totalX += c.x;
            totalY += c.y;
        }

        Vector2 centerMass = new Vector2(totalX / totalMass, totalY / totalMass);

        int close = 0; 

        for(int i = 0; i < cells.Length; i++)
        {
            if(Vector2.Distance(centerMass, cells[i]) < Vector2.Distance(centerMass, cells[close]))
            {
                close = i;
            }
        }

        Vector2Int offset = cells[close];

        for (int i = 0; i < cells.Length; i++)
        {
            cells[i] -= offset;
        }

        result = cells;

        return result;
    }

    private static Vector2Int CalculateSpawnPositionOffset(Vector2Int[] cells)
    {
        int top = 0;
        for(int i = 0; i < cells.Length; i++)
        {
            if(cells[i].y > top)
            {
                top = cells[i].y;
            }
        }
        return new Vector2Int(0, -top - 1);
    }

    private static bool CheckIsCentrified(Vector2Int[] cells)
    {
        bool result;

        float totalMass = 0;
        float totalX = 0;
        float totalY = 0;

        foreach (var c in cells)
        {
            totalMass++;

            totalX += c.x;
            totalY += c.y;
        }

        Vector2 centerMass = new Vector2(totalX / totalMass, totalY / totalMass);

        if (centerMass.x % 1 == 0 && centerMass.y % 1 == 0)
        {
            result = true;
        }
        else
        {
            result = false;
        }

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
