using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndCinematic : MonoBehaviour
{
    private void OnEnable()
    {
        if(GameController.Instance != null)
        {
            GameController.Instance.LoadCharacterSelect();
        }
        else
        {
            Debug.LogError("GameController is null");
        }
    }
}
