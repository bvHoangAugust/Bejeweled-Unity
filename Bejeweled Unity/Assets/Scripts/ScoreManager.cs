using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {
    private Board board;
    public Text scoreText;
    public int score;

    public Image scoreBar;
	// Use this for initialization
	void Start () {
        board = FindObjectOfType<Board>();
	}
	
	// Update is called once per frame
	void Update () {
        scoreText.text = score.ToString();
	}
    public void InscreaseScore(int scoreToInscrease)
    {
        score += scoreToInscrease;
        if(board != null && scoreBar != null)
        {
            int length = board.scoreGoals.Length;
            scoreBar.fillAmount = (float)score / (float)board.scoreGoals[length - 1];
        }
    }
}
