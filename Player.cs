using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    enum State {
        PREPARE,
        ATTACK,
        BLOCK,
        NEUTRAL
    }
    private Keyboard keyboard = Keyboard.current;
    private State state = State.NEUTRAL;
    [CanBeNull] private PlayerListener _playerListener;

    public float prepareTime = 0.5f;
    public float attackTime = 0.5f;
    public float blockTime = 0.5f;

    public Key attackKey = Key.Q;
    public Key blockKey = Key.W;

    public void setPlayerListener(PlayerListener playerListener)
    {
        _playerListener = playerListener;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (keyboard == null)
        {
            Debug.Log("no keyboard");
        }
        if (state == State.NEUTRAL){
            if (keyboard[attackKey].wasPressedThisFrame)
            {
                prepare();
            }

            if (keyboard[blockKey].wasPressedThisFrame)
            {
                block();
            }
        }
        if (state == State.PREPARE){
            if (keyboard[blockKey].wasPressedThisFrame) {
                Debug.Log("Cancel!");
                neutral();
            }
        }
    }

    State GetState(){
        return state;
    }

    void setPreparing(){
        state = State.PREPARE;
    }
    void setAttacking(){
        state = State.ATTACK;
        _playerListener?.sendAttack();
    }
    void setBlocking(){
        state = State.BLOCK;
    }
    void setNeutral(){
        state = State.NEUTRAL;
    }

    void prepare(){
        setPreparing();
        Debug.Log("Preparing...");
        StartCoroutine(preparing());
    }
    void attack(){
        if (state == State.PREPARE) {
            setAttacking();
            Debug.Log("Attack!");
            StartCoroutine(attacking());
        }
    }
    void block(){
        setBlocking();
        Debug.Log("Block!");
        StartCoroutine(blocking());
    }
    void neutral(){
        setNeutral();
        Debug.Log("Neutral!");
    }

    IEnumerator preparing()
    {
        yield return new WaitForSeconds(prepareTime);
        attack();
    }
    IEnumerator attacking()
    {
        yield return new WaitForSeconds(attackTime);
        neutral();
    }
    IEnumerator blocking()
    {
        yield return new WaitForSeconds(blockTime);
        neutral();
    }

}
