using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Gear;

public class PlayerCharacter : Character
{
    // Start is called before the first frame update
    [SerializeField]
    List<GameObject> _selectedSkills = new List<GameObject>();
    [SerializeField]
    List<GameObject> _skills = new List<GameObject>();

    [Header("Equipment")]
    [SerializeField]
    GameObject _helmet;
    [SerializeField]
    GameObject _chest;
    [SerializeField]
    GameObject _legs;
    [SerializeField]
    GameObject _weapon;

    GameObject _leftArm;
    GameObject _rightArm;
    GameObject _leftLeg;

    [Header("Equipment Locations")]
    [SerializeField]
    Transform _headLocation;
    [SerializeField]
    Transform _chestLocation;
    [SerializeField]
    Transform _weaponLocation;
    [SerializeField]
    Transform _leftArmLoction;
    [SerializeField]
    Transform _rightArmLocation;
    [SerializeField]
    Transform _leftLegLocation;
    [SerializeField]
    Transform _rightLegLocation;

    CharacterSelecter _characterSelecter;

    void Start()
    {
        if(_helmet != null)
        {
            OnEquip(_helmet, true);
        }
        if (_chest != null)
        {
            OnEquip(_chest, true);
        }
        if (_legs != null)
        {
            OnEquip(_legs, true);
        }
        if (_weapon != null)
        {
            OnEquip(_weapon, true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnMouseDown()
    {
        if (_combatManager != null)
        {
            if (_combatManager.IsPlayerTurn())
            {
                _combatManager.SelectCharacter(gameObject);
                _uiManager.OnCharacterSelect(this);
            }
        }
    }

    public bool HasAction()
    {
        return !_usedAction;
    }

    public override void OnMove(int distance)
    {
        base.OnMove(distance);
        _uiManager.UpdateCharacterStats(this);
    }

    public List<GameObject> GetSkills()
    {
        return _skills;
    } 
    
    public List<GameObject> GetSelectedSkills()
    {
        return _selectedSkills;
    }

    public void AddSkill(GameObject skill)
    {
        if(skill.GetComponent<Skill>() != null)
        {
            _selectedSkills.Add(skill);
        }
        
    }

    public void SetSkills(List<GameObject> skills)
    {
        _selectedSkills = skills;
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        if (_combatManager.IsCharacterSelected(this))
        {
            _uiManager.UpdateCharacterStats(this);
        }
    }

    public override void OnTurnStart()
    {
        base.OnTurnStart();
        foreach(GameObject skill in _skills)
        {
            skill.GetComponent<Skill>().DecrementCooldown();
        }
    }

    public void OnCombatEnd()
    {
        foreach(GameObject skill in _skills)
        {
            Destroy(skill);
        }
        _skills = new List<GameObject>();
        _usedAction = false;
        TakeDamage(-5);
    }

    public override void OnCreate()
    {
        base.OnCreate();
        _skills =  new List<GameObject>();
        foreach(GameObject skill in _selectedSkills)
        {
            GameObject newSkill = Instantiate(skill, skill.transform.position, skill.transform.rotation, transform);
            _skills.Add(newSkill);
        }
    }

    public void OnEquip(GameObject gearPiece, bool isStart)
    {
        Gear gear = gearPiece.GetComponent<Gear>();
        Dictionary<StatType, int> stats = gear.GetStats();
        List<StatType> statsToAlter = stats.Keys.ToList();
        foreach (StatType stat in statsToAlter)
        {
            switch (stat)
            {
                case StatType.Health:
                    ChangeMaxHealth(stats[stat]);
                    break;
                case StatType.Attack:
                    ChangeAttack(stats[stat]);
                    break;
                case StatType.Defense:
                    ChangeDefence(stats[stat]);
                    break;
                case StatType.Magic:
                    ChangeMagic(stats[stat]);
                    break;
                case StatType.Movement:
                    ChangeMovement(stats[stat]);
                    break;
            }
        }

        switch (gear.GetGearType())
        {
            case GearType.Head:
                if (_helmet != null && !isStart)
                {
                    OnUnequip(GearType.Head);
                }
                _helmet = Instantiate(gearPiece, _headLocation);
                _helmet.transform.localScale = new Vector3(1, 1, 1);

                if (_characterSelecter != null)
                {
                    _characterSelecter.ChangeChildLayers(_headLocation, 5);
                }
                break;
            case GearType.Body:
                if (_chest != null && !isStart)
                {
                    OnUnequip(GearType.Body);
                }
                _chest = Instantiate(gearPiece, _chestLocation);
                _chest.transform.localScale = new Vector3(1, 1, 1);
                if (gear.HasArms())
                {
                    _leftArm = Instantiate(gear.GetArms(), _leftArmLoction);
                    _rightArm = Instantiate(gear.GetArms(), _rightArmLocation);

                    if (_characterSelecter != null)
                    {
                        _characterSelecter.ChangeChildLayers(_leftArmLoction, 5);
                        _characterSelecter.ChangeChildLayers(_rightArmLocation, 5);
                    }
                }

                if (_characterSelecter != null)
                {
                    _characterSelecter.ChangeChildLayers(_chestLocation, 5);
                }
                break;
            case GearType.Hand:
                if (_weapon != null && !isStart)
                {
                    OnUnequip(GearType.Hand);
                }
                _weapon = Instantiate(gearPiece, _weaponLocation);
                _weapon.transform.localScale = new Vector3(1, 1, 1);

                if (_characterSelecter != null)
                {
                    _characterSelecter.ChangeChildLayers(_weaponLocation, 5);
                }
                break;
            case GearType.Feet:
                if ( _legs !=null && !isStart)
                {
                    OnUnequip(GearType.Feet);
                }
                _legs = Instantiate(gearPiece, _rightLegLocation);
                _leftLeg = Instantiate(gearPiece, _leftLegLocation);

                if (_characterSelecter != null)
                {
                    _characterSelecter.ChangeChildLayers(_rightLegLocation, 5);
                    _characterSelecter.ChangeChildLayers(_leftLegLocation, 5);
                }
                break;
        }

    }

    public void SetCharacterSelecter(CharacterSelecter characterSelecter)
    {
        _characterSelecter = characterSelecter;
    }

    public void OnUnequip(GearType type)
    {
        GameObject gearToRemove = null;
        switch (type)
        {
            case GearType.Head:
                if (_helmet != null)
                {
                    gearToRemove = _helmet;
                }
                break;
            case GearType.Body:
                if (_chest != null)
                {
                   gearToRemove = _chest;
                }
                break;
            case GearType.Hand:
                if (_weapon != null)
                {
                    gearToRemove = _weapon;
                }
                break;
            case GearType.Feet:
                if (_legs != null)
                {
                    gearToRemove = _legs;
                }
                break;

        }
        Gear gear = gearToRemove.GetComponent<Gear>();
        Dictionary<StatType, int> stats = gear.GetStats();
        List<StatType> statsToAlter = stats.Keys.ToList();
        foreach (StatType stat in statsToAlter)
            {
                switch (stat)
                {
                    case StatType.Health:
                        ChangeMaxHealth(-1* stats[stat]);
                        break;
                    case StatType.Attack:
                        ChangeAttack(-1 * stats[stat]);
                        break;
                    case StatType.Defense:
                        ChangeDefence(-1 * stats[stat]);
                        break;
                    case StatType.Magic:
                        ChangeMagic(-1 * stats[stat]);
                        break;
                    case StatType.Movement:
                        ChangeMovement(-1 * stats[stat]);
                        break;
                }
            }
        if (type == GearType.Body && gear.HasArms())
        {
            DestroyImmediate(_leftArm);
            DestroyImmediate(_rightArm);
        }
        if(type == GearType.Feet && _leftLeg !=null)
        {
            DestroyImmediate(_leftLeg);
        }
        DestroyImmediate(gearToRemove);
           
    }


    public override void ChangeAttack(int amount)
    {
        base.ChangeAttack(amount);
        if (_characterSelecter != null)
        {
            _characterSelecter.UpdateUI();
        }
    }
    
    public override void ChangeDefence(int amount)
    {
        base.ChangeDefence(amount);
        if(_characterSelecter != null)
        {
            _characterSelecter.UpdateUI();
        }
    }

    public override void ChangeMagic(int amount)
    {
        base.ChangeMagic(amount);
        if (_characterSelecter != null)
        {
            _characterSelecter.UpdateUI();
        }
    }

    public override void ChangeMovement(int amount)
    {
        base.ChangeMovement(amount);
        if (_characterSelecter != null)
        {
            _characterSelecter.UpdateUI();
        }
    }

    public override void ChangeMaxHealth(int amount)
    {
        base.ChangeMaxHealth(amount);
        if (_characterSelecter != null)
        {
            _characterSelecter.UpdateUI();
        }
    }

    public List<GameObject> GetGear()
    {
        List<GameObject> gear = new List<GameObject>();
        if (_helmet != null)
        {
            gear.Add(_helmet);
        }
        if (_chest != null)
        {
            gear.Add(_chest);
        }
        if (_legs != null)
        {
            gear.Add(_legs);
        }
        if (_weapon != null)
        {
            gear.Add(_weapon);
        }
        return gear;
    }

}
