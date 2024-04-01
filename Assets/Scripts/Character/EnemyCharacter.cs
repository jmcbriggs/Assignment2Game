using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : Character
{
    [SerializeField]
    int _difficulty = 1;
    [SerializeField]
    int _minLevel;
    [SerializeField]
    int _maxLevel;
    EnemyBrain _brain;

    public override void OnTurnStart()
    {
        base.OnTurnStart();
    }

    public override void FinishMove()
    {
        base.FinishMove();
        if(_brain != null)
        {
            _brain.FinishMoving();
        }
        else
        {
            Debug.LogError("No brain assigned to enemy character");
        }
    }

    public override void FinishAttack()
    {
        base.FinishAttack();
        if (_brain != null)
        {
            _brain.FinishTurn();
        }
        else
        {
            Debug.LogError("No brain assigned to enemy character");
        }
    }

    public override void FinishAttack(float delay)
    {
        base.FinishAttack(delay);
        if (_brain != null)
        {
            _brain.FinishTurn();
        }
        else
        {
            Debug.LogError("No brain assigned to enemy character");
        }
    }


    public int GetDifficulty()
    {
        return _difficulty;
    }

    public void SetEnemyBrain(EnemyBrain brain)
    {
        _brain = brain;
    }

    public int GetMinLevel()
    {
        return _minLevel;
    }
    public int GetMaxLevel()
    {
        return _maxLevel;
    }

}
