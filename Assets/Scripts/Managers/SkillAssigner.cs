using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SkillAssigner : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    List<SkillBox> _skillBoxes = new List<SkillBox>();
    GameObject _currentCharacter;

    public void Initialise(GameObject character)
    {
        if (_skillBoxes == null || _skillBoxes.Count == 0)
        {
            _skillBoxes = new List<SkillBox>(GetComponentsInChildren<SkillBox>());
        }
        _currentCharacter = character;
        UpdateUI();
    }
    public void UpdateUI()
    {
        if (_currentCharacter != null)
        {
            PlayerCharacter character = _currentCharacter.GetComponent<PlayerCharacter>();
            if (character != null)
            {
                for (int i = 0; i < _skillBoxes.Count; i++)
                {
                    if (i < character.GetSelectedSkills().Count)
                    {
                        _skillBoxes[i].SetSkill(character.GetSelectedSkills()[i]);
                    }
                    else
                    {
                        _skillBoxes[i].SetSkill(null);
                    }
                }
            }
        }
    }
    public void SetCharacterSkills()
    {
        if (_currentCharacter != null)
        {
            PlayerCharacter character = _currentCharacter.GetComponent<PlayerCharacter>();
            if (character != null)
            {
                List<GameObject> skills = new List<GameObject>();
                for (int i = 0; i < _skillBoxes.Count; i++)
                {
                    if (_skillBoxes[i].GetSkill() != null)
                    {
                        skills.Add(_skillBoxes[i].GetSkill());
                    }
                }
                character.SetSkills(skills);
            }
        }
    }

}
