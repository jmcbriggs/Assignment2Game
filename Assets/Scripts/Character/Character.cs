using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using FMODUnity;

public class Character : MonoBehaviour
{
    [Header("Character Stats")]
    [SerializeField]
    public string _characterClass;
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
    [SerializeField]
    protected List<GameObject> _selectedSkills = new List<GameObject>();
    [SerializeField]
    protected List<GameObject> _skills = new List<GameObject>();
    bool _cameraFocused = false;


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

    [Header("Extra Effects")]
    [SerializeField] private EventReference _walkingSound;
    [SerializeField] private EventReference _attackSound;
    FMOD.Studio.EventInstance _walkingInstance;
    FMOD.Studio.EventInstance _attackInstance;

    [SerializeField]
    GameObject _damageNumber;


    [SerializeField]
    int _damageDone =0;
    [SerializeField]
    int _healingDone =0;
    [SerializeField]
    int _damageTaken = 0;
    [SerializeField]
    int _defeats = 0; 


    bool _attackFinished = false;
    Vector3 _setPosition;
    float _yOffset = 0.5f;

    Transform _combatCamera;
    protected CharacterCamera _characterCamera;


    public virtual void OnCreate()
    {
        _uiManager = GameObject.Find("GameManager").TryGetComponent(out _uiManager) ? GameObject.Find("GameManager").GetComponent<UIManager>() : null;
        _combatManager = GameObject.Find("GameManager").TryGetComponent(out _combatManager) ? GameObject.Find("GameManager").GetComponent<CombatManager>() : null;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _characterMovement = GetComponent<CharacterMovement>();
        _animator = GetComponent<Animator>(); 
        _combatCamera = GameObject.Find("CombatCamera").transform;
        _characterCamera = GetComponent<CharacterCamera>();
        if(_characterCamera == null)
        {
            Debug.LogWarning("Character Camera is null");
        }
        else if (_combatCamera == null)
        {
            Debug.LogError("Camera Dolly is null");
        }
        else
        {
            _characterCamera.CreateCamera(_combatCamera);
        }
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
        _animator.SetBool("HasAction", true);

        _yOffset = _characterMovement.GetYOffset();

        _skills = new List<GameObject>();
        foreach (GameObject skill in _selectedSkills)
        {
            GameObject newSkill = Instantiate(skill, skill.transform.position, skill.transform.rotation, transform);
            _skills.Add(newSkill);
        }
        _walkingInstance = RuntimeManager.CreateInstance(_walkingSound);
        _attackInstance = RuntimeManager.CreateInstance(_attackSound);
    }

    public virtual void OnMove(int distance)
    {
        _movementRemaining -= distance;
    }

    public void StartMoveSound()
    {
        _walkingInstance.start();
    }

    public void PlayAttackSound()
    {
        _attackInstance.start();
    }

