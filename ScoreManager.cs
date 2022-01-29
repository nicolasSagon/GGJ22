using System.Collections.Generic;
using UnityEngine;

public class ScoreManager
{

    public List<GameObject> player1ScoreItems;
    public List<GameObject> player2ScoreItems;

    public ScoreManager(List<GameObject> player1ScoreItems, List<GameObject> player2ScoreItems)
    {
        this.player1ScoreItems = player1ScoreItems;
        this.player2ScoreItems = player2ScoreItems;
    }

    public void init()
    {
        foreach (var player1ScoreItem in player1ScoreItems)
        {
            player1ScoreItem.SetActive(false);
        }
        
        foreach (var player2ScoreItem in player2ScoreItems)
        {
            player2ScoreItem.SetActive(false);
        }
    }

    public void displayScore(int score1, int score2)
    {
        handleScore(score1, player1ScoreItems);
        handleScore(score2, player2ScoreItems);
    }

    public void handleScore(int score, List<GameObject> gameObjects)
    {
        if (score == 0)
        {
            gameObjects.ForEach(item => item.SetActive(false));
        }
        else if (score == 1)
        {
            gameObjects[0].SetActive(true);
            gameObjects[1].SetActive(false);
            gameObjects[2].SetActive(false);
        }
        else if (score == 2)
        {
            gameObjects[0].SetActive(true);
            gameObjects[1].SetActive(true);
            gameObjects[2].SetActive(false);
        }
        else
        {
            gameObjects[0].SetActive(true);
            gameObjects[1].SetActive(true);
            gameObjects[2].SetActive(true);
        }
    }
}