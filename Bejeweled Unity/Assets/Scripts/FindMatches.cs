using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindMatches : MonoBehaviour
{

    private Board board;
    public List<GameObject> currentMatches = new List<GameObject>();

    // Use this for initialization
    void Start()
    {
        board = FindObjectOfType<Board>();
    }

    public void FindAllMaches()
    {
        StartCoroutine(FindAllMatchesCo());
    }
    private IEnumerator FindAllMatchesCo()
    {
        yield return new WaitForSeconds(.2f);
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                GameObject currentCake = board.allCakes[i, j];
                if (currentCake != null)
                {
                    if (i > 0 && i < board.width - 1)
                    {
                        GameObject leftCake = board.allCakes[i - 1, j];
                        GameObject rightCake = board.allCakes[i + 1, j];
                        if (leftCake != null && rightCake != null)
                        {
                            if (leftCake.tag == currentCake.tag && rightCake.tag == currentCake.tag)
                            {
                                if (!currentMatches.Contains(leftCake))
                                {
                                    currentMatches.Add(leftCake);
                                }
                                leftCake.GetComponent<Cake>().isMatched = true;
                                if (!currentMatches.Contains(rightCake))
                                {
                                    currentMatches.Add(rightCake);
                                }
                                rightCake.GetComponent<Cake>().isMatched = true;
                                if (!currentMatches.Contains(currentCake))
                                {
                                    currentMatches.Add(currentCake);
                                }
                                currentCake.GetComponent<Cake>().isMatched = true;
                            }
                        }
                    }
                    if (j > 0 && j < board.height - 1)
                    {
                        GameObject upCake = board.allCakes[i, j + 1];
                        GameObject downCake = board.allCakes[i, j - 1];
                        if (upCake != null && downCake != null)
                        {
                            if (upCake.tag == currentCake.tag && downCake.tag == currentCake.tag)
                            {
                                if (!currentMatches.Contains(upCake))
                                {
                                    currentMatches.Add(upCake);
                                }
                                upCake.GetComponent<Cake>().isMatched = true;
                                if (!currentMatches.Contains(downCake))
                                {
                                    currentMatches.Add(downCake);
                                }
                                downCake.GetComponent<Cake>().isMatched = true;
                                if (!currentMatches.Contains(currentCake))
                                {
                                    currentMatches.Add(currentCake);
                                }
                                currentCake.GetComponent<Cake>().isMatched = true;
                            }
                        }
                    }
                }
            }
        }
    }
List<GameObject> GetAdjacentPieces(int column, int row)
{
    List<GameObject> cakes = new List<GameObject>();
    for (int i = column - 1; i <= column + 1; i++)
    {
        for (int j = row - 1; j <= row + 1; j++)
        {
            if (i >= 0 && i < board.width && j >= 0 && j < board.height)
            {
                if (board.allCakes[i, j] != null)
                {
                    cakes.Add(board.allCakes[i, j]);
                    board.allCakes[column, i].GetComponent<Cake>().isMatched = true;
                }
            }
        }
    }
    return cakes;
}
List<GameObject> GetColumnPieces(int column)
{
    List<GameObject> cakes = new List<GameObject>();
    for (int i = 0; i < board.height; i++)
    {
        if (board.allCakes[column, i] != null)
        {
            cakes.Add(board.allCakes[column, i]);
            board.allCakes[column, i].GetComponent<Cake>().isMatched = true;
        }
    }
    return cakes;
}
List<GameObject> GetRowPieces(int row)
{
    List<GameObject> cakes = new List<GameObject>();
    for (int i = 0; i < board.width; i++)
    {
        if (board.allCakes[i, row] != null)
        {
            cakes.Add(board.allCakes[i, row]);
            board.allCakes[i, row].GetComponent<Cake>().isMatched = true;
        }
    }
    return cakes;
}
}
