using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using System.Linq;

public class Player : MonoBehaviour
{
    public GameObject feedbackStateNeutral, feedbackStatePrepare, feedbackStateAttack, feedbackStateBlock, feedbackStateStunned;
    public GameObject feedbackCancel, feedbackHit;
    private Animator anim;
    private Sfx sound;
    public AnimationClip hitAnim;
    public bool isDebug = false;

    public enum State {
        PREPARE,
        CANCEL,
        ATTACK,
        HIT,
        BLOCK,
        NEUTRAL,
        STUN
    }

    public string playerName;
    private State state;
    private int score;
    [CanBeNull] private Action _playerAttackFunc;
    [CanBeNull] private Action<string> _playerStartSuper;
    public CustomInputDevice CustomInputDevice;

    public float prepareTime = 0.5f;
    public float attackTime = 0.5f;
    public float blockTime = 1.0f;
    public float stunTime = 2.0f;
    public float recoveryTimeCancel = 0.1f, recoveryTimeAttack = 0.1f, recoveryTimeBlock = 0.1f;

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

    public void setCustomInputDevice(CustomInputDevice customInputDevice)
    {
        CustomInputDevice = customInputDevice;
    }

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        sound = FindObjectOfType<Sfx>();
        neutral();

        if (!isDebug) {
            feedbackStateNeutral.SetActive(false);
            feedbackStatePrepare.SetActive(false);
            feedbackStateAttack.SetActive(false);
            feedbackStateBlock.SetActive(false);
            feedbackStateStunned.SetActive(false);
            feedbackCancel.SetActive(false);
            feedbackHit.SetActive(false);
        }
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

        if (CustomInputDevice == null)
        {
            return;
        }

        if (CustomInputDevice.isAttackPressed())
        {
            attackPressed = true;
        } else if (CustomInputDevice.isBlockPressed())
        {
            blockPressed = true;
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
                anim.SetBool("neutral", false);
                prepare();
            }
            else if (blockPressed)
            {
                feedbackStateNeutral.SetActive(false);
                anim.SetBool("neutral", false);
                block();
            }

            if (CustomInputDevice.isSuperPressed())
            {
                if (isSuperReady()){
                    _playerStartSuper?.Invoke(playerName);
                }
            }
        }
        else if (state == State.PREPARE){
            if (blockPressed) {
                setCancelling();
                feedbackStatePrepare.SetActive(false);
                StartCoroutine(feedback(feedbackCancel));
                sound.playCancel();
                StartCoroutine(recovering(recoveryTimeCancel));
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
        if(isDebug){
            feedbackStatePrepare.SetActive(true);
        }
        anim.SetBool("punch", true);
    }
    void setCancelling(){
        state = State.CANCEL;
        anim.SetBool("punch", false);
    }
    void setAttacking(){
        state = State.ATTACK;
        if(isDebug){
            feedbackStateAttack.SetActive(true);
        }
    }
    void setBlocking(){
        state = State.BLOCK;
        if(isDebug){
           feedbackStateBlock.SetActive(true);
        }
        anim.SetBool("block", true);
    }
    void setNeutral(){
        state = State.NEUTRAL;
        if(isDebug){
           feedbackStateNeutral.SetActive(true);
        }
        anim.SetBool("neutral", true);
    }
    void setStunned(){
        state = State.STUN;
        if(isDebug){
           feedbackStateStunned.SetActive(true);
        }
        anim.SetBool("stun", true);
    }
    void setHit(){
        state = State.HIT;
        anim.SetBool("hit", true);
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
            anim.SetBool("punch", false);
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
    public void die(){
        foreach (var p in anim.parameters.Where(item => item.type == AnimatorControllerParameterType.Bool)){
            anim.SetBool(p.name, false);
        }
        CustomInputDevice = null;
        anim.SetBool("death", true);
        sound.playDeath();
    }
    public void fall(){
        foreach (var p in anim.parameters.Where(item => item.type == AnimatorControllerParameterType.Bool)){
            anim.SetBool(p.name, false);
        }
        CustomInputDevice = null;
        anim.SetBool("fall", true);
        sound.playDeath();
    }
    public void win(){
        foreach (var p in anim.parameters.Where(item => item.type == AnimatorControllerParameterType.Bool)){
            anim.SetBool(p.name, false);
        }
        CustomInputDevice = null;
        anim.SetBool("victory", true);
        StartCoroutine(winning());
    }
    public void takeHit(){
        foreach (var p in anim.parameters.Where(item => item.type == AnimatorControllerParameterType.Bool)){
            anim.SetBool(p.name, false);
        }
        setHit();
        StartCoroutine(hit());
    }

    IEnumerator preparing()
    {
        yield return new WaitForSeconds(prepareTime);
        feedbackStatePrepare.SetActive(false);
        attack();
    }
    IEnumerator attacking()
    {
        sound.playPunch();
        yield return new WaitForSeconds(attackTime);
        _playerAttackFunc?.Invoke();
        feedbackStateAttack.SetActive(false);
        anim.SetBool("punch", false);
        StartCoroutine(recovering(recoveryTimeAttack));
    }
    IEnumerator blocking()
    {
        yield return new WaitForSeconds(blockTime);
        feedbackStateBlock.SetActive(false);
        anim.SetBool("block", false);
        StartCoroutine(recovering(recoveryTimeBlock));
    }
    IEnumerator stunned()
    {
        yield return new WaitForSeconds(stunTime);
        feedbackStateStunned.SetActive(false);
        anim.SetBool("stun", false);
        neutral(force: true);
    }
    IEnumerator hit(){
        sound.playDamage();
        yield return new WaitForSeconds(recoveryTimeAttack);
        anim.SetBool("hit", false);
        neutral();
    }
    IEnumerator winning(){
        yield return new WaitForSeconds(1);
        sound.playVictory();
    }
    IEnumerator recovering(float recoveryTime){
        yield return new WaitForSeconds(recoveryTime);
        neutral();
    }
    IEnumerator feedback(GameObject o) {
        if(isDebug) {
            o.SetActive(true);
            yield return new WaitForSeconds(1);
            o.SetActive(false);
        }
    }

}
