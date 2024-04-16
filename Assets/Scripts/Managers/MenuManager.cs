using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
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
                HairController hairController = bodyColour1.GetComponent<HairController>();
                if (hairController != null)
                {
                    int randomHairIndex = Random.Range(0, hairController.GetHairCount());
                    Sprite front = hairController.GetHairFront(randomHairIndex);
                    Sprite back = hairController.GetHairBack(randomHairIndex);
                    bodyColour1.SetHair(front, back);
                    int coinToss = Random.Range(0, 2);
                    if (coinToss == 0)
                    {
                        bodyColour1.SetBeard(false);
                    }
                    else
                    {
                        bodyColour1.SetBeard(true);
                    }
                }
            }
            if(bodyColour2 != null)
            {
                bodyColour2.SetColour(GameController.Instance.GetCharacterBodyColour(1));
                bodyColour2.SetSkinColor(GameController.Instance.GetRandomSkinColor());
                bodyColour2.SetHairColor(GameController.Instance.GetRandomHairColor());
                HairController hairController = bodyColour1.GetComponent<HairController>();
                if (hairController != null)
                {
                    int randomHairIndex = Random.Range(0, hairController.GetHairCount());
                    Sprite front = hairController.GetHairFront(randomHairIndex);
                    Sprite back = hairController.GetHairBack(randomHairIndex);
                    bodyColour2.SetHair(front, back);
                    int coinToss = Random.Range(0, 2);
                    if (coinToss == 0)
                    {
                        bodyColour2.SetBeard(false);
                    }
                    else
                    {
                        bodyColour2.SetBeard(true);
                    }
                }
            }
            if(bodyColour3 != null)
            {
                bodyColour3.SetColour(GameController.Instance.GetCharacterBodyColour(2));
                bodyColour3.SetSkinColor(GameController.Instance.GetRandomSkinColor());
                bodyColour3.SetHairColor(GameController.Instance.GetRandomHairColor());
                HairController hairController = bodyColour3.GetComponent<HairController>();
                if (hairController != null)
                {
                    int randomHairIndex = Random.Range(0, hairController.GetHairCount());
                    Sprite front = hairController.GetHairFront(randomHairIndex);
                    Sprite back = hairController.GetHairBack(randomHairIndex);
                    bodyColour3.SetHair(front, back);
                    int coinToss = Random.Range(0, 2);
                    if (coinToss == 0)
                    {
                        bodyColour3.SetBeard(false);
                    }
                    else
                    {
                        bodyColour3.SetBeard(true);
                    }
                }
            }
            if(bodyColour4 != null)
            {
                bodyColour4.SetColour(GameController.Instance.GetCharacterBodyColour(3));
                bodyColour4.SetSkinColor(GameController.Instance.GetRandomSkinColor());
                bodyColour4.SetHairColor(GameController.Instance.GetRandomHairColor());
                HairController hairController = bodyColour4.GetComponent<HairController>();
                if (hairController != null)
                {
                    int randomHairIndex = Random.Range(0, hairController.GetHairCount());
                    Sprite front = hairController.GetHairFront(randomHairIndex);
                    Sprite back = hairController.GetHairBack(randomHairIndex);
                    bodyColour4.SetHair(front, back);
                    int coinToss = Random.Range(0, 2);
                    if (coinToss == 0)
                    {
                        bodyColour4.SetBeard(false);
                    }
                    else
                    {
                        bodyColour4.SetBeard(true);
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
