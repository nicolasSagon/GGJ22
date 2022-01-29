using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public enum State {
        PREPARE,
        ATTACK,
        BLOCK,
        NEUTRAL,
        STUN
    }
    private Keyboard keyboard = Keyboard.current;
    private State state = State.NEUTRAL;
    [CanBeNull] private Action _playerAttackFunc; 

    public float prepareTime = 0.5f;
    public float attackTime = 0.5f;
    public float blockTime = 0.5f;
    public float stunTime = 1.0f;

    public Key attackKey = Key.Q;
    public Key blockKey = Key.W;
    // public Animation anim; // TODO: uncomment when animation is ready

    public void setPlayerAction(Action playerAttackFunc)
    {
        _playerAttackFunc = playerAttackFunc;
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
        if (state == State.STUN){
            if (keyboard.anyKey.wasPressedThisFrame){
                Debug.Log("You're stunned...");
            }
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

    public State GetState(){
        return state;
    }

    void setPreparing(){
        state = State.PREPARE;
    }
    void setAttacking(){
        state = State.ATTACK;
    }
    void setBlocking(){
        state = State.BLOCK;
    }
    void setNeutral(){
        state = State.NEUTRAL;
    }
    void setStunned(){
        state = State.STUN;
    }

    void prepare(){
        // anim.Play("prepare"); // TODO: uncomment when animation is ready
        setPreparing();
        Debug.Log("Preparing...");
        StartCoroutine(preparing());
    }
    void attack(){
        if (state == State.PREPARE) {
            // anim.Play("attack"); // TODO: uncomment when animation is ready
            setAttacking();
            Debug.Log("Attack!");
            StartCoroutine(attacking());
        }
    }
    void block(){
        // anim.Play("block"); // TODO: uncomment when animation is ready
        setBlocking();
        Debug.Log("Block!");
        StartCoroutine(blocking());
    }
    void neutral(){
        // anim.Play("neutral"); // TODO: uncomment when animation is ready
        setNeutral();
        Debug.Log("Neutral!");
    }
    public void stun(){
        // anim.Play("stun");
        setStunned();
        Debug.Log("Stunned!");
        StartCoroutine(stunned());
    }

    IEnumerator preparing()
    {
        yield return new WaitForSeconds(prepareTime);
        attack();
    }
    IEnumerator attacking()
    {
        yield return new WaitForSeconds(attackTime);
        _playerAttackFunc?.Invoke();
        neutral();
    }
    IEnumerator blocking()
    {
        yield return new WaitForSeconds(blockTime);
        neutral();
    }
    IEnumerator stunned()
    {
        yield return new WaitForSeconds(stunTime);
        neutral();
    }

}
