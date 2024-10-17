using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButton : MonoBehaviour
{
    [Header("Acive Stuff")]
    public bool isActive;
    public Sprite activeSprite;
    public Sprite lockedSprite;
    int starsActive;
    Image buttonImage;
    Button button;

    [Header("level Ui")]
    public Image[] stars;
    public TextMeshProUGUI levelText;
    public int level;
    public GameObject confirmPanel;
    


    GameData gameData;

    // Start is called before the first frame update
    void Start()
    {
        gameData = FindObjectOfType<GameData>();
        buttonImage = GetComponent<Image>();
        button = GetComponent<Button>();
        LoadData();
        ActivateStars();
        ShowLevel();
        DecideSprite();
    }

    void LoadData()
    {
        //is gamedata present?
        if(gameData  != null)
        {
            //decide level active
            if (gameData.saveData.isAcive[level - 1])
            {
                isActive = true;
            }
            else
            {
                isActive = false;
            }

            //Stars to activat?
            starsActive = gameData.saveData.stars[level - 1];
        }
    }

    void ActivateStars()
    {

        
        for(int i = 0; i < starsActive; i++)
        {

            stars[i].enabled = true;
        }
    }
    void DecideSprite()
    {
        if (isActive)
        {
            buttonImage.sprite = activeSprite;
            button.enabled = true;
            levelText.enabled = true;
        }
        else
        {
            buttonImage.sprite = lockedSprite;
            button.enabled = false;
            levelText.enabled = false;
        }
    }

    void ShowLevel()
    {
        levelText.text = "" + level;
    }

    public  void ConfirmPanel(int level)
    {
        confirmPanel.GetComponent<ConfirmPanel>().Level = level;
        confirmPanel.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
