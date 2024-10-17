using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStartManager : MonoBehaviour
{
    public GameObject StartPanel;
    public GameObject LevelPanel;
    [SerializeField] Button startGameButton;    
    [SerializeField] Button homeGameButton;    
    
    // Start is called before the first frame update
    void Start()
    {
        StartPanel.SetActive(true);
        LevelPanel.SetActive(false);

        startGameButton.onClick.AddListener(PlayGame);
        homeGameButton.onClick.AddListener(HomeButton);
    }

    public void PlayGame()
    {
        StartPanel.SetActive(false);
        LevelPanel.SetActive(true);
    }

    void HomeButton()
    {
        StartPanel.SetActive(true);
        LevelPanel.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
