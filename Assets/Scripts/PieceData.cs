using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New piece")]
public class PieceData : ScriptableObject
{
    [SerializeField] private GameObject _tile;
    private Vector2Int[,] _wallKicks;
    private Vector2Int[] _cells;

    public GameObject Tile { get => _tile; }
    public Vector2Int[,] WallKicks { get => _wallKicks; }
    public Vector2Int[] Cells { get => _cells; }

    public void Init(Vector2Int[,] wallKicks, Vector2Int[] cells)
    {
        _wallKicks = wallKicks;
        _cells = cells;
    }
}
