using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinScreenManager : MonoBehaviour
{
    [SerializeField]
    GameObject button;
    // Start is called before the first frame update
    void Start()
    {
        Invoke(nameof(RevealButton), 5);
    }

    void RevealButton()
    {
        button.SetActive(true);
    }

    public void ReturnToMenu()
    {
        if(GameController.Instance != null)
        {
            GameController.Instance.ReturnToMenu();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
