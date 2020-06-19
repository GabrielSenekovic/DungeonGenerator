using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHPBar : MonoBehaviour
{
  public HealthModel HP = null;
    [SerializeField] SpriteRenderer HealthBar = null;
    public float currentFill;
    public float BarChangeSpeed;

    public float buffer;

    private void FixedUpdate()
    {
        UpdateHealthBarLength();
        UpdateHealthBarColor();
    }
    private void UpdateHealthBarLength()
    {
        currentFill = HP.GetHealthPercentage();
        if (currentFill != HealthBar.size.x)
        {
            HealthBar.size = new Vector2(Mathf.Lerp(HealthBar.size.x, currentFill, BarChangeSpeed), HealthBar.size.y);
        }
    }
    private void UpdateHealthBarColor()
    {
        if(HP.GetHealthPercentage() > 0.75f)
        {
            //Green to yellow
            HealthBar.color = new Color(1 - Mathf.Pow(HP.GetHealthPercentage(), 16), 1, 0, 1);
        }
        else if(HP.GetHealthPercentage() > 0.50f)
        {
            //Yellow to red
            buffer = Mathf.Pow(HP.GetHealthPercentage(HP.maxHealth * 0.25f), 6);
            HealthBar.color = new Color(1, buffer, 0, 1);
        }
        else if(HP.GetHealthPercentage() > 0.25f)
        {
            //red to blue
            buffer = Mathf.Pow(HP.GetHealthPercentage(HP.maxHealth * 0.5f), 6);
            HealthBar.color = new Color(buffer, 0, 1 - buffer, 1);
        }
        else
        {
            //blue to black
            buffer = Mathf.Pow(HP.GetHealthPercentage(HP.maxHealth* 0.75f), 6);
            HealthBar.color = new Color(0, 0, buffer, 1);
        }
    }
}
