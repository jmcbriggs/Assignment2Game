using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitGameButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(QuitGame);
    }


    void QuitGame()
    {
        if(GameController.Instance != null)
        {
            GameController.Instance.QuitGame();
        }
        else
        {
            Application.Quit();
        }
    }

}