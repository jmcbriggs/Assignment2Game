using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillDescriptionBox : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI _skillName;
    [SerializeField]
    TextMeshProUGUI _skillDescription;
    [SerializeField]
    TextMeshProUGUI _skillPower;
    [SerializeField]
    TextMeshProUGUI _skillRange;
    [SerializeField]
    TextMeshProUGUI _skillCooldown;
    // Start is called before the first frame update
    public void Set(Skill skill)
    {
        _skillName.text = "Skill Name: " + skill.GetSkillName();
        _skillDescription.text = "Description: " + skill.GetSkillDescription();
        _skillPower.text = "Power: " + skill.GetPower().ToString();
        _skillRange.text = "Range: " + skill.GetRange().ToString();
        _skillCooldown.text = "Cooldown: " + skill.GetCooldown().ToString();
    }

    public void Clear()
    {
        _skillName.text = "Skill Name: ";
        _skillDescription.text = "Description: ";
        _skillPower.text = "Power: ";
        _skillRange.text = "Range: ";
        _skillCooldown.text = "Cooldown: ";
    }
}
