using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switcher : MonoBehaviour
{
    [SerializeField]
    GameObject Object1;
    [SerializeField]
    GameObject Object2;
    
    void Start()
    {
        if(Object1.activeSelf)
        {
            Object2.SetActive(false);
        }
        else
        {
            Object1.SetActive(false);
        }
    }
    public void Switch()
    {
        if(Object1.activeSelf)
        {
            Object1.SetActive(false);
            Object2.SetActive(true);
        }
        else
        {
            Object1.SetActive(true);
            Object2.SetActive(false);
        }
    }

    public void Switch(int index)
    {
        if(index == 1)
        {
            Object1.SetActive(true);
            Object2.SetActive(false);
        }
        else
        {
            Object1.SetActive(false);
            Object2.SetActive(true);
        }
    }
}
