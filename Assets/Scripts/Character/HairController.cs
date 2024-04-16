using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairController : MonoBehaviour
{
    [SerializeField]
    GameObject _beard;
    [SerializeField]
    List<Sprite> _hairFront;
    [SerializeField]
    List<Sprite> _hairBack;
    [SerializeField]
    int _hairIndex;

    BodyColour _bodyColour;
    // Start is called before the first frame update
    void Start()
    {
        _bodyColour = GetComponent<BodyColour>();
        _hairIndex = Random.Range(0, _hairFront.Count);
    }

    public void ToggleBeard()
    {
        if(_beard != null)
        {
            _beard.SetActive(!_beard.activeSelf);
        }
    }

    public int CycleHair() { 
        _hairIndex++;
        if(_hairIndex >= _hairFront.Count)
        {
            _hairIndex = 0;
        }
        return _hairIndex;
    }

    public int GetHairIndex()
    {
        return _hairIndex;
    }

    public Sprite GetHairFront(int index)
    {
        if(index < _hairFront.Count)
        {
            return _hairFront[index];
        }
        return null;
    }

    public Sprite GetHairBack(int index)
    {
        if(index < _hairBack.Count)
        {
            return _hairBack[index];
        }
        return null;
    }

    public void SetBeard(bool active)
    {
        if(_beard != null)
        {
            _beard.SetActive(active);
        }
    }

    public bool GetBeardState()
    {
        if(_beard != null)
        {
            return _beard.activeSelf;
        }
        return false;
    }

    public int GetHairCount()
    {
        return _hairFront.Count;
    }
}
