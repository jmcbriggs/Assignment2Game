using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenMenu : MonoBehaviour
{
    GameObject _menu;
    // Start is called before the first frame update
    void Start()
    {
        if(GameController.Instance != null)
        {
            _menu = GameController.Instance.GetMenu();
        }
        else
        {
            _menu = GameObject.Find("Menu");
            if(_menu == null)
            {
                Debug.Log("Menu not found");
            }
            else
            {
                _menu.SetActive(false);
            }

        }

    }

    public void OpenMenuPanel()
    {
        if (GameController.Instance != null && _menu == null)
        {
            _menu = GameController.Instance.GetMenu();
        }
        if (_menu)
        {
            if(_menu.activeSelf)
            {
                _menu.SetActive(false);
            }
            else
            {
                _menu.SetActive(true);
            }
        }
        else
        {
            Debug.Log("Menu not found");
        }

    }
}
