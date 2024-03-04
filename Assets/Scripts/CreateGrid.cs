using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateGrid : MonoBehaviour
{
    [SerializeField]
    GameObject _gridTile;
    [SerializeField]
    private float _gridStartX;
    [SerializeField]
    private float _gridStartY;
    [SerializeField]
    private int _gridWidth;
    [SerializeField]
    private int _gridHeight;
    [SerializeField]
    private float _tileSize;

    private float _gridSpacing;

    public GameObject[][] _grid;
   
    // Start is called before the first frame update
    void Start()
    {
        _grid = new GameObject[_gridWidth][];
        for (int i = 0; i < _gridWidth; i++)
        {
            _grid[i] = new GameObject[_gridHeight];
        }
        BoxCollider2D tileCollider = _gridTile.GetComponent<BoxCollider2D>();
        _gridSpacing = (tileCollider.size.x * _tileSize);
        CreateTiles();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateTiles()
    {
        for (int i = 0; i < _gridWidth; i++)
        {
            for (int j = 0; j < _gridHeight; j++)
            {
                float x = _gridStartX + (i * _gridSpacing);
                float y = _gridStartY - (j * _gridSpacing);
                GameObject newTile = Instantiate(_gridTile, new Vector3(x, y, 0), Quaternion.identity);
                _grid[i][j] = newTile;
            }
        }
    }
}
