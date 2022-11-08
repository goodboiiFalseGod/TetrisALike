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

    [SerializeField] private PieceData[] _tetrominoes;
    [SerializeField] private Vector2Int _boardSize = new Vector2Int(10, 20);
    [SerializeField] private Vector2Int _spawnPosition = new Vector2Int(-1, 8);

    private List<Vector2Int> _activePieceCells = new List<Vector2Int>();

    private PieceData _nextPiece;

    public PieceData NextPiece { get => _nextPiece; }

    public Vector2Int BoardSize { get => _boardSize; }

    public RectInt Bounds {
        get
        {
            Vector2Int position = new Vector2Int(-_boardSize.x / 2, -_boardSize.y / 2);
            return new RectInt(position, _boardSize);
        }
    }

    private void Start()
    {
        SpawnPiece(ChooseNextPiece());
        _nextPiece = ChooseNextPiece();
        _showNextPiece.UpdatePreview();
    }

    public PieceData ChooseNextPiece()
    {
        int random = Random.Range(0, _tetrominoes.Length);
        PieceData data = _tetrominoes[random];

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

    public void SpawnPiece(PieceData data)
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
            Vector2Int tilePosition = piece.Cells[i] + piece.Position;
            _tilemap.SetTile((Vector2Int)tilePosition, piece.PieceData.Tile);
            _activePieceCells.Add((Vector2Int)tilePosition);
        }
    }

    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.Cells.Length; i++)
        {
            Vector2Int tilePosition = piece.Cells[i] + piece.Position;
            _tilemap.ClearCell((Vector2Int)tilePosition);
        }
    }

    public bool IsValidPosition(Piece piece, Vector2Int position)
    {
        RectInt bounds = Bounds;

        for (int i = 0; i < piece.Cells.Length; i++)
        {
            Vector2Int tilePosition = piece.Cells[i] + position;

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

        sequence.OnComplete(() => ClearLine(rows));
    }


    public void ClearLine(List<int> rows)
    {
        for (int i = 0; i < rows.Count; i++)
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
                        Vector2Int newPosition = new Vector2Int(col, row);
                        _tilemap.ReplaceTile(newPosition, _tilemap.GetTile(position));
                        _tilemap.ClearCell(position);
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

        /*foreach (int r in rows)
        {
            for (int col = _tilemap.Size.xMin; col < _tilemap.Size.xMax; col++)
            {
                Vector2Int position = new Vector2Int(col, r);
                _tilemap.ClearCell(position);
            }
        }*/

    }

}
