using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class BackToSplash : MonoBehaviour
{
    public string sceneToLoad;
    GameData gameData;
    Board board;
    public GameObject feedBAckPanel;
    public void WinOk()
    {
        if(gameData != null)
        {
            gameData.saveData.isAcive[board.level + 1] = true;
            gameData.Save();
        }
        feedBAckPanel.SetActive(true);
    } 
    
    public void LoseOk()
    {
        
        SceneManager.LoadScene(sceneToLoad);
    }
    // Start is called before the first frame update
    void Start()
    {
        gameData = FindObjectOfType<GameData>();
        board = FindObjectOfType<Board>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
