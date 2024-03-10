using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gear : MonoBehaviour
{
    [SerializeField]
    string _gearName;
    [SerializeField]
    string _gearDescription;
    [SerializeField]
    GearType _gearType;
    [SerializeField]
    int _health;
    [SerializeField]
    int _attack;
    [SerializeField]
    int _defense;
    [SerializeField]
    int _magic;
    [SerializeField]
    int _movement;
    [SerializeField]
    int gearLevel;

    [Header("Visuals")]
    [SerializeField]
    GameObject _arms;
    
    Dictionary<StatType, int> _stats = new Dictionary<StatType, int>();

   private void Awake()
    {
        if(_gearType != GearType.Body && _arms != null)
        {
            Debug.LogError("Arms should only be set on body gear");
            _arms.SetActive(false);
        }
        if(_health != 0)
        {
            _stats.Add(StatType.Health, _health);
        }
        if(_attack != 0)
        {
            _stats.Add(StatType.Attack, _attack);
        }
        if(_defense != 0)
        {
            _stats.Add(StatType.Defense, _defense);
        }
        if(_magic != 0)
        {
            _stats.Add(StatType.Magic, _magic);
        }
        if(_movement != 0)
        {
            _stats.Add(StatType.Movement, _movement);
        }
    }
    public enum GearType
    {
        Head,
        Body,
        Hand,
        Feet
    }

    public enum StatType
    {
        Health,
        Attack,
        Defense,
        Magic,
        Movement
    }

    public int GetGearLevel()
    {
        return gearLevel;
    }   

    public Dictionary<StatType, int> GetStats()
    {
        if (_stats == null || _stats.Count == 0)
        {
            Awake(); // Ensure initialization if not done yet
        }
        return _stats;
    }

    public GearType GetGearType()
    {
        return _gearType;
    }

    public string GetGearName()
    {
        return _gearName;
    }

    public string GetGearDescription()
    {
        return _gearDescription;
    }

    public bool HasArms()
    {
        return _arms != null;
    }

    public GameObject GetArms()
    {
        return _arms;
    }
}
