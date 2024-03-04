using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : Character
{
    [SerializeField]
    Tile _targetTile;
    [SerializeField]
    int _difficulty = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        if (_combatManager.IsPlayerTurn())
        {
            _combatManager.EnemyClick(gameObject);
        }
    }

    public override void OnTurnStart()
    {
        base.OnTurnStart();
        _targetTile = _combatManager.GetClosestPlayer(_characterMovement.GetCurrentTile());
    }

    public Tile GetTargetTile()
    {
        return _targetTile;
    }

    public void SetTargetTile(Tile tile)
    {
        _targetTile = tile;
    }

    public override void FinishMove()
    {
        base.FinishMove();
        _combatManager.SetEnemyAttack(true);
    }

    public override void FinishAttack()
    {
        base.FinishAttack();
        _combatManager.EnemyAttackFinished();
    }

    public int GetDifficulty()
    {
        return _difficulty;
    }

}
