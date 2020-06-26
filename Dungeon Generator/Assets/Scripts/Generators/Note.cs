using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Note: MonoBehaviour
{
    public enum Tone
    {
        C = 0,
        C_SHARP = 1,
        D = 2,
        D_SHARP = 3,
        E = 4,
        F = 5,
        F_SHARP = 6,
        G = 7,
        G_SHARP = 8, 
        A = 9,
        A_SHARP = 10,
        B = 11
    }
    public Tone tone;
    public AudioClip clip;
}