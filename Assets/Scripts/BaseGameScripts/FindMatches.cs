using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public class FindMatches : MonoBehaviour
{
     Board board;
    public  List<GameObject> currenMatches = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
    }

    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }

    List<GameObject> IsAdjacentBomb(Dot dot1,Dot dot2,Dot dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();
        if (dot1.isAdjacentBomb)
        {
            currenMatches.Union(GetAdjacentPieces(dot1.column,dot1.row));
        }
        if (dot2.isAdjacentBomb)
        {
            currenMatches.Union(GetAdjacentPieces(dot2.column, dot2.row));
        }

        if (dot3.isAdjacentBomb)
        {
            currenMatches.Union(GetAdjacentPieces(dot3.column, dot3.row)); ;
        }
        return currentDots;
    }
     
    List<GameObject> IsRowBomb(Dot dot1, Dot dot2, Dot dot3)
    {

        List<GameObject> currentDots = new List<GameObject>();
        if (dot1.isRowBomb)
        {
            currenMatches.Union(GetRowPieces(dot1.row));
            board.BombRow(dot1.row);
        }
        if (dot2.isRowBomb)
        {
            currenMatches.Union(GetRowPieces(dot2.row));
            board.BombRow(dot2.row);
        }

        if (dot3.isRowBomb)
        {
            currenMatches.Union(GetRowPieces(dot3.row));
            board.BombRow(dot3.row);
        }
       return currentDots;

    } 
    
    List<GameObject> IsColumnBomb(Dot dot1, Dot dot2, Dot dot3)
    {

        List<GameObject> currentDots = new List<GameObject>();
        if (dot1.isColumnBomb)
        {
            currenMatches.Union(GetColumnPieces(dot1.column));
            board.BombColumn(dot1.column);
        }
        if (dot2.isColumnBomb)
        {
            currenMatches.Union(GetColumnPieces(dot2.column));
            board.BombColumn(dot2.column);
        }

        if (dot3.isColumnBomb)
        {
            currenMatches.Union(GetColumnPieces(dot3.column));
            board.BombColumn(dot3.column);
        }
       return currentDots;

    }
        
    void AddTolistAndMatch(GameObject dot)
    {
        if (!currenMatches.Contains(dot))
        {
            currenMatches.Add(dot);
        }
        dot.GetComponent<Dot>().isMatch = true;
    }
    void GetNearByPieces(GameObject dot1,GameObject dot2,GameObject dot3)
    {

        if (dot1 != null && dot2 != null && dot3 != null)
        {
            // Make sure that only adjacent pieces are added to the match list
            if (Vector2.Distance(dot1.transform.position, dot2.transform.position) == 1 &&
                Vector2.Distance(dot2.transform.position, dot3.transform.position) == 1)
            {
                AddTolistAndMatch(dot1);
                AddTolistAndMatch(dot2);
                AddTolistAndMatch(dot3);
            }
        }


    }

    IEnumerator FindAllMatchesCo()
    {
        //yield return new WaitForSeconds(0.2f);
        yield return null;
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                GameObject currentDot = board.allDots[i, j];
                if (currentDot != null)
                {
                    Dot currentDotComponent = currentDot.GetComponent<Dot>();
                    // Check left and right pieces
                    if (i > 0 && i < board.width - 1)
                    {
                        GameObject leftDot = board.allDots[i - 1, j];
                        GameObject rightDot = board.allDots[i + 1, j];

                        if (leftDot != null && rightDot != null)
                        {
                            if (leftDot.tag == currentDot.tag && rightDot.tag == currentDot.tag)
                            {
                                GetNearByPieces(leftDot, currentDot, rightDot);
                            }
                        }
                    }
                    // Check up and down pieces
                    if (j > 0 && j < board.height - 1)
                    {
                        GameObject upDot = board.allDots[i, j + 1];
                        GameObject downDot = board.allDots[i, j - 1];

                        if (upDot != null && downDot != null)
                        {
                            if (upDot.tag == currentDot.tag && downDot.tag == currentDot.tag)
                            {
                                GetNearByPieces(upDot, currentDot, downDot);
                            }
                        }
                    }
                }
            }
        }
    }


    public void MatchPiecesofColor(string color)
    {
        for(int i =0; i < board.width; i++)
        {
            for(int j =0; j < board.height; j++)
            {
                //check if the pieces exists
                if (board.allDots[i,j] != null)
                {
                    //check tag on the dot
                    if (board.allDots[i,j].tag == color)
                    {
                        //Set that fot to be matched
                        board.allDots[i,j].GetComponent<Dot>().isMatch = true;

                    }
                }
            }
        }
    }

    List<GameObject> GetAdjacentPieces(int column,int row)
    {
        List<GameObject> dots = new List<GameObject>();
        for(int k = column -1; k <= column + 1; k++)
        {
            for(int i = row -1; i <= row +1; i++)
            {
                //Check if piece is inside the board
                if(k >= 0 && k < board.width && i >=0 && i < board.height)
                {
                    if (board.allDots[i,k] != null)
                    {
                        dots.Add(board.allDots[k,i]);
                        board.allDots[k, i].GetComponent<Dot>().isMatch = true;
                    }
                }
            }
        }
        return dots;
    }


    List<GameObject> GetColumnPieces(int column)
    {
        List<GameObject> dots = new List<GameObject>();

        for(int i = 0; i < board.height;i++)
        {
            if (board.allDots[column,i] != null)
            {
                Dot dot = board.allDots[column,i].GetComponent<Dot>();
                if (dot.isRowBomb)
                {
                    dots.Union(GetRowPieces(i)).ToList();
                }

                dots.Add(board.allDots[column,i]);
                dot.isMatch = true;
            }
        }
        return dots;
    }
    
    List<GameObject> GetRowPieces(int row)
    {
        List<GameObject> dots = new List<GameObject>();

        for(int i = 0; i < board.width;i++)
        {
            if (board.allDots[i,row] != null)
            {
                Dot dot = board.allDots[i,row].GetComponent<Dot>();
                if (dot.isColumnBomb)
                {
                    dots.Union(GetColumnPieces(i)).ToList();
                }
                dots.Add(board.allDots[i,row]);
               dot.isMatch = true;
            }
        }
        return dots;
    }

    public void CheckBombMatch(MatchType matchType)
    {
        //Did player move
        if(board.currentDot != null)
        {
            //is the moved piece matches
            if (board.currentDot.isMatch && board.currentDot.tag == matchType.color)
            {
                //make it unmatch
                board.currentDot.isMatch = false;
                //Decide what kind of Bomb to make
               
                
                //int typeOfBomb = Random.Range(0, 100);
                //if(typeOfBomb < 50)
                //{
                //    //make a row bomb
                //    board.currentDot.MakeRowBomb();
                //}
                //else if(typeOfBomb >= 50)
                //{
                //    //make column bomb
                //    board.currentDot.MakeColumnBomb();
                //}
                if ((board.currentDot.swipeAngle > -45 && board.currentDot.swipeAngle <= 45) || (board.currentDot.swipeAngle < -135 || board.currentDot.swipeAngle >= 135))
                {
                    board.currentDot.MakeRowBomb();
                }
                else
                {
                    board.currentDot.MakeColumnBomb();

                }
            }
            //is other piece matched
            else if (board.currentDot.otherDot != null)
            {
                Dot otherdot = board.currentDot.otherDot.GetComponent<Dot>();
                //check other dot Match
                if (otherdot.isMatch && otherdot.tag == matchType.color)
                {
                    //make it unmatch
                    otherdot.isMatch = false;

                    //Decide bomb type


                    //int typeOfBomb = Random.Range(0, 100);
                    //if (typeOfBomb < 50)
                    //{
                    //    //make a row bomb
                    //    otherdot.MakeRowBomb();
                    //}
                    //else if (typeOfBomb >= 50)
                    //{
                    //    //make column bomb
                    //    otherdot.MakeColumnBomb();
                    //}
                    if ((board.currentDot.swipeAngle > -45 && board.currentDot.swipeAngle <= 45) || (board.currentDot.swipeAngle < -135 || board.currentDot.swipeAngle >= 135))
                    {
                        otherdot.MakeRowBomb();
                    }
                    else
                    {
                        otherdot.MakeColumnBomb();

                    }
                }


            }
        }
    }
}
