using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = System.Random;

public class SamuraiSuper: MonoBehaviour
{

    public GameObject goSymbol;
    
    private Boolean _isListeningKeyboard = false;
    private Boolean _isAlowedToPushKey = false;

    private Action _callBackForResult;
    private Key _firstPlayerAttackingKey;
    private Key _secondPlayerAttackingKey;

    public int timmingRandomMin = 5;
    public int timmingRandomMax = 10;

    public void initSamuraiSuper(Action callBackForResult, Key firstPlayerKey, Key secondPlayerKey)
    {
        _callBackForResult = callBackForResult;
        _firstPlayerAttackingKey = firstPlayerKey;
        _secondPlayerAttackingKey = secondPlayerKey;

        var timming = new Random().Next(timmingRandomMin, timmingRandomMax);
        _isListeningKeyboard = true;
        StartCoroutine(startMiniGame(timming));   
    }

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return;
        }

        if (keyboard[_firstPlayerAttackingKey].wasPressedThisFrame && _isListeningKeyboard)
        {
            _isListeningKeyboard = false;
            if (_isAlowedToPushKey)
            {
                Debug.Log("Player one as win");
            }
            else
            {
                Debug.Log("Player one as lost");
            }
        }
        
        if (keyboard[_secondPlayerAttackingKey].wasPressedThisFrame && _isListeningKeyboard)
        {
            _isListeningKeyboard = false;
            if (_isAlowedToPushKey)
            {
                Debug.Log("Player two as win");
            }
            else
            {
                Debug.Log("Player two as lost");
            }
        }
        
    }

    IEnumerator startMiniGame(int timming)
    {
        yield return new WaitForSeconds(timming);
        goSymbol.SetActive(true);
        _isAlowedToPushKey = true;
    }

}