using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Number : MonoBehaviour
{
    [SerializeField] Sprite[] m_numbers;
    [SerializeField] SpriteRenderer[] m_numbersToDisplay;

    public void OnDisplayNumber(int value)
    {
        if(value > 9)
        {
            m_numbersToDisplay[1].sprite = m_numbers[(int)value % 10];
            m_numbersToDisplay[0].sprite = m_numbers[(int)value / 10];
            transform.position = new Vector2(transform.position.x - 4, transform.position.y);
        }
        else
        {
            m_numbersToDisplay[0].sprite = m_numbers[value];
        }
    }
}
