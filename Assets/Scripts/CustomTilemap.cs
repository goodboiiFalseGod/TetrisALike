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
        return _cells[cell]._tile;
    }

    public SpriteRenderer GetTileSpriteRenderer(Vector2Int cell)
    {
        return _cells[cell]._spriteRenderer;
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
        public GameObject _tile;
        public CustomTilemap _customTilemap;
        public SpriteRenderer _spriteRenderer;

        private Vector2Int _position;
        private int _x;
        private int _y;

        public int X => _x;
        public int Y => _y;
        public SpriteRenderer SpriteRenderer => _spriteRenderer;

        public Vector2Int Position 
        {
            get => _position;
            private set 
            {
                _position = value;
                _x = value.x; 
                _y = value.y;
            } 
        }

        public Cell(CustomTilemap customTilemap, Vector2Int position)
        {
            this._customTilemap = customTilemap;
            Position = position;
        }

        public Cell(CustomTilemap customTilemap, Vector2Int position, GameObject tile)
        {
            this._customTilemap = customTilemap;
            Position = position;
            SetTile(tile);
        }

        public bool HasTile()
        {
            if (_tile != null)
            {
                return true;
            }

            else return false;   
        }

        public bool ClearTile()
        {
            if (_tile != null)
            {
                Destroy(_tile);
                _tile = null;

                return true;
            }
            return false;
        }

        public bool SetTile(GameObject newTile)
        {
            if (_tile != null)
            {
                return false;
            }

            _tile = Instantiate(newTile, _customTilemap.transform);
            _tile.transform.localPosition = Vector3.zero + (Vector3Int)this.Position;
            _spriteRenderer = _tile.GetComponent<SpriteRenderer>();

            return true;
        }

        public bool ReplaceTile(GameObject newTile)
        {
            if (_tile != null)
            {
                ClearTile();
            }

            _tile = Instantiate(newTile, _customTilemap.transform);
            _tile.transform.localPosition = Vector3.zero + (Vector3Int)this.Position;
            _spriteRenderer = _tile.GetComponent<SpriteRenderer>();

            return true;
        }
    }
}
