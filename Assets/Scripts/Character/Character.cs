using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("Character Stats")]
    [SerializeField]
    public string _characterName;
    [SerializeField]
    private int _maxHealth = 15;
    [SerializeField]
    private int _currentHealth = 15;
    [SerializeField]
    private int _attack = 10;
    [SerializeField]
    private int _defence = 5;
    [SerializeField]
    public int _movement = 5;
    [SerializeField]
    public int _movementRemaining = 5;
    [SerializeField]
    public int _magic = 8;
    [SerializeField]
    public bool _usedAction = false;

    [Header("Related Scripts")]
    [SerializeField]
    protected UIManager _uiManager;
    [SerializeField]
    protected CombatManager _combatManager;
    protected SpriteRenderer _spriteRenderer;
    protected Animator _animator;
    [SerializeField]
    protected CharacterMovement _characterMovement;
    [SerializeField]
    protected HealthBar _healthBar;
    

    bool _attackFinished = false;
    Vector3 _setPosition;
    float _yOffset = 0.5f;


    public virtual void OnCreate()
    {
        _uiManager = GameObject.Find("GameManager").TryGetComponent(out _uiManager) ? GameObject.Find("GameManager").GetComponent<UIManager>() : null;
        _combatManager = GameObject.Find("GameManager").TryGetComponent(out _combatManager) ? GameObject.Find("GameManager").GetComponent<CombatManager>() : null;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _characterMovement = GetComponent<CharacterMovement>();
        _animator = GetComponent<Animator>(); 
        if (_combatManager == null)
        {
            Debug.LogError("Combat Manager is null");
        }
        if (_uiManager == null)
        {
            Debug.LogError("UI Manager is null");
        }
        if (_characterMovement == null)
        {
            Debug.LogError("Character Movement is null");
        }
        if (_animator == null)
        {
            Debug.LogError("Animator is null");
        }
        _movementRemaining = _movement;

        _yOffset = _characterMovement.GetYOffset();
    }

    public virtual void OnMove(int distance)
    {
        _movementRemaining -= distance;
    }

    public virtual void FinishMove()
    {
        _animator.SetBool("isMoving", false);
    }

    public void OnSkill(CombatManager.SkillTargetParameters parameters, List<int> damages, Skill skill)
    {
        _usedAction = true;
        skill.StartCooldown();
        switch (skill.GetAnimationType())
        {
            case Skill.AnimationType.Attack:
                UseAttackAnimation(parameters, damages, skill);
                break;
            case Skill.AnimationType.OffensiveSpell:
                UseOffensiveSpellAnimation(parameters, damages, skill);
                break;
            case Skill.AnimationType.DefensiveSpell:
                UseDefensiveSpellAnimation(parameters, damages, skill);
                break;

        }
    }

    public void OnAttack(Character targetCharacter, int damage)
    {
        _usedAction = true;
        List<GameObject> targets = new List<GameObject>();
        targets.Add(targetCharacter.gameObject);
        List<int> damages = new List<int>();
        damages.Add(damage);
        CombatManager.SkillTargetParameters parameters = new CombatManager.SkillTargetParameters();
        parameters._targets = targets;
        UseAttackAnimation(parameters, damages, null);
        
    }

    public void UseAttackAnimation(CombatManager.SkillTargetParameters parameters, List<int> damages, Skill skill)
    {
        _attackFinished = false;
        _animator.SetTrigger("Attack");
        StartCoroutine(AttackAnimation(parameters, damages, skill, Skill.AnimationType.Attack));
    }

    public void UseOffensiveSpellAnimation(CombatManager.SkillTargetParameters parameters, List<int> damages, Skill skill)
    {
        _attackFinished = false;
        _animator.SetTrigger("OffensiveSpell");
        StartCoroutine(AttackAnimation(parameters, damages, skill, Skill.AnimationType.OffensiveSpell));
    }

    public void UseDefensiveSpellAnimation(CombatManager.SkillTargetParameters parameters, List<int> damages, Skill skill)
    {
        _attackFinished = false;
        _animator.SetTrigger("DefensiveSpell");
        StartCoroutine(AttackAnimation(parameters, damages, skill, Skill.AnimationType.DefensiveSpell));
    }
    
    IEnumerator AttackAnimation(CombatManager.SkillTargetParameters parameters, List<int> damages, Skill skill, Skill.AnimationType transition)
    {
        
        while(!_animator.IsInTransition(0) && !_animator.GetAnimatorTransitionInfo(0).IsName(transition.ToString() + " -> Idle"))
        {
            yield return null;
        }
        if (skill != null && skill.HasExtraEffect())
        {
            if(skill.GetSkillHitType() == Skill.SkillHitType.POINT && skill.GetSplash() > 0)
            {
                skill.TriggerExtraEffect(transform, parameters._tile.transform);
            }
            else if (skill.GetSkillType() == Skill.SkillType.AREA && skill.GetSkillHitType() == Skill.SkillHitType.POINT)
            {
                skill.TriggerExtraEffect(transform, parameters._tile.transform);
            }
            else if (skill.GetSkillType() == Skill.SkillType.AREA)
            {
                skill.TriggerExtraEffect(transform, transform, parameters._targets);
            }
            else if(skill.GetSkillHitType() == Skill.SkillHitType.DIRECTIONAL)
            {
                skill.TriggerExtraEffect(transform, null, parameters._allTiles);
            }
            else
            {
                skill.TriggerExtraEffect(transform, parameters._targets[0].transform);
            }
        }
        else
        {
            FinishAttack();
        }
        while(_attackFinished == false)
        {
            yield return null;
        }
        for (int i = 0; i < parameters._targets.Count; i++)
        {
            parameters._targets[i].GetComponent<Character>().TakeDamage(damages[i]);

        }

    }


    public virtual void FinishAttack()
    {
        _attackFinished = true;
    }

    public void SetPosition(Vector3 position)
    {
        _setPosition = position;
    }

    public int GetAttack()
    {
        return _attack;
    }
    public int GetDefence()
    {
        return _defence;
    }

    public int GetMagic()
    {
        return _magic;
    }

    public int GetHealth()
    {
        return _currentHealth;
    }

    public int GetMaxHealth()
    {
        return _maxHealth;
    }

    public int GetMovement()
    {
        return _movement;
    }

    public int GetMovementRemaining()
    {
        return _movementRemaining;
    }   

    public virtual void TakeDamage(int damage)
    {
        _currentHealth = Mathf.Clamp(_currentHealth - damage, 0, _maxHealth);
        if (_healthBar != null)
        {
            _healthBar.UpdateHealthBar(_maxHealth, _currentHealth);
        }
        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            OnDeath();
            StartCoroutine(Die()); 
        }
       
    }

    IEnumerator Die()
    {
        while (transform.position.y <= _setPosition.y + 1.5f)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y +(1f * Time.deltaTime) , transform.position.z);
            float transparency = -(0.5f * Time.deltaTime);
            ChangeSpriteTransparency(transform, transparency);
            yield return null;
        }
        gameObject.SetActive(false);
        _combatManager.WinLossCheck();
    }

    public void ChangeSpriteTransparency(Transform parent, float change)
    {
        foreach (Transform child in parent)
        {
            if (child.GetComponent<SpriteRenderer>() != null)
            {
                SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
                child.GetComponent<SpriteRenderer>().color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, spriteRenderer.color.a + change); ;
            }
            ChangeSpriteTransparency(child, change);
        }
    }

    private void OnDeath()
    {
        Tile currentTile = _characterMovement.GetCurrentTile();
        currentTile.RemoveOccupant();
        _combatManager.RemoveCharacter(gameObject);
    }

    public virtual void OnTurnStart()
    {
        _usedAction = false;
        _movementRemaining = _movement;
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public virtual void ChangeMaxHealth(int amount)
    {
        _maxHealth += amount;
        _currentHealth += amount;
        _healthBar.UpdateHealthBar(_maxHealth, _currentHealth);
    }

    public virtual void ChangeAttack(int amount)
    {
        _attack += amount;
    }

    public virtual void ChangeDefence(int amount)
    {
        _defence += amount;
    }

    public virtual void ChangeMagic(int amount)
    {
        _magic += amount;
    }

    public virtual void ChangeMovement(int amount)
    {
        _movement += amount;
        _movementRemaining += amount;
    }

}