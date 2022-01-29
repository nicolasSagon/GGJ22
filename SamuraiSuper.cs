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

    private Action _callBackForResult;
    private PlayerData _firstPlayer;
    private PlayerData _secondPlayer;

    public int timmingRandomMin = 5;
    public int timmingRandomMax = 10;

    private float _frameCounter = 0;
    private Boolean isCountingFrame = false;

    public void initSamuraiSuper(Action callBackForResult, PlayerData firstPlayer, PlayerData secondPlayer)
    {
        _callBackForResult = callBackForResult;
        _firstPlayer = firstPlayer;
        _secondPlayer = secondPlayer;

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
        if (keyboard[currentPlayer.PlayerKey].wasPressedThisFrame && _isListeningKeyboard)
        {
            _isListeningKeyboard = false;
            if (_isAlowedToPushKey)
            {
                displayResult($"{currentPlayer.PlayerName} as win !");
                displayFrameCounter();
                isCountingFrame = false;
            }
            else
            {
                if (currentPlayer.Equals(_firstPlayer))
                {
                    displayResult($"{_secondPlayer.PlayerName} as win !");
                }
                else
                {
                    displayResult($"{_firstPlayer.PlayerName} as win !");
                }
                isCountingFrame = false;
            }
        }
    }

    private void displayFrameCounter()
    {
        frameCounter.SetActive(true);
        frameCounter.GetComponent<TextMeshProUGUI>().text = $"{_frameCounter:0.00}";
    }

    private void displayResult(string resultString)
    {
        resultText.SetActive(true);
        resultText.GetComponent<TextMeshProUGUI>().text = resultString;
    }

    IEnumerator startMiniGame(int timming)
    {
        yield return new WaitForSeconds(timming);
        isCountingFrame = true;
        goSymbol.SetActive(true);
        _isAlowedToPushKey = true;
    }

}