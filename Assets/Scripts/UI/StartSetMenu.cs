using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSetMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(GameController.Instance != null)
        {
            GameController.Instance.SetMenu(gameObject);
            gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("GameController not found");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
