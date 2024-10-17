using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ConfirmPanel : MonoBehaviour
{
    public string levelToLoad;
    public Image[] stars;
    int starsActive;
    public int Level;

    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI starsText;
    int highScore;

    GameData gameData;
    // Start is called before the first frame update
    void OnEnable()
    {
        gameData = FindObjectOfType<GameData>();
        LoadData();
        ActivateStars();
        SetText();
    }

    void LoadData()
    {
        if(gameData != null)
        {
            starsActive = gameData.saveData.stars[Level-1];
            highScore = gameData.saveData.highScore[Level-1];
        }
    }

   void SetText()
    {
        highScoreText.text = "" + highScore;
        starsText.text = "" + starsActive + "/3";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Cancel()
    {
        this.gameObject.SetActive(false);
    }

    public void Play()
    {
        PlayerPrefs.SetInt("Current Level",Level - 1);
        SceneManager.LoadScene(levelToLoad);
    }

    void ActivateStars()
    {

        //Come Back to This when Binary file is done
        for (int i = 0; i < starsActive; i++)
        {
            stars[i].enabled = true;
        }
    }
}
