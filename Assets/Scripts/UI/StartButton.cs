using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Find the button in the scene
        // If game controller on OnClick event is missing then set to gamcontroller.instance

        Button btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(GameController.Instance.LoadCinematic);

    }
}
