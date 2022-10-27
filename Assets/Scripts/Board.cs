using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;

public class Board : MonoBehaviour
{
    [SerializeField] private Tilemap _tilemap;
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
        _tilemap.ClearAllTiles();
        _scoreManager.GameOver();
    }

    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.Cells.Length; i++)
        {
            Vector3Int tilePosition = piece.Cells[i] + piece.Position;
            _tilemap.SetTile(tilePosition, piece.PieceData.tile);
        }
    }

    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.Cells.Length; i++)
        {
            Vector3Int tilePosition = piece.Cells[i] + piece.Position;
            _tilemap.SetTile(tilePosition, null);
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


            if (_tilemap.HasTile(tilePosition)) {
                return false;
            }
        }

        return true;
    }

    public void ClearLines()
    {
        RectInt bounds = Bounds;
        int row = bounds.yMin;
        int linesCleared = 0;

        while (row < bounds.yMax)
        {
            if (IsLineFull(row)) {
                LineClear(row);
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

            if (!_tilemap.HasTile(position)) {
                return false;
            }
        }

        return true;
    }

    public void LineClear(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            _tilemap.SetTile(position, null);
        }

        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = _tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                _tilemap.SetTile(position, above);
            }

            row++;
        }
    }

}
