using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomTilemap : MonoBehaviour
{
    [SerializeField] public RectInt Size;

    [SerializeField] private Dictionary<Vector2Int,Cell> _cells = new Dictionary<Vector2Int,Cell>();

    private void Start()
    {
        foreach (var pos in Size.allPositionsWithin)
        {
            _cells.Add(pos, new Cell(this, pos));
        }
    }

    public bool HasTile(Vector2Int cell)
    {
        if(!IsInbound(cell))
        {
            return false;
        }    

        if (_cells[cell].HasTile())
        {
            return true;
        }

        return false;
    }

    public GameObject GetTile(Vector2Int cell)
    {
        return _cells[cell].tile;
    }

    public List<Vector2Int> GetAllOccupiedCells()
    {
        List<Vector2Int> cells = new List<Vector2Int>();

        foreach (var pos in Size.allPositionsWithin)
        {
            if(HasTile(pos))
            {
                cells.Add(pos);
            }
        }

        return cells;
    }

    public bool ClearCell(Vector2Int cell)
    {
        return _cells[cell].ClearTile();
    }

    public void ClearAll()
    {
        foreach (var pos in Size.allPositionsWithin)
        {
            ClearCell(pos);
        }
    }

    public bool SetTile(Vector2Int cell, GameObject tile)
    {
        return _cells[cell].SetTile(tile);
    }

    public bool ReplaceTile(Vector2Int cell, GameObject tile)
    {
        return _cells[cell].ReplaceTile(tile);
    }

    public bool TransferCell(Vector2Int from, Vector2Int to)
    {
        if (!IsInbound(to) || !IsInbound(from))
        {
            return false;
        }

        if (!HasTile(from) || HasTile(to))
        {
            return false;
        }

        if (_cells[to].SetTile(GetTile(from)))
        {
            _cells[from].ClearTile();
        }

        return true;
    }

    public bool IsInbound(Vector2Int pos)
    {
        return Size.Contains(pos);
    }

    private class Cell
    {
        public GameObject tile;
        public CustomTilemap customTilemap;
        private int x;
        private int y;

        public int X { get => x; }
        public int Y { get => y; }

        public Vector2Int Position 
        { 
            get => new Vector2Int(x, y); 
            private set 
            {
                x = value.x; 
                y = value.y;
            } 
        }

        public Cell(CustomTilemap customTilemap, Vector2Int position)
        {
            this.customTilemap = customTilemap;
            Position = position;
        }

        public Cell(CustomTilemap customTilemap, Vector2Int position, GameObject tile)
        {
            this.customTilemap = customTilemap;
            Position = position;
            SetTile(tile);
        }

        public bool HasTile()
        {
            if (tile != null)
            {
                return true;
            }

            else return false;   
        }

        public bool ClearTile()
        {
            if (tile != null)
            {
                Destroy(tile);
                tile = null;

                return true;
            }
            return false;
        }

        public bool SetTile(GameObject newTile)
        {
            if (tile != null)
            {
                return false;
            }

            tile = Instantiate(newTile, customTilemap.transform);
            tile.transform.localPosition = Vector3.zero + (Vector3Int)this.Position;

            return true;
        }

        public bool ReplaceTile(GameObject newTile)
        {
            if (tile != null)
            {
                ClearTile();
            }

            tile = Instantiate(newTile, customTilemap.transform);
            tile.transform.localPosition = Vector3.zero + (Vector3Int)this.Position;

            return true;
        }
    }
}
