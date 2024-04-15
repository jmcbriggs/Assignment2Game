using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CinematicSceneManager : MonoBehaviour
{
    [SerializeField]
    List<GameObject> sceneCommoners = new List<GameObject>();

    [SerializeField]
    List<string> helpTextOptions = new List<string>();
    [SerializeField]
    List<string> underAttackTextOptions = new List<string>();
    [SerializeField]
    List<string> bandOfHeroesTextOptions = new List<string>();
    [SerializeField]
    List<string> volunteerTextOptions = new List<string>();
    [SerializeField]
    TextMeshPro helpText;
    [SerializeField]
    TextMeshPro underAttackText;
    [SerializeField]
    TextMeshPro bandOfHeroesText;
    [SerializeField]
    TextMeshPro volunteerText;
    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject commoner in sceneCommoners)
        {
            if(GameController.Instance != null)
            {
                BodyColour bodyColour = commoner.GetComponent<BodyColour>();
                if(bodyColour != null)
                {
                    bodyColour.SetColour(GameController.Instance.GetRandomBodyColor());
                    bodyColour.SetSkinColor(GameController.Instance.GetRandomSkinColor());
                    bodyColour.SetHairColor(GameController.Instance.GetRandomHairColor());
                }
            }
        }
        if(GameController.Instance != null)
        {
            if (GameController.Instance.FirstRun())
            {
                helpText.text = helpTextOptions[0];
                underAttackText.text = underAttackTextOptions[0];
                bandOfHeroesText.text = bandOfHeroesTextOptions[0];
                volunteerText.text = volunteerTextOptions[0];
            }
            else
            {
                helpText.text = helpTextOptions[Random.Range(1, helpTextOptions.Count)];
                underAttackText.text = underAttackTextOptions[Random.Range(1, underAttackTextOptions.Count)];
                bandOfHeroesText.text = bandOfHeroesTextOptions[Random.Range(1, bandOfHeroesTextOptions.Count)];
                volunteerText.text = volunteerTextOptions[Random.Range(1, volunteerTextOptions.Count)];
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
