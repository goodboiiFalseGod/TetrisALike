using System.Collections.Generic;
using UnityEngine;

public class ShowNextPiece : MonoBehaviour
{
    [SerializeField] private Board _mainBoard;
    [SerializeField] private CustomTilemap _tilemap;

    public void UpdatePreview()
    {
        _tilemap.ClearAll();
        PieceData.ColoredCell[] cells = _mainBoard.NextPiece.Tiles;
        Set(cells);
    }

    private void Set(IEnumerable<PieceData.ColoredCell> cells)
    {
        foreach (PieceData.ColoredCell cell in cells)
        {
            _tilemap.SetTile(cell.Position, cell.Tile);
        }
    }
}
