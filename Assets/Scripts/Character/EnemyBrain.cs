using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyCharacter))]
public class EnemyBrain : MonoBehaviour
{
    EnemyCharacter _enemyCharacter;
    CharacterMovement _characterMovement;
    List<Skill> _skillList;
    [SerializeField]
    Priority _targetingPriority;
    [SerializeField]
    int _priorityWeighting = 2;

    bool moveSuccess;

    CombatManager _combatManager;

    struct PotentialTarget
    {
        public GameObject walkingTile;
        public int distance;
        public GameObject tileTarget;
        public List<GameObject> targets;
        public List<GameObject> friendlyTargets;
        public Skill skill;
        public int priority;
    }

    enum Priority
    {
        WEAKEST,
        STRONGEST,
        CLOSEST,
        HEAL,
        DYING,
        HEALTHY
    }

    // Start is called before the first frame update
    void Start()
    {
        _enemyCharacter = GetComponent<EnemyCharacter>();
        _characterMovement = GetComponent<CharacterMovement>();
        if (_enemyCharacter == null)
        {
            Debug.LogError("EnemyCharacter component not found");
        }
        else
        {
            _enemyCharacter.SetEnemyBrain(this);
        }
        _combatManager = GameObject.Find("GameManager").TryGetComponent(out _combatManager) ? GameObject.Find("GameManager").GetComponent<CombatManager>() : null;
        if (_combatManager == null)
        {
            Debug.LogError("CombatManager not found");
        }
    }

    public void TriggerTurn()
    {
        StartCoroutine(ExecuteTurn());
    }

