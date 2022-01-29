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
    public float blockTime = 1.0f;
    public float stunTime = 2.0f;

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
        if (gamepadEnabled) {
            try {
                if (Gamepad.all.Count > 1) {
                    gamepad = Gamepad.all[gamepadNumber];
                }
                else if (Gamepad.all.Count == 1){
                    gamepad = Gamepad.all[0];
                }
                else {
                    gamepad = null;
                }
            }
            catch (ArgumentOutOfRangeException) {
                gamepad = null;
            }
        }
        else{
            gamepad = null;
        }
        neutral();
    }

    private void debugWithPlayerName(string logString)
    {
        //Debug.Log($"{playerName} => {logString}");
    }

    // Update is called once per frame
    void Update()
    {
        bool attackPressed = false;
        bool blockPressed = false;

        if (keyboard == null)
        {
            Debug.Log("no keyboard");
            return;
        }

        if (gamepad != null){
            if (gamepad.aButton.wasPressedThisFrame){
                attackPressed = true;
            }
            else if (gamepad.bButton.wasPressedThisFrame){
                blockPressed = true;
            }
        } else {
            if (keyboard[attackKey].wasPressedThisFrame){
                attackPressed = true;
            }
            else if (keyboard[blockKey].wasPressedThisFrame){
                blockPressed = true;
            }
        }
        

        if (state == State.STUN){
            if (attackPressed || blockPressed){
                debugWithPlayerName("You're stunned...");
            }
        }
        else if (state == State.NEUTRAL){
            if (attackPressed)
            {
                feedbackStateNeutral.SetActive(false);
                prepare();
            }
            else if (blockPressed)
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
        else if (state == State.PREPARE){
            if (blockPressed) {
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
