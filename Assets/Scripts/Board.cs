using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width;
    public int height;
    public GameObject tilePrefab;
    public GameObject[] Dots;

    BackgroundTile[,] allTiles;
    public GameObject[,] allDots;

    // Start is called before the first frame update
    void Start()
    {
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
                Vector2 tempPos = new Vector2(i,j);
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
    }
}
