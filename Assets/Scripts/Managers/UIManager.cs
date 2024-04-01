using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    GameObject _characterInfo;
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
    GameObject _endTurnButton;
    [SerializeField]
    GameObject _skillsPage;
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
    Button _attackButton;
    [SerializeField]
    Button _moveButton;
    [SerializeField]
    Button _skillButton;
    [SerializeField]
    TextMeshProUGUI _turnAnnouncementText;
    [SerializeField]
    Animator _turnAnnouncementAnimator;
    [SerializeField]
    Animator _winAnimator;
    [SerializeField]
    Animator _loseAnimator;
    [SerializeField]
    TextMeshProUGUI _moveText;
    [SerializeField]
    Animator _moveAnimator;
    bool _animating = false;

    List<Button> skillButtons = new List<Button>();

    private void OnEnable()
    {
        _endTurnButton.SetActive(false);
    }
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
        _characterInfo.SetActive(true);
        UpdateCharacterStats(character);
    }

    public void UpdateCharacterStats(PlayerCharacter character)
    {
       
        _characterName.text = character.GetName();
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
        if(!character.HasAction())
        {
            EnableSkillPage(false);
            _skillButton.interactable = false;
            _attackButton.interactable = false;
        }
        else
        {
            _skillButton.interactable = true;
            _attackButton.interactable = true;
        }
        if(character._movementRemaining <= 0)
        {
            _moveButton.interactable = false;
        }
        else
        {
            _moveButton.interactable = true;
        }


    }

    public void SetState(string state)
    {
        if(state == "PLAYER")
        {
            _turnAnnouncementText.text = "Player Turn!";
        }
        else
        {
            _turnAnnouncementText.text = "Enemy Turn!";
        }
        _animating = true;
        _turnAnnouncementAnimator.SetTrigger("NewTurn");
        StartCoroutine(TurnAnimation(state));
    }

    public bool GetAnimating()
    {
        return _animating;
    }

    IEnumerator TurnAnimation(string state)
    {
        if (state == "PLAYER")
        {
            yield return new WaitForSeconds(4f);
            _endTurnButton.SetActive(true);
        }
        else
        {
            _endTurnButton.SetActive(false);
            yield return new WaitForSeconds(4f);
        }
        _animating = false;
    }

    public void EnableSkillPage(bool enable)
    {
        _skillsPage.SetActive(enable);
    }

    public bool SkillsPageActive()
    {
        return _skillsPage.activeSelf;
    }

    public void OnCharacterDeselect()
    {
        _characterInfo.SetActive(false);
    }

    public void EndBattle()
    {
        _endTurnButton.SetActive(false);
        _winAnimator.SetTrigger("NewTurn");
    }

    public void ShowDeathScreen()
    {
        _loseAnimator.SetTrigger("NewTurn");
    }

    public void StartMove(string characterName)
    {
        if(_moveText == null || _moveAnimator == null)
        {
            return;
        }
        _moveText.text = characterName + " went for an attack!";
        _moveAnimator.SetTrigger("AttackStarted");
    }

    public void StartMove(string characterName, string skillName)
    {
        if (_moveText == null || _moveAnimator == null)
        {
            return;
        }
        _moveText.text = characterName + " used " + skillName + "!";
        _moveAnimator.SetTrigger("AttackStarted");
    }

    public void StopMove()
    {
        if (_moveText == null || _moveAnimator == null)
        {
            return;
        }
        _moveAnimator.SetTrigger("AttackFinished");
    }
}
