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
        var character = Random.Range(((int)Character.EAGLE), ((int)Character.FENEC)+1);
        switch (character){
            case ((int)Character.EAGLE):
                spawn(eagle, "Eagle");
                break;
            case ((int)Character.SNAKE):
                spawn(snake, "Cobra");
                break;
            case ((int)Character.CROCO):
                spawn(croco, "Croco");
                break;
            case ((int)Character.FENEC):
                spawn(fenec, "Fenec");
                break;
            default:
                spawn(eagle, "Eagle");
                break;
        }
    }

    void spawn(GameObject o, string name){
        var fighter = Instantiate(o, transform.position, transform.rotation);
        fighter.name = name;
        fighter.transform.parent = transform;
    }
}
