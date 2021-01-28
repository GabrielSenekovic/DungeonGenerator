using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSounds : MonoBehaviour
{
    public AudioClip clickSound;
    public void PlayClickSound()
    {
        GetComponent<AudioSource>().clip = clickSound;
        GetComponent<AudioSource>().Play();
    }
}
