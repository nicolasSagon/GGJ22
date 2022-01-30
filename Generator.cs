using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public GameObject speakerPrefab;

    void Awake(){
        var speaker = FindObjectOfType<Music>();
        if (speaker == null){
            Instantiate(speakerPrefab, transform);
        }
    }
    void Start(){
        DontDestroyOnLoad(gameObject);
    }

}
