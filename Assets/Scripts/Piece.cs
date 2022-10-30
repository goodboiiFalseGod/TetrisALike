using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField] private float stepDelay = 0.75f;
    [SerializeField] private float moveDelay = 0.05f;
    [SerializeField] private float lockDelay = 0f;

    private float stepTime;
    private float moveTime;
    private float lockTime;
    private Board _board;
    private TetrominoData _data;
    private Vector3Int[] _cells;
    private Vector3Int _position;
    private int _rotationIndex;

    public Vector3Int Position { get => _position; }
    public Vector3Int[] Cells { get => _cells; }
    public TetrominoData PieceData { get => _data; }

    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        this._data = data;
        this._board = board;
        this._position = position;

        _rotationIndex = 0;
        stepTime = Time.time + stepDelay;
        moveTime = Time.time + moveDelay;
        lockTime = 0f;

        if (_cells == null) {
            _cells = new Vector3Int[data.cells.Length];
        }

        for (int i = 0; i < _cells.Length; i++) {
            _cells[i] = (Vector3Int)data.cells[i];
        }
    }

    private void Update()
    {
        _board.Clear(this);
        lockTime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Q)) {
            Rotate(-1);
        } else if (Input.GetKeyDown(KeyCode.E)) {
            Rotate(1);
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            HardDrop();
        }

        if (Time.time > moveTime) {
            HandleMoveInputs();
        }

        if (Time.time > stepTime) {
            Step();
        }

        _board.Set(this);
    }

    private void HandleMoveInputs()
    {
        if (Input.GetKey(KeyCode.S))
        {
            if (Move(Vector2Int.down)) {
                stepTime = Time.time + stepDelay;
            }
        }

        if (Input.GetKey(KeyCode.A)) {
            Move(Vector2Int.left);
        } else if (Input.GetKey(KeyCode.D)) {
            Move(Vector2Int.right);
        }
    }

    private void Step()
    {
        stepTime = Time.time + stepDelay;
        if (!Move(Vector2Int.down))
        {
            Lock();
        }
    }

    public void PauseGame()
    {

    }

    public void ResumeGame()
    {

    }

    private void HardDrop()
    {
        while (Move(Vector2Int.down)) {
            continue;
        }

        Lock();
    }

    private void Lock()
    {
        _board.Set(this);
        _board.CheckAndClearLines();
        _board.SpawnPiece();
    }

    private bool Move(Vector2Int translation)
    {
        Vector3Int newPosition = _position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        bool valid = _board.IsValidPosition(this, newPosition);
        if (valid)
        {
            _position = newPosition;
            moveTime = Time.time + moveDelay;
            lockTime = 0f;
        }

        return valid;
    }

    private void Rotate(int direction)
    {
        int originalRotation = _rotationIndex;
        _rotationIndex = Wrap(_rotationIndex + direction, 0, 4);
        ApplyRotationMatrix(direction);
        if (!TestWallKicks(_rotationIndex, direction))
        {
            _rotationIndex = originalRotation;
            ApplyRotationMatrix(-direction);
        }
    }

    private void ApplyRotationMatrix(int direction)
    {
        float[] matrix = Data.RotationMatrix;
        for (int i = 0; i < _cells.Length; i++)
        {
            Vector3 cell = _cells[i];

            int x, y;

            switch (_data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                    break;

                default:
                    x = Mathf.RoundToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                    break;
            }

            _cells[i] = new Vector3Int(x, y, 0);
        }
    }

    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for (int i = 0; i < _data.wallKicks.GetLength(1); i++)
        {
            Vector2Int translation = _data.wallKicks[wallKickIndex, i];

            if (Move(translation)) {
                return true;
            }
        }

        return false;
    }

    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = rotationIndex * 2;

        if (rotationDirection < 0) {
            wallKickIndex--;
        }

        return Wrap(wallKickIndex, 0, _data.wallKicks.GetLength(0));
    }

    private int Wrap(int input, int min, int max)
    {
        if (input < min) {
            return max - (min - input) % (max - min);
        } else {
            return min + (input - min) % (max - min);
        }
    }

}
