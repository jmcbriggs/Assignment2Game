using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

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
    string _chosenName;

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
    [SerializeField]
    List<Color> _availableCharacterColors;
    [SerializeField]
    List<Color> _availableHairColors;
    [SerializeField]
    Color _characterColor;
    [SerializeField]
    Color _hairColor;
    void Start()
    {
        _selectedCharacterIndex = SelecterIndex;
        SetCharacterToSelector();
        if(_availableCharacterColors.Count > 0)
        {
            _characterColor = _availableCharacterColors[0];
        }
        if(_availableHairColors.Count > 0)
        {
            _hairColor = _availableHairColors[0];
        }
        CreatePrefab();
        UpdatePrefab();

    }

    void CreatePrefab()
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
    }

    public void NextCharacter()
    {
        _selectedCharacterIndex++;
        if (_selectedCharacterIndex >= GameController.Instance.GetCharacterCount())
        {
            _selectedCharacterIndex = 0;
        }
        SetCharacterToSelector();
    }

    public void PreviousCharacter()
    {
        _selectedCharacterIndex--;
        if (_selectedCharacterIndex < 0)
        {
            _selectedCharacterIndex = GameController.Instance.GetCharacterCount() - 1;
        }
        SetCharacterToSelector();
    }

    void SetCharacterToSelector()
    {
        if(GameController.Instance == null)
        {
            return;
        }
        CurrentCharacter = GameController.Instance.GetAvailableCharacter(_selectedCharacterIndex);
        if(_chosenName == null || _chosenName == "")
        {
            _chosenName = "The Unloved One";
        }
        GameController.Instance.SetCharacter(SelecterIndex, CurrentCharacter);
        CreatePrefab();
        UpdatePrefab();
    }

    public void CycleBodyColors()
    {
        int currentIndex = _availableCharacterColors.IndexOf(_characterColor);
        currentIndex++;
        if (currentIndex >= _availableCharacterColors.Count)
        {
            currentIndex = 0;
        }
        _characterColor = _availableCharacterColors[currentIndex];
        UpdatePrefab();
    }

    public void CycleHairColors()
    {
        int currentIndex = _availableHairColors.IndexOf(_hairColor);
        currentIndex++;
        if (currentIndex >= _availableHairColors.Count)
        {
            currentIndex = 0;
        }
        _hairColor = _availableHairColors[currentIndex];
        UpdatePrefab();
    }

    public void CycleHairStyle()
    {
        int index = _currentPrefab.GetComponent<HairController>().CycleHair();
        Sprite front = _currentPrefab.GetComponent<HairController>().GetHairFront(index);
        Sprite back = _currentPrefab.GetComponent<HairController>().GetHairBack(index);
        _currentPrefab.GetComponent<BodyColour>().SetHair(front, back);
        UpdatePrefab();
    }

    public void ToggleBeard()
    {
        _currentPrefab.GetComponent<HairController>().ToggleBeard();
        bool active = _currentPrefab.GetComponent<HairController>().GetBeardState();
        _currentPrefab.GetComponent<BodyColour>().SetBeard(active);
        UpdatePrefab();
    }



    public Color GetCharacterColor()
    {
        return _characterColor;
    }

    public Color GetHairColor()
    {
        return _hairColor;
    }

    public int GetSelecterIndex()
    {
        return SelecterIndex;
    }

    public bool GetBeardState()
    {
        return _currentPrefab.GetComponent<HairController>().GetBeardState();
    }

    public Sprite GetHairFront()
    {
        int index = _currentPrefab.GetComponent<HairController>().GetHairIndex();
        return _currentPrefab.GetComponent<HairController>().GetHairFront(index);
    }

    public Sprite GetHairBack()
    {
        int index = _currentPrefab.GetComponent<HairController>().GetHairIndex();
        return _currentPrefab.GetComponent<HairController>().GetHairBack(index);
    }

    public void UpdatePrefab()
    {
        _currentPrefab.GetComponent<PlayerCharacter>().SetSkinColour(_characterColor);
        _currentPrefab.GetComponent<PlayerCharacter>().SetHairColour(_hairColor);
        if(GameController.Instance != null)
        {
            _currentPrefab.GetComponent<PlayerCharacter>().SetBodyColour(GameController.Instance.GetCharacterBodyColour(SelecterIndex));
        }
        ChangeChildLayers(_currentPrefab.transform, 5);
        _currentPrefab.GetComponent<SortingGroup>().sortingOrder = 0;
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

    public void SetChosenName(string name)
    {
        _chosenName = name;
        SetCharacterToSelector();
    }

    public string GetName()
    {
        return _chosenName;
    }

    public void UpdateUI()
    {
        _characterName.text = "Class Name: " + _currentPrefab.GetComponent<PlayerCharacter>()._characterClass;
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
