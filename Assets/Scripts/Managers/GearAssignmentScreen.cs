using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearAssignmentScreen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        if (GameController.Instance != null)
        {
            foreach(GameObject character in GameController.Instance.GetSelectedCharacters())
            {
                character.transform.position = new Vector3(1000, 1000, 1000);
            }
        }
    }

    public void StartBattle()
    {
        if(GameController.Instance != null)
        {
            GameController.Instance.EnterBattle();
        }
    }

   

    // Update is called once per frame
    void Update()
    {
        
    }
}
