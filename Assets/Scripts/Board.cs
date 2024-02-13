using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    wait, 
    move
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

    BackgroundTile[,] allTiles;

    // Start is called before the first frame update
    void Start()
    {
        findMatches = FindAnyObjectByType<FindMatches>();
        allTiles = new BackgroundTile[width, height];
        allDots = new GameObject[width, height];
        Setup();
    }

   void Setup()
    {
        for (int i = 0; i < width; i++) 
        {
            for(int j = 0; j < height; j++)
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

    private bool MatchesAt(int column,int row, GameObject piece)
    {
        if(column > 1 && row > 1)
        {
            if (allDots[column - 1, row].CompareTag(piece.tag) && allDots[column-2,row].CompareTag(piece.tag))
            {
                return true;
            }
            if (allDots[column, row-1].CompareTag(piece.tag) && allDots[column, row-2].CompareTag(piece.tag))
            {
                return true;
            }
        }
        else if(column <= 1 ||  row <= 1)
        {
            if(row > 1)
            {
                if (allDots[column,row -1].CompareTag(piece.tag) && allDots[column,row - 2].CompareTag(piece.tag))
                {
                    return true;
                }
            } 
            if(column > 1)
            {
                if (allDots[column - 1,row].CompareTag(piece.tag) && allDots[column - 2, row ].CompareTag(piece.tag))
                {
                    return true;
                }
            }
        }
        return false; 
    }

    private void DestroyMatchesAt(int column,int row)
    {
        if (allDots[column, row].GetComponent<Dot>().isMatch)
        {
            //Check How many pieces are Matched
            if(findMatches.currenMatches.Count == 4 || findMatches.currenMatches.Count == 7)
            {
                findMatches.CheckBombMatch();
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
        StartCoroutine(DecreaseRowCo());
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
                if (allDots[i,j] == null)
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
