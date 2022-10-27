using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class ShowNextPiece : MonoBehaviour
{
    [SerializeField] private Text _nextPieceText;
    [SerializeField] private Board _mainBoard;
    [SerializeField] private Tilemap _tilemap;

    private Vector3Int[] _cells;

    public void UpdatePreview()
    {
        _nextPieceText.text = _mainBoard.NextPiece.tetromino.ToString();
        _cells = new Vector3Int[_mainBoard.NextPiece.cells.Length];
        _tilemap.ClearAllTiles();

        for(int i = 0; i < _mainBoard.NextPiece.cells.Length; i++)
        {
            _cells[i] = (Vector3Int)_mainBoard.NextPiece.cells[i];
        }

        Set();
    }

    private void Set()
    {
        for (int i = 0; i < _cells.Length; i++)
        {
            Vector3Int tilePosition = _cells[i];
            _tilemap.SetTile(tilePosition, _mainBoard.NextPiece.tile);
        }
    }
}
