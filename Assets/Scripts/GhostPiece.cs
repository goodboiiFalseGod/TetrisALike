using UnityEngine;
using UnityEngine.Tilemaps;

public class GhostPiece : MonoBehaviour
{
    [SerializeField] private GameObject _tile;
    [SerializeField] private Board _mainBoard;
    [SerializeField] private Piece _trackingPiece;
    [SerializeField] private CustomTilemap _tilemap;

    private Vector2Int[] _cells;
    private Vector2Int _position;

    private void Awake()
    {
        _cells = new Vector2Int[4];
        _trackingPiece.PiecePositionChanged += OnGameStepped;
    }

    private void OnGameStepped()
    {
        Clear();
        Copy();
        Drop();
        Set();
    }

    private void Clear()
    {
        for (int i = 0; i < _cells.Length; i++)
        {
            Vector2Int tilePosition = _cells[i] + _position;
            _tilemap.ClearCell(tilePosition);
        }
    }

    private void Copy()
    {
        _cells = new Vector2Int[_trackingPiece.Cells.Length];
        for (int i = 0; i < _cells.Length; i++) {
            _cells[i] = _trackingPiece.Cells[i].Position;
        }
    }

    private void Drop()
    {
        Vector2Int position = _trackingPiece.Position;

        int current = position.y;
        int bottom = _mainBoard.Bounds.yMin;

        for (int row = bottom; row <= current; row++)
        {
            position.y = row;

            if (!_mainBoard.IsValidPosition(position, _trackingPiece.Cells)) 
            {
                continue;
            } 
            else 
            {
                _position = position;
                return;
            }
        }
    }

    private void Set()
    {
        for (int i = 0; i < _cells.Length; i++)
        {
            Vector2Int tilePosition = _cells[i] + _position;
            _tilemap.SetTile(tilePosition, _tile);
        }
    }

}
