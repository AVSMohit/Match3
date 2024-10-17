
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Security.Cryptography;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public enum GameState
{
    wait, 
    move,
    win,
    lose,
    pause
}
public enum TypeOfTile
{
    Breakable,
    Blank,
    Lock,
    Concrete,
    Slime,
    Normal
}

[System.Serializable]
public class MatchType
{
    public int type;
    public string color;
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
    public MatchType matchType;

    [Header("Scriptable Objects")]
    public World world;
    public int level;

    [Header("board dimension")]
    public int width;
    public int height;
    public int offset;
    public int basePieceValue = 20;
    public int[] scoreGoals;
    int streakValue = 1;

    [Header("prefabs")]
    public GameObject tilePrefab;
    public GameObject breakableTilePrefab;
    public GameObject LockTilePrefab;
    public GameObject ConcreteTilePrefab;
    public GameObject SlimeTilePrefab;
    public GameObject[] Dots;
    public GameObject destroyEffect;

    public float refillDelay = 0.5f;

    ScoreManager scoreManager;
    GoalManager goalManager;
    SoundManager soundManager;

    [Header("layout")]
    public Dot currentDot;
    public TileType[] boardLayout;
    bool[,] blankSpaces;
    BackgroundTile[,] breakbleTiles;
    public BackgroundTile[,] lockTiles;
    BackgroundTile[,] concreteTiles;
    BackgroundTile[,] slimeTiles;
    public GameObject[,] allDots;

    bool makeSlime = false;

    // Start is called before the first frame update
    private void Awake()
    {
        if (PlayerPrefs.HasKey("Current Level"))
        {
            level = PlayerPrefs.GetInt("Current Level");
        }
        if (world != null)
        {
            if (level < world.levels.Length)
            {

                if (world.levels[level] != null)
                {
                    width = world.levels[level].width;
                    height = world.levels[level].height;
                    Dots = world.levels[level].dots;
                    scoreGoals = world.levels[level].scoreGoals;
                    boardLayout = world.levels[level].boardLayout;
                }
            }
        }
    }

    void Start()
    {
        goalManager = FindObjectOfType<GoalManager>();
        scoreManager = FindObjectOfType<ScoreManager>();
        findMatches = FindObjectOfType<FindMatches>();
        soundManager = FindObjectOfType<SoundManager>();
        blankSpaces = new bool[width, height];
        allDots = new GameObject[width, height];
        breakbleTiles = new BackgroundTile[width, height];
        lockTiles = new BackgroundTile[width, height];
        concreteTiles = new BackgroundTile[width, height];
        slimeTiles = new BackgroundTile[width, height];
        Setup();
        currenState = GameState.pause;
    }

