using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.IO;

public class PiecesEditorWindow : EditorWindow
{
    private List<PieceData.ColoredCell> _tiles = new List<PieceData.ColoredCell>();
    private GameObject[] _palette;
    private GameObject _activeTile = null;
    private int _activeTileIndex = 0;
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

        int boolOffsetX = 10 / 2;
        int boolOffsetY = 20 / 2;

        if(_pieceData != null && (_pieceData != _previousPieceData || _saved))
        {
            _saved = false;

            _name = _pieceData.name;
            _tiles = new List<PieceData.ColoredCell>(_pieceData.Tiles);

            _previousPieceData = _pieceData;
        }

        var topOffset = 40f;
        var off = 40f;

        //Grid
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 20; j++)
                {

                    Vector2Int cell = new Vector2Int((i - boolOffsetX), (-j + boolOffsetY));
                    Vector2Int[] pieceCells = new Vector2Int[_tiles.Count];

                    for(int k = 0; k < _tiles.Count; k++)
                    {
                        pieceCells[k] = _tiles[k].Position;
                    }

                    if (pieceCells.Contains(cell))
                    {
                        int a = 0;
                        for ( ; a < _tiles.Count; a++)
                        {
                            if (_tiles[a].Position == cell)
                            {                                
                                break;
                            }
                        }

                        if (GUI.Button(new Rect(i * off + off, j * off + topOffset * 2, 40, 40), _tiles[a].Tile.GetComponent<SpriteRenderer>().sprite.texture))
                        {
                            _tiles.RemoveAt(a);
                        }
                    }
                    else
                    {
                        if (GUI.Button(new Rect(i * off + off, j * off + topOffset * 2, 40, 40), ""))
                        {
                            PieceData.ColoredCell colored = new PieceData.ColoredCell();
                            colored.Position = cell;
                            colored.Tile = _activeTile;
                            _tiles.Add(colored);
                        }
                    }
                }
            }
        }

        //Palette
        {
            var info = new DirectoryInfo("Assets/Resources/TilesPrefabs/");
            var fileInfo = info.GetFiles();
            List<GameObject> prefabs = new List<GameObject>();
            foreach (FileInfo file in fileInfo)
            {
                if(file.Name.Contains(".meta")) continue;

                prefabs.Add(AssetDatabase.LoadAssetAtPath<GameObject>(info + file.Name));
            }

            _palette = new GameObject[prefabs.Count];
            for(int i = 0; i < prefabs.Count; i++)
            {
                _palette[i] = prefabs[i];
            }

            if(_activeTile == null)
            {
                _activeTileIndex = 0;
                _activeTile = _palette[_activeTileIndex];
            }

            int k = 0;

            for(int i = 0; i < 5; i++)
            {
                if (k >= _palette.Length)
                {
                    break;
                }

                for (int j = 0; j < Mathf.RoundToInt(_palette.Length / 5) + 1; j++)
                {
                    if (k >= _palette.Length)
                    {
                        break;
                    }

                    if (k == _activeTileIndex)
                    {
                        GUI.DrawTexture(new Rect(i * off + off * 13, j * off + topOffset * 2, 40, 40), _palette[k].GetComponent<SpriteRenderer>().sprite.texture);
                        GUI.Button(new Rect(i * off + off * 13 + 5, j * off + topOffset * 2 + 5, 30, 30), _palette[k].GetComponent<SpriteRenderer>().sprite.texture);
                    }
                    else
                    {
                        if (GUI.Button(new Rect(i * off + off * 13, j * off + topOffset * 2, 40, 40), _palette[k].GetComponent<SpriteRenderer>().sprite.texture))
                        {
                            _activeTileIndex = k;
                            _activeTile = _palette[k];
                        }
                    }

                    k++;

                }

            }            
        }

        if (GUI.Button(new Rect(off * 5 - 4f, off * 20 + topOffset * 2.2f, off * 3, off), "Clear"))
        {
            _pieceData = null;
            _name = string.Empty;
            _tiles = new List<PieceData.ColoredCell>();
        }

        if (_pieceData != null)
        {
            if (GUI.Button(new Rect(off - 4f, off *20 + topOffset * 2.2f, off * 3, off), "Save"))
            {
                if (_pieceData != null)
                {
                    _pieceData.UpdateData(_tiles.ToArray());
                    _saved = true;
                }
            }
        }

        if(_pieceData == null)
        {
            if (GUI.Button(new Rect(off - 4f, off * 20 + topOffset * 2.2f, off * 3, off), "Save as"))
            {
                if (_tiles != null && _tiles.Count != 0 && _name != string.Empty)
                {
                    _pieceData = SaveAs(_name);
                    _pieceData.UpdateData(_tiles.ToArray());
                    _saved = true;
                    return;
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
