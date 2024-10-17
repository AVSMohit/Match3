using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class Dot : MonoBehaviour
{
    [Header("Board Variables")]
    public int column, row, previousColumn, previousRow;
    public int targetX, targetY;
    public bool isMatch = false;


    public GameObject otherDot;

    Board board;
    FindMatches findMatches;
    HintManager hintManager;
    EndGameManager endGameManager;

    Vector2 firstTouchPosition  = Vector2.zero;
    Vector2 finalTouchPosition = Vector2.zero;
    Vector2 tempPosition;

    [Header("Swipe Values")]
    public float swipeAngle = 0;
    public float swipeResist = 1f;

    [Header("Powerups")]
    public bool isColorBomb,isColumnBomb,isRowBomb,isAdjacentBomb;
    public GameObject RowArrow, ColumnArrow , ColorBomb, AdjacentBomb;

    Touch touch;

    SwitchToggle toggle;


    // Start is called before the first frame update
    void Start()
    {
        isColumnBomb = false;
        isRowBomb = false;
        isColorBomb = false;
        isAdjacentBomb = false;
        
        board = GameObject.FindWithTag("Board").GetComponent<Board>();
        findMatches = FindObjectOfType<FindMatches>();
        hintManager = FindObjectOfType<HintManager>();  
        endGameManager = FindObjectOfType<EndGameManager>();
        toggle = FindObjectOfType<SwitchToggle>();  
       
    }

  
    
    // Update is called once per frame
    void Update()
    {

        targetX = column;
        targetY = row;

        HandleMovement();

        #region Touch Input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                OnTouchDown(touch);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                OnTouchUp(touch);
            }
        }
        #endregion
        // Mouse input handling (for testing in the editor)
        if (Input.GetMouseButtonDown(0))
        {
            OnMouseDown();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            OnMouseUp();
        }


    }

    private void HandleMovement()
    {
        if (Mathf.Abs(targetX - transform.position.x) > .1f)
        {
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, 0.2f);
            if (board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
                findMatches.FindAllMatches();
            }
        }
        else
        {
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
        }

        if (Mathf.Abs(targetY - transform.position.y) > .1f)
        {
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, 0.2f);
            if (board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
                findMatches.FindAllMatches();
            }
        }
        else
        {
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
        }
    }

    private void OnTouchDown(Touch touch)
    {
        if (board.currenState == GameState.move)
        {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(touch.position);
            if (hintManager != null)
            {
                hintManager.DestroyHint();
            }
        }
    }

    private void OnTouchUp(Touch touch)
    {
        if (board.currenState == GameState.move)
        {
            finalTouchPosition = Camera.main.ScreenToWorldPoint(touch.position);
            CalculateAngle();
        }
    }

    private void OnMouseDown()
    {
        if (board.currenState == GameState.move)
        {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (hintManager != null)
            {
                hintManager.DestroyHint();
            }
        }
    }


    private void OnMouseUp()
    {
        if (board.currenState == GameState.move)
        {
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
    }

    private void CalculateAngle()
    {
        if (Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist || Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist)
        {
            board.currenState = GameState.wait;
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
            MoveDot();
            board.currentDot = this;
        }
        else
        {
            board.currenState = GameState.move;
        }
    }

    private void MovePiecesActual(Vector2 direction)
    {
        otherDot = board.allDots[column + (int)direction.x, row + (int)direction.y];
        previousRow = row;
        previousColumn = column;

        if (board.lockTiles[column, row] == null && board.lockTiles[column + (int)direction.x, row + (int)direction.y] == null)
        {
            if (otherDot != null)
            {
                otherDot.GetComponent<Dot>().column += -1 * (int)direction.x;
                otherDot.GetComponent<Dot>().row += -1 * (int)direction.y;
                column += (int)direction.x;
                row += (int)direction.y;
                StartCoroutine(CheckMoveCo());
            }
            else
            {
                board.currenState = GameState.move;
            }
        }
        else
        {
            board.currenState = GameState.move;
        }
    }


    private void MoveDot()
    {
        if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1)
        {
            MovePiecesActual(Vector2.right);
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1)
        {
            MovePiecesActual(Vector2.up);
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {
            MovePiecesActual(Vector2.left);
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            MovePiecesActual(Vector2.down);
        }
        else
        {
            board.currenState = GameState.move;
        }
    }

    void FindMatches()
    {
        if(column > 0 && column < board.width - 1)
        {
            GameObject  leftDot1 = board.allDots[column-1, row];
            GameObject rightDot1 = board.allDots[column+1, row];
            if(leftDot1 != null && rightDot1 != null)
            {

                if(leftDot1.tag == this.gameObject.tag && rightDot1.tag == this.gameObject.tag)
                {
                    leftDot1.GetComponent<Dot>().isMatch = true;
                    rightDot1.GetComponent<Dot>().isMatch = true;
                    isMatch = true;
                }
            }
        }
        if (row > 0 && row < board.height - 1)
        {
            GameObject upDot1 = board.allDots[column, row + 1];
            GameObject downDot1 = board.allDots[column, row - 1];
            if(upDot1 != null && downDot1 != null)
            {

                if (upDot1.tag == this.gameObject.tag && downDot1.tag == this.gameObject.tag)
                {
                    upDot1.GetComponent<Dot>().isMatch = true;
                    downDot1.GetComponent<Dot>().isMatch = true;
                    isMatch = true;
                }
            }
        }
    }

    IEnumerator CheckMoveCo()
    {
        yield return new WaitForSeconds(0.5f);
        // Check for matches and revert the move if needed
        if (!isMatch && !otherDot.GetComponent<Dot>().isMatch)
        {
            otherDot.GetComponent<Dot>().row = row;
            otherDot.GetComponent<Dot>().column = column;
            row = previousRow;
            column = previousColumn;
            yield return new WaitForSeconds(0.5f);
            board.currenState = GameState.move;
        }
        else
        {
            board.DestroMatches();
        }
    }

    public void MakeRowBomb()
    {
        if(!isColumnBomb && !isColorBomb && !isAdjacentBomb)
        {

            isRowBomb = true;
            GameObject arrow = Instantiate(RowArrow, transform.position, Quaternion.identity) as GameObject;
            arrow.transform.parent = this.transform;
        }
    }

    public void MakeColumnBomb()
    {
        if (!isRowBomb && !isColorBomb && !isAdjacentBomb)
        {
            isColumnBomb = true;
            GameObject arrow = Instantiate(ColumnArrow, transform.position, Quaternion.identity) as GameObject;
            arrow.transform.parent = this.transform;
        }
        

    }

    public void MakeColorBomb()
    {
        if (!isRowBomb && !isColumnBomb && !isAdjacentBomb)
        {
            isColorBomb = true;
            GameObject color = Instantiate(ColorBomb, transform.position, Quaternion.identity) as GameObject;
            color.transform.parent = this.transform;
            this.gameObject.tag = "Color";
        }
           
    }
    
    public void MakeAdjacentBomb()
    {
        if (!isRowBomb && !isColumnBomb && !isColorBomb)
        {
            isAdjacentBomb = true;
            GameObject marker = Instantiate(AdjacentBomb, transform.position, Quaternion.identity) as GameObject;
            marker.transform.parent = this.transform;
        }
            
    }
}
