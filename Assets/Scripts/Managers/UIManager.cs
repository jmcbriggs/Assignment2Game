using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    Image _charcterInfo;
    [SerializeField]
    TextMeshProUGUI _characterName;
    [SerializeField]
    TextMeshProUGUI _characterHealth;
    [SerializeField]
    TextMeshProUGUI _characterMovement;
    [SerializeField]
    TextMeshProUGUI _characterAttack;
    [SerializeField]
    TextMeshProUGUI _characterDefence;
    [SerializeField]
    TextMeshProUGUI _characterMagic;
    [SerializeField]
    TextMeshProUGUI _stateText;
    [SerializeField]
    Button _endTurnButton;
    [SerializeField]
    Button _exitBattleButton;
    [SerializeField]
    Image _skillsPage;
    [SerializeField]
    Button _skill1;
    [SerializeField]
    Button _skill2;
    [SerializeField]
    Button _skill3;
    [SerializeField]
    Button _skill4;
    [SerializeField]
    Image _deathScreen;
    [SerializeField]
    Button _returnToMenu;

    List<Button> skillButtons = new List<Button>();

    // Start is called before the first frame update
    void Start()
    {
        skillButtons.Add(_skill1);
        skillButtons.Add(_skill2);
        skillButtons.Add(_skill3);
        skillButtons.Add(_skill4);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnCharacterSelect(PlayerCharacter character)
    {
        _charcterInfo.gameObject.SetActive(true);
        UpdateCharacterStats(character);
    }

    public void UpdateCharacterStats(PlayerCharacter character)
    {
       
        _characterName.text = "Name: " + character._characterName;
        _characterHealth.text = "Health: " + character.GetHealth() + "/" + character.GetMaxHealth();
        _characterMovement.text = "Movement: " + character._movementRemaining + "/" + character._movement;
        _characterAttack.text = "Attack: " + character.GetAttack();
        _characterDefence.text = "Defence: " + character.GetDefence();
        _characterMagic.text = "Magic: " + character._magic;
        List<GameObject> skills = character.GetSkills();
        for (int i = 0; i < skillButtons.Count; i++)
        {
            if(skills.Count > 4)
            {
                Debug.LogError("Too many skills");
            }
            if (i < skills.Count)
            {
                Skill skill = skills[i].GetComponent<Skill>();
                skillButtons[i].gameObject.SetActive(true);
                skillButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = skill.GetSkillName();
                if (skill.OnCooldown())
                {
                    skillButtons[i].interactable = false;
                }
                else
                {
                    skillButtons[i].interactable = true;
                }
            }
            else
            {
                skillButtons[i].gameObject.SetActive(false);
            }
        }

    }

    public void SetState(string state)
    {
        _stateText.text = state;
        if(state == "PLAYER")
        {
            _endTurnButton.gameObject.SetActive(true);
        }
        else
        {
            _endTurnButton.gameObject.SetActive(false);
        }
    }

    public void TurnOffSkillsPage()
    {
        _skillsPage.gameObject.SetActive(false);
    }

    public void OnCharacterDeselect()
    {
        _charcterInfo.gameObject.SetActive(false);
    }

    public void EndBattle()
    {
        _endTurnButton.gameObject.SetActive(false);
        _exitBattleButton.gameObject.SetActive(true);
    }

    public void ShowDeathScreen()
    {
        _deathScreen.color = new Color(0, 0, 0, 0);
        _deathScreen.gameObject.SetActive(true);
        _endTurnButton.gameObject.SetActive(false);
        StartCoroutine(DeathScreen());
    }

    IEnumerator DeathScreen()
    {
        while(_deathScreen.color.a < 1)
        {
            _deathScreen.color = new Color(0, 0, 0, _deathScreen.color.a + (0.2f * Time.deltaTime));
            yield return null;
        }
        _returnToMenu.gameObject.SetActive(true);
    }

}
