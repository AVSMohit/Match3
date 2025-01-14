using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintManager : MonoBehaviour
{
    
    Board board;
    public float hintDelay;
    private float hintDelaySeconds;
    public GameObject hintParticle;
    public GameObject currentHint;
    public Toggle hintToggle;
    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        hintDelaySeconds = hintDelay;
    }

    // Update is called once per frame
    void Update()
    {
        if(hintToggle.isOn)
        {
            hintDelaySeconds -= Time.deltaTime;
            if (hintDelaySeconds <= 0 && currentHint == null)
            {
                CreateHint();
                hintDelaySeconds = hintDelay;
            }
        }
       
    }

    //Find all possible matches first
    List<GameObject> FindAllMatches()
    {
        List<GameObject> possibleMoves = new List<GameObject>();

        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                if (board.allDots[i, j] != null)
                {
                    if (i < board.width - 1)
                    {
                        if (board.SwitchAndCheck(i, j, Vector2.right))
                        {
                            possibleMoves.Add(board.allDots[i,j]);
                        }
                    }
                    if (j < board.height - 1)
                    {
                        if (board.SwitchAndCheck(i, j, Vector2.up))
                        {
                            possibleMoves.Add(board.allDots[i, j]);
                        }
                    }
                }
            }
        }
        return possibleMoves;
    }
    //pick one match randomly
    GameObject PickOneRandomly()
    {
        List<GameObject> possibleMoves = new List<GameObject> ();
        possibleMoves = FindAllMatches();
        if(possibleMoves.Count > 0)
        {
            int pieceToUSe = Random.Range(0, possibleMoves.Count);

            return possibleMoves[pieceToUSe];
        }

        return null;
    }
    //create hint behind the chosen match
    void CreateHint()
    {
        GameObject move = PickOneRandomly();
        if(move != null)
        {
            currentHint = Instantiate(hintParticle,move.transform.position, Quaternion.identity);

        }
    }
    //destroy hint
  public void DestroyHint()
    {
        if(currentHint != null)
        {
            Destroy(currentHint);
            currentHint = null;
            hintDelaySeconds = hintDelay;
        }
    }

}
