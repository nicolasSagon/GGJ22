using System;
using System.Collections.Generic;
using Core.Keyboard;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public int worldPositionMax = 10;
    public string movingGameObjectName = "MOVING_PLAYERS";
    public Player playerOne;
    public Player playerTwo;
    
    private int currentWorlPosition = 0;

    private string playerOnes = "Player 1";
    private string playerTwos = "Player 2";
    private GameObject ground;

    private List<PlayerKey> playerKeys;

    public void Start()
    {
        playerKeys = new()
        {
            new PlayerKey(Key.RightArrow, playerOnes),
            new PlayerKey(Key.LeftArrow, playerTwos)
        };

        ground = GameObject.Find(movingGameObjectName);
    }

    private void FixedUpdate()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return; // No gamepad connected.
        }

        foreach (var playerKey in playerKeys)
        {
            if (keyboard[playerKey.keyCode].isPressed && !playerKey.isPressed)
            {
                playerKey.isPressed = true;
                pushPlayer(playerKey.playerName);
            }

            if (!keyboard[playerKey.keyCode].isPressed)
            {
                playerKey.isPressed = false;
            }
        }
    }

    public void pushPlayer(string playerName)
    {
        if (playerName == playerOnes)
        {
            currentWorlPosition++;
            moveGround(true);
        }
        else
        {
            currentWorlPosition--;
            moveGround(false);
        }

        updateDisplay();
    }

    private void moveGround(Boolean isForward)
    {
        var value = isForward ? 3 : -3;
        var localPosition = ground.transform.localPosition;
        localPosition = new Vector3(
            localPosition.x + value,
            localPosition.y,
            localPosition.z
        );
        ground.transform.localPosition = localPosition;
    }

    private void updateDisplay()
    {
        Debug.Log("Current score = " + currentWorlPosition);
    }
}