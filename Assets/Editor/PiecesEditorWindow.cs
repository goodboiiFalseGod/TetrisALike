using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PiecesEditorWindow : EditorWindow
{
    private bool[,] _cellsBool = new bool[10,20];
    private Dictionary<Vector2Int, GameObject> _tiles = new Dictionary<Vector2Int, GameObject>(); 
    private GameObject[] _palette;
    private GameObject _activeTile = null;
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
        _activeTile = EditorGUILayout.ObjectField("Tile", _activeTile, typeof(GameObject), false) as GameObject;
        if(_pieceData == null)
        {
            _name = EditorGUILayout.TextField("Piece name", _name);
        }        

        int boolOffsetX = _cellsBool.GetLength(0) / 2;
        int boolOffsetY = _cellsBool.GetLength(1) / 2;

        if(_pieceData != null && (_pieceData != _previousPieceData || _saved))
        {
            _saved = false;
            ClearField();

            _name = _pieceData.name;
            _tiles = _pieceData.Tiles.ToDictionary(entry => entry.Key, entry => entry.Value);
            foreach (var cell in _pieceData.Cells)
            {
                _cellsBool[cell.x + boolOffsetX, cell.y + boolOffsetY] = true;                
            }

            _previousPieceData = _pieceData;
        }

        var topOffset = 40f;
        var off = 40f;

        //Grid
        {
            for (int i = 0; i < _cellsBool.GetLength(0); i++)
            {
                for (int j = 0; j < _cellsBool.GetLength(1); j++)
                {

                    Vector2Int cell = new Vector2Int(i - boolOffsetX, j - boolOffsetY);

                    if (_tiles.ContainsKey(cell))
                    {
                        if(GUI.Button(new Rect(i * off + off, j * off + topOffset * 2, 40, 40), _tiles[cell].GetComponent<SpriteRenderer>().sprite.texture))
                        {
                            _cellsBool[i, j] = false;
                            _tiles.Remove(cell);
                        }                        

                    }
                    else
                    {
                        if(GUI.Button(new Rect(i * off + off, j * off + topOffset * 2, 40, 40), ""))
                        {
                            _cellsBool[i,j] = true;
                            _tiles.Add(cell, _activeTile);
                        }
                    }
                }
            }
        }

        //Palette
        {
            //_palette = (GameObject[])Resources.LoadAll("TilesPrefabs", typeof(GameObject));
            //_activeTile = _palette[0];
        }

        if (GUI.Button(new Rect(off * 11, off * _cellsBool.GetLength(1) + topOffset * 2.2f, off * 5, off), "Clear"))
        {
            _pieceData = null;
            _name = string.Empty;
            ClearField();
        }

        if(_pieceData != null)
        {
            if (GUI.Button(new Rect(off - 4f, off * _cellsBool.GetLength(1) + topOffset * 2.2f, off * 5, off), "Save"))
            {
                if (_pieceData != null)
                {
                    _pieceData.UpdateData(_tiles, Vector2IntFromBools(_cellsBool));
                    ClearField();
                    _saved = true;
                }
            }
        }

        if(_pieceData == null)
        {
            if (GUI.Button(new Rect(off - 4f, off * _cellsBool.GetLength(1) + topOffset * 2.2f, off * 5, off), "Save as"))
            {
                foreach (var b in _cellsBool)
                {
                    if (b && _tiles != null && _name != string.Empty)
                    {
                        _pieceData = SaveAs(_name);
                        _pieceData.UpdateData(_tiles, Vector2IntFromBools(_cellsBool));
                        ClearField();
                        _saved = true;
                        return;
                    }
                }

            }
        }

    }

    private PieceData SaveAs(string name)
    {
        PieceData pieceData = ScriptableObject.CreateInstance<PieceData>();
        string path = EditorUtility.SaveFilePanel("Chose folder to save", Application.dataPath + "Assets/", name, "asset");
        string appDir = Application.dataPath;

        path = path.Replace(appDir, "Assets/");

        AssetDatabase.CreateAsset(pieceData, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return pieceData;   
    }

    private void ClearField()
    {
        for (int i = 0; i < _cellsBool.GetLength(0); i++)
        {
            for (int j = 0; j < _cellsBool.GetLength(1); j++)
            {
                _cellsBool[i, j] = false;
                //_tiles[new Vector2Int(i, j)] = null;
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
