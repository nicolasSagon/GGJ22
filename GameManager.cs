using System;
using System.Collections.Generic;
using Core.Keyboard;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public int worldPositionMax = 10;
    private int currentWorlPosition = 0;

    private string playerOne = "Player 1";
    private string playerTwo = "Player 2";
    private GameObject ground;

    private List<PlayerKey> playerKeys;

    public void Start()
    {
        playerKeys = new()
        {
            new PlayerKey(Key.RightArrow, playerOne),
            new PlayerKey(Key.LeftArrow, playerTwo)
        };

        ground = GameObject.Find("Ground");
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
        if (playerName == playerOne)
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
        var value = isForward ? 1 : -1;
        var localPosition = ground.transform.localPosition;
        localPosition = new Vector3(
            localPosition.x,
            localPosition.y,
            localPosition.z + value
        );
        ground.transform.localPosition = localPosition;
    }

    private void updateDisplay()
    {
        Debug.Log("Current score = " + currentWorlPosition);
    }
}