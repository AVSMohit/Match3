using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;

public enum GameType
{
    Moves,
    Time,
    
}

[System.Serializable]
public class EndGameRequirements
{
    public GameType gameType;
    public int counterValue;
}
public class EndGameManager : MonoBehaviour
{

    
    public EndGameRequirements requirements;

    public GameObject movesTextObject;
    public GameObject timeTextObject;
    public GameObject youWinPanel;
    public GameObject tryAgainPanel;

    public TextMeshProUGUI counterText;

    public int currentCounterValue;

    float timerSeconds;

    Board board;
    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        currentCounterValue = requirements.counterValue;
        SetGameType();
        SetupGame();
    }

    void SetGameType()
    {
        if(board.world != null)
        {
            if (board.level < board.world.levels.Length)
            {
                if (board.world.levels != null)
                {
                    requirements = board.world.levels[board.level].endGameRequirements;
                }
            }
        }
    }

    void SetupGame()
    {
        timerSeconds = 1;
        if(requirements.gameType == GameType.Moves)
        {
            movesTextObject.SetActive(true);
            timeTextObject.SetActive(false);
        }
        else
        {
            movesTextObject.SetActive(false);
            timeTextObject.SetActive(true);
        }

        currentCounterValue = board.world.levels[board.level].endGameRequirements.counterValue;
        counterText.text = "" + currentCounterValue;
    }

    public void DecreaseCounterValue()
    {
       if(board.currenState != GameState.pause)
        {
            currentCounterValue--;
            counterText.text = "" + currentCounterValue;
             if(currentCounterValue <= 0)
             {
                LoseGame();
             }
        }
        
    }
    // Update is called once per frame
    void Update()
    {
        if(requirements.gameType == GameType.Time && currentCounterValue > 0)
        {
            timerSeconds -= Time.deltaTime;
            if(timerSeconds <= 0)
            {
                DecreaseCounterValue();
                timerSeconds = 1;
            }
        }
    }

    public void WinGame()
    {
        youWinPanel.SetActive(true);
        board.currenState = GameState.win;
        currentCounterValue = 0;
        counterText.text = "" + currentCounterValue;
        FadePanelController fade = FindObjectOfType<FadePanelController>();
        fade.GameOver();
    }

    public void LoseGame()
    {
        tryAgainPanel.SetActive(true);
        board.currenState = GameState.lose;
        Debug.Log("You Lose!!");
        currentCounterValue = 0;
        counterText.text = "" + currentCounterValue;
        FadePanelController fade = FindObjectOfType<FadePanelController>();
        fade.GameOver();
    }
         
}
