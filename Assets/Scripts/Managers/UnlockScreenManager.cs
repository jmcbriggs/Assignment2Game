using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class UnlockScreenManager : MonoBehaviour
{
    [SerializeField]
    GameObject CharacterPoint;
    [SerializeField]
    TextMeshProUGUI UnlockText;
    // Start is called before the first frame update
    void Start()
    {
        if (GameController.Instance != null)
        {
            if (GameController.Instance.GetNewCharacter() != null)
            {
                GameObject character = Instantiate(GameController.Instance.GetNewCharacter(), CharacterPoint.transform.position, Quaternion.identity);
                character.transform.SetParent(CharacterPoint.transform);
                character.transform.localScale = new Vector3(1, 1, 1);
                character.GetComponent<SortingGroup>().sortingOrder = 3;
                BodyColour bodyColour = character.GetComponent<BodyColour>();
                if (bodyColour != null)
                {
                    bodyColour.SetColour(GameController.Instance.GetRandomBodyColor());
                    bodyColour.SetSkinColor(GameController.Instance.GetRandomSkinColor());
                    bodyColour.SetHairColor(GameController.Instance.GetRandomHairColor());
                }
                SetToUILayer(character.transform);
                SetToMaskable(character.transform);
                UnlockText.text =  character.GetComponent<PlayerCharacter>()._characterClass;
            }
            GameController.Instance.GetComponent<MusicManager>().PlaySting(MusicManager.Sting.Win);
        }
        Invoke("ReturnToMenu", 5);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SetToUILayer(Transform transform)
    {
        transform.gameObject.layer = 5;
        foreach (Transform child in transform)
        {
            SetToUILayer(child);
        }
    }

    void SetToMaskable(Transform transform)
    {
        foreach (Transform child in transform)
        {
            SetToMaskable(child);
        }
        if (transform.GetComponent<SpriteRenderer>() != null)
        {
            transform.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }
    }

    void ReturnToMenu()
    {
        if(GameController.Instance != null)
        {
            GameController.Instance.ReturnToMenu(false);
        }
        else
        {
            Debug.LogError("GameController is null");
        }
    }
}
