using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    GameObject greenBar;
    BoxCollider2D boxCollider;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }
    public void UpdateHealthBar(int maxHealth, int currentHealth)
    {
        //Shrinks down the green bar and moves it so the left edge never moves
        if(currentHealth < 0)
        {
            currentHealth = 0;
        }
        float percentage = (float)currentHealth / (float)maxHealth;
        if(float.TryParse(percentage.ToString(), out float result) == false)
        {
            Debug.LogError("Health bar percentage is not a number");
            Debug.LogError("Max Health: " + maxHealth);
            Debug.LogError("Current Health: " + currentHealth);
        }
        greenBar.transform.localScale = new Vector3(percentage, greenBar.transform.localScale.y, greenBar.transform.localScale.z);
        greenBar.transform.localPosition = new Vector3(0-((1 - percentage) / 2), greenBar.transform.localPosition.y, greenBar.transform.localPosition.z);

    }
}
