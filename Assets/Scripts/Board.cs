using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;

public class Board : MonoBehaviour
{
    //Review 
    //1. Убрать публичные методы, там где не нужны
    
    [SerializeField] private CustomTilemap _tilemap;
    [SerializeField] private Piece _activePiece;
    [SerializeField] private ScoreManager _scoreManager;
    [SerializeField] private ShowNextPiece _showNextPiece;

    [SerializeField] private PieceData[] _tetrominoes;
    [SerializeField] private Vector2Int _boardSize = new Vector2Int(10, 20);

    private Vector2Int _top;
    private PieceData _nextPiece;

    public PieceData NextPiece => _nextPiece;
    public Vector2Int BoardSize => _boardSize;

    //Review 
    //Заменить проперти на поле
    private RectInt Bounds {
        get
        {
            Vector2Int position = new Vector2Int(-_boardSize.x / 2, -_boardSize.y / 2);
            return new RectInt(position, _boardSize);
        }
    }

    private void Start()
    {
        _top = new Vector2Int(-Bounds.xMax / 2 + 1, Bounds.yMax);
        SpawnPiece(ChooseNextPiece());
        _nextPiece = ChooseNextPiece();
        _showNextPiece.UpdatePreview();
    }

    private PieceData ChooseNextPiece() => _tetrominoes[Random.Range(0, _tetrominoes.Length)];

    public void SpawnPiece()
    {
        _activePiece.Initialize(this, _nextPiece.SpawnPositionOffset + _top, _nextPiece);

        if (IsValidPosition(_nextPiece.SpawnPositionOffset + _top, _activePiece.Cells)) {
            Set(_activePiece);
        } else {
            GameOver();
        }

        _nextPiece = ChooseNextPiece();
        _showNextPiece.UpdatePreview();
    }

    private void SpawnPiece(PieceData data)
    {
        _activePiece.Initialize(this, data.SpawnPositionOffset + _top, data);

        if (IsValidPosition(data.SpawnPositionOffset + _top, _activePiece.Cells))
        {
            Set(_activePiece);
        }
        else
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        _tilemap.ClearAll();
        _scoreManager.GameOver();
    }

    //Review
    //Поменять параметр на IEnumerable<PieceData.ColoredCell> coloredCells
    public void Set(Piece piece)
    {
        foreach (PieceData.ColoredCell cell in piece.Cells)
        {
            Vector2Int tilePosition = cell.Position + piece.Position;
            _tilemap.SetTile(tilePosition, cell.Tile);
        }
    }

    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.Cells.Length; i++)
        {
            Vector2Int tilePosition = piece.Cells[i].Position + piece.Position;
            if(_tilemap.HasTile(tilePosition))
            {
                _tilemap.ClearCell(tilePosition);
            }
        }
    }

    public bool IsValidPosition(Vector2Int position, IEnumerable<PieceData.ColoredCell> coloredCells)
    {
        RectInt bounds = Bounds;

        foreach (PieceData.ColoredCell coloredCell in coloredCells)
        {
            Vector2Int tilePosition = coloredCell.Position + position;

            if (!bounds.Contains(tilePosition))
                return false;

            if (_tilemap.HasTile(tilePosition))
                return false;
        }
        
        return true;
    }

    public void CheckAndClearLines()
    {
        RectInt bounds = Bounds;
        int linesCleared = 0;
        List<int> toClear = new List<int>();

        for (int row = bounds.yMax; row >= bounds.yMin; row--)
        {
            if (!IsLineFull(row)) continue;
            
            toClear.Add(row);
            linesCleared++;
        }

        if (linesCleared <= 0) return;
        
        AnimateClear(toClear);
        _scoreManager.AddScore(linesCleared);
    }

    private bool IsLineFull(int row)
    {
        for (int column = Bounds.xMin; column < Bounds.xMax; column++)
        {
            Vector2Int position = new Vector2Int(column, row);

            if (!_tilemap.HasTile(position))
            {
                return false;
            }
        }

        return true;
    }

    public void AnimateClear(List<int> rows)
    {
        Sequence sequence = DOTween.Sequence();

        foreach(int row in rows)
        {
            for (int col = _tilemap.Size.xMin; col < _tilemap.Size.xMax; col++)
            {
                Vector2Int position = new Vector2Int(col, row);
                //Review
                // ВЫЗОВ ГЕТ КОМПОНЕНТ В ДВОЙНОМ ЦИКЛЕ?!?!??!?!?!?!?!?!?!?!
                sequence.Join(_tilemap.GetTile(position).GetComponent<SpriteRenderer>().DOFade(0f, 0.5f));
            }
        }

        sequence.OnComplete(() => ClearLine(rows));
    }


    //Review
    //Вынести основную логику в отдельный метод
    private void ClearLine(List<int> rows)
    {
        foreach (int rowIndex in rows)
        {
            for (int row = rowIndex; row < _tilemap.Size.yMax; row++)
            {
                for (int col = _tilemap.Size.xMin; col < _tilemap.Size.xMax; col++)
                {
                    Vector2Int position = new Vector2Int(col, row + 1);
                    Vector2Int newPosition = new Vector2Int(col, row);
                    
                    if (_activePiece.Cells
                        .Select(cell => cell.Position + _activePiece.Position)
                        .Contains(position)) continue;

                    if (!_tilemap.HasTile(position))
                    {
                        _tilemap.ClearCell(newPosition);
                        continue;
                    }

                    _tilemap.ClearCell(position);
                    _tilemap.ReplaceTile(newPosition, _tilemap.GetTile(position));
                }
            }
        }
    }

}
