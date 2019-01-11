using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    wait,
    move,
    win,
    lose,
    pause
}


public class Board : MonoBehaviour {

    public GameState currentState = GameState.move;
    public int width;
    public int height;
    public int offSet;
    public GameObject tilePrefab;
    public GameObject[] cakes;
    public GameObject destroyEffect;
    private bool[,] blankSpace;
    public GameObject[,] allCakes;
    private FindMatches findMatches;
    public int basePieceValue = 10;
    private int streakValue = 1;
    private ScoreManager scoreManager;
    public int[] scoreGoals;
    void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
        findMatches = FindObjectOfType<FindMatches>();
        allCakes = new GameObject[width, height];
        SetUp();
    }
    void Update()
    {
        
    }
    private void SetUp()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                    Vector2 tempPos = new Vector2(i, j + offSet);
                    Vector2 tilePosition = new Vector2(i, j);
                    GameObject backgroundTile = Instantiate(tilePrefab, tilePosition, Quaternion.identity) as GameObject;
                    backgroundTile.transform.parent = this.transform;
                    backgroundTile.name = "(" + i + "," + j + ")";

                    int cakeToUse = Random.Range(0, cakes.Length);
                    int maxIterations = 0;
                    while (MatchesAt(i, j, cakes[cakeToUse]) && maxIterations < 9999)
                    {
                        cakeToUse = Random.Range(0, cakes.Length);
                        maxIterations++;
                    }
                    maxIterations = 0;
                    GameObject cake = Instantiate(cakes[cakeToUse], tempPos, Quaternion.identity) as GameObject;
                    cake.GetComponent<Cake>().row = j;
                    cake.GetComponent<Cake>().column = i;
                    cake.transform.parent = this.transform;
                    cake.name = "(" + i + "," + j + ")";
                    allCakes[i, j] = cake;
            }
        }
        StartCoroutine(DecreaseRowCo());
    }

    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if(column > 1 && row > 1)
        {
            if(allCakes[column - 1, row] != null && allCakes[column-2,row] != null)
            {
                if (allCakes[column - 1, row].tag == piece.tag && allCakes[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }

            if (allCakes[column, row-1] != null && allCakes[column, row-2] != null)
            {
                if (allCakes[column, row - 1].tag == piece.tag && allCakes[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }
                
        }
        else if(column <= 1 || row <= 1)
        {
            if(row > 1)
            {
                if (allCakes[column, row - 1] != null && allCakes[column, row - 2] != null)
                {
                    if (allCakes[column, row - 1].tag == piece.tag && allCakes[column, row - 2].tag == piece.tag)
                    {
                        return true;
                    }
                }
                    
            }
            if (column > 1)
            {
                if (allCakes[column-1, row] != null && allCakes[column-2, row] != null)
                {
                    if (allCakes[column - 1, row].tag == piece.tag && allCakes[column - 2, row].tag == piece.tag)
                    {
                        return true;
                    }
                }
                    
            }

        }
        return false;
    }

    private void DestroyMathesAt(int column, int row)
    {
        if(allCakes[column, row].GetComponent<Cake>().isMatched)
        {
            findMatches.currentMatches.Remove(allCakes[column, row]);
            //GameObject particle =  Instantiate(destroyEffect, allCakes[column, row].transform.position, Quaternion.identity);
            Destroy(allCakes[column, row]);
            scoreManager.InscreaseScore(basePieceValue * streakValue);
            allCakes[column, row] = null;
        }
    }

    public void DestroyMatches()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if (allCakes[i, j] != null)
                {
                    DestroyMathesAt(i, j);
                }
            }
        }
        findMatches.currentMatches.Clear();
        StartCoroutine(DecreaseRowCo());
    }

    private IEnumerator DecreaseRowCo()
    {
        int nullCount = 0;
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if(allCakes[i, j] == null)
                {
                    nullCount++;
                }
                else if(nullCount > 0)
                {
                    allCakes[i, j].GetComponent<Cake>().row -= nullCount;
                    allCakes[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(.3f);
        StartCoroutine(FillBoardCo());
    }
    private void RefillBoard()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if(allCakes[i, j] == null)
                {
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    int cakeToUse = Random.Range(0, cakes.Length);
                    int maxIteration = 0;
                    while (MatchesAt(i, j, cakes[cakeToUse]) && maxIteration < 9999)
                    {
                        maxIteration++;
                        cakeToUse = Random.Range(0, cakes.Length);
                    }
                    maxIteration = 0;
                    GameObject piece = Instantiate(cakes[cakeToUse], tempPosition, Quaternion.identity);
                    allCakes[i, j] = piece;
                    piece.GetComponent<Cake>().row = j;
                    piece.GetComponent<Cake>().column = i;
                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if(allCakes[i,j] != null)
                {
                    if (allCakes[i, j].GetComponent<Cake>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoardCo()
    {
        RefillBoard();
        yield return new WaitForSeconds(.4f);
        while (MatchesOnBoard())
        {
            streakValue ++;
            yield return new WaitForSeconds(.4f);
            DestroyMatches();
        }
        yield return new WaitForSeconds(.4f);
        if (IsDeadlocked())
        {
            ShuffleBoard();
        }
        currentState = GameState.move;
        streakValue = 1;
    }
    private void SwitchPiece(int column, int row, Vector2 direction)
    {
        GameObject holder = allCakes[column +(int)direction.x, row+(int)direction.y] as GameObject;
        allCakes[column + (int)direction.x, row + (int)direction.y] = allCakes[column, row];
        allCakes[column, row] = holder;
    }
    private bool CheckForMatches()
    {
        for(int i = 0; i < width; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if(allCakes[i,j] != null)
                {
                    if (i < width - 2)
                    {
                        if (allCakes[i + 1, j] != null && allCakes[i + 2, j] != null)
                        {
                            if (allCakes[i + 1, j].tag == allCakes[i, j].tag && allCakes[i + 2, j].tag == allCakes[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }
                    if (j < height - 2)
                    {
                        if (allCakes[i, j + 1] != null && allCakes[i, j + 2] != null)
                        {
                            if (allCakes[i, j + 1].tag == allCakes[i, j].tag && allCakes[i, j + 2].tag == allCakes[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }

    private bool SwitchAndCheck(int column, int row,Vector2 direction)
    {
        SwitchPiece(column, row, direction);
        if (CheckForMatches())
        {
            SwitchPiece(column, row, direction);
            return true;
        }
        SwitchPiece(column, row, direction);
        return false;
    }
    private bool IsDeadlocked()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0;j < height; j++)
            {
                if (allCakes[i, j] != null)
                {
                    if(i < width - 1)
                    {
                        if(SwitchAndCheck(i, j, Vector2.right))
                        {
                            return false;
                        }
                    }
                    if (j < height - 1)
                    {
                        if(SwitchAndCheck(i,j, Vector2.up))
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }
    private void ShuffleBoard()
    {
        List<GameObject> newBoard = new List<GameObject>();
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if(allCakes[i,j] != null)
                {
                    newBoard.Add(allCakes[i, j]);
                }
            }
        }
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                int pieceToUse = Random.Range(0, newBoard.Count);

                int maxIterations = 0;
                while (MatchesAt(i, j, newBoard[pieceToUse]) && maxIterations < 9999)
                {
                    pieceToUse = Random.Range(0, newBoard.Count);
                    maxIterations++;
                }
                Cake piece = newBoard[pieceToUse].GetComponent<Cake>();
                maxIterations = 0;
                piece.column = i;
                piece.row = j;

                allCakes[i, j] = newBoard[pieceToUse];
                newBoard.Remove(newBoard[pieceToUse]);
            }
        }
        if (IsDeadlocked())
        {
            ShuffleBoard();
        }
    }
}
