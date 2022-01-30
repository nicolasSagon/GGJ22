using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    public static Music Instance;
    private AudioSource themeSource, superSource;
    public AudioClip theme, super;
    public float musicVol = 0.5f;

    void Awake()
    {
        
        themeSource = gameObject.AddComponent<AudioSource>();
        themeSource.spatialize = false;
        themeSource.volume = musicVol;
        themeSource.loop = true;
        themeSource.clip = theme;

        superSource = gameObject.AddComponent<AudioSource>();
        superSource.spatialize = false;
        superSource.volume = musicVol;
        superSource.loop = true;
        superSource.clip = super;

        if (Instance != null)
        {
            Debug.LogError("Multiple instances of MusicController!");
        }

        themeSource.Play();
        Instance = this;

    }

    public void playTheme(){
        if (superSource.isPlaying) {
            superSource.Stop();
        }
        themeSource.UnPause();
    }
    public void playSuper(){
        if (themeSource.isPlaying) {
            themeSource.Pause();
        }
        superSource.Play();
    }
}