    public virtual void FinishMove()
    {
        _animator.SetBool("isMoving", false);
        _walkingInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public virtual void OnSkill(CombatManager.SkillTargetParameters parameters, Skill skill)
    {
        _usedAction = true;
        skill.StartCooldown();
        switch (skill.GetAnimationType())
        {
            case Skill.AnimationType.Attack:
                UseAttackAnimation(parameters, skill);
                break;
            case Skill.AnimationType.OffensiveSpell:
                UseOffensiveSpellAnimation(parameters, skill);
                break;
            case Skill.AnimationType.DefensiveSpell:
                UseDefensiveSpellAnimation(parameters, skill);
                break;

        }
    }

    public virtual void OnAttack(Character targetCharacter, int damage)
    {
        _usedAction = true;
        List<GameObject> targets = new List<GameObject>();
        targets.Add(targetCharacter.gameObject);
        List<int> damages = new List<int>();
        damages.Add(damage);
        CombatManager.SkillTargetParameters parameters = new CombatManager.SkillTargetParameters();
        parameters._targets = targets;
        parameters._damages = damages;
        UseAttackAnimation(parameters, null);
        
    }

    public void UseAttackAnimation(CombatManager.SkillTargetParameters parameters, Skill skill)
    {
        _attackFinished = false;
        StartCoroutine(AttackAnimation(parameters, skill, Skill.AnimationType.Attack, "Attack"));
    }

    public void UseOffensiveSpellAnimation(CombatManager.SkillTargetParameters parameters, Skill skill)
    {
        _attackFinished = false;
        StartCoroutine(AttackAnimation(parameters, skill, Skill.AnimationType.OffensiveSpell, "OffensiveSpell"));
    }

    public void UseDefensiveSpellAnimation(CombatManager.SkillTargetParameters parameters, Skill skill)
    {
        _attackFinished = false;
        StartCoroutine(AttackAnimation(parameters, skill, Skill.AnimationType.DefensiveSpell, "DefensiveSpell"));
    }
    
    IEnumerator AttackAnimation(CombatManager.SkillTargetParameters parameters, Skill skill, Skill.AnimationType transition, string animationTrigger)
    {
        if (_combatCamera != null)
        {
            _characterCamera.CharacterFocus();
        }
        if(_uiManager != null)
        {
            string name = GetName();
            if(skill != null)
            {
                _uiManager.StartMove(name, skill.GetSkillName());
            }
            else
            {
                _uiManager.StartMove(name);
            }
        }
        yield return new WaitForSeconds(1f);
        _animator.SetTrigger(animationTrigger);
        if (skill != null && skill.HasExtraEffect())
        {
            skill.TriggerCastSound();
        }
        else
        {
            PlayAttackSound();
        }
        while (!_animator.IsInTransition(0) && !_animator.GetAnimatorTransitionInfo(0).IsName(transition.ToString() + " -> Idle"))
        {
            yield return null;
        }
        if (skill != null && skill.HasExtraEffect())
        {
 
            if (skill.GetSkillHitType() == Skill.SkillHitType.POINT && skill.GetSplash() > 0)
            {
                skill.TriggerExtraEffect(transform, parameters._tile.transform);
            }
            else if (skill.GetSkillType() == Skill.SkillType.AREA && skill.GetSkillHitType() == Skill.SkillHitType.POINT)
            {
                skill.TriggerExtraEffect(transform, parameters._tile.transform);
            }
            else if (skill.GetSkillType() == Skill.SkillType.AREA || skill.GetSkillHitType() == Skill.SkillHitType.AREA)
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
            FinishAttack(1);
        }
        while(_attackFinished == false)
        {
            yield return null;
        }
        for (int i = 0; i < parameters._targets.Count; i++)
        {
            parameters._targets[i].GetComponent<Character>().TakeDamage(parameters._damages[i]);
            if (skill != null)
            {
                if (skill.GetSkillTarget() == Skill.SkillTarget.ENEMY)
                {
                    _damageDone += parameters._damages[i];
                }
                else
                {
                    _healingDone += parameters._damages[i];
                }
            } 
            else
            {
                   _damageDone += parameters._damages[i];
            }
        }
    }


    public virtual void FinishAttack()
    {
        _animator.SetBool("HasAction", false);
        _attackFinished = true;
        FinishAttackUI();
    }

    public virtual void FinishAttack(float delay)
    {
        _animator.SetBool("HasAction", false);
        _attackFinished = true;
        Invoke("FinishAttackUI", delay);
    }

    void FinishAttackUI()
    {
        CameraUnfocus();
        if (_uiManager != null)
        {
            _uiManager.StopMove();
        }
    }

    public void CameraUnfocus()
    {
        if(_characterCamera != null)
        {
            _characterCamera.CharacterUnfocus();
            _cameraFocused = false;
        }
    }

    public void CameraUnfocus(float delay)
    {
        Invoke("CameraUnfocus", delay);
    }

    public void CameraFocus()
    {
        if (_characterCamera != null)
        {
            _characterCamera.CharacterFocus();
        }
        Invoke("IsFocused", 1f);    
    }

    void IsFocused() { 
        _cameraFocused = true;
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

    public int GetDamageDone()
    {
        return _damageDone;
    }

    public int GetHealingDone()
    {
        return _healingDone;
    }

    public int GetDamageTaken()
    {
        return _damageTaken;
    }

    public int GetDefeats()
    {
        return _defeats;
    }

    public List<GameObject> GetSkills()
    {
        return _skills;
    }

    public virtual void TakeDamage(int damage)
    {
        GameObject damageText = Instantiate(_damageNumber, transform.position + new Vector3(0, _yOffset, 0), Quaternion.identity);
        string damageString = damage.ToString();
        if(damage > 0)
        {
            _damageTaken += damage;
            damageText.GetComponent<DamageNumber>().SetColour(Color.red);
            damageString = "-" + damageString;
        }
        else
        {
            damageText.GetComponent<DamageNumber>().SetColour(Color.green);
            damageString = damageString.Replace("-", "");
            damageString = "+" + damageString;
        }
        damageText.GetComponent<TextMeshPro>().text = damageString;
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
        _defeats ++;
        currentTile.RemoveOccupant();
        _combatManager.RemoveCharacter(gameObject);
    }

    public virtual void OnTurnStart()
    {
        _usedAction = false;
        _animator.SetBool("HasAction", true);
        _movementRemaining = _movement;
        foreach (GameObject skill in _skills)
        {
            skill.GetComponent<Skill>().DecrementCooldown();
        }
    }

    // Update is called once per frame
    void Update()
    {
       if(GameController.Instance != null)
        {

            _walkingInstance.setParameterByName("Volume", GameController.Instance.GetEffectsVolume());
            _attackInstance.setParameterByName("Volume", GameController.Instance.GetEffectsVolume());
            
        }
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

    public virtual string GetName()
    {
        return _characterClass;
    }

}
