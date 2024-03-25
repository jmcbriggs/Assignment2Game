using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;


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
    int _maxEnemies = 6;
    [SerializeField]
    float _difficultyMultiplier = 2f;

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
    [SerializeField]
    private Actions _selectedAction = Actions.NONE;

    public struct SkillTargetParameters
    {
        public List<GameObject> _targets;
        public GameObject _tile;
        public List<GameObject> _allTiles;
        public List<int> _damages;
    }

    void Start()
    {
        if (GameController.Instance != null)
        {
            _playerCharacters = GameController.Instance.GetSelectedCharacters();
            if(GameController.Instance.GetLevel() == 1)
            {
                _levelDifficulty = 2;
            }
            else
            {
                _levelDifficulty = (Mathf.RoundToInt(GameController.Instance.GetLevel() * _difficultyMultiplier));
            }
        }
        _grid = GetComponent<GridManager>();
        _uiManager = GetComponent<UIManager>();
        BuildEnemyList();
        _grid.CreateTiles(_playerCharacters.Count, _selectedEnemies.Count);
        CreateCharacter();
        StartCombat(CombatState.PLAYER);
        EnableColliders(true);

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
        if(GameController.Instance != null)
        {
            if(GameController.Instance.GetLevel() == 10)
            {
                _selectedEnemies.Add(GameController.Instance.GetBoss());
            }
        }
        if(_levelDifficulty == 0)
        {
            _levelDifficulty = 1;
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
            if(_selectedEnemies.Count > _maxEnemies || currentDifficulty >= _levelDifficulty && _selectedEnemies.Count == 1)
            {
                _selectedEnemies = new List<GameObject>();
                currentDifficulty = 0;
                if (GameController.Instance != null)
                {
                    if (GameController.Instance.GetLevel() == 10)
                    {
                        GameObject boss = GameController.Instance.GetBoss();
                        _selectedEnemies.Add(boss);
                        currentDifficulty += boss.GetComponent<EnemyCharacter>().GetDifficulty();
                    }
                }
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
                    _selectedTiles = _grid.GetTilePath(characterTile._x, characterTile._y, tile.GetComponent<Tile>()._x, tile.GetComponent<Tile>()._y, _selectedCharacter.GetComponent<PlayerCharacter>().GetMovementRemaining(), true);
                }
            }
        }
        if (_state == CombatState.PLAYER && _selectedAction == Actions.SKILL)
        {
            _selectedTargetTiles = new List<GameObject>();
            if (_selectedTiles.Contains(tile))
            {
                _selectedTargetTiles = _grid.GetSkillTargetTiles(tile, _selectedSkill, _selectedCharacter.GetComponent<CharacterMovement>().GetCurrentTile());
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

    void EnableColliders(bool enable)
    {
        foreach (GameObject player in _activePlayers)
        {

          player.GetComponent<Collider>().enabled = enable;
        }
        foreach (GameObject enemy in _activeEnemies)
        {
            enemy.GetComponent<Collider>().enabled = enable;
        }
    }

    public bool IsActionSelected()
    {
        return _selectedAction != Actions.NONE;
    }

    public void MoveButton()
    {
        if (_selectedAction == Actions.MOVE)
        {
            _selectedAction = Actions.NONE;
            EnableColliders(true);
        }
        else if (_selectedCharacter != null)
        {
            _selectedTiles = new List<GameObject>();
            _selectedTiles.Add(_selectedCharacter.GetComponent<CharacterMovement>().GetCurrentTile().gameObject);
            _selectedTargetTiles = new List<GameObject>();
            _selectedAction = Actions.MOVE;
            EnableColliders(false);
        }
    }

    public void AttackButton()
    {
        if(_selectedAction == Actions.ATTACK)
        {
            _selectedAction = Actions.NONE;
            EnableColliders(true);
        }
        else if (_selectedCharacter != null && _selectedCharacter.GetComponent<PlayerCharacter>().HasAction() && !_selectedCharacter.GetComponent<CharacterMovement>().IsMoving())
        {
            _selectedAction = Actions.ATTACK;
            _selectedTiles = _grid.GetRangeTiles(_selectedCharacter.GetComponent<CharacterMovement>().GetCurrentTile(), 1, Skill.SkillType.AREA);
            EnableColliders(false);
        }
    }


    public void TileClick(GameObject tile)
    {
        if (_selectedAction == Actions.MOVE && _selectedTiles != null && _selectedCharacter != null)
        {
            _selectedCharacter.GetComponent<CharacterMovement>().MoveThroughPath(_selectedTiles);
            _selectedTiles = null;
            _grid.ClearColour();
            EnableColliders(true);
            _selectedAction = Actions.NONE;
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
                EnableColliders(true);
            }

        }
        else if (_selectedAction == Actions.SKILL && _selectedTargetTiles != null && _selectedCharacter != null)
        {
           SkillTargetParameters parameters = GetSkillParameters(tile, _selectedSkill, _selectedTargetTiles, _selectedCharacter);
            if (parameters._targets.Count > 0)
            {
                _selectedAction = Actions.NONE;
                _selectedTiles = null;
                _grid.ClearColour();
                _selectedCharacter.GetComponent<PlayerCharacter>().OnSkill(parameters, _selectedSkill);
                EnableColliders(true);
            }
            _selectedTargetTiles = new List<GameObject>();
        }
    }

    public SkillTargetParameters GetSkillParameters(GameObject tile, Skill selectedSkill, List<GameObject> selectedTargetTiles, GameObject character)
    {
        List<GameObject> targets = new List<GameObject>();
        List<int> damages = new List<int>();
        foreach (GameObject t in selectedTargetTiles)
        {
            if (selectedSkill.GetSkillTarget() == Skill.SkillTarget.ENEMY)
            {
                GameObject occupant = t.GetComponent<Tile>().GetOccupant();
                if (occupant != null)
                {
                    targets.Add(occupant);
                    damages.Add(CalculateDamageSkill(character, occupant, selectedSkill));
                }
            }
            else
            {
                GameObject occupant = t.GetComponent<Tile>().GetOccupant();
                if (occupant != null)
                {
                    targets.Add(occupant);
                    damages.Add(CalculateDamageSkill(character, occupant, selectedSkill) * -1);
                }
            }

        }
        SkillTargetParameters parameters = new SkillTargetParameters();
        parameters._targets = targets;
        parameters._tile = tile;
        parameters._allTiles = selectedTargetTiles;
        parameters._damages = damages;
        return parameters;
    }

    public int CalculateDamageAttack(GameObject attacker, GameObject target)
    {
        int attack = attacker.GetComponent<Character>().GetAttack();
        int defence = target.GetComponent<Character>().GetDefence();

        float multiplyer = 1+ (attack - defence);
        int damage = (int)Mathf.Clamp(multiplyer, 1, 100);
        Debug.Log(damage + " damage has been done by " + attacker.name);

        return damage;
    }

    public int CalculateDamageSkill(GameObject attacker, GameObject target, Skill skill)
    {
        float skillDamage = skill.GetPower();
        float baseDamage = 0;
        if (skill.IsMagic())
        {
            int attackerMagic = attacker.GetComponent<Character>().GetMagic();
            int defenderMagic = target.GetComponent<Character>().GetMagic();
            if (skill.GetSkillTarget() == Skill.SkillTarget.FRIENDLY)
            {
                baseDamage = Mathf.Clamp(1+(attackerMagic - 10), 1, 100);
            }
            else
            {

                baseDamage = Mathf.Clamp(1+(attackerMagic - defenderMagic), 1, 100);
            }
        }
        else
        {
            int attack = attacker.GetComponent<Character>().GetAttack();
            int defence = target.GetComponent<Character>().GetDefence();

            baseDamage = Mathf.Clamp((attack - defence),1, 100);
        }

        int damage = (int)Mathf.Round(skillDamage * baseDamage);
        Debug.Log(damage + " damage has been done by " + attacker.name + " using the skill " + skill.name);

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
        StartCoroutine(EndTurnDelay());
    }

    IEnumerator EndTurnDelay()
    {
        while(_uiManager.GetAnimating())
        {
            yield return null;
        }
        BeginTurn();
    }

    public void BeginTurn()
    {
        if (_state == CombatState.ENEMY)
        {
            TriggerEnemyTurn();
        }
        else
        {
            EnableColliders(true);
        }
    }

    void TriggerEnemyTurn()
    {
        _selectedEnemy = 0;
        StartCoroutine(EnemyTurn());
    }

    public GameObject GetClosestPlayer(Tile enemyTile)
    {
        List<Tile> playerTiles = new List<Tile>();
        foreach (GameObject player in _activePlayers)
        {
            playerTiles.Add(player.GetComponent<CharacterMovement>().GetCurrentTile());
        }
        Tile closestTile = null;
        float closestDistance = Mathf.Infinity;
        foreach (Tile playerTile in playerTiles)
        {
            float distance = Mathf.Abs(playerTile._x - enemyTile._x) + Mathf.Abs(playerTile._y - enemyTile._y);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTile = playerTile;
            }
        }
        if(closestTile == null)
        {
            Debug.LogError("No closest tile found");
            if (playerTiles.Count > 0)
            {
                return playerTiles[0].GetOccupant();
            }
            return null;
        }
        return closestTile.GetOccupant();
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
            OnCharacterDeselect();
            _uiManager.EndBattle();
        }
    }

    IEnumerator EnemyTurn()
    {
        int check = -1;
        float timeCheck = 0;    
        while (_selectedEnemy < _activeEnemies.Count)
        {
            if(check != _selectedEnemy)
            {
                if (_activeEnemies[_selectedEnemy].GetComponent<EnemyBrain>() != null)
                {
                    _activeEnemies[_selectedEnemy].GetComponent<EnemyBrain>().TriggerTurn();
                }
                else
                {
                    Debug.LogError("No brain assigned to enemy character");
                    EnemyAttackFinished();
                }
            }
            check = _selectedEnemy;
            timeCheck += Time.deltaTime;
            if(timeCheck > 1000)
            {
                break;
            }
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(0.5f);
        EndTurn();
    }

    public void EnemyAttackFinished()
    {
        _selectedEnemy++;
    }

    public bool IsCharacterSelected(PlayerCharacter playerCharacter)
    {
        return _selectedCharacter == playerCharacter.gameObject;
    }

    public void LoadSkill(int num)
    {
        _selectedTargetTiles = null;
        _selectedTiles = null;
        if(_selectedAction == Actions.SKILL)
        {
            _selectedAction = Actions.NONE;
            EnableColliders(true);
            _selectedTiles = null;
            _selectedTargetTiles = null;
            _grid.ClearColour();
        }
        else if (_selectedCharacter != null && _selectedCharacter.GetComponent<PlayerCharacter>().HasAction() && !_selectedCharacter.GetComponent<CharacterMovement>().IsMoving())
        {
            _selectedSkill = _selectedCharacter.GetComponent<PlayerCharacter>().GetSkills()[num].GetComponent<Skill>();
            if (_selectedSkill != null && !_selectedSkill.OnCooldown())
            {
                _selectedAction = Actions.SKILL;
                _selectedTiles = _grid.GetRangeTiles(_selectedCharacter.GetComponent<CharacterMovement>().GetCurrentTile(), _selectedSkill.GetRange(), _selectedSkill.GetSkillType());
            }
            EnableColliders(false);
        }
    }

    public GridManager GetGrid()
    {
        return _grid;
    }

    public void SkillButton()
    {
        if(_uiManager.SkillsPageActive())
        {
            _uiManager.EnableSkillPage(false);
            _selectedAction = Actions.NONE;
            EnableColliders(true);
            _selectedTiles = null;
            _selectedTargetTiles = null;
            _grid.ClearColour();
        }
        else
        {
            _uiManager.EnableSkillPage(true);
        }
    }

    public GameObject GetSelectedCharacter()
    {
        return _selectedCharacter;
    }

    public void ExitBattle()
    {
        OnCharacterDeselect();
        foreach (GameObject player in _slainPlayers)
        {
            player.SetActive(true);
            player.GetComponent<Character>().ChangeSpriteTransparency(player.transform, 1);
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

    public GameObject GetWeakestPlayer()
    {
        GameObject weakestPlayer = null;
        foreach(GameObject player in _activePlayers)
        {
            if(weakestPlayer == null)
            {
                weakestPlayer = player;
            }
            else
            {
                if(player.GetComponent<Character>().GetDamageDone() < weakestPlayer.GetComponent<Character>().GetDamageDone())
                {
                    weakestPlayer = player;
                }
            }
        }
        return weakestPlayer;
    }

    public GameObject GetStrongestPlayer()
    {
        GameObject strongestPlayer = null;
        foreach (GameObject player in _activePlayers)
        {
            if (strongestPlayer == null)
            {
                strongestPlayer = player;
            }
            else
            {
                if (player.GetComponent<Character>().GetDamageDone() > strongestPlayer.GetComponent<Character>().GetDamageDone())
                {
                    strongestPlayer = player;
                }
            }
        }
        return strongestPlayer;
    }

    public GameObject GetLowestHealthPlayer()
    {
        GameObject lowestHealthPlayer = null;
        foreach (GameObject player in _activePlayers)
        {
            if (lowestHealthPlayer == null)
            {
                lowestHealthPlayer = player;
            }
            else
            {
                float healthPercentage = player.GetComponent<Character>().GetHealth() / player.GetComponent<Character>().GetMaxHealth();
                float lowestHealthPercentage = lowestHealthPlayer.GetComponent<Character>().GetHealth() / lowestHealthPlayer.GetComponent<Character>().GetMaxHealth();
                if (healthPercentage < lowestHealthPercentage)
                {
                    lowestHealthPlayer = player;
                }
            }
        }
        return lowestHealthPlayer;
    }

    public GameObject GetHighestHealthPlayer()
    {
        GameObject highestHealthPlayer = null;
        foreach (GameObject player in _activePlayers)
        {
            if (highestHealthPlayer == null)
            {
                highestHealthPlayer = player;
            }
            else
            {
                float healthPercentage = player.GetComponent<Character>().GetHealth() / player.GetComponent<Character>().GetMaxHealth();
                float highestHealthPercentage = highestHealthPlayer.GetComponent<Character>().GetHealth() / highestHealthPlayer.GetComponent<Character>().GetMaxHealth();
                if (healthPercentage > highestHealthPercentage)
                {
                    highestHealthPlayer = player;
                }
            }
        }
        return highestHealthPlayer;
    }

    public GameObject GetLowestHealthEnemy()
    {
        GameObject lowestHealthPlayer = null;
        foreach (GameObject player in _activeEnemies)
        {
            if (lowestHealthPlayer == null)
            {
                lowestHealthPlayer = player;
            }
            else
            {
                float healthPercentage = player.GetComponent<Character>().GetHealth() / player.GetComponent<Character>().GetMaxHealth();
                float lowestHealthPercentage = lowestHealthPlayer.GetComponent<Character>().GetHealth() / lowestHealthPlayer.GetComponent<Character>().GetMaxHealth();
                if (healthPercentage < lowestHealthPercentage)
                {
                    lowestHealthPlayer = player;
                }
            }
        }
        return lowestHealthPlayer;
    }

}
