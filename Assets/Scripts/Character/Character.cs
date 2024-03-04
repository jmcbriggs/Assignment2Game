using System.Collections;
using System.Collections.Generic;
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
    

    bool _goingUp = false;
    bool _goingDown = false;
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

    public void OnSkill(List<GameObject> targets, List<int> damages, Skill skill)
    {
        _usedAction = true;
        _goingUp = true;
        skill.StartCooldown();
        switch (skill.GetAnimationType())
        {
            case Skill.AnimationType.Attack:
                UseAttackAnimation(targets, damages);
                break;
            case Skill.AnimationType.OffensiveSpell:
                UseOffensiveSpellAnimation(targets, damages, skill);
                break;
            case Skill.AnimationType.DefensiveSpell:
                UseDefensiveSpellAnimation(targets, damages, skill);
                break;

        }
    }

    public void OnAttack(Character targetCharacter, int damage)
    {
        _usedAction = true;
        _goingUp = true;
        List<GameObject> targets = new List<GameObject>();
        targets.Add(targetCharacter.gameObject);
        List<int> damages = new List<int>();
        damages.Add(damage);
        UseAttackAnimation(targets, damages);
        
    }

    public void UseAttackAnimation(List<GameObject> targets, List<int> damages)
    {
        _animator.SetTrigger("Attack");
        StartCoroutine(AttackAnimation(targets, damages, null, Skill.AnimationType.Attack));
    }

    public void UseOffensiveSpellAnimation(List<GameObject> targets, List<int> damages, Skill skill)
    {
        _animator.SetTrigger("OffensiveSpell");
        StartCoroutine(AttackAnimation(targets, damages, skill, Skill.AnimationType.OffensiveSpell));
    }

    public void UseDefensiveSpellAnimation(List<GameObject> targets, List<int> damages, Skill skill)
    {
        _animator.SetTrigger("DefensiveSpell");
        StartCoroutine(AttackAnimation(targets, damages, skill, Skill.AnimationType.DefensiveSpell));
    }
    
    IEnumerator AttackAnimation(List<GameObject> targets, List<int> damages, Skill skill, Skill.AnimationType transition)
    {
        
        while(!_animator.IsInTransition(0) && !_animator.GetAnimatorTransitionInfo(0).IsName(transition.ToString() + " -> Idle"))
        {
            yield return null;
        }
        for (int i = 0; i < targets.Count; i++)
        {
            targets[i].GetComponent<Character>().TakeDamage(damages[i]);
            if (skill != null && skill.HasExtraEffect())
            {

            }
        }
        FinishAttack();
    }


    public virtual void FinishAttack()
    {
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
            ChangeAllSpriteColours(transform);
            yield return null;
        }
        gameObject.SetActive(false);
        _combatManager.WinLossCheck();
    }

    public void ChangeAllSpriteColours(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child.GetComponent<SpriteRenderer>() != null)
            {
                SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
                child.GetComponent<SpriteRenderer>().color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, spriteRenderer.color.a - (0.5f * Time.deltaTime)); ;
            }
            ChangeAllSpriteColours(child);
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
