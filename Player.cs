using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class Player : MonoBehaviour
{
    public GameObject feedbackStateNeutral, feedbackStatePrepare, feedbackStateAttack, feedbackStateBlock, feedbackStateStunned;
    public GameObject feedbackCancel, feedbackHit;

    public enum State {
        PREPARE,
        ATTACK,
        BLOCK,
        NEUTRAL,
        STUN
    }

    public string playerName;
    private Keyboard keyboard = Keyboard.current;
    private State state;
    private int score;
    [CanBeNull] private Action _playerAttackFunc;
    [CanBeNull] private Action<string> _playerStartSuper;

    public float prepareTime = 0.5f;
    public float attackTime = 0.5f;
    public float blockTime = 0.5f;
    public float stunTime = 1.0f;

    public Key attackKey = Key.Q;
    public Key blockKey = Key.W;
    public Key superKey = Key.E;

    private Boolean isSupperEnabled;

    private Coroutine lastAttackingCoroutine;
    private bool superReady = false;
    // public Animation anim; // TODO: uncomment when animation is ready

    public void setPlayerActions(Action playerAttackFunc, Action<string> playerStartSuper)
    {
        _playerAttackFunc = playerAttackFunc;
        _playerStartSuper = playerStartSuper;
    }

    // Start is called before the first frame update
    void Start()
    {
        gamepad = Gamepad.all[gamepadNumber]; 
        neutral();
    }

    private void debugWithPlayerName(string logString)
    {
        //Debug.Log($"{playerName} => {logString}");
    }

    // Update is called once per frame
    void Update()
    {
        if (keyboard == null)
        {
            Debug.Log("no keyboard");
            return;
        }
        if (state == State.STUN){
            if (keyboard.anyKey.wasPressedThisFrame || gamepad.aButton.wasPressedThisFrame || gamepad.bButton.wasPressedThisFrame){
                debugWithPlayerName("You're stunned...");
            }
        }
        if (state == State.NEUTRAL){
            if (keyboard[attackKey].wasPressedThisFrame || gamepad.aButton.wasPressedThisFrame)
            {
                feedbackStateNeutral.SetActive(false);
                prepare();
            }

            if (keyboard[blockKey].wasPressedThisFrame || gamepad.bButton.wasPressedThisFrame)
            {
                feedbackStateNeutral.SetActive(false);
                block();
            }

            if (keyboard[superKey].wasPressedThisFrame)
            {
                if (isSuperReady()){
                    _playerStartSuper?.Invoke(playerName);
                }
            }
        }
        if (state == State.PREPARE){
            if (keyboard[blockKey].wasPressedThisFrame || gamepad.bButton.wasPressedThisFrame) {
                debugWithPlayerName("Cancel!");
                StartCoroutine(feedback(feedbackCancel));
                feedbackStatePrepare.SetActive(false);
                neutral();
            }
        }
    }

    public State GetState(){
        return state;
    }
    public int GetScore(){
        return score;
    }
    public bool isSuperReady(){
        return superReady;
    }
    public void setSuperReady(){
        Debug.Log($"{playerName} super is ready!");
        superReady = true;
    }
    public void unsetSuperReady(){
        Debug.Log($"{playerName} super is unavailable!");
        superReady = false;
    }
    public void scoreUp(){
        score++;
    }
    public void scoreDown(){
        score--;
    }
    void setPreparing(){
        state = State.PREPARE;
        feedbackStatePrepare.SetActive(true);
    }
    void setAttacking(){
        state = State.ATTACK;
        feedbackStateAttack.SetActive(true);
    }
    void setBlocking(){
        state = State.BLOCK;
        feedbackStateBlock.SetActive(true);
    }
    void setNeutral(){
        state = State.NEUTRAL;
        feedbackStateNeutral.SetActive(true);
    }
    void setStunned(){
        state = State.STUN;
        feedbackStateStunned.SetActive(true);
    }

    void prepare(){
        // anim.Play("prepare"); // TODO: uncomment when animation is ready
        setPreparing();
        debugWithPlayerName("Preparing...");
        StartCoroutine(preparing());
    }
    void attack(){
        if (state == State.PREPARE) {
            // anim.Play("attack"); // TODO: uncomment when animation is ready
            setAttacking();
            debugWithPlayerName("Attack!");
            lastAttackingCoroutine = StartCoroutine(attacking());
        }
    }
    public void doubleAttack(){
        if (lastAttackingCoroutine != null) {
            StopCoroutine(lastAttackingCoroutine);
            feedbackStateAttack.SetActive(false);
        }
        // anim.Play("doubleattack"); // TODO: uncomment when animation is ready
        debugWithPlayerName("Double Attack!");
        neutral();
    }
    void block(){
        // anim.Play("block"); // TODO: uncomment when animation is ready
        setBlocking();
        debugWithPlayerName("Block!");
        StartCoroutine(blocking());
    }
    void neutral(bool force = false){
        if (state == State.STUN && !force){
            return;
        }
        // anim.Play("neutral"); // TODO: uncomment when animation is ready
        setNeutral();
        debugWithPlayerName("Neutral!");
    }
    public void stun(){
        // anim.Play("stun"); // TODO: uncomment when animation is ready
        setStunned();
        debugWithPlayerName("Stunned!");
        StartCoroutine(stunned());
    }

    IEnumerator preparing()
    {
        yield return new WaitForSeconds(prepareTime);
        feedbackStatePrepare.SetActive(false);
        attack();
    }
    IEnumerator attacking()
    {
        yield return new WaitForSeconds(attackTime);
        _playerAttackFunc?.Invoke();
        feedbackStateAttack.SetActive(false);
        neutral();
    }
    IEnumerator blocking()
    {
        yield return new WaitForSeconds(blockTime);
        feedbackStateBlock.SetActive(false);
        neutral();
    }
    IEnumerator stunned()
    {
        yield return new WaitForSeconds(stunTime);
        feedbackStateStunned.SetActive(false);
        neutral(force: true);
    }
    IEnumerator feedback(GameObject o) {
        o.SetActive(true);
        yield return new WaitForSeconds(1);
        o.SetActive(false);
    }

}
