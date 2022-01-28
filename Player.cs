using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private State state = State.NEUTRAL;

    enum State {
        ATTACK,
        BLOCK,
        NEUTRAL
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    State GetState(){
        return this.state;
    }

    void setAttacking(){
        this.state = State.ATTACK;
    }
    void setBlocking(){
        this.state = State.BLOCK;
    }
    void setNeutral(){
        this.state = State.NEUTRAL;
    }
}
