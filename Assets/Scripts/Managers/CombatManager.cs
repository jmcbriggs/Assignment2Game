using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CombatManager : MonoBehaviour
{
    [SerializeField]
    GridManager _grid;
    UIManager _uiManager;

    [SerializeField]
    List<GameObject> _playerCharacters;

    [SerializeField]
    List<GameObject> _enemyCharacters;

    [SerializeField]
    List<GameObject> _selectedEnemies;

    [SerializeField]
    GameObject _selectedCharacter;
    [SerializeField]
    List<GameObject> _selectedTiles;

    [SerializeField]
    List<GameObject> _activeEnemies;
    [SerializeField]
    List<GameObject> _activePlayers;
    [SerializeField]
    List<GameObject> _slainEnemies;
    [SerializeField]
    List<GameObject> _slainPlayers;

    [SerializeField]
    int _selectedEnemy;
    [SerializeField]
    bool _enemyMove;
    [SerializeField]
    bool _enemyAttack;

    [SerializeField]
    int _levelDifficulty = 1;

    [SerializeField]
    Skill _selectedSkill;
    List<GameObject> _selectedTargetTiles = new List<GameObject>();

    private enum Actions
    {
        MOVE,
        ATTACK,
        SKILL,
        NONE
    }

    private enum CombatState
    {
        PLAYER,
        ENEMY
    }
    private CombatState _state;
    private Actions _selectedAction = Actions.NONE;
    void Start()
    {
        if (GameController.Instance != null)
        {
            _playerCharacters = GameController.Instance.GetSelectedCharacters();
            _levelDifficulty = 2 + (GameController.Instance.GetLevel() * 2);
        }
        _grid = GetComponent<GridManager>();
        _uiManager = GetComponent<UIManager>();
        BuildEnemyList();
        _grid.CreateTiles(_playerCharacters.Count, _selectedEnemies.Count);
        CreateCharacter();
        StartCombat(CombatState.PLAYER);

    }

    // Update is called once per frame
    void Update()
    {
        if (_selectedTiles != null)
        {
            _grid.ClearColour();
            if (_selectedAction == Actions.MOVE)
            {
                _grid.SetColour(_selectedTiles, Color.blue);
            }
            else if (_selectedAction == Actions.ATTACK)
            {
                _grid.SetColour(_selectedTiles, Color.red);
                if (_selectedTargetTiles != null)
                {
                    _grid.SetColour(_selectedTargetTiles, Color.yellow);
                }
            }
            else if (_selectedAction == Actions.SKILL)
            {
                if (_selectedSkill.GetSkillTarget() == Skill.SkillTarget.ENEMY)
                {
                    _grid.SetColour(_selectedTiles, Color.red);
                    if (_selectedTargetTiles != null)
                    {
                        _grid.SetColour(_selectedTargetTiles, Color.yellow);
                    }
                }
                else
                {
                    _grid.SetColour(_selectedTiles, Color.green);
                    if (_selectedTargetTiles != null)
                    {
                        _grid.SetColour(_selectedTargetTiles, Color.yellow);
                    }
                }
            }
        }
    }

    void StartCombat(CombatState startState)
    {
        _state = startState;
        _uiManager.SetState(_state.ToString());
    }

    void BuildEnemyList()
    {
        int currentDifficulty = 0;
        float loopExitTimer = 0;
        if (GameController.Instance != null)
        {
            _enemyCharacters = GameController.Instance.GetAvailableEnemies();
        }
        while (currentDifficulty != _levelDifficulty)
        {
            GameObject newEnemy = _enemyCharacters[Random.Range(0, _enemyCharacters.Count)];
            if (currentDifficulty + newEnemy.GetComponent<EnemyCharacter>().GetDifficulty() <= _levelDifficulty)
            {
                _selectedEnemies.Add(newEnemy);
                currentDifficulty += newEnemy.GetComponent<EnemyCharacter>().GetDifficulty();
            }
            loopExitTimer += Time.deltaTime;
            if (loopExitTimer > 100)
            {
                Debug.Log("Could not build enemy list for difficulty " + _levelDifficulty);
                break;
            }
        }
    }

    void CreateCharacter()
    {
        List<Tile> playerStartTiles = _grid.GetPlayerStartTiles();
        for (int i = 0; i < _playerCharacters.Count; i++)
        {
            float yOffset = _playerCharacters[i].GetComponent<CharacterMovement>().GetYOffset();
            GameObject tile = playerStartTiles[i].gameObject;
            Vector3 characterPosition = new Vector3(tile.transform.position.x, tile.transform.position.y + yOffset, tile.transform.position.z);
            GameObject newCharacter;
            if (GameController.Instance != null)
            {
                newCharacter = _playerCharacters[i];
                _playerCharacters[i].transform.position = characterPosition;
            }
            else
            {
                newCharacter = Instantiate(_playerCharacters[i], characterPosition, Quaternion.identity);
            }
            newCharacter.GetComponent<CharacterMovement>().SetCombatManager(this);
            newCharacter.GetComponent<CharacterMovement>().InitialTile(tile);
            newCharacter.GetComponent<Character>().OnCreate();
            _activePlayers.Add(newCharacter);
        }
        List<Tile> enemyStartTiles = _grid.GetEnemyStartTiles();
        for (int i = 0; i < _selectedEnemies.Count; i++)
        {
            float yOffset = _selectedEnemies[i].GetComponent<CharacterMovement>().GetYOffset();
            GameObject tile = enemyStartTiles[i].gameObject;
            Vector3 characterPosition = new Vector3(tile.transform.position.x, tile.transform.position.y + yOffset, tile.transform.position.z);
            GameObject newCharacter = Instantiate(_selectedEnemies[i], characterPosition, Quaternion.identity);
            newCharacter.transform.Rotate(0,180, 0);    
            newCharacter.GetComponent<CharacterMovement>().SetCombatManager(this);
            newCharacter.GetComponent<CharacterMovement>().InitialTile(tile);
            newCharacter.GetComponent<Character>().OnCreate();
            _activeEnemies.Add(newCharacter);
        }
    }

    public void SelectCharacter(GameObject character)
    {
        _selectedTiles = null;
        if (_selectedCharacter != null)
        {
            OnCharacterDeselect();
        }
        _selectedCharacter = character;
    }

    public void OverTile(GameObject tile)
    {
        if (_state == CombatState.PLAYER && _selectedAction == Actions.MOVE)
        {
            if (_selectedCharacter != null && !_selectedCharacter.GetComponent<CharacterMovement>().IsMoving() && _selectedCharacter.GetComponent<PlayerCharacter>().GetMovementRemaining() > 0)
            {
                Tile characterTile = _selectedCharacter.GetComponent<CharacterMovement>().GetCurrentTile();
                if (tile.GetComponent<Tile>()._isWalkable)
                {
                    _selectedTiles = _grid.GetTilePath(characterTile._x, characterTile._y, tile.GetComponent<Tile>()._x, tile.GetComponent<Tile>()._y, _selectedCharacter.GetComponent<PlayerCharacter>().GetMovementRemaining());
                }
            }
        }
        if (_state == CombatState.PLAYER && _selectedAction == Actions.SKILL)
        {
            _selectedTargetTiles = new List<GameObject>();
            if (_selectedTiles.Contains(tile))
            {
                if (_selectedSkill.GetSkillHitType() == Skill.SkillHitType.POINT)
                {
                    if (_selectedSkill.GetSplash() > 0)
                    {
                        _selectedTargetTiles = _grid.GetRangeTiles(tile.GetComponent<Tile>(), _selectedSkill.GetSplash(), Skill.SkillType.AREA);
                    }
                    else
                    {
                        _selectedTargetTiles = new List<GameObject>();
                    }
                    _selectedTargetTiles.Add(tile);
                }
                else if (_selectedSkill.GetSkillHitType() == Skill.SkillHitType.DIRECTIONAL)
                {
                    if (_selectedSkill.GetSkillType() == Skill.SkillType.LINE)
                    {
                        if (tile.GetComponent<Tile>()._x > _selectedCharacter.GetComponent<CharacterMovement>().GetCurrentTile()._x)
                        {
                            foreach (GameObject t in _selectedTiles)
                            {
                                if (t.GetComponent<Tile>()._x > _selectedCharacter.GetComponent<CharacterMovement>().GetCurrentTile()._x)
                                {
                                    _selectedTargetTiles.Add(t);
                                }
                            }
                        }
                        else if (tile.GetComponent<Tile>()._x < _selectedCharacter.GetComponent<CharacterMovement>().GetCurrentTile()._x)
                        {
                            foreach (GameObject t in _selectedTiles)
                            {
                                if (t.GetComponent<Tile>()._x < _selectedCharacter.GetComponent<CharacterMovement>().GetCurrentTile()._x)
                                {
                                    _selectedTargetTiles.Add(t);
                                }
                            }
                        }
                        else if (tile.GetComponent<Tile>()._y > _selectedCharacter.GetComponent<CharacterMovement>().GetCurrentTile()._y)
                        {
                            foreach (GameObject t in _selectedTiles)
                            {
                                if (t.GetComponent<Tile>()._y > _selectedCharacter.GetComponent<CharacterMovement>().GetCurrentTile()._y)
                                {
                                    _selectedTargetTiles.Add(t);
                                }
                            }
                        }
                        else if (tile.GetComponent<Tile>()._y < _selectedCharacter.GetComponent<CharacterMovement>().GetCurrentTile()._y)
                        {
                            foreach (GameObject t in _selectedTiles)
                            {
                                if (t.GetComponent<Tile>()._y < _selectedCharacter.GetComponent<CharacterMovement>().GetCurrentTile()._y)
                                {
                                    _selectedTargetTiles.Add(t);
                                }
                            }
                        }

                    }
                    else if (_selectedSkill.GetSkillType() == Skill.SkillType.CROSS)
                    {
                        if (tile.GetComponent<Tile>()._x > _selectedCharacter.GetComponent<CharacterMovement>().GetCurrentTile()._x && tile.GetComponent<Tile>()._y > _selectedCharacter.GetComponent<CharacterMovement>().GetCurrentTile()._y)
                        {
                            foreach (GameObject t in _selectedTiles)
                            {
                                if (t.GetComponent<Tile>()._x >= _selectedCharacter.GetComponent<CharacterMovement>().GetCurrentTile()._x && t.GetComponent<Tile>()._y >= _selectedCharacter.GetComponent<CharacterMovement>().GetCurrentTile()._y)
                                {
                                    _selectedTargetTiles.Add(t);
                                }
                            }
                        }
                        else if (tile.GetComponent<Tile>()._x < _selectedCharacter.GetComponent<CharacterMovement>().GetCurrentTile()._x && tile.GetComponent<Tile>()._y > _selectedCharacter.GetComponent<CharacterMovement>().GetCurrentTile()._y)
                        {
                            foreach (GameObject t in _selectedTiles)
                            {
                                if (t.GetComponent<Tile>()._x <= _selectedCharacter.GetComponent<CharacterMovement>().GetCurrentTile()._x && t.GetComponent<Tile>()._y >= _selectedCharacter.GetComponent<CharacterMovement>().GetCurrentTile()._y)
                                {
                                    _selectedTargetTiles.Add(t);
                                }
                            }
                        }
                        else if (tile.GetComponent<Tile>()._x > _selectedCharacter.GetComponent<CharacterMovement>().GetCurrentTile()._x && tile.GetComponent<Tile>()._y < _selectedCharacter.GetComponent<CharacterMovement>().GetCurrentTile()._y)
                        {
                            foreach (GameObject t in _selectedTiles)
                            {
                                if (t.GetComponent<Tile>()._x >= _selectedCharacter.GetComponent<CharacterMovement>().GetCurrentTile()._x && t.GetComponent<Tile>()._y <= _selectedCharacter.GetComponent<CharacterMovement>().GetCurrentTile()._y)
                                {
                                    _selectedTargetTiles.Add(t);
                                }
                            }
                        }
                        else if (tile.GetComponent<Tile>()._x < _selectedCharacter.GetComponent<CharacterMovement>().GetCurrentTile()._x && tile.GetComponent<Tile>()._y < _selectedCharacter.GetComponent<CharacterMovement>().GetCurrentTile()._y)
                        {
                            foreach (GameObject t in _selectedTiles)
                            {
                                if (t.GetComponent<Tile>()._x <= _selectedCharacter.GetComponent<CharacterMovement>().GetCurrentTile()._x && t.GetComponent<Tile>()._y <= _selectedCharacter.GetComponent<CharacterMovement>().GetCurrentTile()._y)
                                {
                                    _selectedTargetTiles.Add(t);
                                }
                            }
                        }
                    }

                }
                else if (_selectedSkill.GetSkillHitType() == Skill.SkillHitType.AREA)
                {
                    _selectedTargetTiles = _selectedTiles;
                }
            }
            else
            {
                _selectedTargetTiles = new List<GameObject>();
            }
        }
        if (_state == CombatState.PLAYER && _selectedAction == Actions.ATTACK)
        {
            if (_selectedTiles.Contains(tile))
            {
                _selectedTargetTiles = new List<GameObject>();
                _selectedTargetTiles.Add(tile);
            }
        }

    }

    public bool IsActionSelected()
    {
        return _selectedAction != Actions.NONE;
    }

    public void MoveButton()
    {
        if (_selectedCharacter != null)
        {
            _selectedTiles = new List<GameObject>();
            _selectedTargetTiles = new List<GameObject>();
            _selectedAction = Actions.MOVE;
        }
    }

    public void AttackButton()
    {
        if (_selectedCharacter != null && _selectedCharacter.GetComponent<PlayerCharacter>().HasAction() && !_selectedCharacter.GetComponent<CharacterMovement>().IsMoving())
        {
            _selectedAction = Actions.ATTACK;
            _selectedTiles = _grid.GetRangeTiles(_selectedCharacter.GetComponent<CharacterMovement>().GetCurrentTile(), 1, Skill.SkillType.AREA);
        }
    }


    public void TileClick(GameObject tile)
    {
        if (_selectedAction == Actions.MOVE && _selectedTiles != null && _selectedCharacter != null)
        {
            _selectedCharacter.GetComponent<CharacterMovement>().OnSetTile(_selectedTiles);
            _selectedTiles = null;
            _grid.ClearColour();
        }
        else if (_selectedAction == Actions.ATTACK && _selectedTiles != null && _selectedCharacter != null)
        {

            if (_selectedTiles.Contains(tile))
            {
                Tile t = tile.GetComponent<Tile>();
                GameObject occupant = t.GetOccupant();
                if (occupant != null)
                {
                    int damage = CalculateDamageAttack(_selectedCharacter, occupant);
                    Character targetCharacter = occupant.GetComponent<Character>();
                    _selectedCharacter.GetComponent<PlayerCharacter>().OnAttack(targetCharacter, damage);
                    _selectedAction = Actions.NONE;
                    _selectedTiles = null;
                    _grid.ClearColour();
                }
            }

        }
        else if (_selectedAction == Actions.SKILL && _selectedTargetTiles != null && _selectedCharacter != null)
        {
            List<GameObject> targets = new List<GameObject>();
            List<int> damages = new List<int>();
            foreach (GameObject t in _selectedTargetTiles)
            {
                if (_selectedSkill.GetSkillTarget() == Skill.SkillTarget.ENEMY)
                {
                    GameObject occupant = t.GetComponent<Tile>().GetOccupant();
                    if (occupant != null)
                    {
                        targets.Add(occupant);
                        damages.Add(CalculateDamageSkill(_selectedCharacter, occupant));
                        _selectedAction = Actions.NONE;
                        _selectedTiles = null;
                        _grid.ClearColour();
                    }
                }
                else
                {
                    GameObject occupant = t.GetComponent<Tile>().GetOccupant();
                    if (occupant != null)
                    {
                        targets.Add(occupant);
                        damages.Add(_selectedSkill.GetPower() * -1);
                        _selectedAction = Actions.NONE;
                        _selectedTiles = null;
                        _grid.ClearColour();

                    }
                }
            }
            _selectedCharacter.GetComponent<PlayerCharacter>().OnSkill(targets, damages, _selectedSkill);
        }
    }

    public void EnemyClick(GameObject enemy)
    {
        if (_selectedAction == Actions.ATTACK && _selectedTiles != null && _selectedCharacter != null)
        {
            if (_selectedTiles.Contains(enemy.GetComponent<CharacterMovement>().GetCurrentTile().gameObject))
            {
                int damage = CalculateDamageAttack(_selectedCharacter, enemy);
                Character targetCharacter = enemy.GetComponent<Character>();
                _selectedCharacter.GetComponent<PlayerCharacter>().OnAttack(targetCharacter, damage);
                _selectedAction = Actions.NONE;
                _selectedTiles = null;
                _grid.ClearColour();
            }
        }
    }

    public int CalculateDamageAttack(GameObject attacker, GameObject target)
    {
        int attack = attacker.GetComponent<Character>().GetAttack();
        int defence = target.GetComponent<Character>().GetDefence();

        float multiplyer = (1 + ((float)attack - (float)defence) / 10);
        int damage = (int)Mathf.Clamp((2 * multiplyer), 1, 100);
        Debug.Log(damage + " damage has been done by " + attacker.name);

        return damage;
    }

    public int CalculateDamageSkill(GameObject attacker, GameObject target)
    {
        int skillDamage = _selectedSkill.GetPower();
        float multiplyer = 0;
        if (_selectedSkill.IsMagic())
        {
            int attackerMagic = attacker.GetComponent<Character>().GetMagic();
            int defenderMagic = target.GetComponent<Character>().GetMagic();

            multiplyer = (1 + ((float)attackerMagic - (float)defenderMagic) / 10);
        }
        else
        {
            int attack = attacker.GetComponent<Character>().GetAttack();
            int defence = target.GetComponent<Character>().GetDefence();

            multiplyer = (1 + ((float)attack - (float)defence) / 10);
        }

        int damage = (int)Mathf.Clamp((skillDamage * multiplyer), 1, 100);
        Debug.Log(damage + " damage has been done by " + attacker.name + " using the skill " + _selectedSkill.name);

        return damage;
    }

    public void OnCharacterDeselect()
    {
        _selectedAction = Actions.NONE;
        _selectedCharacter = null;
        _uiManager.OnCharacterDeselect();
        _grid.ClearColour();
    }

    public bool IsPlayerTurn()
    {
        return _state == CombatState.PLAYER;
    }

    public void EndTurn()
    {
        if (_state == CombatState.PLAYER)
        {
            _state = CombatState.ENEMY;
            OnCharacterDeselect();
            foreach (GameObject enemy in _activeEnemies)
            {
                enemy.GetComponent<Character>().OnTurnStart();
            }
            _enemyMove = true;
            _enemyAttack = false;
            _selectedEnemy = 0;
            StartCoroutine(EnemyTurn());
        }
        else
        {
            _state = CombatState.PLAYER;
            foreach (GameObject player in _activePlayers)
            {
                player.GetComponent<Character>().OnTurnStart();
            }
        }
        _uiManager.SetState(_state.ToString());
    }

    public Tile GetClosestPlayer(Tile enemyTile)
    {
        List<Tile> playerTiles = new List<Tile>();
        foreach (GameObject player in _activePlayers)
        {
            playerTiles.Add(player.GetComponent<CharacterMovement>().GetCurrentTile());
        }
        Tile closestTile = null;
        int closestDistance = 1000;
        foreach (Tile playerTile in playerTiles)
        {
            int distance = Mathf.Abs(playerTile._x - enemyTile._x) + Mathf.Abs(playerTile._y - enemyTile._y);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTile = playerTile;
            }
        }
        return closestTile;
    }

    public void RemoveCharacter(GameObject character)
    {
        if (_activePlayers.Contains(character))
        {
            _activePlayers.Remove(character);
            _slainPlayers.Add(character);
        }
        else if (_activeEnemies.Contains(character))
        {
            _activeEnemies.Remove(character);
            _slainEnemies.Add(character);
        }
    }

    public void WinLossCheck()
    {
        if (_activePlayers.Count == 0)
        {
            Debug.Log("ENEMY WINS");
            _uiManager.ShowDeathScreen();
        }
        else if (_activeEnemies.Count == 0)
        {
            Debug.Log("PLAYER WINS");
            _uiManager.EndBattle();
        }
    }

    IEnumerator EnemyTurn()
    {
        while (_selectedEnemy < _activeEnemies.Count)
        {
            if (_enemyMove)
            {
                Tile enemyTarget = _activeEnemies[_selectedEnemy].GetComponent<EnemyCharacter>().GetTargetTile();
                if (enemyTarget != null && enemyTarget.GetOccupant() != null)
                {
                    enemyTarget = GetClosestPlayer(_activeEnemies[_selectedEnemy].GetComponent<CharacterMovement>().GetCurrentTile());
                }
                Tile enemyTile = _activeEnemies[_selectedEnemy].GetComponent<CharacterMovement>().GetCurrentTile();
                if (_grid.IsTileInNeighbours(enemyTarget, enemyTile))
                {
                    SetEnemyAttack(true);

                }
                else
                {
                    CharacterMovement enemyMovement = _activeEnemies[_selectedEnemy].GetComponent<CharacterMovement>();
                    Character enemyCharacter = _activeEnemies[_selectedEnemy].GetComponent<Character>();
                    Tile closestWalkable = _grid.FindClosestWalkableTile(enemyTarget, enemyTile);
                    if (closestWalkable != null)
                    {
                        _selectedTiles = _grid.GetTilePath(enemyTile._x, enemyTile._y, closestWalkable._x, closestWalkable._y, enemyCharacter.GetMovement());
                        _grid.ClearColour();
                        enemyMovement.OnSetTile(_selectedTiles);
                        _enemyMove = false;
                    }
                    else
                    {
                        SetEnemyAttack(true);
                    }
                }
            }
            if (_enemyAttack)
            {
                if (_activeEnemies[_selectedEnemy].GetComponent<EnemyCharacter>().GetTargetTile() == null)
                {
                    _activeEnemies[_selectedEnemy].GetComponent<EnemyCharacter>().SetTargetTile(GetClosestPlayer(_activeEnemies[_selectedEnemy].GetComponent<CharacterMovement>().GetCurrentTile()));
                    if (_activeEnemies[_selectedEnemy].GetComponent<EnemyCharacter>().GetTargetTile() == null)
                    {
                        break;
                    }
                }
                GameObject enemyTarget = _activeEnemies[_selectedEnemy].GetComponent<EnemyCharacter>().GetTargetTile().gameObject;
                _selectedTiles = _grid.GetRangeTiles(_activeEnemies[_selectedEnemy].GetComponent<CharacterMovement>().GetCurrentTile(), 1, Skill.SkillType.AREA);
                _grid.ClearColour();
                if (_selectedTiles.Contains(enemyTarget))
                {
                    GameObject occupant = enemyTarget.GetComponent<Tile>().GetOccupant();
                    if (occupant != null)
                    {
                        int damage = CalculateDamageAttack(_activeEnemies[_selectedEnemy], occupant);
                        Character targetCharacter = occupant.GetComponent<Character>();
                        _activeEnemies[_selectedEnemy].GetComponent<EnemyCharacter>().OnAttack(targetCharacter, damage);
                        _enemyAttack = false;
                    }
                    else
                    {
                        Tile newTarget = GetClosestPlayer(_activeEnemies[_selectedEnemy].GetComponent<CharacterMovement>().GetCurrentTile());
                        _activeEnemies[_selectedEnemy].GetComponent<EnemyCharacter>().SetTargetTile(newTarget);
                    }
                }
                else
                {
                    _selectedEnemy++;
                    _enemyMove = true;
                    _enemyAttack = false;
                }

            }
            yield return null;
        }
        EndTurn();
    }

    public void SetEnemyAttack(bool active)
    {
        _enemyAttack = active;
        _enemyMove = false;
    }

    public void EnemyAttackFinished()
    {
        _enemyMove = true;
        _selectedEnemy++;
    }

    public void LoadSkill(int num)
    {
        _selectedTargetTiles = null;
        _selectedTiles = null;
        if (_selectedCharacter != null && _selectedCharacter.GetComponent<PlayerCharacter>().HasAction() && !_selectedCharacter.GetComponent<CharacterMovement>().IsMoving())
        {
            _selectedSkill = _selectedCharacter.GetComponent<PlayerCharacter>().GetSkills()[num].GetComponent<Skill>();
            if (_selectedSkill != null && !_selectedSkill.OnCooldown())
            {
                _selectedAction = Actions.SKILL;
                _selectedTiles = _grid.GetRangeTiles(_selectedCharacter.GetComponent<CharacterMovement>().GetCurrentTile(), _selectedSkill.GetRange(), _selectedSkill.GetSkillType());
            }
        }
    }

    public void ExitBattle()
    {
        OnCharacterDeselect();
        foreach (GameObject player in _slainPlayers)
        {
            player.SetActive(true);
            SpriteRenderer s = player.GetComponent<SpriteRenderer>();
            s.color = new Color(s.color.r, s.color.g, s.color.b, 1);
            player.GetComponent<PlayerCharacter>().OnCombatEnd();
        }
        foreach (GameObject player in _activePlayers)
        {
            player.GetComponent<PlayerCharacter>().OnCombatEnd();
        }
        _selectedTiles = null;
        _selectedTargetTiles = null;
        _activeEnemies.Clear();
        _activePlayers.Clear();

        if (GameController.Instance != null)
        {
            GameController.Instance.ExitBattle();
        }
        else
        {
            Debug.Log("GameController not found");
            Debug.Log("Pretend to exit battle");
        }
    }

    public void ReturnToMenu()
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.ReturnToMenu();
        }
        else
        {
            Debug.Log("GameController not found");
            Debug.Log("Pretend to return to menu");
        }
    }

}
