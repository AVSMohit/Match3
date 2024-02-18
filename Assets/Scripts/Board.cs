
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography;
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
    public GameObject breakableTilePrefab;
    public GameObject[] Dots;
    public GameObject destroyEffect;
    public GameObject[,] allDots;



    public Dot currentDot;

    bool[,] blankSpaces;
    BackgroundTile[,] breakbleTiles;
    
    public TileType[] boardLayout;

    // Start is called before the first frame update
    void Start()
    {
        findMatches = FindObjectOfType<FindMatches>();
        blankSpaces = new bool[width, height];
        allDots = new GameObject[width, height];
        breakbleTiles = new BackgroundTile[width, height];
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

    public void GenerateBreakcaleTiles()
    {

        //Lookat all tiles in layout
        for(int i = 0;i < boardLayout.Length; i++)
        {
            //if tile is a jelly tile
            if (boardLayout[i].typeOfTile == TypeOfTile.Breakable)
            {

                //Create a jelly tile at that position
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(breakableTilePrefab, tempPosition, Quaternion.identity);
                breakbleTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();

            }
        }
    }

   void Setup()
    {
        GenerateBlankSpace();
        GenerateBreakcaleTiles();
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
            //Does a tile need to break?
            if (breakbleTiles[column,row] != null)
            {
                //if it does give one damage;
                breakbleTiles[column, row].TakeDamage(1);
                if (breakbleTiles[column,row].hitPoints <= 0)
                {
                    breakbleTiles[column, row] = null;
                }

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
        if (isDeadlocked())
        {
            ShuffleBoard();
        }
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

    #region CheckDeadlock and Shuffle
    private void SwitchPieces(int column,int row,Vector2 direction)
    {
        //Save second piece in a holder
        GameObject holder = allDots[column + (int)direction.x, row + (int)direction.y] as GameObject;
        //Switching the first dot to be the second dot
        allDots[column + (int)direction.x,row + (int)direction.y] = allDots[column,row];
        //Set the first dot to be the second dot
        allDots[column,row] = holder;


    }

    bool CheckForMatches()
    {
        for(int i = 0; i < width;i++)
        {
            for(int j = 0  ; j < height; j++)
            {
                if (allDots[i,j] != null)
                {
                    //Makin Sure one and two to right are in the board
                    if(i < width - 2)
                    {
                        //Check the dot to right and two to right exist
                        if (allDots[i+1,j] != null && allDots[i+2,j] != null)
                        {
                            if (allDots[i+1,j].tag == allDots[i,j].tag && allDots[i+2,j].tag == allDots[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }
                    if(j < height - 2)
                    {
                            //check if above dots exist
                        if (allDots[i,j+1] != null && allDots[i,j+2] != null)
                        {
                                if (allDots[i ,j +1].tag == allDots[i, j].tag && allDots[i , j +2].tag == allDots[i, j].tag)
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

   public bool SwitchAndCheck(int colum,int row, Vector2 direction)
    {
        SwitchPieces(colum,row,direction);
        if (CheckForMatches())
        {
            SwitchPieces(colum, row,direction);
            return true;
        }
        SwitchPieces(colum, row,direction);
        return false;
    }

    bool isDeadlocked()
    {
        for(int i = 0; i < width;i++)
        {
            for(int j = 0; j < height; j++)
            {
                if (allDots[i,j] != null)
                {
                    if(i < width - 1)
                    {
                       if( SwitchAndCheck(i, j, Vector2.right))
                        {
                            return false;
                        }
                    }
                    if(j < height - 1)
                    {
                        if(SwitchAndCheck(i, j, Vector2.up))
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }

    void ShuffleBoard()
    {
        //Create a list of Game Objects
        List<GameObject> newBoard = new List<GameObject>();

        //Add everypieace to list

        for(int i = 0; i < width; i++)
        {
            for(int j = 0;j < height; j++)
            {
                if (allDots[i,j] != null)
                {
                    newBoard.Add(allDots[i,j]);
                }
            }
        }

        for(int i = 0;i < width; i++)
        {
            for(int j=0;j < height; j++)
            {
                //if this spot shouldnt be a blank
                if (!blankSpaces[i, j])
                {
                    //pick a number
                    int pieceToUse = Random.Range(0, newBoard.Count);

                    //assign collumn and row

                    int maxIteration = 0;
                    while (MatchesAt(i, j, newBoard[pieceToUse]) && maxIteration < 100)
                    {
                        pieceToUse = Random.Range(0, newBoard.Count);
                        maxIteration++;
                        Debug.Log(maxIteration);
                    }

                    //container for piece
                    Dot piece = newBoard[pieceToUse].GetComponent<Dot>();
                    maxIteration = 0;
                    piece.column = i;
                    piece.row = j;

                    //fill the dots array
                    allDots[i,j] = newBoard[pieceToUse];

                    //remove from list
                    newBoard.Remove(newBoard[pieceToUse]);
                }
            }
        }
        //check if still deadlocks
        if (isDeadlocked())
        {
            ShuffleBoard();
        }
    }

    
    #endregion
}
