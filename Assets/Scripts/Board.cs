using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;

public class Board : MonoBehaviour
{
    [SerializeField] private CustomTilemap _tilemap;
    [SerializeField] private Piece _activePiece;
    [SerializeField] private ScoreManager _scoreManager;
    [SerializeField] private ShowNextPiece _showNextPiece;

    [SerializeField] private TetrominoData[] _tetrominoes;
    [SerializeField] private Vector2Int _boardSize = new Vector2Int(10, 20);
    [SerializeField] private Vector3Int _spawnPosition = new Vector3Int(-1, 8, 0);      

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
        for (int i = 0; i < piece.Cells.Length; i++)
        {
            Vector3Int tilePosition = piece.Cells[i] + piece.Position;
            _tilemap.SetTile((Vector2Int)tilePosition, _tilemap.CustomTiles[0]);
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
        int row = bounds.yMin;
        int linesCleared = 0;

        while (row < bounds.yMax)
        {
            if (IsLineFull(row)) {
                ClearLine(row);
                linesCleared++;
            } else {
                row++;
            }
        }

        _scoreManager.AddScore(linesCleared);
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

    public void ClearLine(int row)
    {
        for (int col = _tilemap.Size.xMin; col < _tilemap.Size.xMax; col++)
        {
            Vector2Int position = new Vector2Int(col, row);
            _tilemap.ClearCell(position);
        }

        while (row < _tilemap.Size.yMax)
        {
            for (int col = _tilemap.Size.xMin; col < _tilemap.Size.xMax; col++)
            {
                Vector2Int position = new Vector2Int(col, row + 1);

                if(_tilemap.HasTile(position))
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
