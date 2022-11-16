using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField] private float stepDelay = 0.75f;
    [SerializeField] private float moveDelay = 0.05f;

    private float stepTime;
    private float moveTime;
    private float lockTime;
    private Board _board;
    private PieceData _data;    
    private Vector2Int[] _cells;
    private Vector2Int _position;
    private int _rotationIndex;

    public Vector2Int Position { get => _position; }
    public Vector2Int[] Cells { get => _cells; }
    public PieceData PieceData { get => _data; }

    public void Initialize(Board board, Vector2Int position, PieceData data)
    {
        this._data = data;
        this._board = board;
        this._position = position;

        _rotationIndex = 0;
        stepTime = Time.time + stepDelay;
        moveTime = Time.time + moveDelay;
        lockTime = 0f;

        _cells = new Vector2Int[data.Cells.Length];

        for (int i = 0; i < _cells.Length; i++) {
            _cells[i] = data.Cells[i];
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
        Vector2Int newPosition = _position;
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
        if (!TestWallKicks((Vector2Int[])_cells, _rotationIndex, direction))
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
            Vector2 cell = _cells[i];

            int x, y;

            if(!_data.IsCentrified)
            {
                x = Mathf.RoundToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                y = Mathf.RoundToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
            }
            else
            {
                cell.x -= 0.5f;
                cell.y -= 0.5f;
                x = Mathf.CeilToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                y = Mathf.CeilToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
            }

            _cells[i] = new Vector2Int(x, y);
        }
    }

    private bool TestWallKicks(Vector2Int[] cells, int rotationIndex, int rotationDirection)
    {
        Vector2Int[] wallkicks = _data.Wallkicks;

        for (int i = 0; i < wallkicks.Length; i++)
        {
            Vector2Int translation = wallkicks[i];

            if (Move(translation)) {
                return true;
            }
        }

        return false;
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
