using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class AssignmentScreenContoller : MonoBehaviour
{
    [SerializeField]
    List<AssignerControl> Assigners;
    [SerializeField]
    GameObject _menu;

    [Header("Skill Pool")]
    [SerializeField]
    private List<GameObject> _skillPool = new List<GameObject>();
    private List<GameObject> _persistantSkillPool;
    [SerializeField]
    int _skillInstanceCount = 3;
    [SerializeField]
    SkillPool _skillPoolScript;

    [Header("Gear Pool")]
    [SerializeField]
    private List<GameObject> _gearPool = new List<GameObject>();
    [SerializeField]
    int _gearInstanceCount = 3;
    private List<GameObject> _persistantGearPool;
    [SerializeField]
    GearPool _gearPoolScript;

    [SerializeField]
    GameObject _gearDescription;
    [SerializeField]
    GameObject _skillDescription;
   

    // Start is called before the first frame update
    void Awake()
    {
        if (_skillPoolScript == null)
        {
            _skillPoolScript = GameObject.Find("SkillPool").GetComponent<SkillPool>();
        }

        if (Assigners.Count == 0 || Assigners == null)
        {
            Assigners = new List<AssignerControl>(FindObjectsByType<AssignerControl>(FindObjectsSortMode.InstanceID));
        }

        if (GameController.Instance != null)
        {
            if(GameController.Instance.HasGearPool())
            {
                _persistantGearPool = GameController.Instance.GetGearPool();
            }
            else
            {
                _persistantGearPool = CreateGearPool();
                GameController.Instance.SetGearPool(_persistantGearPool);
            }
            if (GameController.Instance.HasSkillPool())
            {
                _persistantSkillPool = GameController.Instance.GetSkillPool();
            }
            else
            {
                _persistantSkillPool = CreateSkillPool();
                GameController.Instance.SetSkillPool(_persistantSkillPool);
            }
            foreach(GameObject character in GameController.Instance.GetSelectedCharacters())
            {
                character.transform.position = new Vector3(1000, 1000, 1000);
            }
            
            GameController.Instance.SetMenu(_menu);
        }
        else
        {
            _persistantGearPool = CreateGearPool();
            _persistantSkillPool = CreateSkillPool();
        }
        SetSkillPools();
        SetGearPools();
    }

    void SetSkillPools()
    {
        Random.InitState(System.DateTime.Now.Millisecond);
        List<GameObject> randomSkills = new List<GameObject>();
        if (GameController.Instance != null)
        {
            int level = GameController.Instance.GetLevel();
            List<GameObject> weightedSkills = WeightSkillChanceForLevel(level);
            for (int i = 0; i < 4; i++)
            {
                if (weightedSkills.Count > 0)
                {
                    GameObject skill = weightedSkills[Random.Range(0, weightedSkills.Count)];
                    randomSkills.Add(skill);
                    weightedSkills.Remove(skill);
                    _persistantSkillPool.Remove(skill);
                }
                else
                {
                    Debug.LogError("No gear found for level " + level);
                }
            }
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                GameObject skill = _skillPool[Random.Range(0, _skillPool.Count)];
                randomSkills.Add(skill);
                _persistantSkillPool.Remove(skill);
            }
        }
        if (_skillPoolScript != null)
        {
            _skillPoolScript.SetSkillPool(randomSkills);
        }
    }

    List <GameObject> CreateGearPool()
    {
        List<GameObject> persistantGear = new List<GameObject>();
        foreach (GameObject gear in _gearPool)
        {
            for (int i = 0; i < _gearInstanceCount; i++)
            {
                persistantGear.Add(gear);
            }
        }
        return persistantGear;
    }

    List<GameObject> CreateSkillPool()
    {
        List<GameObject> persistantSkills = new List<GameObject>();
        foreach (GameObject skill in _skillPool)
        {
            for (int i = 0; i < _skillInstanceCount; i++)
            {
                persistantSkills.Add(skill);
            }
        }
        return persistantSkills;
    }

    void SetGearPools()
    {
        if (_gearPoolScript == null)
        {
            _gearPoolScript = GameObject.Find("GearPool").GetComponent<GearPool>();
        }
        Random.InitState(System.DateTime.Now.Millisecond);
        List<GameObject> randomGear = new List<GameObject>();
        if(GameController.Instance != null)
        {
            int level = GameController.Instance.GetLevel();
            List<GameObject> weightedGear = WeightGearChanceForLevel(level);
            for (int i = 0; i < 4; i++)
            {
                if (weightedGear.Count > 0)
                {
                    GameObject gear = weightedGear[Random.Range(0, weightedGear.Count)];
                    randomGear.Add(gear);
                    weightedGear.Remove(gear);
                    _persistantGearPool.Remove(gear);
                }
                else
                {
                   Debug.LogError("No gear found for level " + level);
                }
            }
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                randomGear.Add(_gearPool[Random.Range(0, _gearPool.Count)]);
            }
        }

        foreach (AssignerControl assigner in Assigners)
        {
            GearAssigner gearAssigner = assigner.GetComponentInChildren<GearAssigner>();
            gearAssigner.SetGearPool(_gearPool);
        }
        if (_gearPoolScript != null)
        {
            _gearPoolScript.SetGearPool(randomGear);
        }
    }

    public void SwitchUI(TextMeshProUGUI buttonText)
    {
        if(_skillPoolScript.gameObject.activeSelf)
        {
            _skillPoolScript.gameObject.SetActive(false);
            _gearPoolScript.gameObject.SetActive(true);
            buttonText.text = "Switch To Skill Select";
        }
        else
        {
            _skillPoolScript.gameObject.SetActive(true);
            _gearPoolScript.gameObject.SetActive(false);
            buttonText.text = "Switch To Gear Select";
        }
    }

    public void RemoveGear(GameObject gear)
    {
        _persistantGearPool.Remove(gear);
    }
    public void RemoveSkill(GameObject skill)
    {
        _persistantSkillPool.Remove(skill);
    }

    private List<GameObject> WeightGearChanceForLevel(int level)
    {
        List<GameObject> weightedGear = new List<GameObject>();
        foreach(GameObject gear in _persistantGearPool)
        {
            Gear gearScript = gear.GetComponent<Gear>();
            int gearLevel = gearScript.GetGearLevel();
            if(gearLevel == level)
            {
                for(int i = 0; i < 10; i++)
                {
                    weightedGear.Add(gear);
                }
            }
            if(gearLevel < level && gearLevel > level-5 && gearLevel != 0)
            {
                int levelDifference = level - gearLevel;
                int gearCount = Mathf.Clamp(10 - (2*levelDifference), 1, 10);
                for (int i = 0; i < gearCount; i++)
                {
                    weightedGear.Add(gear);
                }
            }
        }
        return weightedGear;
    }

    private List<GameObject> WeightSkillChanceForLevel(int level)
    {
        List<GameObject> weightedSkills = new List<GameObject>();
        foreach (GameObject skill in _persistantSkillPool)
        {
            Skill skillScript = skill.GetComponent<Skill>();
            int gearLevel = skillScript.GetLevel();
            if (gearLevel == level)
            {
                for (int i = 0; i < 10; i++)
                {
                    weightedSkills.Add(skill);
                }
            }
            if (gearLevel < level && gearLevel > level - 5 && gearLevel != 0)
            {
                int levelDifference = level - gearLevel;
                int gearCount = Mathf.Clamp(10 - (2 * levelDifference), 1, 10);
                for (int i = 0; i < gearCount; i++)
                {
                    weightedSkills.Add(skill);
                }
            }
        }
        return weightedSkills;
    }

    public GameObject GetGearDescription()
    {
        return _gearDescription;
    }

    public GameObject GetSkillDescription()
    {
        return _skillDescription;
    }
    public void StartBattle()
    {
        if(GameController.Instance != null)
        {
            GameController.Instance.SetGearPool(_persistantGearPool);
            GameController.Instance.SetSkillPool(_persistantSkillPool);
            GameController.Instance.EnterBattle();
        }
    }

    public void ReturnToMenu()
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.ReturnToMenu(false);
        }
        else
        {
            Debug.Log("GameController not found");
            Debug.Log("Pretend to return to menu");
        }
    }


}
