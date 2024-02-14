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



    Vector2 firstTouchPosition;
    Vector2 finalTouchPosition;
    Vector2 tempPosition;

    [Header("Swipe Values")]
    public float swipeAngle = 0;
    public float swipeResist = 1f;

    [Header("Powerups")]
    public bool isColorBomb,isColumnBomb,isRowBomb,isAdjacentBomb;
    public GameObject RowArrow, ColumnArrow , ColorBomb, AdjacentBomb;

    Touch touch;


    // Start is called before the first frame update
    void Start()
    {
        isColumnBomb = false;
        isRowBomb = false;
        isColorBomb = false;
        isAdjacentBomb = false;
        board = FindObjectOfType<Board>();
        findMatches = FindObjectOfType<FindMatches>();
        //targetX = (int)transform.position.x;
        //targetY = (int)transform.position.y;
        //row = targetY;
        //column = targetX;
        //previousRow = row;
        //previousColumn = column;
    }

    //This is for Testing and DebugOnly;________________________________________________________________________________________\

    private void OnMouseOver()
    {
        if(Input.GetMouseButtonDown(1))
        {
            isAdjacentBomb = true;
            GameObject marker =Instantiate(AdjacentBomb,transform.position,Quaternion.identity) as GameObject; //
            marker.transform.parent = this.transform;
        }
    }
    //____________________________________________________________________________________________________________________________/
   
    
    
    // Update is called once per frame
    void Update()
    {


        /*
        if (isMatch)
        {
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            mySprite.color = new Color(1f, 1f, 1f, .2f);
        }
        */
        targetX = column;
        targetY = row;
        #region Touch Input
      //  if (Input.touchCount > 0)
      //  {
      //      if(touch.phase == TouchPhase.Began)
      //      {
      //          OnMouseDown();
      //      }
      //      if(touch.phase == TouchPhase.Ended)
      //      {
      //          OnMouseUp();
      //      }
      //}
        #endregion
        if (Mathf.Abs(targetX - transform.position.x) > .1f)
        {
            //Move Towards the Target
            tempPosition = new Vector2 (targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, 0.2f);
            if (board.allDots[column,row] != this.gameObject) 
            {
                board.allDots[column, row] = this.gameObject;
            }
            findMatches.FindAllMatches();
        }
        else
        {
            //Directl Set Position
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
           
        }
        if(Mathf.Abs(targetY - transform.position.y) > .1f)
        {
            //Move Towards the Target
            tempPosition = new Vector2 (transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, 0.2f);
            if (board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
            }
            findMatches.FindAllMatches();
        }
        else
        {
            //Directl Set Position
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;  
            
        }

        
    }

    private void OnMouseDown()
    {
        if(board.currenState == GameState.move)
        {

            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //firstTouchPosition = touch.position;
        }

    }

    private void OnMouseUp()
    {
        if(board.currenState == GameState.move)
        {

            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //finalTouchPosition = touch.position;
            CalculateAngle();
        }
       
    }

    void CalculateAngle()
    {
        if(Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist || Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist)
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

    void MovePiecesActual(Vector2 direction)
    {
        otherDot = board.allDots[column + (int)direction.x, row + (int)direction.y];
        previousRow = row;
        previousColumn = column;

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

    void MoveDot()
    {
        if (swipeAngle > -45 && swipeAngle <= 45  && column < board.width - 1 )
        {
            //right swipe
            MovePiecesActual(Vector2.right);

            /*  otherDot = board.allDots[column + 1, row];
              previousRow = row;
              previousColumn = column;
              otherDot.GetComponent<Dot>().column -= 1;
              column += 1;
              StartCoroutine(CheckMoveCo());
             */

        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1 )
        {
            //Up swipe
            MovePiecesActual(Vector2.up);
            /*
            otherDot = board.allDots[column, row + 1];
            previousRow = row;
            previousColumn = column;
            otherDot.GetComponent<Dot>().row -= 1;
            row += 1;
            StartCoroutine(CheckMoveCo());
            */
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0 )
        {
            //Left swipe
            MovePiecesActual(Vector2.left);
            /*
            otherDot = board.allDots[column - 1, row];
            previousRow = row;
            previousColumn = column;
            otherDot.GetComponent<Dot>().column += 1;
            column -= 1;
            StartCoroutine(CheckMoveCo());
            */
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            //Down swipe
            MovePiecesActual(Vector2.down);
            /*
            otherDot = board.allDots[column, row - 1];
            previousRow = row;
            previousColumn = column;
            otherDot.GetComponent<Dot>().row += 1;
            row -= 1;
            StartCoroutine(CheckMoveCo());
            */
        }

        board.currenState = GameState.move;

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
        if(isColorBomb)
        {
            //this piece is collor bomb and other piece is the color to destroy
            findMatches.MatchPiecesofColor(otherDot.tag);
            isMatch = true;
        }
        else if (otherDot.GetComponent<Dot>().isColorBomb)
        {
            //the other piece is color bomb and this piece is to be destroyed
            findMatches.MatchPiecesofColor(this.gameObject.tag);
            otherDot.GetComponent<Dot>().isMatch = true;
        }

        yield return new WaitForSeconds(.3f);
        if(otherDot != null)
        {
            if (!isMatch && !otherDot.GetComponent<Dot>().isMatch)
            {
                otherDot.GetComponent<Dot>().row = row;
                otherDot.GetComponent<Dot>().column = column;
                row = previousRow;
                column = previousColumn;
                yield return new WaitForSeconds(.5f);
                board.currentDot = null;
                board.currenState = GameState.move;
            }
            else
            {
                board.DestroMatches();
            }

            otherDot = null;
        }
    }

    public void MakeRowBomb()
    {
        isRowBomb = true;
        GameObject arrow = Instantiate(RowArrow, transform.position, Quaternion.identity) as GameObject;
        arrow.transform.parent = this.transform;
    }

    public void MakeColumnBomb()
    {
        isColumnBomb = true;
        GameObject arrow = Instantiate(ColumnArrow, transform.position, Quaternion.identity) as GameObject;
        arrow.transform.parent = this.transform;

    }

    public void MakeColorBomb()
    { 
        isColorBomb = true;
        GameObject color = Instantiate(ColorBomb, transform.position, Quaternion.identity) as GameObject;
        color.transform.parent = this.transform;
    }
    
    public void MakeAdjacentBomb()
    { 
        isAdjacentBomb = true;
        GameObject marker = Instantiate(AdjacentBomb, transform.position, Quaternion.identity) as GameObject;
        marker.transform.parent = this.transform;
    }
}
