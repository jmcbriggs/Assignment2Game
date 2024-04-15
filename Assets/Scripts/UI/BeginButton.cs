using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeginButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        Button btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        if (GameController.Instance != null)
        {
            btn.onClick.AddListener(GameController.Instance.BeginGame);
        }
    }
}
