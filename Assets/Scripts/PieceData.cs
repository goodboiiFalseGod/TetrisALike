using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New piece")]
public class PieceData : ScriptableObject
{
    [SerializeField] private ColoredCell[] _tiles;
    [SerializeField] private bool _isCentrified;
    [SerializeField] private Vector2Int _spawnPositionOffset;
    [SerializeField] private Vector2Int[] _wallkicks;
    
    [Serializable]
    public struct ColoredCell
    {
        public Vector2Int Position;
        public GameObject Tile;
    }

    public Vector2Int SpawnPositionOffset { get => _spawnPositionOffset; }
    public bool IsCentrified { get { return _isCentrified; } }
    public ColoredCell[] Tiles { get => _tiles; }
    public Vector2Int[] Wallkicks { get => _wallkicks; }

    public void UpdateData(ColoredCell[] tiles)
    {
        _tiles = Centralize(tiles);
        _isCentrified = CheckIsCentrified(tiles);
        _wallkicks = CalculateWallkicks(tiles).ToArray();
        _spawnPositionOffset = CalculateSpawnPositionOffset(tiles);        
    }

    private static List<Vector2Int> CalculateWallkicks(ColoredCell[] cells)
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

    public static ColoredCell[] Centralize(ColoredCell[] cells)
    {
        ColoredCell[] result = new ColoredCell[cells.Length];

        float totalMass = 0;
        float totalX = 0;
        float totalY = 0;

        foreach (var c in cells)
        {
            totalMass++;

            totalX += c.Position.x;
            totalY += c.Position.y;
        }

        Vector2 centerMass = new Vector2(totalX / totalMass, totalY / totalMass);

        int close = 0;

        for (int i = 0; i < cells.Length; i++)
        {
            if (Vector2.Distance(centerMass, cells[i].Position) < Vector2.Distance(centerMass, cells[close].Position))
            {
                close = i;
            }
        }

        Vector2Int offset = cells[close].Position;

        for (int i = 0; i < cells.Length; i++)
        {
            result[i].Position = cells[i].Position - offset;
            result[i].Tile = cells[i].Tile;
        }

        return result;
    }

    private static Vector2Int CalculateSpawnPositionOffset(ColoredCell[] cells)
    {
        int top = 0;
        for(int i = 0; i < cells.Length; i++)
        {
            if(cells[i].Position.y > top)
            {
                top = cells[i].Position.y;
            }
        }
        return new Vector2Int(0, -top - 1);
    }

    private static bool CheckIsCentrified(ColoredCell[] cells)
    {
        bool result;

        float totalMass = 0;
        float totalX = 0;
        float totalY = 0;

        foreach (var c in cells)
        {
            totalMass++;

            totalX += c.Position.x;
            totalY += c.Position.y;
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

    private static int CalculateWidth(ColoredCell[] cells)
    {
        int xMin = 0;
        int xMax = 0;

        foreach (var cell in cells)
        {
            if (cell.Position.x < xMin)
            {
                xMin = cell.Position.x;
            }
            else
            {
                xMax = cell.Position.x;
            }
        }

        return xMax - xMin + 1;
    }

    private static int CalculateHeight(ColoredCell[] cells)
    {
        int yMin = 0;
        int yMax = 0;

        foreach (var cell in cells)
        {
            if (cell.Position.y < yMin)
            {
                yMin = cell.Position.y;
            }
            else
            {
                yMax = cell.Position.y;
            }
        }

        return yMax - yMin + 1;
    }
}
