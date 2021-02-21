using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
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

    AudioSource music_source;
    AudioSource[] SFX_source = new AudioSource[100];


    private void Start() 
    {
        music_source = gameObject.AddComponent<AudioSource>();
        StartCoroutine(playMusic());
    }
    
    public IEnumerator playMusic()
    {
        Debug.Log(music.clip.length);
        music.clip = intro;
        music.Play();
        yield return new WaitForSeconds(music.clip.length);
        music.Stop();
        music.loop = true;
        music.clip = mainTheme;
        music.Play();
    }

    void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    void Start()
    {
        Play("BGM");
    }

    public void Play (string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("sound: " + name + "not found");
            return;
        }
        s.source.Play();
    }
    public void PlayMusic(string name)
    {
        Music m = Array.Find(music, music => music.name == name);
        if (m == null)
        {
            Debug.LogWarning("sound: " + name + "not found");
            return;
        }
        musicPlayer.intro = m.intro;
        musicPlayer.mainTheme = m.theme;
        StartCoroutine(musicPlayer.playMusic());
    }
}
