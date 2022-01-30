using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = System.Random;

public class SamuraiSuper: MonoBehaviour
{

    public GameObject goSymbol;
    public GameObject resultText;
    public GameObject frameCounter;
    
    private Boolean _isListeningKeyboard = false;
    private Boolean _isAlowedToPushKey = false;

    private Action<PlayerData> _callBackForResult;
    private PlayerData _firstPlayer;
    private PlayerData _secondPlayer;

    public int timmingRandomMin = 5;
    public int timmingRandomMax = 10;

    private float _frameCounter = 0;
    private Boolean isCountingFrame = false;
    private Sfx sound;

    public void initSamuraiSuper(Action<PlayerData> callBackForResult, PlayerData firstPlayer, PlayerData secondPlayer)
    {
        sound = FindObjectOfType<Sfx>();
        _callBackForResult = callBackForResult;
        _firstPlayer = firstPlayer;
        _secondPlayer = secondPlayer;
        goSymbol.SetActive(false);
        isCountingFrame = false;
        frameCounter.SetActive(false);
        _frameCounter = 0;
        resultText.SetActive(false);


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

        handlePlayerInputs(keyboard, _firstPlayer);
        handlePlayerInputs(keyboard, _secondPlayer);
    }

    private void FixedUpdate()
    {
        if (isCountingFrame)
        {
            _frameCounter += Time.fixedDeltaTime;
        }
    }

    private void handlePlayerInputs(Keyboard keyboard, PlayerData currentPlayer)
    {
        if (currentPlayer.InputDevice.isAttackPressed() && _isListeningKeyboard)
        {
            _isListeningKeyboard = false;
            if (_isAlowedToPushKey)
            {
                displayResult($"{currentPlayer.PlayerName} as win !", currentPlayer);
                displayFrameCounter();
                isCountingFrame = false;
            }
            else
            {
                if (currentPlayer.Equals(_firstPlayer))
                {
                    displayResult($"{_secondPlayer.PlayerName} as win !", _secondPlayer);
                }
                else
                {
                    displayResult($"{_firstPlayer.PlayerName} as win !", _firstPlayer);
                }
                isCountingFrame = false;
            }
            sound.playSuperHit();
        }
    }

    private void displayFrameCounter()
    {
        frameCounter.SetActive(true);
        frameCounter.GetComponent<TextMeshProUGUI>().text = $"{_frameCounter:0.00}";
    }

    private void displayResult(string resultString, PlayerData winnerData)
    {
        resultText.SetActive(true);
        resultText.GetComponent<TextMeshProUGUI>().text = resultString;
        StartCoroutine(waitDisplay(winnerData));
    }

    IEnumerator startMiniGame(int timming)
    {
        yield return new WaitForSeconds(timming);
        isCountingFrame = true;
        goSymbol.SetActive(true);
        sound.playSuperGo();
        _isAlowedToPushKey = true;
    }
    IEnumerator waitDisplay(PlayerData winnerData){
        yield return new WaitForSeconds(2);
        _callBackForResult?.Invoke(winnerData);
    }

}