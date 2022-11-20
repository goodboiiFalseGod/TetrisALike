using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class ShowNextPiece : MonoBehaviour
{
    [SerializeField] private Board _mainBoard;
    [SerializeField] private CustomTilemap _tilemap;

    private PieceData.ColoredCell[] _cells;

    public void UpdatePreview()
    {
        _cells = new PieceData.ColoredCell[_mainBoard.NextPiece.Tiles.Length];
        _tilemap.ClearAll();

        for(int i = 0; i < _mainBoard.NextPiece.Tiles.Length; i++)
        {
            _cells[i] = _mainBoard.NextPiece.Tiles[i];
        }

        Set();
    }

    private void Set()
    {
        for (int i = 0; i < _cells.Length; i++)
        {
            PieceData.ColoredCell tilePosition = _cells[i];
            _tilemap.SetTile(tilePosition.Position, _mainBoard.NextPiece.Tiles[i].Tile);
        }
    }
}
