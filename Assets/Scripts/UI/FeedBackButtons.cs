using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FeedBackButtons : MonoBehaviour
{
    public string sceneToLoad;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FeedBackRecieved()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    IEnumerator GameStartCo()
    {
        yield return new WaitForSeconds(1);
        Board board = FindObjectOfType<Board>();
        board.currenState = GameState.move;
    }
}
