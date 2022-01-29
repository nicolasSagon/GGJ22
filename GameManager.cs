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
    private int dangerZone = 2;
    public int superDamage = 4;
    private bool superUsedPlayerOne = false;
    private bool superUsedPlayerTwo = false;
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
        Debug.Log($"Player1 Super: {playerOne.isSuperReady()} / Player2 Super: {playerTwo.isSuperReady()}");

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

        if (playerOne.GetScore() < (1 - scoreToWin + dangerZone) && !playerOne.isSuperReady()){
            playerOne.setSuperReady();
        }
        if (playerTwo.GetScore() < (1 - scoreToWin + dangerZone) && !playerTwo.isSuperReady()){
            playerTwo.setSuperReady();
        }

        if (playerOne.GetScore() >= (1 - scoreToWin + dangerZone) && playerOne.isSuperReady()){
            playerOne.unsetSuperReady();
        }
        if (playerTwo.GetScore() >= (1 - scoreToWin + dangerZone) && playerTwo.isSuperReady()){
            playerTwo.unsetSuperReady();
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

    private void startSuper(string playerName)
    {

        if (playerName == playerOne.playerName){
            if (superUsedPlayerOne){
                Debug.Log($"{playerName} super already used!");
                return;
            }
        } else {
            if (superUsedPlayerTwo){
                Debug.Log($"{playerName} super already used!");
                return;
            }
        }

        superPanel.SetActive(true);
        playerOne.enabled = false;
        playerTwo.enabled = false;
        var samuraiSuper = superPanel.GetComponent<SamuraiSuper>();
        samuraiSuper.enabled = true;
        samuraiSuper.initSamuraiSuper(
            handleSuper,
            new PlayerData(playerOne.playerName, playerOne.attackKey),
            new PlayerData(playerTwo.playerName, playerTwo.attackKey)
            );

        if (playerName == playerOne.playerName){
            superUsedPlayerOne = true;
        }
        else {
            superUsedPlayerTwo = true;
        }
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
    private void handleSuper(PlayerData winnerData){
        Player winner, loser;
        bool isMovingRight;
        if (winnerData.PlayerName == playerOne.playerName){
            winner = playerOne;
            loser = playerTwo;
            isMovingRight = true;
        }
        else {
            winner = playerTwo;
            loser = playerOne;
            isMovingRight = false;
        }
        for (var i=0; i < superDamage; i++){
            moveGround(isMovingRight);
            winner.scoreUp();
            loser.scoreDown();
        }
        updateHits(isMovingRight);
        updateHits(isMovingRight);
        superPanel.SetActive(false);;
        playerOne.enabled = true;
        playerTwo.enabled = true;
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