    public void GenerateBlankSpace()
    {
        for (int i = 0; i < boardLayout.Length; i++)
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
        for (int i = 0; i < boardLayout.Length; i++)
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

    void GenerateLockTiles()
    {
        //Lookat all tiles in layout
        for (int i = 0; i < boardLayout.Length; i++)
        {
            //if tile is a "Lock" tile
            if (boardLayout[i].typeOfTile == TypeOfTile.Lock)
            {

                //Create a jelly tile at that position
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(LockTilePrefab, tempPosition, Quaternion.identity);
                lockTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();

            }
        }
    }
    void GenerateLConcreteTiles()
    {
        //Lookat all tiles in layout
        for (int i = 0; i < boardLayout.Length; i++)
        {
            //if tile is a "Lock" tile
            if (boardLayout[i].typeOfTile == TypeOfTile.Concrete)
            {

                //Create a jelly tile at that position
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(ConcreteTilePrefab, tempPosition, Quaternion.identity);
                concreteTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();

            }
        }
    } 
    
    void GenerateSlimeTiles()
    {
        //Lookat all tiles in layout
        for (int i = 0; i < boardLayout.Length; i++)
        {
            //if tile is a "Lock" tile
            if (boardLayout[i].typeOfTile == TypeOfTile.Slime)
            {

                //Create a jelly tile at that position
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(SlimeTilePrefab, tempPosition, Quaternion.identity);
                slimeTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();

            }
        }
    }
    void Setup()
    {
        GenerateBlankSpace();
        GenerateBreakcaleTiles();
        GenerateLockTiles();
        GenerateLConcreteTiles();
        GenerateSlimeTiles();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!blankSpaces[i, j] && !concreteTiles[i,j] && !slimeTiles[i,j])
                {

                    Vector2 tempPos = new Vector2(i, j + offset);
                    Vector2 tilePosition = new Vector2(i, j);
                    GameObject backgroundTile = Instantiate(tilePrefab, tilePosition, Quaternion.identity) as GameObject;
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
                    allDots[i, j] = dot;
                }
            }
        }
    }

    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if (column > 1 && row > 1)
        {
            if (allDots[column - 1, row] != null && allDots[column - 2, row] != null)
            {

                if (allDots[column - 1, row].CompareTag(piece.tag) && allDots[column - 2, row].CompareTag(piece.tag))
                {
                    return true;
                }
            }

            if (allDots[column, row - 1] != null && allDots[column, row - 2] != null)
            {

                if (allDots[column, row - 1].CompareTag(piece.tag) && allDots[column, row - 2].CompareTag(piece.tag))
                {
                    return true;
                }
            }
        }
        else if (column <= 1 || row <= 1)
        {

            if (row > 1)
            {
                if (allDots[column, row - 1] != null && allDots[column, row - 2] != null)
                {
                    if (allDots[column, row - 1].CompareTag(piece.tag) && allDots[column, row - 2].CompareTag(piece.tag))
                    {
                        return true;
                    }
                }
            }
            if (column > 1)
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

    MatchType ColumnOrRow()
    {
        //make copy of current matches

        List<GameObject> matchCopy = findMatches.currenMatches as List<GameObject>;

        matchType.type = 0;
        matchType.color = "";
        //Cycle Through all of match copy and decide if a bomb need to be made
        for (int i = 0; i < matchCopy.Count; i++)
        {
            Dot thisDot = matchCopy[i].GetComponent<Dot>();
            string color = matchCopy[i].tag;
            int column = thisDot.column;
            int row = thisDot.row;
            int columnMatch = 0;
            int rowMatch = 0;

            for (int j = 0; j < matchCopy.Count; j++)
            {
                Dot nextDot = matchCopy[j].GetComponent<Dot>();
                if (nextDot == thisDot)
                {
                    continue;
                }
                if (nextDot.column == thisDot.column && nextDot.tag == color)
                {
                    columnMatch++;
                }
                if (nextDot.row == thisDot.row && nextDot.tag == color)
                {
                    rowMatch++;
                }

            }

            if (columnMatch == 4 || rowMatch == 4)
            {
                matchType.type = 1;
                matchType.color = color;
                return matchType;
            }
           else if (columnMatch == 2 || rowMatch == 2)
            {
                matchType.type = 2;
                matchType.color = color;
                return matchType;
            }
            else if (columnMatch == 3 || rowMatch == 3)
            {
                matchType.type = 3;
                matchType.color = color;
                return matchType;
            }
        }
        matchType.type = 0;
        matchType.color = "";
        return matchType;

    }

    void CheckToMakeBombs()
    {
        if (findMatches.currenMatches.Count > 3)
        {

            MatchType typeOfMatch = ColumnOrRow();
            if (typeOfMatch.type == 1)
            {
                //Make a ColorBomb
                //is the current dot matched ?
                if (currentDot != null && currentDot.isMatch && currentDot.tag == typeOfMatch.color)
                {

                    currentDot.isMatch = false;
                    currentDot.MakeColorBomb();
                }
                else
                {
                    if (currentDot.otherDot != null)
                    {
                        Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                        if (otherDot.isMatch && currentDot.tag == typeOfMatch.color)
                        {
                            otherDot.isMatch = false;
                            otherDot.MakeColorBomb();
                        }
                    }

                }
            }
            else if (typeOfMatch.type == 2)
            {
                //make a adjacent bomb
                if (currentDot != null && currentDot.isMatch && currentDot.tag == typeOfMatch.color)
                {
                    currentDot.isMatch = false;
                    currentDot.MakeAdjacentBomb();
                }
                else if (currentDot.otherDot != null)
                {
                    
                        Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                        if (otherDot.isMatch && otherDot.tag == typeOfMatch.color)
                        {
                           
                                otherDot.isMatch = false;
                                otherDot.MakeAdjacentBomb();
                            
                        }
                    
                }
            }
            else if (typeOfMatch.type == 3)
            {
                findMatches.CheckBombMatch(typeOfMatch);
            }
        }
    }
           
   public void BombRow(int row)
    {
        for (int i = 0; i < width; i++)
        {
           
            if (concreteTiles[i, row] != null) 
            {
                concreteTiles[i, row].TakeDamage(1);
                if (concreteTiles[i, row].hitPoints <= 0)
                {
                    concreteTiles[i, row] = null;
                }
            }
            
        }
   }

    public void BombColumn(int column) 
    {
        for (int i = 0; i < width; i++)
        {
          
            if (concreteTiles[column,i] != null)
            {
                concreteTiles[column, i].TakeDamage(1);
                if (concreteTiles[column, i].hitPoints <= 0)
                {
                    concreteTiles[column, i] = null;
                }
            }
            
        }
    }

    private void DestroyMatchesAt(int column,int row)
    {
        if (allDots[column, row].GetComponent<Dot>().isMatch)
        {
            //Check How many pieces are Matched
           
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
            if (lockTiles[column, row] != null)
            {
                //if it does give one damage;
                lockTiles[column, row].TakeDamage(1);
                if (lockTiles[column, row].hitPoints <= 0)
                {
                    lockTiles[column, row] = null;
                }

            }

            DamageConcrete(column,row);
            DamageSlime(column,row);

            if (goalManager != null)
            {
                goalManager.CompareGoal(allDots[column, row].tag.ToString());
                goalManager.UpdateGoals();
            }
            if (soundManager != null)
            {
                soundManager.PlayRandomDestroyNoise();
            }
            GameObject particle = Instantiate(destroyEffect, allDots[column, row].transform.position,Quaternion.identity) as GameObject;
            Destroy(particle, 0.3f);
            Destroy(allDots[column, row]);
            scoreManager.IncreaseScore(basePieceValue * streakValue);
            allDots[column,row] = null;
        }
      
    }


    public void DestroMatches()
    {
        if (findMatches.currenMatches.Count >= 4)
        {
            CheckToMakeBombs();
        }
        findMatches.currenMatches.Clear();
        for (int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if (allDots[i,j] != null)
                {
                    DestroyMatchesAt(i,j);
                }
            } 
        }
        
        StartCoroutine(DecreaseRowCo2());
    }

    private void DamageConcrete(int column,int row)
    {
        if (column > 0)
        { 
            if(concreteTiles[column - 1, row] != null)
            {
                concreteTiles[column - 1, row].TakeDamage(1);
                if (concreteTiles[column-1, row].hitPoints <= 0)
                {
                    concreteTiles[column -1 , row] = null;
                }
            }
        }
        if (column < width -1)
        {
            if (concreteTiles[column + 1, row] != null)
            {
                concreteTiles[column + 1, row].TakeDamage(1);
                if (concreteTiles[column + 1, row].hitPoints <= 0)
                {
                    concreteTiles[column + 1, row] = null;
                }
            }
        }
        if (row > 0)
        {
            if (concreteTiles[column , row -1 ] != null)
            {
                concreteTiles[column, row -1 ].TakeDamage(1);
                if (concreteTiles[column , row - 1].hitPoints <= 0)
                {
                    concreteTiles[column , row - 1] = null;
                }
            }
        }
        if (row < height - 1)
        {
            if (concreteTiles[column, row +1] != null)
            {
                concreteTiles[column, row + 1].TakeDamage(1);
                if (concreteTiles[column , row + 1].hitPoints <= 0)
                {
                    concreteTiles[column , row + 1] = null;
                }
            }
        }

    } 
    private void DamageSlime (int column,int row)
    {
        if (column > 0)
        { 
            if(slimeTiles[column - 1, row] != null)
            {
                slimeTiles[column - 1, row].TakeDamage(1);
                if (slimeTiles[column-1, row].hitPoints <= 0)
                {
                    slimeTiles[column -1 , row] = null;
                }
                makeSlime = false;
            }
        }
        if (column < width -1)
        {
            if (slimeTiles[column + 1, row] != null)
            {
                slimeTiles[column + 1, row].TakeDamage(1);
                if (slimeTiles[column + 1, row].hitPoints <= 0)
                {
                    slimeTiles[column + 1, row] = null;
                }
                makeSlime = false;
            }
        }
        if (row > 0)
        {
            if (slimeTiles[column , row -1 ] != null)
            {
                slimeTiles[column, row -1 ].TakeDamage(1);
                if (slimeTiles[column , row - 1].hitPoints <= 0)
                {
                    slimeTiles[column , row - 1] = null;
                }
                makeSlime = false;
            }
        }
        if (row < height - 1)
        {
            if (slimeTiles[column, row +1] != null)
            {
                slimeTiles[column, row + 1].TakeDamage(1);
                if (slimeTiles[column , row + 1].hitPoints <= 0)
                {
                    slimeTiles[column , row + 1] = null;
                }
                makeSlime = false;
            }
        }

    }

    void CheckToMakeSlime()
    {
        for (int i = 0; i < width; i++) 
        {
            for (int j = 0; j < height; j++)
            {
                if(slimeTiles[i, j] != null && makeSlime)
                {
                    MakeNewSlime();
                    return;
                }
            }
        }
        
    }

    Vector2 CheckForAdjacentPiece(int column,int row)
    {
        if (column < width - 1 && allDots[column + 1, row])
        {
            return Vector2.right;
        }
        if (column > 0 && allDots[column - 1, row])
        {
            return Vector2.left;
        }
        if (row < height - 1 && allDots[column, row + 1])
        {
            return Vector2.up;
        }
        if (row > 0 && allDots[column, row - 1])
        {
            return Vector2.down;
        }

        return Vector2.zero;
    }

    void MakeNewSlime()
    {
        bool slime = false;
        int loop = 0;
        while (!slime && loop < 200) 
        {
            int newX = Random.Range(0, width);
            int newY = Random.Range(0, height);
            if (slimeTiles[newX, newY]) 
            {
                Vector2 adjacent = CheckForAdjacentPiece(newX,newY);
                if(adjacent != Vector2.zero)
                {
                    Destroy(allDots[newX + (int)adjacent.x, newY + (int)adjacent.y]);
                    Vector2 tempPosition = new Vector2(newX + (int)adjacent.x, newY + (int)adjacent.y);
                    GameObject tile = Instantiate(SlimeTilePrefab,tempPosition,Quaternion.identity);
                    slimeTiles[newX + (int)adjacent.x, newY + (int)adjacent.y] = tile.GetComponent<BackgroundTile>();
                    slime = true;
                }
            }
            loop++;
        }
    }
    IEnumerator DecreaseRowCo2()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                //if current spot isnt blank and is empty;
                if (!blankSpaces[i,j] && allDots[i,j] == null && !concreteTiles[i,j] && !slimeTiles[i,j] )
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
        yield return new WaitForSeconds(refillDelay  * 0.5f);
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
        yield return new WaitForSeconds(refillDelay * 0.5f);
       StartCoroutine(FillBoardCo());
    }

    private IEnumerator FillBoardCo()
    {
        yield return new WaitForSeconds(refillDelay);
        RefillBoard();
        yield return new WaitForSeconds(refillDelay);
        while (MatchesonBoard())
        {
            streakValue++;
            DestroMatches();
            yield break;
            //yield return new WaitForSeconds(refillDelay * 2);
        }
        currentDot = null;
        CheckToMakeSlime();
        if (isDeadlocked())
        {
            StartCoroutine(ShuffleBoard());
        }
        if(currenState != GameState.pause)
        {
            currenState = GameState.move;
        }
        makeSlime = true;
        streakValue = 1;
    }

    private void RefillBoard()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if (allDots[i,j] == null && !blankSpaces[i,j] && !concreteTiles[i,j] && !slimeTiles[i,j])
                {
                    Vector2 tempPosition = new Vector2(i,j + offset);
                    int dotToUse = Random.Range(0,Dots.Length);
                    int maxIteration = 0;
                    while (MatchesAt(i, j, Dots[dotToUse]) && maxIteration < 100)
                    {
                        maxIteration++;
                        dotToUse = Random.Range(0, Dots.Length);
                    }
                    maxIteration = 0;
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
        findMatches.FindAllMatches();
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
        if (allDots[column + (int)direction.x, row + (int)direction.y] != null)
        {
            //Save second piece in a holder
            GameObject holder = allDots[column + (int)direction.x, row + (int)direction.y] as GameObject;
            //Switching the first dot to be the second dot
            allDots[column + (int)direction.x,row + (int)direction.y] = allDots[column,row];
            //Set the first dot to be the second dot
            allDots[column,row] = holder;
        }


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

    IEnumerator ShuffleBoard()
    {
        yield return new WaitForSeconds(0.5f);
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
                if (!blankSpaces[i, j] && !concreteTiles[i,j] && !slimeTiles[i,j])
                {
                    //pick a number
                    int pieceToUse = Random.Range(0, newBoard.Count);

                    //assign collumn and row

                    int maxIteration = 0;
                    while (MatchesAt(i, j, newBoard[pieceToUse]) && maxIteration < 100)
                    {
                        pieceToUse = Random.Range(0, newBoard.Count);
                        maxIteration++;
                        //Debug.Log(maxIteration);
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
          StartCoroutine(ShuffleBoard());
        }
    }

    
    #endregion
}
