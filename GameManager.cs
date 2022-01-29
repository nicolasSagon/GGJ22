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
    private GameObject ground;

    public void Start()
    {
        ground = GameObject.Find(movingGameObjectName);
        playerOne.setPlayerListener(new firstPlayerListener());
        playerTwo.setPlayerListener(new secondPlayerListener());
    }

    public void pushPlayer(string playerName)
    {
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

    private class firstPlayerListener : PlayerListener
    {
        public void sendAttack()
        {
            Debug.Log("First player attack");
        }
    }

    private class secondPlayerListener : PlayerListener
    {
        public void sendAttack()
        {
            Debug.Log("Second player attack");
        }
    }
}