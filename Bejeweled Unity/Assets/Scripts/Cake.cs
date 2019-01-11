using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cake : MonoBehaviour {

    [Header("Board Variables")]
    public int column;
    public int row;
    public int previousColumn;
    public int previousRow;
    public int targetX;
    public int targetY;
    public bool isMatched = false;

    private EndGameManager endGameManager;
    private FindMatches findMaches;
    private Board board;
    private GameObject otherCake;
    private Vector2 firstTouchPos;
    private Vector2 finalTouchPos;
    private Vector2 tempPosition;
    public float swipeAngle = 0;
    public float swipeResist = 1f;
	
	void Start () {
        board = FindObjectOfType<Board>();
        findMaches = FindObjectOfType<FindMatches>();
        endGameManager = FindObjectOfType<EndGameManager>();
        //targetX = (int)transform.position.x;
        //targetY = (int)transform.position.y;
        //column = targetX;
        //row = targetY;
        //previousColumn = column;
        //previousRow = row;
    }
	void Update () {
        targetX = column;
        targetY = row;
        FindMatches();
        //if (isMatched)
        //{
        //    SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
        //    mySprite.color = new Color(0f, 0f, 0f, .2f);
        //}
        
        if(Mathf.Abs(targetX - transform.position.x) > .1) //Move Towards the target
        {
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .4f);
            if(board.allCakes[column,row] != this.gameObject)
            {
                board.allCakes[column, row] = this.gameObject;
            }
            findMaches.FindAllMaches();
        }
        else //Directly set the position
        {
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
        }
        if (Mathf.Abs(targetY - transform.position.y) > .1) //Move Towards the target
        {
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .4f);
            if (board.allCakes[column, row] != this.gameObject)
            {
                board.allCakes[column, row] = this.gameObject;
            }
            findMaches.FindAllMaches();
        }
        else //Directly set the position
        {
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
        }
    }

    public IEnumerator CheckMoveCo()
    {
        yield return new WaitForSeconds(.5f);
        if(otherCake != null)
        {
            if (!isMatched && !otherCake.GetComponent<Cake>().isMatched)
            {
                otherCake.GetComponent<Cake>().row = row;
                otherCake.GetComponent<Cake>().column = column;
                row = previousRow;
                column = previousColumn;
                yield return new WaitForSeconds(.5f);
                board.currentState = GameState.move;
            }
            else
            {
                if (endGameManager != null)
                {
                    if (endGameManager.requirement.gameType == GameType.Move)
                    {                       
                            endGameManager.DecreaseCounterValue(findMaches.currentMatches.Count);                         
                    }
                }
                board.DestroyMatches();             
            }
            //otherCake = null;
        }
    }
    private void OnMouseDown()
    {
        if(board.currentState == GameState.move)
        {
            firstTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }
    private void OnMouseUp()
    {
        if(board.currentState == GameState.move)
        {
            finalTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CaculateAngle();
        }   
    }
    void CaculateAngle()
    {
        if(Mathf.Abs(finalTouchPos.y - firstTouchPos.y) > swipeResist || Mathf.Abs(finalTouchPos.x - firstTouchPos.x) > swipeResist)
        {
            swipeAngle = Mathf.Atan2(finalTouchPos.y - firstTouchPos.y, finalTouchPos.x - firstTouchPos.x) * 180 / Mathf.PI;
            board.currentState = GameState.wait;
            MovePiece();      
        }
        else
        {
            board.currentState = GameState.move;
        }    
    }
    void MovePieceActual(Vector2 direction)
    {
        otherCake = board.allCakes[column + (int)direction.x, row + (int)direction.y];
        previousRow = row;
        previousColumn = column;
        if(otherCake != null)
        {
            otherCake.GetComponent<Cake>().column += -1 * (int)direction.x;
            otherCake.GetComponent<Cake>().row += -1 * (int)direction.y;
            column += (int)direction.x;
            row += (int)direction.y;
            StartCoroutine(CheckMoveCo());
        }
        else
        {
            board.currentState = GameState.move;
        }
    }
    void MovePiece()
    {
        if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0) //Left swipe
        {
            //otherCake = board.allCakes[column - 1, row];
            //previousColumn = column;
            //previousRow = row;
            //otherCake.GetComponent<Cake>().column += 1;
            //column -= 1;

            MovePieceActual(Vector2.left);
        }
        else if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1) //Right swipe
        {
            //otherCake = board.allCakes[column + 1, row];
            //previousColumn = column;
            //previousRow = row;
            //otherCake.GetComponent<Cake>().column -= 1;
            //column += 1;
            MovePieceActual(Vector2.right);
        }
        else if(swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1) //Up swipe
        {
            //otherCake = board.allCakes[column, row + 1];
            //previousColumn = column;
            //previousRow = row;
            //otherCake.GetComponent<Cake>().row -= 1;
            //row += 1;
            MovePieceActual(Vector2.up);
        }
        else if(swipeAngle < -45 && swipeAngle >= -135 && row > 0) //Down swipe
        {
            //otherCake = board.allCakes[column, row - 1];
            //previousColumn = column;
            //previousRow = row;
            //otherCake.GetComponent<Cake>().row += 1;
            //row -= 1;
            MovePieceActual(Vector2.down);
        }
        else
        {
            board.currentState = GameState.move;
        }
    }

    void FindMatches()
    {
        if(column > 0 && column < board.width - 1)
        {
            GameObject leftCake1 = board.allCakes[column - 1, row];
            GameObject rightCake1 = board.allCakes[column + 1, row];
            if(leftCake1 != null && rightCake1 != null)
            {
                if (leftCake1.tag == this.gameObject.tag && rightCake1.tag == this.gameObject.tag) //Check bên trái và bên phải nếu tag giống nhau thì Match = true
                {
                    leftCake1.GetComponent<Cake>().isMatched = true;
                    rightCake1.GetComponent<Cake>().isMatched = true;
                    isMatched = true;
                }
            } 
        }
        if (row > 0 && row < board.height - 1)
        {
            GameObject upCake1 = board.allCakes[column, row + 1];
            GameObject downCake1 = board.allCakes[column, row - 1];
            if(upCake1 != null && downCake1 != null)
            {
                if (upCake1.tag == this.gameObject.tag && downCake1.tag == this.gameObject.tag) //Check bên trên và bên dưới nếu tag giống nhau thì Match = true
                {
                    upCake1.GetComponent<Cake>().isMatched = true;
                    downCake1.GetComponent<Cake>().isMatched = true;
                    isMatched = true;
                }
            }   
        }
    }
}
