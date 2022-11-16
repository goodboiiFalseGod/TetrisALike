using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PiecesEditorWindow : EditorWindow
{
    private GameObject _tile;
    private bool[,] _cellsBool = new bool[10,20];
    private PieceData _pieceData;
    private PieceData _previousPieceData;
    private string _name;
    private bool _saved = false;

    [MenuItem("Window/PiecesEditor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(PiecesEditorWindow));
    }    

    void OnGUI()
    {
        _pieceData = EditorGUILayout.ObjectField("PieceData", _pieceData, typeof(PieceData), false) as PieceData;
        _name = EditorGUILayout.TextField("Piece name",_name);
        _tile = EditorGUILayout.ObjectField("Tile", _tile, typeof(GameObject), false) as GameObject;

        int boolOffsetX = _cellsBool.GetLength(0) / 2;
        int boolOffsetY = _cellsBool.GetLength(1) / 2;

        if(_pieceData != null && (_pieceData != _previousPieceData || _saved))
        {
            _saved = false;
            ClearCellBool();

            _name = _pieceData.name;
            _tile = _pieceData.Tile;
            foreach(var cell in _pieceData.Cells)
            {
                _cellsBool[cell.x + boolOffsetX, cell.y + boolOffsetY] = true;
            }

            _previousPieceData = _pieceData;
        }

        var topOffset = 40f;
        var off = 20f;

        //Grid
        {
            for (int i = 0; i < _cellsBool.GetLength(0); i++)
            {
                for (int j = 0; j < _cellsBool.GetLength(1); j++)
                {
                    //Debug.Log(j.ToString() + " " + _cellsBool.GetLength(1).ToString());
                    _cellsBool[i, _cellsBool.GetLength(1) - 1 - j] = GUI.Toggle(new Rect(i * off + off, j * off + topOffset * 2, 20, 20), _cellsBool[i, _cellsBool.GetLength(1) - 1 - j], "");
                }
            }
        }

        if (GUI.Button(new Rect(off - 4f, off * _cellsBool.GetLength(1) + topOffset * 2, off * 5, off), "Save"))
        {
            if(_pieceData != null)
            {
                AssetDatabase.RenameAsset("Assets/PieceData/" + name + ".asset", _name + ".asset");
                _pieceData.UpdateData(_tile, Vector2IntFromBools(_cellsBool), _name);
                ClearCellBool();
                _saved = true;
            }
        }

        if (GUI.Button(new Rect(off - 4f, off * _cellsBool.GetLength(1) + topOffset * 3, off * 5, off), "Save as"))
        {
            foreach (var b in _cellsBool)
            {
                if (b && _tile != null && _name != string.Empty) 
                {
                    _pieceData = SaveAs(_name);
                    _pieceData.UpdateData(_tile, Vector2IntFromBools(_cellsBool), _name);
                    ClearCellBool();
                    _saved = true;
                    return;
                }
            }
            
        }
    }

    private PieceData SaveAs(string name)
    {
        PieceData pieceData = ScriptableObject.CreateInstance<PieceData>();
        AssetDatabase.CreateAsset(pieceData, "Assets/PieceData/" + name +  ".asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return pieceData;   
    }

    private void ClearCellBool()
    {
        for (int i = 0; i < _cellsBool.GetLength(0); i++)
        {
            for (int j = 0; j < _cellsBool.GetLength(1); j++)
            {
                _cellsBool[i, j] = false;
            }
        }
    }

    private Vector2Int[] Vector2IntFromBools(bool[,] bools)
    {
        List<Vector2Int> result = new List<Vector2Int>();

        for(int i = 0; i < bools.GetLength(0); i++)
        {
            for(int j = 0; j < bools.GetLength(1); j++)
            {
                if(bools[i, j])
                {
                    result.Add(new Vector2Int(i, j));
                }
            }
        }

        return result.ToArray();
    }
}
