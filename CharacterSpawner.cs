using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    public GameObject eagle, snake, croco, fenec;
    private enum Character {
        EAGLE,
        SNAKE,
        CROCO,
        FENEC
    }
    // Start is called before the first frame update
    void Start()
    {
        var character = Random.Range(((int)Character.EAGLE), ((int)Character.FENEC));
        switch (character){
            case ((int)Character.EAGLE):
                spawn(eagle);
                break;
            case ((int)Character.SNAKE):
                spawn(eagle);
                break;
            case ((int)Character.CROCO):
                spawn(croco);
                break;
            case ((int)Character.FENEC):
                spawn(fenec);
                break;
            default:
                spawn(eagle);
                break;
        }
    }

    void spawn(GameObject o){
        Instantiate(o, transform.position, transform.rotation);
    }
}
