using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField]
    GameObject _plane;
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
    [SerializeField]
    private float _tileOpacity = 1;

    private float _gridSpacing;

    public GameObject[][] _grid;
    private GameObject _gridHolder;

    private List<Tile> _playerStartTiles;
    private List<Tile> _enemyStartTiles;
   
    // Start is called before the first frame update
    void Start()
    {
        _gridHolder = new GameObject("Grid");
        _grid = new GameObject[_gridWidth][];
        for (int i = 0; i < _gridWidth; i++)
        {
            _grid[i] = new GameObject[_gridHeight];
        }
        BoxCollider tileCollider = _gridTile.GetComponent<BoxCollider>();
        _gridSpacing = (tileCollider.size.x * _tileSize);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CreateTiles(int playerCount, int enemyCount)
    {
        for (int i = 0; i < _gridWidth; i++)
        {
            for (int j = 0; j < _gridHeight; j++)
            {
                float x = _gridStartX + (i * _gridSpacing);
                float y = _gridStartY - (j * _gridSpacing);
                Vector3 position = new Vector3(x + _plane.transform.position.x, _plane.transform.position.y +0.1f, y + _plane.transform.position.z);
                GameObject newTile = Instantiate(_gridTile, position, _plane.transform.rotation );
                newTile.transform.Rotate(new Vector3(90, 0, 0));
                newTile.transform.localScale = new Vector3(_tileSize, _tileSize);
                newTile.transform.SetParent(_gridHolder.transform);
                newTile.GetComponent<Tile>()._x = i;
                newTile.GetComponent<Tile>()._y = j;
                newTile.GetComponent<Tile>()._isWalkable = true;
                newTile.GetComponent<Tile>().SetCombatManager(GetComponent<CombatManager>());
                _grid[i][j] = newTile;
            }
        }
        _playerStartTiles = new List<Tile>();
        _enemyStartTiles = new List<Tile>();
        for(int i = 0; i < playerCount; i++)
        {
            Random.InitState(System.DateTime.Now.Millisecond);
            if (playerCount > _gridWidth / 2 * _gridHeight / 2)
            {
                Debug.LogError("Too many players for the grid size");
                i = playerCount;
            }
            int x = Random.Range(0, _gridWidth/2);
            int y = Random.Range(0, _gridHeight/2);
            if (!_playerStartTiles.Contains(_grid[x][y].GetComponent<Tile>()))
            {
                _playerStartTiles.Add(_grid[x][y].GetComponent<Tile>());
            }
            else
            {
                i--;
            }
        } 
        for(int i = 0; i < enemyCount; i++)
        {
            Random.InitState(System.DateTime.Now.Millisecond);
            if (enemyCount > _gridWidth/2 * _gridHeight/2)
            {
                Debug.LogError("Too many enemies for the grid size");
                i = enemyCount;
            }
            int x = Random.Range(_gridWidth/2, _gridWidth);
            int y = Random.Range(0, _gridHeight);
            if (!_enemyStartTiles.Contains(_grid[x][y].GetComponent<Tile>()))
            {
                _enemyStartTiles.Add(_grid[x][y].GetComponent<Tile>());
            }
            else
            {
                i--;
            }
        }

    }

    public GameObject GetTile(int x, int y)
    {
        return _grid[x][y];
    }

    public Tile FindClosestWalkableTile(Tile targetTile, Tile startTile)
    {
        List<GameObject> neighbours = GetRangeTiles(targetTile, 1, Skill.SkillType.AREA);
        Tile closestTile = null;
        float closestDistance = Mathf.Infinity; // Initialize to positive infinity
        foreach (GameObject neighbourObj in neighbours)
        {
            Tile neighbour = neighbourObj.GetComponent<Tile>();
            if (neighbour._isWalkable && neighbour.GetOccupant() == null)
            {
                float distance = Vector3.Distance(startTile.transform.position, neighbour.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTile = neighbour;
                }
            }
        }
        return closestTile;
    }

    public bool IsTileInNeighbours(Tile targetTile, Tile startTile)
    {
        List<GameObject> neighbours = GetRangeTiles(startTile, 1, Skill.SkillType.AREA);
        foreach(GameObject neighbour in neighbours)
        {
            if(neighbour.GetComponent<Tile>() == targetTile)
            {
                return true;
            }
        }
        return false;
    }

    public List<GameObject> GetTilePath(int startx, int starty, int endx, int endy, int distance, bool checkDistance)
    {
        GameObject startTile = _grid[startx][starty];
        GameObject endTile = _grid[endx][endy];
        List<GameObject> path = new List<GameObject>();
        if (startTile == endTile)
        {
            path.Add(startTile);
            if (checkDistance)
            {
                return CheckPathDistance(path, distance);
            }
            return path;

        }
        List<GameObject> openList = new List<GameObject>();
        List<GameObject> closedList = new List<GameObject>();
        openList.Add(startTile);
        while (openList.Count > 0)
        {
            GameObject currentTile = openList[0];
            openList.Remove(currentTile);
            closedList.Add(currentTile);
            if (currentTile == endTile)
            {
                // Reconstruct the path by backtracking from the end tile to the start tile
                while (currentTile != startTile)
                {
                    path.Add(currentTile);
                    currentTile = currentTile.GetComponent<Tile>()._parent; // Update currentTile to its parent
                }
                path.Add(startTile);
                path.Reverse(); // Reverse the path to get it in the correct order
                if (checkDistance)
                {
                    return CheckPathDistance(path, distance);
                }
                return path;
            }
            if(startx > endx)
            {
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (i == 0 && j == 0)
                        {
                            continue;
                        }
                        if (i != 0 && j != 0)
                        {
                            continue;
                        }
                        int x = currentTile.GetComponent<Tile>()._x + i;
                        int y = currentTile.GetComponent<Tile>()._y + j;
                        if (x >= 0 && x < _gridWidth && y >= 0 && y < _gridHeight)
                        {
                            GameObject neighbour = _grid[x][y];
                            if (neighbour.GetComponent<Tile>()._isWalkable && !closedList.Contains(neighbour))
                            {
                                if (!openList.Contains(neighbour))
                                {
                                    neighbour.GetComponent<Tile>()._parent = currentTile; // Update the neighbor's parent
                                    openList.Add(neighbour);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = 1; i >= -1; i--)
                {
                    for (int j = 1; j >= -1; j--)
                    {
                        if (i == 0 && j == 0)
                        {
                            continue;
                        }
                        if (i != 0 && j != 0)
                        {
                            continue;
                        }
                        int x = currentTile.GetComponent<Tile>()._x + i;
                        int y = currentTile.GetComponent<Tile>()._y + j;
                        if (x >= 0 && x < _gridWidth && y >= 0 && y < _gridHeight)
                        {
                            GameObject neighbour = _grid[x][y];
                            if (neighbour.GetComponent<Tile>()._isWalkable && !closedList.Contains(neighbour))
                            {
                                if (!openList.Contains(neighbour))
                                {
                                    neighbour.GetComponent<Tile>()._parent = currentTile; // Update the neighbor's parent
                                    openList.Add(neighbour);
                                }
                            }
                        }
                    }
                }
            }
           
        }
        if(checkDistance)
        {
            return CheckPathDistance(path, distance);
        }
        return path;
    }
    public List<GameObject> GetTilePath(Tile tile1, Tile tile2,  int distance, bool checkDistance)
    {
        int startx = tile1._x;
        int starty = tile1._y;
        int endx = tile2._x;
        int endy = tile2._y;

        return GetTilePath(startx, starty, endx, endy, distance, checkDistance);
    }

    public List<GameObject> GetRangeTiles(Tile characterTile, int range, Skill.SkillType type)
    {
        List<GameObject> Range = new List<GameObject>();
        int x = characterTile._x;
        int y = characterTile._y;
        if (type == Skill.SkillType.LINE)
        {
           
            for(int i = 1; i <= range; i++)
            {
                if(x + i < _gridWidth)
                {
                    Range.Add(_grid[x + i][y]);
                }
                if(x - i >= 0)
                {
                    Range.Add(_grid[x - i][y]);
                }
                if(y + i < _gridHeight)
                {
                    Range.Add(_grid[x][y + i]);
                }
                if(y - i >= 0)
                {
                    Range.Add(_grid[x][y - i]);
                }
            }
        }
        else if(type == Skill.SkillType.CROSS)
        {
            for (int i = 1; i <= range; i++)
            {
                if (x + i < _gridWidth && y + i < _gridHeight)
                {
                    Range.Add(_grid[x + i][y + i]);
                }
                if (x - i >= 0 && y - i >= 0)
                {
                    Range.Add(_grid[x - i][y - i]);
                }
                if (x + i < _gridWidth && y - i >= 0)
                {
                    Range.Add(_grid[x + i][y - i]);
                }
                if (x - i >= 0 && y + i < _gridHeight)
                {
                    Range.Add(_grid[x - i][y + i]);
                }
            }
        }
        else if(type == Skill.SkillType.AREA)
        {
            for (int i = 0; i < _gridWidth; i++)
            {
                for (int j = 0; j < _gridHeight; j++)
                {
                    if (_grid[x][y] != _grid[i][j])
                    {
                        if(i == x)
                        {
                            if(j > y && j - y <= range)
                            {
                                Range.Add(_grid[i][j]);
                            }
                            else if(j < y && y - j <= range)
                            {
                                Range.Add(_grid[i][j]);
                            }
                        }
                        if(i > x)
                        {
                            if(i - x <= range && j == y)
                            {
                                Range.Add(_grid[i][j]);
                            }
                            if(j > y && j - y <= range && i - x <= range)
                            {
                                Range.Add(_grid[i][j]);
                            }
                            if(j < y && y - j <= range && i - x <= range)
                            {
                                Range.Add(_grid[i][j]);
                            }
                        }
                        if(i < x)
                        {
                            if(x - i <= range && j == y)
                            {
                                Range.Add(_grid[i][j]);
                            }
                            if(j > y && j - y <= range && x - i <= range)
                            {
                                Range.Add(_grid[i][j]);
                            }
                            if(j < y && y - j <= range && x - i <= range)
                            {
                                Range.Add(_grid[i][j]);
                            }
                        }
                    }
                }
            }
        }
        return Range;
    }

    public List<Tile> GetTilesInWalkingRange(Tile characterTile, int range)
    {
        List<Tile> Range = new List<Tile>();
        int x = characterTile._x;
        int y = characterTile._y;
        for (int i = 0; i < _gridWidth; i++)
        {
            for (int j = 0; j < _gridHeight; j++)
            {
                if (_grid[i][j].GetComponent<Tile>()._isWalkable && _grid[i][j].GetComponent<Tile>().GetOccupant() == null)
                {
                    List<GameObject> path = GetTilePath(x, y, i, j, range, false);
                    if (path.Count <= range + 1)
                    {
                        Range.Add(_grid[i][j].GetComponent<Tile>());
                    }
                }
            }
        }
        return Range;
    }

    public int GetWalkingDistance(Tile startTile, Tile endTile)
    {
        List<GameObject> path = GetTilePath(startTile._x, startTile._y, endTile._x, endTile._y, 0, false);
        return path.Count - 1;
    }

   List<GameObject> CheckPathDistance(List<GameObject> path, int distance)
    {
        if(path.Count > distance + 1)
        {
            path.RemoveRange(distance, path.Count - distance);
        }
        return path;
    }

    public void SetColour(List<GameObject> path, Color color)
    {
        foreach (GameObject tile in path)
        {
            tile.GetComponent<Tile>().ColourChange(color);
        }
    }

    public void ClearColour()
    {
        foreach (GameObject[] row in _grid)
        {
            foreach (GameObject tile in row)
            {
                if (tile.GetComponent<Tile>() != null)
                {
                    tile.GetComponent<Tile>().ColourChange(new Color(1, 1, 1, _tileOpacity));
                }
            }
        }
    }

    public List<Tile> GetPlayerStartTiles()
    {
        return _playerStartTiles;
    }

    public List<Tile> GetEnemyStartTiles()
    {
        return _enemyStartTiles;
    }

    public List<GameObject> GetSkillTargetTiles(GameObject tile, Skill selectedSkill, Tile characterTile)
    {
        List<GameObject> selectedTargetTiles = new List<GameObject>();
        List<GameObject> rangeTiles = GetRangeTiles(characterTile, selectedSkill.GetRange(), selectedSkill.GetSkillType());
        if (selectedSkill.GetSkillHitType() == Skill.SkillHitType.POINT)
        {
            if (selectedSkill.GetSplash() > 0)
            {
                selectedTargetTiles = GetRangeTiles(tile.GetComponent<Tile>(), selectedSkill.GetSplash(), Skill.SkillType.AREA);
            }
            else
            {
                selectedTargetTiles = new List<GameObject>();
            }
            selectedTargetTiles.Add(tile);
        }
        else if (selectedSkill.GetSkillHitType() == Skill.SkillHitType.DIRECTIONAL)
        {
            if (selectedSkill.GetSkillType() == Skill.SkillType.LINE)
            {
                if (tile.GetComponent<Tile>()._x > characterTile._x)
                {
                    foreach (GameObject t in rangeTiles)
                    {
                        if (t.GetComponent<Tile>()._x > characterTile._x)
                        {
                            selectedTargetTiles.Add(t);
                        }
                    }
                }
                else if (tile.GetComponent<Tile>()._x < characterTile._x)
                {
                    foreach (GameObject t in rangeTiles)
                    {
                        if (t.GetComponent<Tile>()._x < characterTile._x)
                        {
                            selectedTargetTiles.Add(t);
                        }
                    }
                }
                else if (tile.GetComponent<Tile>()._y > characterTile._y)
                {
                    foreach (GameObject t in rangeTiles)
                    {
                        if (t.GetComponent<Tile>()._y > characterTile._y)
                        {
                            selectedTargetTiles.Add(t);
                        }
                    }
                }
                else if (tile.GetComponent<Tile>()._y < characterTile._y)
                {
                    foreach (GameObject t in rangeTiles)
                    {
                        if (t.GetComponent<Tile>()._y < characterTile._y)
                        {
                            selectedTargetTiles.Add(t);
                        }
                    }
                }

            }
            else if (selectedSkill.GetSkillType() == Skill.SkillType.CROSS)
            {
                if (tile.GetComponent<Tile>()._x > characterTile._x && tile.GetComponent<Tile>()._y > characterTile._y)
                {
                    foreach (GameObject t in rangeTiles)
                    {
                        if (t.GetComponent<Tile>()._x >= characterTile._x && t.GetComponent<Tile>()._y >= characterTile._y)
                        {
                            selectedTargetTiles.Add(t);
                        }
                    }
                }
                else if (tile.GetComponent<Tile>()._x < characterTile._x && tile.GetComponent<Tile>()._y > characterTile._y)
                {
                    foreach (GameObject t in rangeTiles)
                    {
                        if (t.GetComponent<Tile>()._x <= characterTile._x && t.GetComponent<Tile>()._y >= characterTile._y)
                        {
                            selectedTargetTiles.Add(t);
                        }
                    }
                }
                else if (tile.GetComponent<Tile>()._x > characterTile._x && tile.GetComponent<Tile>()._y < characterTile._y)
                {
                    foreach (GameObject t in rangeTiles)
                    {
                        if (t.GetComponent<Tile>()._x >= characterTile._x && t.GetComponent<Tile>()._y <= characterTile._y)
                        {
                            selectedTargetTiles.Add(t);
                        }
                    }
                }
                else if (tile.GetComponent<Tile>()._x < characterTile._x && tile.GetComponent<Tile>()._y < characterTile._y)
                {
                    foreach (GameObject t in rangeTiles)
                    {
                        if (t.GetComponent<Tile>()._x <= characterTile._x && t.GetComponent<Tile>()._y <= characterTile._y)
                        {
                            selectedTargetTiles.Add(t);
                        }
                    }
                }
            }

        }
        else if (selectedSkill.GetSkillHitType() == Skill.SkillHitType.AREA)
        {
            selectedTargetTiles = rangeTiles;
        }
        return selectedTargetTiles;
    }
   
}
