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
    private int scoreToWin = 5;

    public string movingGameObjectName = "MOVING_PLAYERS";
    public Player playerOne;
    public Player playerTwo;

    private GameObject ground;

    private ScoreManager _scoreManager;

    private int consecHitPlayer1 = 0;
    private int consecHitPlayer2 = 0;
    private int hitsToWin = 3;
    private GameObject superPanel;

    public void Start()
    {
        ground = GameObject.Find(movingGameObjectName);
        superPanel = GameObject.Find("SuperPanel");
        
        superPanel.SetActive(false);
        
        playerOne.setPlayerActions(firstPlayerAttackFunc, startSuper);
        playerTwo.setPlayerActions(secondPlayerAttackFunc, startSuper);

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

    private void Update()
    {
        if (playerOne.GetScore() >= scoreToWin || consecHitPlayer1 >= hitsToWin){
            Debug.Log($"{playerOne.playerName} wins!");
            // TODO: win anim
        }
        else if (playerTwo.GetScore() >= scoreToWin || consecHitPlayer2 >= hitsToWin){
            Debug.Log($"{playerTwo.playerName} wins!");
            // TODO: win anim
        }
        // TODO : end game
    }

    private void firstPlayerAttackFunc()
    {
        handleAttack(playerOne, playerTwo, true);
    }

    private void secondPlayerAttackFunc()
    {
        handleAttack(playerTwo, playerOne, false);
    }

    private void startSuper()
    {
        superPanel.SetActive(true);
        playerOne.enabled = false;
        playerTwo.enabled = false;
        var samuraiSuper = superPanel.GetComponent<SamuraiSuper>();
        samuraiSuper.enabled = true;
        samuraiSuper.initSamuraiSuper(superFinished, playerOne.attackKey, playerTwo.attackKey);
    }

    private void superFinished()
    {
        
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
                updateHits(isMovingRight);
                attackingPlayer.scoreUp();
                defensePlayer.scoreDown();
                break;
        }
        _scoreManager.displayHits(consecHitPlayer1, consecHitPlayer2);
    }
    public IEnumerator feedback(GameObject o) {
        o.SetActive(true);
        yield return new WaitForSeconds(1);
        o.SetActive(false);
    }

    private void updateHits(Boolean isMovingRight)
    {
        if (isMovingRight)
        {
            consecHitPlayer1++;
            consecHitPlayer2 = 0;
        }
        else
        {
            consecHitPlayer2++;
            consecHitPlayer1 = 0;
        }
    }
}