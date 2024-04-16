using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyColour : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer bodyRenderer;
    [SerializeField]
    SpriteRenderer headRenderer;
    [SerializeField]
    SpriteRenderer leftArmRenderer;
    [SerializeField]
    SpriteRenderer rightArmRenderer;
    [SerializeField]
    SpriteRenderer hairRenderer;
    [SerializeField]
    SpriteRenderer hairRendererBack;
    [SerializeField]
    SpriteRenderer beardRenderer;
    // Start is called before the first frame update
    void Start()
    {
    }    

    public void SetColour(Color colour)
    {
        if(bodyRenderer != null)
        {
            bodyRenderer.color = colour;
        }
        else
        {
            Debug.LogWarning("Body renderer not set");
        }
    }

    public void SetSkinColor(Color color)
    {
        if(headRenderer != null)
        {
            headRenderer.color = color;
        }
        else
        {
            Debug.LogWarning("Head renderer not set");
        }
        if(leftArmRenderer != null)
        {
            leftArmRenderer.color = color;
        }
        else
        {
           Debug.LogWarning("Left arm renderer not set");
        }
        if(rightArmRenderer != null)
        {
            rightArmRenderer.color = color;
        }
        else
        {
            Debug.LogWarning("Right arm renderer not set");
        }
    }
    public void SetHairColor(Color color)
    {
        if(hairRenderer != null)
        {
            hairRenderer.color = color;
        }
        else
        {
            Debug.LogWarning("Hair renderer not set");
        }
        if(hairRendererBack != null)
        {
            hairRendererBack.color = color;
        }
        if(beardRenderer != null)
        {
            beardRenderer.color = color;
        }
    }

    public void SetHair(Sprite front, Sprite back)
    {
        if(hairRenderer != null)
        {
            if(front != null)
            {
                hairRenderer.sprite = front;
            }
            else
            {
                hairRenderer.sprite = null;
            }

        }
        if(hairRendererBack != null)
        {
            if(back != null)
            {
                hairRendererBack.sprite = back;
            }
            else
            {
                hairRendererBack.sprite = null;
            }
        }
    }

    public void SetBeard(bool active)
    {
        if(beardRenderer != null)
        {
            beardRenderer.gameObject.SetActive(active);
        }
    }
}
