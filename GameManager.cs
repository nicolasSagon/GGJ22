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
    public GameObject superParticle1, superParticle2;
    private int scoreToWin = 5;
    public string movingGameObjectName = "MOVING_PLAYERS";
    public Player playerOne;
    public Player playerTwo;

    public bool isDebug = false;

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
    private InputManager _inputManager;
    private SelectControllerHUD selectControllerHUD;
    private GameObject menuRetry;
    private GameObject playerHUD;

    public void Start()
    {
        ground = GameObject.Find(movingGameObjectName);
        superPanel = GameObject.Find("SuperPanel");
        _inputManager = FindObjectOfType<InputManager>();
        menuRetry = GameObject.Find("MenuRetry");
        playerHUD = GameObject.Find("PlayersHUD");

        superPanel.SetActive(false);
        menuRetry.SetActive(false);
        
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
        
        _inputManager.setInputManagerCallback((numberController) =>
        {
            if(numberController == 1) {
                selectControllerHUD.setFirstPlayerController(_inputManager.FirstController);
            } else {
                selectControllerHUD.setSecondPlayerController(_inputManager.SecondController);
                StartCoroutine(startGame());
            }
            
        });
        superParticle1.SetActive(false);
        superParticle2.SetActive(false);

        selectControllerHUD = FindObjectOfType<SelectControllerHUD>();

        if (!isDebug) {
            feedbackDoubleAttack.SetActive(false);
        }
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
        

        if (playerOne.GetScore() < (1 - scoreToWin + dangerZone) && !playerOne.isSuperReady()){
            playerOne.setSuperReady();
            if (!superParticle1.activeSelf && !superUsedPlayerOne){
                superParticle1.SetActive(true);
            }
        }
        if (playerTwo.GetScore() < (1 - scoreToWin + dangerZone) && !playerTwo.isSuperReady()){
            playerTwo.setSuperReady();
            if (!superParticle2.activeSelf && !superUsedPlayerTwo){
                superParticle2.SetActive(true);
            }
        }

        if (playerOne.GetScore() >= (1 - scoreToWin + dangerZone) && playerOne.isSuperReady()){
            playerOne.unsetSuperReady();
            if (superParticle1.activeSelf){
                superParticle1.SetActive(false);
            }
        }
        if (playerTwo.GetScore() >= (1 - scoreToWin + dangerZone) && playerTwo.isSuperReady()){
            playerTwo.unsetSuperReady();
            if (superParticle2.activeSelf){
                superParticle2.SetActive(false);
            }
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
        var samuraiSuper = superPanel.GetComponent<SamuraiSuper>();
        samuraiSuper.enabled = true;
        samuraiSuper.initSamuraiSuper(
            handleSuper,
            new PlayerData(playerOne.playerName, playerOne.CustomInputDevice),
            new PlayerData(playerTwo.playerName, playerTwo.CustomInputDevice)
        );
        playerOne.enabled = false;
        playerTwo.enabled = false;

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
                if (isMovingRight){
                    if (consecHitPlayer1 > 0) {
                        consecHitPlayer1--;
                    }
                } else {
                    if (consecHitPlayer2 > 0) {
                        consecHitPlayer2--;
                    }
                }
                _scoreManager.displayHits(consecHitPlayer1, consecHitPlayer2);
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
                defensePlayer.takeHit();
                StartCoroutine(feedback(defensePlayer.feedbackHit));
                updateHits(isMovingRight);
                attackingPlayer.scoreUp();
                defensePlayer.scoreDown();
                break;
        }
        checkVictory();
    }
    private void handleBlock(Player attackingPlayer, Player defensePlayer){

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
        superPanel.SetActive(false);
        playerOne.enabled = true;
        playerTwo.enabled = true;
        checkVictory();
    }
    private void checkVictory(){
        if (playerOne.GetScore() >= scoreToWin ){
            playerOne.win();
            playerTwo.fall();
            StartCoroutine(gameFinished(playerOne.name));
        }
        else if (consecHitPlayer1 >= hitsToWin) {
            playerOne.win();
            playerTwo.die();
            StartCoroutine(gameFinished(playerOne.name));
        }
        else if (playerTwo.GetScore() >= scoreToWin){
            playerTwo.win();
            playerOne.fall();
            StartCoroutine(gameFinished(playerTwo.name));
        }
        else if (consecHitPlayer2 >= hitsToWin){
            playerTwo.win();
            playerOne.die();
            StartCoroutine(gameFinished(playerTwo.name));
        }
    }
    public IEnumerator feedback(GameObject o) {
        if(isDebug) {
            o.SetActive(true);
            yield return new WaitForSeconds(1);
            o.SetActive(false);
        }
    }

    private IEnumerator gameFinished(string playerName) {
        selectControllerHUD.displayFightText($"{playerName} Win");
        yield return new WaitForSeconds(4);
        playerHUD.SetActive(false);
        menuRetry.SetActive(true);

        
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
        _scoreManager.displayHits(consecHitPlayer1, consecHitPlayer2);
    }

    private IEnumerator startGame()
    {
        selectControllerHUD.displayFightText("3");
        yield return new WaitForSeconds(1);
        selectControllerHUD.displayFightText("2");
        yield return new WaitForSeconds(1);
        selectControllerHUD.displayFightText("1");
        yield return new WaitForSeconds(1);
        selectControllerHUD.displayFightText("FIGHT");
        playerOne.setCustomInputDevice(_inputManager.FirstController);
        playerTwo.setCustomInputDevice(_inputManager.SecondController);
        yield return new WaitForSeconds(0.5f);
        selectControllerHUD.hideFightText("FIGHT");
    }
}