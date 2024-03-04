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
        Random.InitState(System.DateTime.Now.Millisecond);
        List<GameObject> randomSkills = new List<GameObject>();
        for(int i = 0; i <4; i++)
        {
            randomSkills.Add(_skillPool[Random.Range(0, _skillPool.Count)]);
        }
        if(_skillPoolScript != null)
        {
            _skillPoolScript.SetSkillPool(randomSkills);
        }
        if(Assigners.Count == 0 || Assigners == null)
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

    void SetGearPools()
    {
        if (_gearPoolScript == null)
        {
            _gearPoolScript = GameObject.Find("GearPool").GetComponent<GearPool>();
        }
        Random.InitState(System.DateTime.Now.Millisecond);
        List<GameObject> randomGear = new List<GameObject>();
        for (int i = 0; i < 4; i++)
        {
            randomGear.Add(_gearPool[Random.Range(0, _gearPool.Count)]);
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
        foreach(AssignerControl assigner in Assigners)
        {
            assigner.SwitchUI();
        }
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

    public void StartBattle()
    {
        if(GameController.Instance != null)
        {
            GameController.Instance.EnterBattle();
        }
    }


}