    IEnumerator ExecuteTurn()
    {
        yield return new WaitForSeconds(0.5f);
        GridManager gridManager = _combatManager.GetGrid();
        int movementRange = _enemyCharacter.GetMovementRemaining();
        List<PotentialTarget> potentialTiles = GetPotentialTargets(gridManager, movementRange);
        PotentialTarget target = new PotentialTarget();

        GameObject priorityTarget = null;
        switch (_targetingPriority)
        {
            case Priority.WEAKEST:
                priorityTarget = _combatManager.GetWeakestPlayer();
                break;
            case Priority.STRONGEST:
                priorityTarget = _combatManager.GetStrongestPlayer();
                break;
            case Priority.DYING:
                priorityTarget = _combatManager.GetLowestHealthPlayer();
                break;
            case Priority.HEALTHY:
                priorityTarget = _combatManager.GetHighestHealthPlayer();
                break;
            case Priority.CLOSEST:
                priorityTarget = _combatManager.GetClosestPlayer(_characterMovement.GetCurrentTile());
                break;
            case Priority.HEAL:
                priorityTarget = _combatManager.GetLowestHealthEnemy();
                break;
        }


        if (potentialTiles.Count > 0)
        {
            target = PrioritiseTargets(potentialTiles, priorityTarget);
        }
        if (target.walkingTile == null)
        {
            PotentialTarget attackTarget = new PotentialTarget();
            if(priorityTarget == null)
            {
                priorityTarget = _combatManager.GetClosestPlayer(_characterMovement.GetCurrentTile());
                if(priorityTarget == null)
                {
                    FinishTurn();
                    StopCoroutine(ExecuteTurn());
                }
            }
            attackTarget.walkingTile = gridManager.FindClosestWalkableTile(priorityTarget.GetComponent<CharacterMovement>().GetCurrentTile(), _characterMovement.GetCurrentTile()).gameObject;
            if(gridManager.IsTileInNeighbours(priorityTarget.GetComponent<CharacterMovement>().GetCurrentTile(), _characterMovement.GetCurrentTile()))
            {
                attackTarget.walkingTile = _characterMovement.GetCurrentTile().gameObject;
            }
            attackTarget.distance = gridManager.GetWalkingDistance(_characterMovement.GetCurrentTile(), priorityTarget.GetComponent<CharacterMovement>().GetCurrentTile());
            attackTarget.priority = 0;
            attackTarget.targets = new List<GameObject>()
            {
                priorityTarget
            };
            target = attackTarget;

        }
        bool attackSuccess = false;
        if (target.walkingTile != null)
        {
            moveSuccess = false;

            if (target.walkingTile != _characterMovement.GetCurrentTile().gameObject)
            {
                List<GameObject> movePath = gridManager.GetTilePath(_characterMovement.GetCurrentTile(), target.walkingTile.GetComponent<Tile>(), movementRange, true);
                _characterMovement.MoveThroughPath(movePath);
            }
            else
            {
                moveSuccess = true;
            }
            while (!moveSuccess)
            {
                yield return new WaitForSeconds(0.5f);
            }

            if (target.walkingTile != _characterMovement.GetCurrentTile().gameObject)
            {
                potentialTiles = GetPotentialTargets(gridManager, 0);
                if (potentialTiles.Count > 0)
                {
                    target = PrioritiseTargets(potentialTiles, priorityTarget);
                }
                else
                {
                    PotentialTarget attackTarget = new PotentialTarget();
                    attackTarget.walkingTile = gridManager.FindClosestWalkableTile(priorityTarget.GetComponent<CharacterMovement>().GetCurrentTile(), _characterMovement.GetCurrentTile()).gameObject;
                    attackTarget.distance = gridManager.GetWalkingDistance(_characterMovement.GetCurrentTile(), priorityTarget.GetComponent<CharacterMovement>().GetCurrentTile());
                    attackTarget.priority = 0;
                    attackTarget.targets = new List<GameObject>()
                    {
                        priorityTarget
                    };
                    target = attackTarget;
                }

            }
            if (target.skill != null)
            {
                List<GameObject> targetTiles = new List<GameObject>();
                if(target.skill.GetSkillTarget() == Skill.SkillTarget.ENEMY)
                {
                    targetTiles = gridManager.GetSkillTargetTiles(target.tileTarget, target.skill, _characterMovement.GetCurrentTile());
                }
                else
                {
                    targetTiles = gridManager.GetSkillTargetTiles(target.tileTarget, target.skill, _characterMovement.GetCurrentTile());
                }
                CombatManager.SkillTargetParameters parameters = _combatManager.GetSkillParameters(target.tileTarget, target.skill, targetTiles, _enemyCharacter.gameObject);
                _enemyCharacter.OnSkill(parameters, target.skill);
            }
            else
            {
               List<GameObject> surroundingTiles = gridManager.GetRangeTiles(_characterMovement.GetCurrentTile(), 1, Skill.SkillType.AREA);
                List<GameObject> tileOccupants = new List<GameObject>();
                foreach (GameObject tile in surroundingTiles)
                {
                    Tile tileComponent = tile.GetComponent<Tile>();
                    if (tileComponent.GetOccupant() != null)
                    {
                        if (tileComponent.GetOccupant().GetComponent<PlayerCharacter>() != null)
                        {
                            tileOccupants.Add(tileComponent.GetOccupant());
                        }
                    }
                }
                if (tileOccupants.Count > 0)
                {
                    GameObject attackTarget = tileOccupants[Random.Range(0, tileOccupants.Count)];
                    if (target.targets != null)
                    {
                        if (target.targets.Count > 0 && tileOccupants.Contains(target.targets[0]))
                        {
                            attackTarget = target.targets[0];
                        }
                    }
                    int damage = _combatManager.CalculateDamageAttack(_enemyCharacter.gameObject, attackTarget);
                    Character targetCharacter = attackTarget.GetComponent<Character>();
                    _enemyCharacter.OnAttack(targetCharacter, damage);
                }
                else
                {
                    FinishTurn();
                }
            }

        }
        else
        {
            FinishTurn();
        }
    }

