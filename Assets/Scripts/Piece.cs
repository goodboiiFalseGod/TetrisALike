using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField] private float stepDelay = 0.75f;
    [SerializeField] private float moveDelay = 0.05f;

    //Review
    //naming?
    private float stepTime;
    private float moveTime;
    private Board _board;

    public Vector2Int Position { get; private set; }
    public PieceData.ColoredCell[] Cells { get; private set; }
    public PieceData PieceData { get; private set; }

    public void Initialize(Board board, Vector2Int position, PieceData data)
    {
        _board = board;
        Position = position;
        PieceData = data;

        stepTime = Time.time + stepDelay;
        moveTime = Time.time + moveDelay;

        Cells = new PieceData.ColoredCell[data.Tiles.Length];

        for (int i = 0; i < Cells.Length; i++) 
        {
            Cells[i] = data.Tiles[i];
        }
    }

    private void Update()
    {
        _board.Clear(this);

        HandleRotationInputs();
        HandleHardDropInputs();

        if (Time.time > moveTime)
        {
            HandleMoveInputs();
        }

        if (Time.time > stepTime)
        {
            Step();
        }

        _board.Set(this);
    }

    private void HandleHardDropInputs()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HardDrop();
        }
    }

    private void HandleRotationInputs()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Rotate(-1);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            Rotate(1);
        }
    }

    private void HandleMoveInputs()
    {
        if (Input.GetKey(KeyCode.S))
        {
            if (Move(Vector2Int.down))
            {
                stepTime = Time.time + stepDelay;
            }
        }

        if (Input.GetKey(KeyCode.A))
        {
            Move(Vector2Int.left);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Move(Vector2Int.right);
        }
    }

    private void Step()
    {
        stepTime = Time.time + stepDelay;
        if (Move(Vector2Int.down)) return;
        Lock();
    }
    private void HardDrop()
    {
        while (Move(Vector2Int.down)) {}

        Lock();
    }

    private void Lock()
    {
        _board.Set(this);
        _board.CheckAndClearLines();
        _board.SpawnPiece();
    }

    //Review
    //Метод не должен ничего двигать, только возврашать значение
    //Переименовать
    private bool Move(Vector2Int translation)
    {
        Vector2Int newPosition = Position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        bool valid = _board.IsValidPosition(newPosition, this.Cells);
        if (!valid) return false;
        
        Position = newPosition;
        moveTime = Time.time + moveDelay;

        return true;
    }

    private void Rotate(int direction)
    {
        ApplyRotationMatrix(direction);
        
        if (TestWallKicks()) return;
        
        ApplyRotationMatrix(-direction);
    }

    private void ApplyRotationMatrix(int direction)
    {
        float[] matrix = Data.RotationMatrix;
        
        for (int i = 0; i < Cells.Length; i++)
        {
            Vector2 cellPosition = Cells[i].Position;

            int x, y;

            if(!PieceData.IsCentrified)
            {
                x = Mathf.RoundToInt((cellPosition.x * matrix[0] * direction) + (cellPosition.y * matrix[1] * direction));
                y = Mathf.RoundToInt((cellPosition.x * matrix[2] * direction) + (cellPosition.y * matrix[3] * direction));
            }
            else
            {
                cellPosition.x -= 0.5f;
                cellPosition.y -= 0.5f;
                x = Mathf.CeilToInt((cellPosition.x * matrix[0] * direction) + (cellPosition.y * matrix[1] * direction));
                y = Mathf.CeilToInt((cellPosition.x * matrix[2] * direction) + (cellPosition.y * matrix[3] * direction));
            }

            Cells[i].Position = new Vector2Int(x, y);
        }
    }

    //Review
    //Метод не должен ничего двигать, только возврашать значение
    private bool TestWallKicks() => PieceData.Wallkicks.Any(Move);
}
