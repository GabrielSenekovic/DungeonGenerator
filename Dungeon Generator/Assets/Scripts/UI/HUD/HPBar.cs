using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    public HealthModel HP = null;
    [SerializeField] Image HealthBar = null;
    Text HealthText;
    float currentFill;
    public float BarChangeSpeed;

    public Color first;
    public Color last;
    Gradient gradient;

    private void Start()
    {
        float startHue = 0, s = 0, v = 0;
        float endHue = 0;
        Color.RGBToHSV(first, out startHue, out s, out v);
        Color.RGBToHSV(last, out endHue, out s, out v);

        gradient = new Gradient();
        int colors = 6;
        GradientColorKey[] colorKey = new GradientColorKey[colors + 1];
        float range = 1 - (endHue - startHue);
        float colorInterval = range / colors;
        float timeInterval = 1.0f / colors;
        float exponent = 2;

        for (int i = 0; i <= 6; i++)
        {
            s = Math.SemiCircle(i, colors, 2);
            v = Math.SemiSuperEllipse(i, timeInterval, colors, exponent);
            colorKey[i].color = Color.HSVToRGB(Math.Mod(startHue - colorInterval * i, 1), s, v);
            colorKey[i].time = timeInterval * i;
        }

        GradientAlphaKey[] alphaKey = new GradientAlphaKey[2];
        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.0f;
        alphaKey[1].alpha = 1.0f;
        alphaKey[1].time = 1.0f;

        gradient.SetKeys(colorKey, alphaKey);
    }

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
        HealthBar.color = gradient.Evaluate(1 - HealthBar.fillAmount);
    }
}