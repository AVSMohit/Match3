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

    GameObject otherDot;

    Board board;

    Vector2 firstTouchPosition;
    Vector2 finalTouchPosition;
    Vector2 tempPosition;


    public float swipeAngle = 0;
    public float swipeResist = 1f;

    public bool isMatch = false;

    Touch touch;


    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        targetX = (int)transform.position.x;
        targetY = (int)transform.position.y;
        row = targetY;
        column = targetX;
        previousRow = row;
        previousColumn = column;
    }

    // Update is called once per frame
    void Update()
    {

        FindMatches();
        if (isMatch)
        {
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            mySprite.color = new Color(1f, 1f, 1f, .2f);
        }

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
            transform.position = Vector2.Lerp(transform.position, tempPosition, 0.4f);
        }
        else
        {
            //Directl Set Position
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
            board.allDots[column, row] = this.gameObject;
        }
        if(Mathf.Abs(targetY - transform.position.y) > .1f)
        {
            //Move Towards the Target
            tempPosition = new Vector2 (transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, 0.4f);
        }
        else
        {
            //Directl Set Position
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;  
            board.allDots[column, row] = this.gameObject;
        }

        
    }

    private void OnMouseDown()
    {
        firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //firstTouchPosition = touch.position;

    }

    private void OnMouseUp()
    {
        finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //finalTouchPosition = touch.position;
        CalculateAngle();
       
    }

    void CalculateAngle()
    {
        if(Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist || Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist)
        {
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
     
            MoveDot();
        }
    }
    void MoveDot()
    {
        if (swipeAngle > -45 && swipeAngle <= 45  && column < board.width - 1 )
        {
            //right swipe
            otherDot = board.allDots[column + 1, row];
            otherDot.GetComponent<Dot>().column -= 1;
            column += 1;

        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1 )
        {
            //Up swipe
            otherDot = board.allDots[column, row + 1];
            otherDot.GetComponent<Dot>().row -= 1;
            row += 1;

        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0 )
        {
            //Left swipe
            otherDot = board.allDots[column - 1, row];
            otherDot.GetComponent<Dot>().column += 1;
            column -= 1;

        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            //Down swipe
            otherDot = board.allDots[column, row - 1];
            otherDot.GetComponent<Dot>().row += 1;
            row -= 1;

        }

        StartCoroutine(CheckMoveCo());

    }

    void FindMatches()
    {
        if(column > 0 && column < board.width - 1)
        {
            GameObject  leftDot1 = board.allDots[column-1, row];
            GameObject rightDot1 = board.allDots[column+1, row];
            if(leftDot1 != null && rightDot1 != null)
            {

                if(leftDot1.CompareTag( this.gameObject.tag) && rightDot1.CompareTag(this.gameObject.tag))
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

                if (upDot1.CompareTag(this.gameObject.tag) && downDot1.CompareTag(this.gameObject.tag))
                {
                    upDot1.GetComponent<Dot>().isMatch = true;
                    downDot1.GetComponent<Dot>().isMatch = true;
                    isMatch = true;
                }
            }
        }
    }

   public IEnumerator CheckMoveCo()
    {
        yield return new WaitForSeconds(.5f);
        if(otherDot != null)
        {
            if (!isMatch && !otherDot.GetComponent<Dot>().isMatch)
            {
                otherDot.GetComponent<Dot>().row = row;
                otherDot.GetComponent<Dot>().column = column;
                row = previousRow;
                column = previousColumn;
            }
            else
            {
                board.DestroMatches();
            }

            otherDot = null;
        }
    }
}
