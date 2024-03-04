using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SkillPool : MonoBehaviour
{
    [SerializeField]
    List<SkillBox> _skillBoxes = new List<SkillBox>();
    [SerializeField]
    List<GameObject> _skillPool = new List<GameObject>();

    void Start()
    {
        if (_skillBoxes == null || _skillBoxes.Count == 0)
        {
            _skillBoxes = new List<SkillBox>(GetComponentsInChildren<SkillBox>());
        }
    }

    private void UpdateUI()
    {

        for (int i = 0; i < _skillBoxes.Count; i++)
        {
            if (i < _skillPool.Count)
            {
                _skillBoxes[i].SetSkill(_skillPool[i]);
            }
            else
            {
                _skillBoxes[i].SetSkill(null);
            }
        }
    }

    public void SetSkillPool(List<GameObject> skillPool)
    {
        _skillPool = skillPool;
        UpdateUI();
    }


}
