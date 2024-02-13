using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                    if(i > 0 && i<board.width - 1)
                    {
                        GameObject leftDot = board.allDots[i - 1,j];
                        GameObject rightDot = board.allDots[i+1,j];
                        if (leftDot != null && rightDot != null)
                        {
                            if(leftDot.tag == currentDot.tag && rightDot.tag == currentDot.tag) 
                            {
                                leftDot.GetComponent<Dot>().isMatch = true;
                                rightDot.GetComponent<Dot>().isMatch = true;
                                currentDot.GetComponent<Dot>().isMatch = true;
                            }
                        }
                    }
                    if (j > 0 && j < board.height - 1)
                    {
                        GameObject upDot = board.allDots[i, j + 1];
                        GameObject downDot = board.allDots[i, j -1 ];
                        if (upDot != null && downDot != null)
                        {
                            if (upDot.tag == currentDot.tag && downDot.tag == currentDot.tag)
                            {
                                upDot.GetComponent<Dot>().isMatch = true;
                                downDot.GetComponent<Dot>().isMatch = true;
                                currentDot.GetComponent<Dot>().isMatch = true;
                            }
                        }
                    }
                }
            }
        }
    }
}