    PotentialTarget PrioritiseTargets(List<PotentialTarget> potentialTargets, GameObject priorityTarget)
    {

        List<PotentialTarget> toRemove = new List<PotentialTarget>();
        for (int i = 0; i < potentialTargets.Count; i++)
        {
            PotentialTarget target = potentialTargets[i];
            if(target.walkingTile == _characterMovement.GetCurrentTile().gameObject)
            {
                target.priority += 1;
            }
            if(target.distance < 0)
            {
                toRemove.Add(potentialTargets[i]);
            }
            if(target.distance > _enemyCharacter.GetMovementRemaining() + 1)
            {
                toRemove.Add(potentialTargets[i]);
            }
            if(target.skill.GetSkillHitType() == Skill.SkillHitType.AREA || target.skill.GetSplash() > 0)
            {
                if (target.skill.GetSkillTarget() == Skill.SkillTarget.ENEMY)
                {
                    if (target.targets.Count < 2) { 
                        toRemove.Add(potentialTargets[i]);
                    }
                }
                else
                {
                    if(target.friendlyTargets.Count < 2)
                    {
                        toRemove.Add(potentialTargets[i]);
                    }
                }
            }
            if (target.targets.Count > 0)
            {
                if (target.skill.GetSkillTarget() == Skill.SkillTarget.ENEMY)
                {
                    target.priority += (2 * target.targets.Count);
                    if (target.targets.Contains(priorityTarget))
                    {
                        target.priority += _priorityWeighting;
                    }
                }
                else
                {
                    target.priority -= target.targets.Count;
                }
            }
            if (target.friendlyTargets.Count > 0)
            {
                if (target.skill.GetSkillTarget() == Skill.SkillTarget.FRIENDLY)
                {
                    foreach (GameObject friendlyTarget in target.friendlyTargets)
                    {
                        if (friendlyTarget.GetComponent<EnemyCharacter>() != null)
                        {
                            if (friendlyTarget.GetComponent<EnemyCharacter>().GetHealth() < friendlyTarget.GetComponent<EnemyCharacter>().GetMaxHealth())
                            {
                                target.priority += 2;
                            }
                        }
                    }
                    if (target.friendlyTargets.Contains(priorityTarget))
                    {
                        target.priority += _priorityWeighting;
                    }
                }
                else
                {
                    target.priority -= target.friendlyTargets.Count;
                }
            }
            if (target.priority > 0)
            {
                if (toRemove.Contains(potentialTargets[i]) == false)
                {
                    potentialTargets[i] = target;
                }
            }
            else
            {
                if(toRemove.Contains(potentialTargets[i]) == false)
                {
                    toRemove.Add(potentialTargets[i]);
                }
            }
        }
        foreach (PotentialTarget target in toRemove)
        {
            potentialTargets.Remove(target);
        }
        if (potentialTargets.Count > 0)
        {
            potentialTargets.Sort((x, y) => {
                int priorityComparison = y.priority.CompareTo(x.priority);
                if (priorityComparison != 0)
                {
                    return priorityComparison;
                }
                return x.distance.CompareTo(y.distance);
            });
            return potentialTargets[0];
        }

        else
        {
            return new PotentialTarget();
        }
    }

