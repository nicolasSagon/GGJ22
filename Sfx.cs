using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sfx : MonoBehaviour
{
    public static Sfx Instance;
    private AudioSource sfx;
    public AudioClip punch, cancel, damage, death, block, victory, supergo, superhit, superready;
    public float musicVol = 0.5f;

    // Start is called before the first frame update
    void Awake()
    {
        sfx = gameObject.AddComponent<AudioSource>();
        sfx.spatialize = false;
        sfx.volume = musicVol;

        if (Instance != null)
        {
            Debug.LogError("Multiple instances of Sfx!");
        }
        Instance = this;
    }

    public void playPunch(){
        makeSound(punch);
    }
    public void playCancel(){
        makeSound(cancel);
    }
    public void playDamage(){
        makeSound(damage);
    }
    public void playDeath(){
        makeSound(death);
    }
    public void playBlock(){
        makeSound(block);
    }
    public void playVictory(){
        makeSound(victory);
    }
    public void playSuperGo(){
        makeSound(supergo);
    }
    public void playSuperHit(){
        makeSound(superhit);
    }
    public void playSuperReady(){
        makeSound(superready);
    }
    private void makeSound(AudioClip originalClip)
    {
        sfx.PlayOneShot(originalClip);
    }
}