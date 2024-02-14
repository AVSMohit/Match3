
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    wait, 
    move
}
public enum TypeOfTile
{
    Breakable,
    Blank,
    Normal
}

[System.Serializable]
public class TileType
{
    public int x, y;
    public TypeOfTile typeOfTile;
}

public class Board : MonoBehaviour
{
    public GameState currenState = GameState.move;
    FindMatches findMatches;
   

    public int width;
    public int height;
    public int offset;

    public GameObject tilePrefab;
    public GameObject[] Dots;
    public GameObject destroyEffect;
    public GameObject[,] allDots;



    public Dot currentDot;

    bool[,] blankSpaces;

    
    public TileType[] boardLayout;

    // Start is called before the first frame update
    void Start()
    {
        findMatches = FindObjectOfType<FindMatches>();
        blankSpaces = new bool[width, height];
        allDots = new GameObject[width, height];
        Setup();
    }

    public void GenerateBlankSpace()
    {
        for(int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].typeOfTile == TypeOfTile.Blank)
            {
                blankSpaces[boardLayout[i].x, boardLayout[i].y] = true;
            }
        }
    }

   void Setup()
    {
        GenerateBlankSpace();
        for (int i = 0; i < width; i++) 
        {
            for(int j = 0; j < height; j++)
            {
                if (!blankSpaces[i, j])
                {

                    Vector2 tempPos = new Vector2(i,j + offset);
                    GameObject backgroundTile = Instantiate(tilePrefab, tempPos , Quaternion.identity) as GameObject;
                    backgroundTile.transform.parent = this.transform;
                    backgroundTile.name = " ( " + i + "," + j + " ) ";
                    int dotToUse = Random.Range(0, Dots.Length);
               

                    int maxIteration = 0;
                    while (MatchesAt(i, j, Dots[dotToUse]) && maxIteration < 100)
                    {
                        dotToUse = Random.Range(0, Dots.Length);
                        maxIteration++;
                        Debug.Log(maxIteration);
                    }
                    maxIteration = 0;


                    GameObject dot = Instantiate(Dots[dotToUse], tempPos, Quaternion.identity) as GameObject;
                    dot.GetComponent<Dot>().row = j;
                    dot.GetComponent<Dot>().column = i;
                    dot.transform.parent = this.transform;
                    dot.name = " ( " + i + "," + j + " ) ";
                    allDots[i , j] = dot;
                }
            }
        }
    }

    private bool MatchesAt(int column,int row, GameObject piece)
    {
        if(column > 1 && row > 1)
        {
            if (allDots[column -1 , row] != null && allDots[column-2,row] != null)
            {

                if (allDots[column - 1, row].CompareTag(piece.tag) && allDots[column-2,row].CompareTag(piece.tag))
                {
                    return true;
                }
            }

            if (allDots[column,row -1] != null && allDots[column,row-2] != null)
            {

                if (allDots[column, row-1].CompareTag(piece.tag) && allDots[column, row-2].CompareTag(piece.tag))
                {
                    return true;
                }
            }
        }
        else if(column <= 1 ||  row <= 1)
        {

                if(row > 1)
                {
                    if (allDots[column,row -1] != null && allDots[column, row-2] != null)
                    {
                            if (allDots[column,row -1].CompareTag(piece.tag) && allDots[column,row - 2].CompareTag(piece.tag))
                            {
                                return true;
                            }
                        }
                } 
            if(column > 1)
            {
                if (allDots[column - 1, row] != null && allDots[column - 2, row] != null)
                {
                    if (allDots[column - 1, row].CompareTag(piece.tag) && allDots[column - 2, row].CompareTag(piece.tag))
                    {
                        return true;
                    }
                }
            }
        }
        return false; 
    }

    bool ColumnOrRow()
    {
        int numberHorizontal = 0;
        int numberVertical = 0;

        Dot firstpiece = findMatches.currenMatches[0].GetComponent<Dot>();
        if(firstpiece != null)
        {
            foreach(GameObject currentPiece in findMatches.currenMatches)
            {
                Dot dot = currentPiece.GetComponent<Dot>();
                if(dot.row == firstpiece.row)
                {
                    numberHorizontal++;
                }
                if(dot.column == firstpiece.column)
                {
                    numberVertical++;
                }
            }
        }

        return (numberVertical == 5 || numberHorizontal == 5);
    }

    void CheckToMakeBombs()
    {
        if(findMatches.currenMatches.Count == 4 || findMatches.currenMatches.Count == 7)
        {
            findMatches.CheckBombMatch();
        }

        if (findMatches.currenMatches.Count == 5 || findMatches.currenMatches.Count == 8)
        {
            if (ColumnOrRow())
            {
                //Make a ColorBomb
                //is the current dot matched ?
                if (currentDot != null)
                {
                    if (currentDot.isMatch)
                    {
                        if (!currentDot.isColorBomb)
                        {
                            currentDot.isMatch = false;
                            currentDot.MakeColorBomb();
                        }
                    }
                    else
                    {
                        if (currentDot.otherDot != null)
                        {
                            Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                            if (otherDot.isMatch)
                            {
                                if (!otherDot.isColorBomb)
                                {
                                    otherDot.isMatch = false;
                                    otherDot.MakeColorBomb();
                                }
                            }
                        }
                    }
                }
            }

            else
            {
                //make a adjacent bomb
                if (currentDot != null)
                {
                    if (currentDot.isMatch)
                    {
                        if (!currentDot.isAdjacentBomb)
                        {
                            currentDot.isMatch = false;
                            currentDot.MakeAdjacentBomb();
                        }
                    }
                    else
                    {
                        if (currentDot.otherDot != null)
                        {
                            Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                            if (otherDot.isMatch)
                            {
                                if (!otherDot.isAdjacentBomb)
                                {
                                    otherDot.isMatch = false;
                                    otherDot.MakeAdjacentBomb();
                                }
                            }
                        }
                    }
                }
            }
        }
      
    }
    private void DestroyMatchesAt(int column,int row)
    {
        if (allDots[column, row].GetComponent<Dot>().isMatch)
        {
            //Check How many pieces are Matched
            if(findMatches.currenMatches.Count >= 4)
            {
                CheckToMakeBombs();
            }
            
            
           GameObject particle = Instantiate(destroyEffect, allDots[column, row].transform.position,Quaternion.identity) as GameObject;
            Destroy(particle, 0.3f);
            Destroy(allDots[column, row]);
            allDots[column,row] = null;
        }
      
    }


    public void DestroMatches()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if (allDots[i,j] != null)
                {
                    DestroyMatchesAt(i,j);
                }
            } 
        }
        findMatches.currenMatches.Clear();
        StartCoroutine(DecreaseRowCo2());
    }

    IEnumerator DecreaseRowCo2()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                //if current spot isnt blank and is empty;
                if (!blankSpaces[i,j] && allDots[i,j] == null)
                {
                    //loop from space aboe to top of column
                    for(int  k = j+1; k < height; k++)
                    {
                        //if dot found
                        if (allDots[i,k] != null)
                        {
                            //move dot to empty space
                            allDots[i, k].GetComponent<Dot>().row = j;
                            //set spot to null
                            allDots[i,k] = null;
                            
                            break;
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoardCo());
    }
    public IEnumerator DecreaseRowCo()
    {
        int nullCount = 0;
        for(int i = 0;i < width; i++)
        {

            for(int j = 0;j < height; j++)
            {
                if (allDots[i,j] == null)
                {
                    nullCount++;
                }
                else if(nullCount > 0)
                {
                    allDots[i,j].GetComponent<Dot>().row -= nullCount;
                    allDots[i,j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(0.2f);
       StartCoroutine(FillBoardCo());
    }

    private IEnumerator FillBoardCo()
    {
        RefillBoard();
        yield return new WaitForSeconds(0.3f);
        while (MatchesonBoard())
        {
            yield return new WaitForSeconds(0.3f);
            DestroMatches();
        }
        findMatches.currenMatches.Clear();
        currentDot = null;
        yield return new WaitForSeconds(.3f);
        currenState = GameState.move;
    }

    private void RefillBoard()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if (allDots[i,j] == null && !blankSpaces[i,j])
                {
                    Vector2 tempPosition = new Vector2(i,j + offset);
                    int dotToUse = Random.Range(0,Dots.Length);
                    GameObject piece = Instantiate(Dots[dotToUse], tempPosition, Quaternion.identity) as GameObject;
                    allDots[i,j] = piece;
                    piece.GetComponent<Dot>().row = j;
                    piece.GetComponent<Dot>().column = i;
                }
            }
        }
    }

    private bool MatchesonBoard()
    {
        for( int i = 0; i < width; i++)
        {
            for (int j = 0;j < height; j++)
            {
                if (allDots[i,j] != null)
                {
                    if (allDots[i, j].GetComponent<Dot>().isMatch)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
