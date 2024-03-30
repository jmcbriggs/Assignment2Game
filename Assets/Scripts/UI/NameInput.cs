using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NameInput : MonoBehaviour
{
    public CharacterSelecter _characterSelecter;
    TMP_InputField _inputField;
    // Start is called before the first frame update
    void Start()
    {
        _inputField = GetComponent<TMP_InputField>();
        _inputField.onValueChanged.AddListener(_characterSelecter.SetChosenName);
    }
}
