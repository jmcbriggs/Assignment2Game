using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AssignmentScreenContoller : MonoBehaviour
{
    [SerializeField]
    List<AssignerControl> Assigners;

    [Header("Skill Pool")]
    [SerializeField]
    private List<GameObject> _skillPool = new List<GameObject>();
    [SerializeField]
    SkillPool _skillPoolScript;

    [Header("Gear Pool")]
    [SerializeField]
    private List<GameObject> _gearPool = new List<GameObject>();
    [SerializeField]
    GearPool _gearPoolScript;
   

    // Start is called before the first frame update
    void Awake()
    {
        if (_skillPoolScript == null)
        {
            _skillPoolScript = GameObject.Find("SkillPool").GetComponent<SkillPool>();
        }

        SetSkillPools();
        if (Assigners.Count == 0 || Assigners == null)
        {
            Assigners = new List<AssignerControl>(FindObjectsByType<AssignerControl>(FindObjectsSortMode.InstanceID));
        }

        SetGearPools();
        if (GameController.Instance != null)
        {
            foreach(GameObject character in GameController.Instance.GetSelectedCharacters())
            {
                character.transform.position = new Vector3(1000, 1000, 1000);
            }
        }
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
                    randomSkills.Add(weightedSkills[Random.Range(0, weightedSkills.Count)]);
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
                randomSkills.Add(_skillPool[Random.Range(0, _skillPool.Count)]);
            }
        }
        if (_skillPoolScript != null)
        {
            _skillPoolScript.SetSkillPool(randomSkills);
        }
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
                    randomGear.Add(weightedGear[Random.Range(0, weightedGear.Count)]);
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
            buttonText.text = "Skill Select";
        }
        else
        {
            _skillPoolScript.gameObject.SetActive(true);
            _gearPoolScript.gameObject.SetActive(false);
            buttonText.text = "Gear Select";
        }
    }

    private List<GameObject> WeightGearChanceForLevel(int level)
    {
        List<GameObject> weightedGear = new List<GameObject>();
        foreach(GameObject gear in _gearPool)
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
        foreach (GameObject skill in _skillPool)
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
    public void StartBattle()
    {
        if(GameController.Instance != null)
        {
            GameController.Instance.EnterBattle();
        }
    }


}
