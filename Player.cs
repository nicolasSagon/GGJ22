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
    private Animator anim;
    private Sfx sound;
    public ParticleSystem hitParticle, blockParticle, stunParticle;
    public AnimationClip hitAnim;
    public bool isDebug = false;

    public enum State {
        PREPARE,
        CANCEL,
        ATTACK,
        HIT,
        BLOCK,
        NEUTRAL,
        RECOVER,
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
    private bool isCurrentlyStunned = false, isCurrentlySuper = false;
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
                anim.SetBool("neutral", false);
                Debug.Log("Prepare est appel??");
                prepare();
            }
            else if (blockPressed)
            {
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
                sound.playCancel();
                StartCoroutine(recovering(recoveryTimeCancel));
            }
        }
    }

    public State GetState(){
        return state;
    }
    public int GetScore(){
        Debug.Log($"{playerName} score is {score}");
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
        ++score;
    }
    public void scoreDown(){
        --score;
    }
    public void setCurrentlySuper(){
        isCurrentlySuper = true;
    }
    public void unsetCurrentlySuper(){
        isCurrentlySuper = false;
    }
    void setPreparing(){
        state = State.PREPARE;
        anim.SetBool("punch", true);
    }
    void setCancelling(){
        state = State.CANCEL;
        anim.SetBool("punch", false);
    }
    void setAttacking(){
        state = State.ATTACK;
    }
    void setBlocking(){
        state = State.BLOCK;
        anim.SetBool("block", true);
    }
    void setNeutral(){
        state = State.NEUTRAL;
        anim.SetBool("neutral", true);
    }
    void setStunned(){
        state = State.STUN;
        anim.SetBool("stun", true);
    }
    void setHit(){
        state = State.HIT;
        anim.SetBool("hit", true);
    }
    void setRecovering(){
        state = State.RECOVER;
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
        stopLastAttack();
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
        foreach (var p in anim.parameters.Where(item => item.type == AnimatorControllerParameterType.Bool)){
            anim.SetBool(p.name, false);
        }
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
    public void stopLastAttack(){
        if (lastAttackingCoroutine != null) {
            StopCoroutine(lastAttackingCoroutine);
            anim.SetBool("punch", false);
        }
    }

    IEnumerator preparing()
    {
        yield return new WaitForSeconds(prepareTime);
        attack();
    }
    IEnumerator attacking()
    {
        sound.playPunch();
        yield return new WaitForSeconds(attackTime);
        if (isCurrentlySuper){
            anim.SetBool("punch", false);
            neutral();
            yield break;
        }
        _playerAttackFunc?.Invoke();
        anim.SetBool("punch", false);
        if (isCurrentlyStunned){
            yield break;
        }
        StartCoroutine(recovering(recoveryTimeAttack));
    }
    IEnumerator blocking()
    {
        blockParticle.Play();
        yield return new WaitForSeconds(blockTime);
        blockParticle.Stop();
        anim.SetBool("block", false);
        StartCoroutine(recovering(recoveryTimeBlock));
    }
    IEnumerator stunned()
    {
        isCurrentlyStunned = true;
        stunParticle.Play();
        Debug.Log($"Je suis dans l'??tat {state}");
        yield return new WaitForSeconds(stunTime);
        stunParticle.Stop();
        anim.SetBool("stun", false);
        Debug.Log($"Je suis dans l'??tat {state}");
        neutral(force: true);
        isCurrentlyStunned = false;
        Debug.Log($"Je suis dans l'??tat {state}");
    }
    IEnumerator hit(){
        sound.playDamage();
        hitParticle.Play();
        yield return new WaitForSeconds(recoveryTimeAttack);
        hitParticle.Stop();
        anim.SetBool("hit", false);
        neutral();
    }
    IEnumerator winning(){
        yield return new WaitForSeconds(1);
        sound.playVictory();
    }
    IEnumerator recovering(float recoveryTime){
        setRecovering();
        yield return new WaitForSeconds(recoveryTime);
        neutral();
    }

}
