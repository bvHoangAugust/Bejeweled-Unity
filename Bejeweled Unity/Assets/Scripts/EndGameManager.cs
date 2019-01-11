using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum GameType
{
    Move
}

[System.Serializable]
public class EndGameRequirement
{
    public GameType gameType;
    public int counterValue;
}
public class EndGameManager : MonoBehaviour {

    public GameObject moveLabel;
    public Text counter;
    public GameObject winPanel;
    public GameObject losePanel;
    public EndGameRequirement requirement;
    public int currentCounterValue;
    private Board board;
    //private ScoreManager scoreManager;
	// Use this for initialization
	void Start () {
        //scoreManager = FindObjectOfType<ScoreManager>();
        board = FindObjectOfType<Board>();
        SetUpGame();
	}

    void SetUpGame()
    {
        currentCounterValue = requirement.counterValue;
        //if(requirement.gameType == GameType.Move)
        //{
        //    moveLabel.SetActive(true);
        //}
        //else
        //{
        //    moveLabel.SetActive(false);
        //}
        counter.text = currentCounterValue.ToString();
    }


    public void DecreaseCounterValue(int currentMatches)
    {
        int length = board.scoreGoals.Length;
        if(currentMatches == 3)
        {
            currentCounterValue--;
            counter.text = currentCounterValue.ToString();
        }
        else if(currentMatches >= 5)
        {
            currentCounterValue++;
            counter.text = currentCounterValue.ToString();
        }            
    }

    //void Win()
    //{
    //    int length = board.scoreGoals.Length;
    //    if (currentCounterValue >= 0 && scoreManager.score >= board.scoreGoals[length - 1])
    //    {
    //        board.currentState = GameState.win;
    //        if (winPanel != null)
    //        {
    //            winPanel.SetActive(true);
    //        }
    //    }       
    //}
    void Lose()
    {
        //int length = board.scoreGoals.Length;
        if(currentCounterValue <= 0 /*&& scoreManager.score < board.scoreGoals[length - 1]*/)
        {
            currentCounterValue = 0;
            counter.text = currentCounterValue.ToString();
            board.currentState = GameState.lose;
            if (losePanel != null)
            {
                losePanel.SetActive(true);
            }
        }      
    }
    void ButtonTryAgain()
    {
        SceneManager.LoadScene(1);
    }
	// Update is called once per frame
	void Update () {
        //Win();
        Lose();
	}
}
