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

     
    List<GameObject> IsRowBomb(Dot dot1, Dot dot2, Dot dot3)
    {

        List<GameObject> currentDots = new List<GameObject>();
        if (dot1.isRowBomb)
        {
            currenMatches.Union(GetRowPieces(dot1.row));
        }
        if (dot2.isRowBomb)
        {
            currenMatches.Union(GetRowPieces(dot2.row));
        }

        if (dot3.isRowBomb)
        {
            currenMatches.Union(GetRowPieces(dot3.row));
        }
       return currentDots;

    } 
    
    List<GameObject> IsColumnBomb(Dot dot1, Dot dot2, Dot dot3)
    {

        List<GameObject> currentDots = new List<GameObject>();
        if (dot1.isColumnBomb)
        {
            currenMatches.Union(GetColumnPieces(dot1.column));
        }
        if (dot2.isColumnBomb)
        {
            currenMatches.Union(GetColumnPieces(dot2.column));
        }

        if (dot3.isColumnBomb)
        {
            currenMatches.Union(GetColumnPieces(dot3.column));
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

        AddTolistAndMatch(dot1 );
        AddTolistAndMatch(dot2 );
        AddTolistAndMatch(dot3 );
     
        
    }
    IEnumerator FindAllMatchesCo()
    {
        yield return new WaitForSeconds(0.2f);
        for(int i = 0;i<board.width;i++)
        {
            for(int j = 0;j<board.height;j++)
            {
                GameObject currentDot = board.allDots[i,j];
                if(currentDot != null)
                {
                   Dot currentdotDot = currentDot.GetComponent<Dot>();
                    if(i > 0 && i<board.width - 1)
                    {

                        GameObject leftDot = board.allDots[i - 1,j];
                        GameObject rightDot = board.allDots[i+1,j];

                            if(leftDot != null && rightDot != null)
                            {
    
                                Dot rightdotDot = rightDot.GetComponent<Dot>();
                                Dot leftdotDot = leftDot.GetComponent<Dot>();
                            if (leftDot != null && rightDot != null)
                            {
                                if(leftDot.tag == currentDot.tag && rightDot.tag == currentDot.tag) 
                                {
                                    currenMatches.Union(IsRowBomb(leftdotDot, currentdotDot, rightdotDot));

                                    currenMatches.Union(IsColumnBomb(leftdotDot,currentdotDot, rightdotDot));

                                    GetNearByPieces(leftDot,currentDot ,rightDot);

                                }
                            }
                            }
                    }
                    if (j > 0 && j < board.height - 1)
                    {
                            GameObject upDot = board.allDots[i, j + 1];
                            GameObject downDot = board.allDots[i, j -1 ];

                            if(upDot != null && downDot != null)
                            {

                                Dot downdotDot = downDot.GetComponent<Dot>();
                                Dot UpdotDot = upDot.GetComponent<Dot>();
                                if (upDot != null && downDot != null)
                                {
                                    if (upDot.tag == currentDot.tag && downDot.tag == currentDot.tag)
                                    {
                                        currenMatches.Union(IsColumnBomb(UpdotDot, currentdotDot, downdotDot));

                                        currenMatches.Union(IsRowBomb(UpdotDot, currentdotDot, downdotDot));

                                        GetNearByPieces(upDot, currentDot, downDot);
                                    }
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

    List<GameObject> GetColumnPieces(int column)
    {
        List<GameObject> dots = new List<GameObject>();

        for(int i = 0; i < board.height;i++)
        {
            if (board.allDots[column,i] != null)
            {
                dots.Add(board.allDots[column,i]);
                board.allDots[column,i].GetComponent<Dot>().isMatch = true;
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
                dots.Add(board.allDots[i,row]);
                board.allDots[i,row].GetComponent<Dot>().isMatch = true;
            }
        }
        return dots;
    }

    public void CheckBombMatch()
    {
        //Did player move
        if(board.currentDot != null)
        {
            //is the moved piece matches
            if (board.currentDot.isMatch)
            {
                //make it unmatch
                board.currentDot.isMatch = false;
                //Decide what kind of Bomb to make
               
                /*
                int typeOfBomb = Random.Range(0, 100);
                if(typeOfBomb < 50)
                {
                    //make a row bomb
                  v
                }
                else if(typeOfBomb >= 50)
                {
                    //make column bomb
                    board.currentDot.MakeColumnBomb();
                }*/
                if((board.currentDot.swipeAngle > -45 && board.currentDot.swipeAngle <= 45) || (board.currentDot.swipeAngle < -135 || board.currentDot.swipeAngle >= 135))
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
                if (otherdot.isMatch)
                {
                    //make it unmatch
                    otherdot.isMatch = false;

                    //Decide bomb type

                    /*
                    int typeOfBomb = Random.Range(0, 100);
                    if (typeOfBomb < 50)
                    {
                        //make a row bomb
                        otherdot.MakeRowBomb();
                    }
                    else if (typeOfBomb >= 50)
                    {
                        //make column bomb
                        otherdot.MakeColumnBomb();
                    }*/
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
