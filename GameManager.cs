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
        playerOne.setPlayerAction(firstPlayerAttackFunc);
        playerTwo.setPlayerAction(secondPlayerAttackFunc);
    }

    public void pushPlayer(string playerName)
    {
    }

    private void moveGround(Boolean isMoveRight)
    {
        var value = isMoveRight ? 3 : -3;
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

    private void firstPlayerAttackFunc()
    {
        handleAttack(playerOne, playerTwo, true);
    }

    private void secondPlayerAttackFunc()
    {
        handleAttack(playerTwo, playerOne, false);
    }

    private void handleAttack(Player attackingPlayer, Player defensePlayer, Boolean isMovingRight)
    {
        var defensePlayerState = defensePlayer.GetState();
        switch (defensePlayerState)
        {
            case Player.State.BLOCK:
                // Don't move the players but stun player one
                attackingPlayer.stun();
                break;
            case Player.State.ATTACK:
                // Don't move the players, the attack is null
                defensePlayer.doubleAttack();
                break;
            case Player.State.PREPARE:
                defensePlayer.doubleAttack();
                break;
            default:
                moveGround(isMovingRight);
                break;
        }
    }
}