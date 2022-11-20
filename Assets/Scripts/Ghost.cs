using UnityEngine;
using UnityEngine.Tilemaps;

public class Ghost : MonoBehaviour
{
    [SerializeField] private Tile _tile;
    [SerializeField] private Board _mainBoard;
    [SerializeField] private Piece _trackingPiece;
    [SerializeField] private Tilemap _tilemap;

    private Vector3Int[] cells;
    private Vector3Int position;

    private void Awake()
    {
        cells = new Vector3Int[4];
    }

    private void LateUpdate()
    {
        Clear();
        Copy();
        Drop();
        Set();
    }

    private void Clear()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3Int tilePosition = cells[i] + position;
            _tilemap.SetTile(tilePosition, null);
        }
    }

    private void Copy()
    {
        cells = new Vector3Int[_trackingPiece.Cells.Length];
        for (int i = 0; i < cells.Length; i++) {
            cells[i] = (Vector3Int)_trackingPiece.Cells[i].Position;
        }
    }

    private void Drop()
    {
        Vector3Int position = (Vector3Int)_trackingPiece.Position;

        int current = position.y;
        int bottom = -_mainBoard.BoardSize.y / 2 - 1;

        _mainBoard.Clear(_trackingPiece);

        for (int row = current; row >= bottom; row--)
        {
            position.y = row;

            if (_mainBoard.IsValidPosition(_trackingPiece, (Vector2Int)position)) {
                this.position = position;
            } else {
                break;
            }
        }

        _mainBoard.Set(_trackingPiece);
    }

    private void Set()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3Int tilePosition = cells[i] + position;
            _tilemap.SetTile(tilePosition, _tile);
        }
    }

}
