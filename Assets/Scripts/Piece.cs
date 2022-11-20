using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField] private float _stepDelay = 0.75f;
    [SerializeField] private float _moveDelay = 0.05f;
    private float _stepTime;
    private float _moveTime;
    private Board _board;

    public Vector2Int Position { get; private set; }
    public PieceData.ColoredCell[] Cells { get; private set; }
    public PieceData PieceData { get; private set; }

    public event Action PiecePositionChanged;

    public void Initialize(Board board, Vector2Int position, PieceData data)
    {
        _board = board;
        Position = position;
        PieceData = data;

        _stepTime = Time.time + _stepDelay;
        _moveTime = Time.time + _moveDelay;

        Cells = new PieceData.ColoredCell[data.Tiles.Length];

        for (int i = 0; i < Cells.Length; i++) 
        {
            Cells[i] = data.Tiles[i];
        }
    }

    private void Update()
    {
        _board.Clear(Cells, Position);

        HandleRotationInputs();
        HandleHardDropInputs();

        if (Time.time > _moveTime)
        {
            HandleMoveInputs();
        }

        if (Time.time > _stepTime)
        {
            Step();
        }

        _board.Set(Cells, Position);
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
            if (IsCanMove(Vector2Int.down))
            {
                Move(Vector2Int.down);
                _stepTime = Time.time + _stepDelay;
            }
        }

        if (Input.GetKey(KeyCode.A) && IsCanMove(Vector2Int.left))
        {
            Move(Vector2Int.left);
        }
        else if (Input.GetKey(KeyCode.D) && IsCanMove(Vector2Int.right))
        {
            Move(Vector2Int.right);
        }
    }

    private void Step()
    {
        _stepTime = Time.time + _stepDelay;
        if (IsCanMove(Vector2Int.down))
        {
            Move(Vector2Int.down);
            return;
        }
        Lock();
    }
    private void HardDrop()
    {
        while (IsCanMove(Vector2Int.down)) 
        {
            Move(Vector2Int.down);
        }

        Lock();
    }

    private void Lock()
    {
        _board.Set(Cells, Position);
        _board.CheckAndClearLines();
        _board.SpawnPiece();
    }

    //Review
    //Метод не должен ничего двигать, только возврашать значение
    //Переименовать
    private bool IsCanMove(Vector2Int translation)
    {
        Vector2Int newPosition = Position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        bool valid = _board.IsValidPosition(newPosition, this.Cells);
        if (!valid) return false;
        return true;
    }

    private void Move(Vector2Int translation)
    {
        Vector2Int newPosition = Position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;
        Position = newPosition;
        _moveTime = Time.time + _moveDelay;
        PiecePositionChanged?.Invoke();
    }

    private void Rotate(int direction)
    {
        RotateCells(direction);
        
        if (IsValidRotation())
        {
            ApplyWallKick();
            PiecePositionChanged?.Invoke();
            return;
        }
        
        RotateCells(-direction);
    }

    private void RotateCells(int direction)
    {
        float[] matrix = RotationMatrixData._rotationMatrix;
        
        for (int i = 0; i < Cells.Length; i++)
        {
            Vector2 cellPosition = Cells[i].Position;

            int x, y;

            if(PieceData.IsCentrified)
            {
                x = Mathf.RoundToInt((cellPosition.x * matrix[0] * direction) + (cellPosition.y * matrix[1] * direction));
                y = Mathf.RoundToInt((cellPosition.x * matrix[2] * direction) + (cellPosition.y * matrix[3] * direction));
            }
            else
            {
                cellPosition.x += PieceData.CenterOfMass.x;
                cellPosition.y -= PieceData.CenterOfMass.y;
                x = Mathf.CeilToInt((cellPosition.x * matrix[0] * direction) + (cellPosition.y * matrix[1] * direction));
                y = Mathf.CeilToInt((cellPosition.x * matrix[2] * direction) + (cellPosition.y * matrix[3] * direction));
            }

            Cells[i].Position = new Vector2Int(x, y);
        }
    }

    private bool IsValidRotation() => PieceData.Wallkicks.Any(IsCanMove);

    private void ApplyWallKick()
    {
        foreach(var wallkick in PieceData.Wallkicks)
        {
            if(IsCanMove(wallkick))
            {
                Move(wallkick);
                return;
            }
        }
    }
}
