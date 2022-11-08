using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class ShowNextPiece : MonoBehaviour
{
    [SerializeField] private Board _mainBoard;
    [SerializeField] private CustomTilemap _tilemap;

    private Vector2Int[] _cells;

    public void UpdatePreview()
    {
        _cells = new Vector2Int[_mainBoard.NextPiece.Cells.Length];
        _tilemap.ClearAll();

        for(int i = 0; i < _mainBoard.NextPiece.Cells.Length; i++)
        {
            _cells[i] = _mainBoard.NextPiece.Cells[i];
        }

        Set();
    }

    private void Set()
    {
        for (int i = 0; i < _cells.Length; i++)
        {
            Vector2Int tilePosition = _cells[i];
            _tilemap.SetTile(tilePosition, _mainBoard.NextPiece.Tile);
        }
    }
}
