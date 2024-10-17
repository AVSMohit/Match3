using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Globalization;

public class ScoreManager : MonoBehaviour
{
    Board board;
    public TextMeshProUGUI scoreText;
    public int score;
    public Image scoreFill;
    GameData gameData;

    int numberStars;
    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        gameData = FindObjectOfType<GameData>();
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = "" + score;
    }

    public void IncreaseScore(int amountToIncrease)
    {
        score += amountToIncrease;

        for(int i = 0; i < board.scoreGoals.Length; i++)
        {
            if(score > board.scoreGoals[i] && numberStars < i + 1)
            {
                numberStars++;
            }
        }
        if(gameData  != null )
        {
            int highscore = gameData.saveData.highScore[board.level];
            if(score > highscore)
            {
                gameData.saveData.highScore[board.level] = score;

            }
            int currentStars = gameData.saveData.stars[board.level];
            if(numberStars > currentStars)
            {
                gameData.saveData.stars[board.level] = numberStars;

            }
            gameData.Save();
        }
        UpdateBar();
    }

    
    void UpdateBar()
    {
        if (board != null && scoreFill != null)
        {
            int length = board.scoreGoals.Length;
            scoreFill.fillAmount = (float)score / (float)board.scoreGoals[length - 1];
        }
    }
}
