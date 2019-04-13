using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance = null;

    public SFX[] sfx;
    public BGM[] bgm;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        foreach (SFX sound in sfx)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = 0.25f;
        }

        foreach (BGM music in bgm)
        {
            music.source = gameObject.AddComponent<AudioSource>();
            music.source.clip = music.clip;
            music.source.volume = 0.25f;

        }
    }

    public void PlayMusic(string son)
    {
        BGM music = System.Array.Find(bgm, item => item.name == son);
        if (music == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        music.source.Play();
    }
    public void PlaySFX(string son)
    {
        SFX music = System.Array.Find(sfx, item => item.name == son);
        if (music == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        music.source.Play();
    }

    public void StopSong(string song)
    {
        BGM music = System.Array.Find(bgm, item => item.name == song);
        if (music == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        music.source.Stop();
    }



}
