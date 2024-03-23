using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterSelecter : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    int SelecterIndex = 0;
    [SerializeField]
    GameObject CurrentCharacter;
    [SerializeField]
    float CharacterScale = 1.0f;
    int _selectedCharacterIndex = 0;

    [SerializeField]
    private Transform _prefabPosition;
    private GameObject _currentPrefab;

    [Header("UI Elements")]
    [SerializeField]
    private TextMeshProUGUI _characterName;
    [SerializeField]
    private TextMeshProUGUI _characterHealth;
    [SerializeField]
    private TextMeshProUGUI _characterMovement;
    [SerializeField]
    private TextMeshProUGUI _characterAttack;
    [SerializeField]
    private TextMeshProUGUI _characterDefence;
    [SerializeField]
    private TextMeshProUGUI _characterMagic;
    [SerializeField]
    private TextMeshProUGUI _characterSkill;
    void Start()
    {
        CurrentCharacter = GameController.Instance.GetAvailableCharacter(SelecterIndex);
        _selectedCharacterIndex = SelecterIndex;
        UpdatePrefab();

    }

    public void NextCharacter()
    {
        _selectedCharacterIndex++;
        if (_selectedCharacterIndex >= GameController.Instance.GetCharacterCount())
        {
            _selectedCharacterIndex = 0;
        }
        CurrentCharacter = GameController.Instance.GetAvailableCharacter(_selectedCharacterIndex);
        GameController.Instance.SetCharacter(SelecterIndex, CurrentCharacter);
        UpdatePrefab();
    }

    public void PreviousCharacter()
    {
        _selectedCharacterIndex--;
        if (_selectedCharacterIndex < 0)
        {
            _selectedCharacterIndex = GameController.Instance.GetCharacterCount() - 1;
        }
        CurrentCharacter = GameController.Instance.GetAvailableCharacter(_selectedCharacterIndex);
        UpdatePrefab();
    }

    public void UpdatePrefab()
    {
        if (_currentPrefab != null)
        {
            Destroy(_currentPrefab);
        }
        _currentPrefab = Instantiate(CurrentCharacter, _prefabPosition.position, _prefabPosition.rotation);
        _currentPrefab.transform.SetParent(_prefabPosition);
        _currentPrefab.transform.localScale = new Vector3(CharacterScale, CharacterScale, CharacterScale);
        _currentPrefab.layer = 5;
        _currentPrefab.GetComponent<PlayerCharacter>().SetCharacterSelecter(this);
        _currentPrefab.GetComponent<Animator>().SetBool("HasAction", true);
        BodyColour bodyColour = _currentPrefab.GetComponentInChildren<BodyColour>();
        if(bodyColour != null)
        {
            bodyColour.SetColour(GameController.Instance.GetCharacterColour(SelecterIndex));
        }
        ChangeChildLayers(_currentPrefab.transform, 5);
    }

    public void ChangeChildLayers(Transform parent, int layer)
    {
        parent.gameObject.layer = layer;
        foreach (Transform child in parent)
        {
            if (child.gameObject.name != "HealthBar")
            {
                ChangeChildLayers(child, layer);
            }
        }
        UpdateUI();
    }

    public void UpdateUI()
    {
        _characterName.text = "Class Name: " + _currentPrefab.GetComponent<PlayerCharacter>()._characterName;
        _characterHealth.text = "Health: " + _currentPrefab.GetComponent<PlayerCharacter>().GetMaxHealth().ToString();
        _characterMovement.text = "Movement: " + _currentPrefab.GetComponent<PlayerCharacter>().GetMovement().ToString();
        _characterAttack.text = "Attack: " + _currentPrefab.GetComponent<PlayerCharacter>().GetAttack().ToString();
        _characterDefence.text = "Defence: " + _currentPrefab.GetComponent<PlayerCharacter>().GetDefence().ToString();
        _characterMagic.text = "Magic: " + _currentPrefab.GetComponent<PlayerCharacter>().GetMagic().ToString();
        if(_currentPrefab.GetComponent<PlayerCharacter>().GetSelectedSkills().Count > 0)
        {
            _characterSkill.text = "Skill: " + _currentPrefab.GetComponent<PlayerCharacter>().GetSelectedSkills()[0].GetComponent<Skill>().GetSkillName();
        }
        else
        {
            _characterSkill.text = "Skill: N/A";
        }
      

    }
}
