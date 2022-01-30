using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplayer : MonoBehaviour
{
    public Sprite p1score0, p1score1, p1score2, p1score3, p1score4, p2score1, p2score2, p2score3, p2score4, p2score0;
    public GameObject p1lifebar, p2lifebar;
    private static Image p1sprite, p2sprite;
    private Player playerOne, playerTwo;
    // Start is called before the first frame update
    void Start()
    {
        playerOne = GameObject.FindWithTag("P1").GetComponent<Player>();
        playerTwo = GameObject.FindWithTag("P2").GetComponent<Player>();

        p1sprite = p1lifebar.AddComponent<Image>();
        p1sprite.sprite = p1score0;
        p2sprite = p2lifebar.AddComponent<Image>();
        p2sprite.sprite = p2score0;
    }

    // Update is called once per frame
    void Update()
    {
        switch (playerOne.GetScore()){
            case 0:
                p1sprite.sprite = p1score0;
                break;
            case 1:
                p1sprite.sprite = p1score1;
                break;
            case 2:
                p1sprite.sprite = p1score2;
                break;
            case 3:
                p1sprite.sprite = p1score3;
                break;
            case 4:
                p1sprite.sprite = p1score4;
                break;
            default:
                break;
        }
        switch (playerTwo.GetScore()){
            case 0:
                p2sprite.sprite = p2score0;
                break;
            case 1:
                p2sprite.sprite = p2score1;
                break;
            case 2:
                p2sprite.sprite = p2score2;
                break;
            case 3:
                p2sprite.sprite = p2score3;
                break;
            case 4:
                p2sprite.sprite = p2score4;
                break;
            default:
                break;
        }
    }
}