    List<PotentialTarget> GetPotentialTargets(GridManager gridManager, int movementRange)
    {
        List<Tile> walkableTiles = gridManager.GetTilesInWalkingRange(_characterMovement.GetCurrentTile(), movementRange);
        walkableTiles.Add(_characterMovement.GetCurrentTile());
        List<GameObject> skills = _enemyCharacter.GetSkills();
        List<PotentialTarget> potentialTiles = new List<PotentialTarget>();
        if (skills.Count > 0)
        {
            foreach (GameObject skillObject in skills)
            {
                Skill skill = skillObject.GetComponent<Skill>();
                if (!skill.OnCooldown())
                {
                    //Check all tiles in walkable tiles and current tile to see if multiple targets can be hit
                    int skillRange = 0;
                    skillRange = skill.GetRange();
                    foreach (Tile tileW in walkableTiles)
                    {
                        List<GameObject> tilesInRange = gridManager.GetRangeTiles(tileW, skillRange, skill.GetSkillType());
                        int distance = gridManager.GetWalkingDistance(_characterMovement.GetCurrentTile(), tileW);
                        if (skill.GetSkillHitType() == Skill.SkillHitType.AREA)
                        {
                            List<GameObject> targets = new List<GameObject>();
                            List<GameObject> friendlyTargets = new List<GameObject>();
                            int splash = skill.GetRange();
                            List<GameObject> splashTiles = gridManager.GetRangeTiles(tileW, splash, Skill.SkillType.AREA);
                            foreach (GameObject splashTile in splashTiles)
                            {
                                Tile splashTileComponent = splashTile.GetComponent<Tile>();
                                if (splashTileComponent.GetOccupant() != null)
                                {
                                    if (splashTileComponent.GetOccupant().GetComponent<PlayerCharacter>() != null)
                                    {
                                        targets.Add(splashTileComponent.GetOccupant());
                                    }
                                    else
                                    {
                                        friendlyTargets.Add(splashTileComponent.GetOccupant());
                                    }
                                }
                            }
                            if (targets.Count > 0 || friendlyTargets.Count > 0)
                            {
                                PotentialTarget potentialTarget = new PotentialTarget();
                                potentialTarget.walkingTile = tileW.gameObject;
                                potentialTarget.tileTarget = tileW.gameObject;
                                potentialTarget.distance = distance;
                                potentialTarget.targets = targets;
                                potentialTarget.friendlyTargets = friendlyTargets;
                                potentialTarget.skill = skill;
                                potentialTarget.priority = 0;
                                potentialTiles.Add(potentialTarget);
                            }
                        }
                        else
                        {
                            foreach (GameObject tileR in tilesInRange)
                            {
                                List<GameObject> targets = new List<GameObject>();
                                List<GameObject> friendlyTargets = new List<GameObject>();
                                Tile tileComponent = tileR.GetComponent<Tile>();
                                if (tileComponent != null)
                                {
                                    if (tileComponent.GetOccupant() != null)
                                    {
                                        if (tileComponent.GetOccupant().GetComponent<PlayerCharacter>() != null)
                                        {
                                            targets.Add(tileComponent.GetOccupant());
                                        }
                                        else
                                        {
                                            friendlyTargets.Add(tileComponent.GetOccupant());
                                        }
                                    }
                                    if (skill.GetSplash() > 0)
                                    {
                                        int splash = skill.GetSplash();
                                        List<GameObject> splashTiles = gridManager.GetRangeTiles(tileComponent, splash, Skill.SkillType.AREA);
                                        foreach (GameObject splashTile in splashTiles)
                                        {
                                            Tile splashTileComponent = splashTile.GetComponent<Tile>();
                                            if (splashTileComponent.GetOccupant() != null)
                                            {
                                                if (splashTileComponent.GetOccupant().GetComponent<PlayerCharacter>() != null)
                                                {
                                                    targets.Add(splashTileComponent.GetOccupant());
                                                }
                                                else
                                                {
                                                    friendlyTargets.Add(splashTileComponent.GetOccupant());
                                                }
                                            }
                                        }
                                    }
                                }
                                if (targets.Count > 0 || friendlyTargets.Count > 0)
                                {
                                    PotentialTarget potentialTarget = new PotentialTarget();
                                    potentialTarget.walkingTile = tileW.gameObject;
                                    potentialTarget.tileTarget = tileR;
                                    potentialTarget.distance = distance;
                                    potentialTarget.targets = targets;
                                    potentialTarget.friendlyTargets = friendlyTargets;
                                    potentialTarget.skill = skill;
                                    potentialTarget.priority = 0;
                                    potentialTiles.Add(potentialTarget);
                                }
                            }
                       
                        }

                    }
                }
            }
        }
        return potentialTiles;
    }

    public void FinishMoving()
    {
        moveSuccess = true;
    }

    public void FinishTurn()
    {
        _combatManager.EnemyAttackFinished();
    }
}
