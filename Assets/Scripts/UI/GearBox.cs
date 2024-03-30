using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GearBox : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    [SerializeField] 
    private GameObject _gear;
    [SerializeField]
    private TextMeshProUGUI _gearName;
    [SerializeField]
    Canvas _canvas;
    [SerializeField]
    GearAssigner _gearAssigner;
    [SerializeField]
    GearDescriptionBox _gearDescription;
    private CanvasGroup _canvasGroup;   
    private RectTransform _rectTransform;
    private Vector2 _initialPosition;
    [SerializeField]
    private Gear.GearType _gearType;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _canvas.overrideSorting = true;
        _canvas.sortingOrder = 1;
        _canvasGroup = GetComponent<CanvasGroup>();
        _rectTransform = GetComponent<RectTransform>();
        _initialPosition = _rectTransform.anchoredPosition;
        if(_gearAssigner == null)
        {
            _gearAssigner = GetComponentInParent<GearAssigner>();
        }
        if(_gearDescription == null)
        {
            _gearDescription = FindObjectOfType<AssignmentScreenContoller>().GetGearDescription().GetComponent<GearDescriptionBox>();
        }
    }
    public void OnDrop(PointerEventData eventData)
    {
        if(eventData.pointerDrag != null)
        {
            if(eventData.pointerDrag.GetComponent<GearBox>() != null)
            {
                GameObject gear = eventData.pointerDrag.GetComponent<GearBox>().GetGear();
                if(gear.GetComponent<Gear>().GetGearType() == _gearType)
                {
                    eventData.pointerDrag.GetComponent<GearBox>().SetGear(_gear);
                    SetGear(gear);
                    if(_gearAssigner != null)
                    {
                        _gearAssigner.SetCharacterGear();
                    }
                    if (eventData.pointerDrag.GetComponent<GearBox>().HasGearAssigner())
                    {
                        eventData.pointerDrag.GetComponent<GearBox>().GetGearAssigner().SetCharacterGear();
                    }
                }
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
        if (_gearDescription != null && _gear != null)
        {
            Switcher switcher = FindObjectOfType<Switcher>();
            if (switcher != null)
            {
                switcher.Switch(2);
            }
            _gearDescription.Set(_gear.GetComponent<Gear>());
        }
    }

    public void OnPointerExit()
    {
        if (_gearDescription != null && _gear != null)
        {
            _gearDescription.Clear();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }

    public GameObject GetGear()
    {
        return _gear;
    }

    public bool HasGearAssigner()
    {
        return _gearAssigner != null;
    }

    public GearAssigner GetGearAssigner()
    {
        return _gearAssigner;
    }

    public void SetGear(GameObject gear)
    {
        _gear = gear;
        if(gear != null)
        {
            _gearName.text = _gear.GetComponent<Gear>().GetGearName();
        }
        else
        {
            _gearName.text = "Empty Gear";
        }

    }   

    public Gear.GearType GetGearType()
    {
        return _gearType;
    }
}
