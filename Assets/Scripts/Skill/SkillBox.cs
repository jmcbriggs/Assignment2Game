using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillBox : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    [SerializeField] 
    private GameObject _skill;
    [SerializeField]
    private TextMeshProUGUI _skillName;
    [SerializeField]
    Canvas _canvas;
    [SerializeField]
    SkillAssigner _skillAssigner;
    [SerializeField]
    SkillDescriptionBox _skillDescription;
    private CanvasGroup _canvasGroup;   
    private RectTransform _rectTransform;
    private Vector2 _initialPosition;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _canvas.overrideSorting = true;
        _canvas.sortingOrder = 1;
        _canvasGroup = GetComponent<CanvasGroup>();
        _rectTransform = GetComponent<RectTransform>();
        _initialPosition = _rectTransform.anchoredPosition;
        if(_skillAssigner == null)
        {
            _skillAssigner = GetComponentInParent<SkillAssigner>();
        }
        if(_skillDescription == null)
        {
            _skillDescription = FindObjectOfType<SkillDescriptionBox>();
        }
    }
    public void OnDrop(PointerEventData eventData)
    {
        if(eventData.pointerDrag != null)
        {
            GameObject skill = eventData.pointerDrag.GetComponent<SkillBox>().GetSkill();
            eventData.pointerDrag.GetComponent<SkillBox>().SetSkill(_skill);
            SetSkill(skill);
            if (_skillAssigner != null)
            {
                _skillAssigner.SetCharacterSkills();
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.alpha = 0.6f;
        _canvas.sortingOrder = 2;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition = _initialPosition;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.alpha = 1f;
        _canvas.sortingOrder = 1;

    }

    public void OnPointerOver()
    {
        if (_skillDescription != null && _skill != null)
        {
            _skillDescription.Set(_skill.GetComponent<Skill>());
        }
    }

    public void OnPointerExit()
    {
        if (_skillDescription != null && _skill != null)
        {
            _skillDescription.Clear();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }

    public GameObject GetSkill()
    {
        return _skill;
    }

    public void SetSkill(GameObject skill)
    {
        _skill = skill;
        if(skill != null)
        {
            _skillName.text = _skill.GetComponent<Skill>().GetSkillName();
        }
        else
        {
            _skillName.text = "Empty Skill";
        }

    }   
}
