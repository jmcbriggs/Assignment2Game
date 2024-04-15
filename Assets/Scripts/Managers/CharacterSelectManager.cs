using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        if(GameController.Instance != null)
        {
            GameController.Instance.CharacterSelectStart();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
