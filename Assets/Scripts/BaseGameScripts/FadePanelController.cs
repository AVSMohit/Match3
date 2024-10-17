using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadePanelController : MonoBehaviour
{
    public Animator PanelAnim;
    public Animator gameInfoAnim;
  

    public void Ok()
    {
        if (PanelAnim != null && gameInfoAnim != null)
        {

            PanelAnim.SetBool("Out", true);
            gameInfoAnim.SetBool("Out", true);
           
            StartCoroutine(GameStartCo());
        }
    }

    public void GameOver()
    {
        PanelAnim.SetBool("Out", false);
        PanelAnim.SetBool("GameOver", true);
    }

    IEnumerator GameStartCo()
    {
        yield return new WaitForSeconds(1);
        Board board = FindObjectOfType<Board>();
        board.currenState = GameState.move;
    }
}
