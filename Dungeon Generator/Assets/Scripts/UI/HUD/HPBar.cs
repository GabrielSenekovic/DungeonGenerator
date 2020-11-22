using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    public HealthModel HP = null;
    [SerializeField] Image HealthBar = null;
    public Text HealthText;
    public float currentFill;
    public float BarChangeSpeed;

    public float buffer;

    private void FixedUpdate()
    {
        UpdateText();
        UpdateHealthBarLength();
        UpdateHealthBarColor();
    }
    void UpdateText()
    {
        if (HealthText != null)
        {
            HealthText.text = Mathf.RoundToInt(HP.currentHealth) + "/" + Mathf.RoundToInt(HP.maxHealth);
        }
    }
    private void UpdateHealthBarLength()
    {
        currentFill = HP.GetHealthPercentage();
        if (currentFill != HealthBar.fillAmount)
        {
            HealthBar.fillAmount = Mathf.Lerp(HealthBar.fillAmount, currentFill, BarChangeSpeed);
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
        //From green to yellow
        //HealthBar.color = new Color(1 * (1 - HP.GetHealthPercentage()), 1, 0, 1);
        //From yellow to green
        //HealthBar.color = new Color(1, HP.GetHealthPercentage(), 0, 1);
        //From green to blue
        //HealthBar.color = new Color(0, HP.GetHealthPercentage(), 1 * (1 - HP.GetHealthPercentage()), 1);
        //Dark blue to black
       // HealthBar.color = new Color(0, 0, HP.GetHealthPercentage(), 1);
        
        //Yellow to cyan
        //HealthBar.color = new Color(HP.GetHealthPercentage(), 1, 1 * (1 - HP.GetHealthPercentage()), 1);
        //Yellow to pink
        //HealthBar.color = new Color(1, HP.GetHealthPercentage(), 1 * (1 - HP.GetHealthPercentage()), 1);

        //From blue to cyan
       // HealthBar.color = new Color(0, 1 * (1 - HP.GetHealthPercentage()), 1, 1);
        //From red to yellow
        //HealthBar.color = new Color(1, 1 * (1 - HP.GetHealthPercentage()), 0, 1);
        //From dark blue to magenta
        //HealthBar.color = new Color(1 * (1 - HP.GetHealthPercentage()), 0, 1, 1);
        //From green to cyan
        //HealthBar.color = new Color(0, 1, 1 * (1 - HP.GetHealthPercentage()), 1);
        //From red to magenta
        //HealthBar.color = new Color(1, 0, 1 * (1 - HP.GetHealthPercentage()), 1);
    }
}