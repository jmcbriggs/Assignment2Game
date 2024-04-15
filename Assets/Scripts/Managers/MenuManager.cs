using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    BodyColour bodyColour1;
    [SerializeField]
    BodyColour bodyColour2;
    [SerializeField]
    BodyColour bodyColour3;
    [SerializeField]
    BodyColour bodyColour4;
    // Start is called before the first frame update
    void Start()
    {
        if(GameController.Instance != null)
        {
            if(bodyColour1 != null)
            {
                bodyColour1.SetColour(GameController.Instance.GetCharacterBodyColour(0));
                bodyColour1.SetSkinColor(GameController.Instance.GetRandomSkinColor());
                bodyColour1.SetHairColor(GameController.Instance.GetRandomHairColor());
            }
            if(bodyColour2 != null)
            {
                bodyColour2.SetColour(GameController.Instance.GetCharacterBodyColour(1));
                bodyColour2.SetSkinColor(GameController.Instance.GetRandomSkinColor());
                bodyColour2.SetHairColor(GameController.Instance.GetRandomHairColor());
            }
            if(bodyColour3 != null)
            {
                bodyColour3.SetColour(GameController.Instance.GetCharacterBodyColour(2));
                bodyColour3.SetSkinColor(GameController.Instance.GetRandomSkinColor());
                bodyColour3.SetHairColor(GameController.Instance.GetRandomHairColor());
            }
            if(bodyColour4 != null)
            {
                bodyColour4.SetColour(GameController.Instance.GetCharacterBodyColour(3));
                bodyColour4.SetSkinColor(GameController.Instance.GetRandomSkinColor());
                bodyColour4.SetHairColor(GameController.Instance.GetRandomHairColor());
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
