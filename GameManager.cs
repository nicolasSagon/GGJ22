using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Keyboard;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public GameObject feedbackDoubleAttack;

    public int worldPositionMax = 10;
    public string movingGameObjectName = "MOVING_PLAYERS";
    public Player playerOne;
    public Player playerTwo;

    private int currentWorlPosition = 0;
    private GameObject ground;

    private ScoreManager _scoreManager;

    private int scorePlayer1 = 0;
    private int scorePlayer2 = 0;
    
    public void Start()
    {
        ground = GameObject.Find(movingGameObjectName);
        playerOne.setPlayerAction(firstPlayerAttackFunc);
        playerTwo.setPlayerAction(secondPlayerAttackFunc);

        var playerScoreItems = GameObject.FindGameObjectsWithTag("PlayerFeedback");
        
        var player1ScoreItems = playerScoreItems
            .Where(item => item.name.Contains("Player1"))
            .OrderBy(item => item.name)
            .ToList();
        var player2ScoreItems = playerScoreItems
            .Where(item => item.name.Contains("Player2"))
            .OrderBy(item => item.name)
            .ToList();

        _scoreManager = new ScoreManager(player1ScoreItems, player2ScoreItems);
        _scoreManager.init();
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
                StartCoroutine(feedback(feedbackDoubleAttack));
                break;
            case Player.State.PREPARE:
                defensePlayer.doubleAttack();
                StartCoroutine(feedback(feedbackDoubleAttack));
                break;
            default:
                moveGround(isMovingRight);
                StartCoroutine(feedback(defensePlayer.feedbackHit));
                updateScore(isMovingRight);
                break;
        }
        _scoreManager.displayScore(scorePlayer1, scorePlayer2);
    }
    public IEnumerator feedback(GameObject o) {
        o.SetActive(true);
        yield return new WaitForSeconds(1);
        o.SetActive(false);
    }

    private void updateScore(Boolean isMovingRight)
    {
        if (isMovingRight)
        {
            scorePlayer1++;
            scorePlayer2 = 0;
        }
        else
        {
            scorePlayer2++;
            scorePlayer1 = 0;
        }
    }
}