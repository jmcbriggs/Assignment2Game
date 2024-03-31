using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

[RequireComponent(typeof(Character))]
public class CharacterMovement : MonoBehaviour
{
    [SerializeField]
    GameObject _currentTile;

    [SerializeField]
    CombatManager _combatManager;

    [SerializeField]
    int _movementRemaining = 5;

    [SerializeField]
    float _yOffset = 0.5f;

    bool _isMoving = false;

    Character _character;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitialTile(GameObject tile)
    {
        _character = GetComponent<Character>();
        _currentTile = tile;
        _currentTile.GetComponent<Tile>().SetOccupant(gameObject);
        _character.SetPosition(_currentTile.transform.position + new Vector3(0, _yOffset));
        _movementRemaining = _character._movementRemaining;
    }
    public void MoveThroughPath(List<GameObject> path)
    {
        _currentTile.GetComponent<Tile>().RemoveOccupant();
        if (path.Count > 0)
        {
            _currentTile = path[path.Count - 1];
            _isMoving = true;
            _character.OnMove(path.Count - 1);
            if(_character.GetComponent<Animator>() != null)
            {
                _character.GetComponent<Animator>().SetBool("isMoving", true);
            }
            _character.StartMoveSound();
            StartCoroutine(Move(path));
        }
        else
        {
            _character.FinishMove();
        }
    }

    IEnumerator Move(List<GameObject> path)
    {
        for (int i = 0; i < path.Count; i++)
        {
            Vector3 target = new Vector3(path[i].transform.position.x, path[i].transform.position.y + _yOffset, path[i].transform.position.z);
            while (transform.position != target)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, 5 * Time.deltaTime);
                yield return null;
            }
            _currentTile = path[i];
        }
        _isMoving = false;
        _currentTile.GetComponent<Tile>().SetOccupant(this.gameObject);
        _character.SetPosition(_currentTile.transform.position + new Vector3(0,_yOffset));
        _character.FinishMove();
    }
    public void SetCombatManager(CombatManager combatManager)
    {
        _combatManager = combatManager;
    }

    public float GetYOffset()
    {
        return _yOffset;
    }   
    public Tile GetCurrentTile()
    {
        return _currentTile.GetComponent<Tile>();
    }

    public bool IsMoving()
    {
        return _isMoving;
    }

}
