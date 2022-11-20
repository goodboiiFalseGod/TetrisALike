using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;

public class Board : MonoBehaviour
{
    [SerializeField] private CustomTilemap _tilemap;
    [SerializeField] private Piece _activePiece;
    [SerializeField] private ScoreManager _scoreManager;
    [SerializeField] private ShowNextPiece _showNextPiece;

    [SerializeField] private PieceData[] _tetrominoes;
    [SerializeField] private Vector2Int _boardSize = new Vector2Int(10, 20);

    private RectInt _bounds;
    private Vector2Int _top;
    private PieceData _nextPiece;

    public PieceData NextPiece => _nextPiece;
    public Vector2Int BoardSize => _boardSize;
    public RectInt Bounds => _bounds;

    private void Start()
    {
        Vector2Int position = new Vector2Int(-_boardSize.x / 2, -_boardSize.y / 2);
        _bounds = new RectInt(position, _boardSize);
        _top = new Vector2Int(-_bounds.xMax / 2 + 1, _bounds.yMax);
        SpawnPiece(ChooseNextPiece());
        _nextPiece = ChooseNextPiece();
        _showNextPiece.UpdatePreview();
    }

    private PieceData ChooseNextPiece() => _tetrominoes[Random.Range(0, _tetrominoes.Length)];

    public void SpawnPiece()
    {
        _activePiece.Initialize(this, _nextPiece.SpawnPositionOffset + _top, _nextPiece);

        if (IsValidPosition(_nextPiece.SpawnPositionOffset + _top, _activePiece.Cells)) {
            Set(_activePiece.Cells, _activePiece.Position);
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
            Set(_activePiece.Cells, _activePiece.Position);
        }
        else
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        _tilemap.ClearAll();
        _scoreManager.GameOver();
    }

    public void Set(IEnumerable<PieceData.ColoredCell> pieceCells, Vector2Int piecePosition)
    {
        foreach (PieceData.ColoredCell cell in pieceCells)
        {
            Vector2Int tilePosition = cell.Position + piecePosition;
            _tilemap.SetTile(tilePosition, cell.Tile);
        }
    }

    public void Clear(IEnumerable<PieceData.ColoredCell> pieceCells, Vector2Int piecePosition)
    {
        foreach (PieceData.ColoredCell cell in pieceCells)
        {
            Vector2Int tilePosition = cell.Position + piecePosition;
            if(_tilemap.HasTile(tilePosition))
            {
                _tilemap.ClearCell(tilePosition);
            }
        }
    }

    public bool IsValidPosition(Vector2Int position, IEnumerable<PieceData.ColoredCell> coloredCells)
    {
        RectInt bounds = _bounds;

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
        RectInt bounds = _bounds;
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
        for (int column = _bounds.xMin; column < _bounds.xMax; column++)
        {
            Vector2Int position = new Vector2Int(column, row);

            if (!_tilemap.HasTile(position))
            {
                return false;
            }
        }

        return true;
    }

    private void AnimateClear(List<int> rows)
    {
        DOTween.KillAll();
        Sequence sequence = DOTween.Sequence();

        foreach(int row in rows)
        {
            for (int col = _tilemap.Size.xMin; col < _tilemap.Size.xMax; col++)
            {
                Vector2Int position = new Vector2Int(col, row);
                sequence.Join(_tilemap.GetTileSpriteRenderer(position).DOFade(0f, 0.5f));
            }
        }

        sequence.OnComplete(() => ClearLinesIndex(rows));
    }

    private void ClearLinesIndex(List<int> rowsIndex)
    {
        foreach (int rowIndex in rowsIndex)
        {
            for (int row = rowIndex; row < _tilemap.Size.yMax; row++)
            {
                ClearLineIndex(row);
            }
        }
    }

    private void ClearLineIndex(int rowIndex)
    {
        for (int col = _tilemap.Size.xMin; col < _tilemap.Size.xMax; col++)
        {
            Vector2Int rowPosition = new Vector2Int(col, rowIndex);
            Vector2Int upperRowPosition = new Vector2Int(col, rowIndex + 1);

            if (_activePiece.Cells
                .Select(cell => cell.Position + _activePiece.Position)
                .Contains(upperRowPosition)) continue;

            if (!_tilemap.HasTile(upperRowPosition))
            {
                _tilemap.ClearCell(rowPosition);
                continue;
            }

            _tilemap.ReplaceTile(rowPosition, _tilemap.GetTile(upperRowPosition));
            _tilemap.ClearCell(upperRowPosition);
        }
    }

}
