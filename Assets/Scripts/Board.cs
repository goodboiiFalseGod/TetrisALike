using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Board : MonoBehaviour
{
    [SerializeField] private CustomTilemap _tilemap;
    [SerializeField] private Piece _activePiece;
    [SerializeField] private ScoreManager _scoreManager;
    [SerializeField] private ShowNextPiece _showNextPiece;

    [SerializeField] private TetrominoData[] _tetrominoes;
    [SerializeField] private Vector2Int _boardSize = new Vector2Int(10, 20);
    [SerializeField] private Vector3Int _spawnPosition = new Vector3Int(-1, 8, 0);

    private List<Vector2Int> _activePieceCells = new List<Vector2Int>();

    private TetrominoData _nextPiece;

    public TetrominoData NextPiece { get => _nextPiece; }

    public Vector2Int BoardSize { get => _boardSize; }

    public RectInt Bounds {
        get
        {
            Vector2Int position = new Vector2Int(-_boardSize.x / 2, -_boardSize.y / 2);
            return new RectInt(position, _boardSize);
        }
    }

    private void Awake()
    {
        for (int i = 0; i < _tetrominoes.Length; i++) {
            _tetrominoes[i].Initialize();
        }
    }

    private void Start()
    {
        SpawnPiece(ChooseNextPiece());
        _nextPiece = ChooseNextPiece();
        _showNextPiece.UpdatePreview();
    }

    public TetrominoData ChooseNextPiece()
    {
        int random = Random.Range(0, _tetrominoes.Length);
        TetrominoData data = _tetrominoes[random];

        return data;
    }

    public void SpawnPiece()
    {
        _activePiece.Initialize(this, _spawnPosition, _nextPiece);

        if (IsValidPosition(_activePiece, _spawnPosition)) {
            Set(_activePiece);
        } else {
            GameOver();
        }

        _nextPiece = ChooseNextPiece();
        _showNextPiece.UpdatePreview();
    }

    public void SpawnPiece(TetrominoData data)
    {
        _activePiece.Initialize(this, _spawnPosition, data);

        if (IsValidPosition(_activePiece, _spawnPosition))
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

    public void Set(Piece piece)
    {
        _activePieceCells.Clear();
        for (int i = 0; i < piece.Cells.Length; i++)
        {
            Vector3Int tilePosition = piece.Cells[i] + piece.Position;
            _tilemap.SetTile((Vector2Int)tilePosition, _tilemap.CustomTiles[0]);
            _activePieceCells.Add((Vector2Int)tilePosition);
        }
    }

    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.Cells.Length; i++)
        {
            Vector3Int tilePosition = piece.Cells[i] + piece.Position;
            _tilemap.ClearCell((Vector2Int)tilePosition);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = Bounds;

        for (int i = 0; i < piece.Cells.Length; i++)
        {
            Vector3Int tilePosition = piece.Cells[i] + position;

            if (!bounds.Contains((Vector2Int)tilePosition)) {
                return false;
            }


            if (_tilemap.HasTile((Vector2Int)tilePosition)) {
                return false;
            }
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
            if (IsLineFull(row))
            {
                toClear.Add(row);
                linesCleared++;
            }
        }

        if(linesCleared > 0)
        {
            AnimateClear(toClear);
            _scoreManager.AddScore(linesCleared);
        }
    }

    public bool IsLineFull(int row)
    {
        RectInt bounds = Bounds;

        for (int column = bounds.xMin; column < bounds.xMax; column++)
        {
            Vector3Int position = new Vector3Int(column, row, 0);

            if (!_tilemap.HasTile((Vector2Int)position)) {
                return false;
            }
        }

        return true;
    }

    public void AnimateClear(List<int> rows)
    {
        Sequence sequence = DOTween.Sequence();

        foreach(int r in rows)
        {
            for (int col = _tilemap.Size.xMin; col < _tilemap.Size.xMax; col++)
            {
                Vector2Int position = new Vector2Int(col, r);
                sequence.Join(_tilemap.GetTile(position).GetComponent<SpriteRenderer>().DOFade(0f, 0.5f));
            }
        }

        /*for (int i = 0; i < rows.Count; i++)
        {
            int row = rows[i];

            while (row < _tilemap.Size.yMax)
            {
                for (int col = _tilemap.Size.xMin; col < _tilemap.Size.xMax; col++)
                {
                    Vector2Int position = new Vector2Int(col, row + 1);

                    if (_activePieceCells.Contains(position))
                    {
                        continue;
                    }

                    if (_tilemap.HasTile(position))
                    {
                        Vector3 targetPosition = _tilemap.GetTile(position).transform.position + Vector3.down * (rows.Count - 1);
                        Vector2Int newPosition = new Vector2Int(col, row);
                        sequence.Join(_tilemap.GetTile(position).transform.DOMove(targetPosition, 0.5f));
                    }
                }

                row++;
            }
        }*/

        sequence.OnComplete(() => ClearLine(rows));
    }


    public void ClearLine(List<int> rows)
    {
        foreach(int r in rows)
        {
            for (int col = _tilemap.Size.xMin; col < _tilemap.Size.xMax; col++)
            {
                Vector2Int position = new Vector2Int(col, r);
                _tilemap.ClearCell(position);
            }
        }

        for(int i = 0; i < rows.Count; i++)
        {
            int row = rows[i];

            while (row < _tilemap.Size.yMax)
            {
                for (int col = _tilemap.Size.xMin; col < _tilemap.Size.xMax; col++)
                {
                    Vector2Int position = new Vector2Int(col, row + 1);

                    if (_activePieceCells.Contains(position))
                    {
                        continue;
                    }

                    if (_tilemap.HasTile(position))
                    {
                        position = new Vector2Int(col, row);
                        _tilemap.SetTile(position, _tilemap.CustomTiles[0]);
                    }
                    else
                    {
                        position = new Vector2Int(col, row);
                        _tilemap.ClearCell(position);
                    }
                }

                row++;
            }
        }

    }

}
