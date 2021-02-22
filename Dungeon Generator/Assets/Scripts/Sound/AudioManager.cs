using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public sealed class AudioManager : MonoBehaviour
{
    static AudioManager instance;

    public static AudioManager Instance 
    {
        get { return instance; }
    }
    public static AudioManager GetInstance() 
    {
        return instance;
    }
    [System.Serializable]public class Sound
    {
        public string name;

        public AudioClip clip;
    }
    [System.Serializable]public class Music
    {
        public string name;

        public AudioClip intro;
        public AudioClip theme;
    }
    public Sound[] sounds;
    public Music[] music;

    static AudioSource music_source;

    static float music_volume = 1;
    static float SFX_volume = 1;
    static float global_volume = 1; //from 0 to 1
    static AudioSource[] SFX_source = new AudioSource[100];

    static int nextSFX_source = 0;


    private void Start() 
    {
        instance = this;
        music_source = gameObject.AddComponent<AudioSource>();
        for(int i = 0; i < 100; i++)
        {
            SFX_source[i] = gameObject.AddComponent<AudioSource>();
            SFX_source[i].volume = SFX_volume * global_volume;
        }
        music_source.volume = music_volume * global_volume;
        StartCoroutine(PlayMusic(music[0]));
    }

    public static void PlaySFX (string name)
    {
        Sound s = Array.Find(GetInstance().sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("sound: " + name + "not found");
            return;
        }
        PlaySFX(s);
    }
    public static void PlaySFX(Sound sound)
    {
        SFX_source[nextSFX_source].clip = sound.clip;
        SFX_source[nextSFX_source].Play();
        Debug.Log("Playing SFX: " + sound.name);
        nextSFX_source++;
        nextSFX_source = nextSFX_source >= 100? 0: nextSFX_source;
    }
    public static void PlayMusic(string name)
    {
        Music m = Array.Find(GetInstance().music, music => music.name == name);
        if (m == null)
        {
            Debug.LogWarning("music: " + name + "not found");
            return;
        }
        AudioManager temp = AudioManager.GetInstance();
        GetInstance().StartCoroutine(PlayMusic(m));
    }
    public static IEnumerator PlayMusic(Music music)
    {
        Debug.Log(music_source.clip.length);
        music_source.clip = music.intro;
        music_source.Play();
        yield return new WaitForSeconds(music_source.clip.length);
        music_source.Stop();
        music_source.loop = true;
        music_source.clip = music.theme;
        music_source.Play();
    }
}
