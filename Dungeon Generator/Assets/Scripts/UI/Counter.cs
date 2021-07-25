using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Counter : MonoBehaviour
{
    class Digit
    {
        public Image sprite;
        public int value;
        List<GraphemeDatabase.Grapheme> digits;

        public Digit(List<GraphemeDatabase.Grapheme> digits_in, Image sprite_in)
        {
            value = 0;
            sprite = sprite_in;
            digits = digits_in;
            sprite.sprite = digits[value].sprite;
        }
        public void Update()
        {
            sprite.sprite = digits[value].sprite;
        }
        public void Increment()
        {
            value++;
            value %= 10;
            Update();
        }
        public void Decrement()
        {
            value--;
            if (value < 0)
            {
                value = 9;
            }
            Update();
        }
        public void Add(int value_in)
        {
            value += value_in;
            value %= 10;
            Update();
        }
    }
    List<Digit> digits = new List<Digit>();
    public int value;

    public int amountOfDigits;

    public GraphemeDatabase graphemeDatabase; //Remove later

    private void Start() 
    {
        for (int i = 0; i < amountOfDigits; i++)
        {
            GameObject temp = new GameObject("Digit: " + (i+1).ToString());
            temp.transform.parent = transform;
            temp.AddComponent<Image>();
            digits.Add(new Digit(graphemeDatabase.fonts[0].numbers, temp.GetComponent<Image>()));
            temp.transform.localPosition = new Vector2(i * digits[0].sprite.sprite.texture.width, 0);
            temp.GetComponent<Image>().SetNativeSize();
            temp.transform.localScale = new Vector3(1,1,1);
        }
    }
    void Increment()
    {
        OnIncrement(digits.Count - 1);
        value++;
    }
    void OnIncrement(int i)
    {
        if (digits[i].value == 9 && i != 0)
        {
            OnIncrement(i - 1);
        }
        digits[i].Increment();
    }
    public void Add(int value_in)
    {
        if (digits[digits.Count - 1].value + value_in > 9 && digits.Count - 1 != 0)
        {
            OnAdd(value_in, 10, value_in + digits[digits.Count - 1].value, 99, digits.Count - 2);
            digits[digits.Count - 1].Add(value_in % 10);
        }
        else
        {
            digits[digits.Count - 1].Add(value_in);
        }
        value += value_in;
    }
    void OnAdd(int value, int modifier, int totalValue, int limit, int i)
    {
        if (digits[i].value * modifier + totalValue > limit
            &&
            i != 0)
            {
                OnAdd(value, modifier * 10, totalValue + digits[i].value * modifier, limit * 10 + 9, i - 1);
            }
        digits[i].Add((totalValue / modifier) % 10);
    }
    void Reset()
    {
        for (int i = 0; i < digits.Count; i++)
        {
            digits[i].value = 0; digits[i].Update();
        }
        value = 0;
    }
}
