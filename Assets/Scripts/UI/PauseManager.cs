using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine.Rendering;

public class PauseManager : MonoBehaviour
{
    public GameObject PausePanel;
    Board board;
    public bool paused=false;
    public Button pauseButton;
    public Button exitButton;
    public Button soundButton;

    public Sprite musicOnSprite;
    public Sprite musicOffSprite;

    public SoundManager soundManager;
    // Start is called before the first frame update
    void Start()
    {
        //In player perfs the "Sound" key is for sound 
        //if sound == 0 then mute, ==1 then unmute
        if (PlayerPrefs.HasKey("Sound"))
        {
            if(PlayerPrefs.GetInt("Sound") == 0)
            {
                soundButton.image.sprite = musicOffSprite;
            }
            else
            {
                soundButton.image.sprite = musicOnSprite;
            }
        }
        else
        {
            soundButton.image.sprite = musicOnSprite;
        }

        PausePanel.SetActive(false);
        board = GameObject.FindGameObjectWithTag("Board").GetComponent<Board>();
        pauseButton.onClick.AddListener(PauseGame);
        exitButton.onClick.AddListener(ExitGame);
        soundButton.onClick.AddListener(SoundButton);
    }

    // Update is called once per frame
    void Update()
    {
        if (paused && !PausePanel.activeInHierarchy)
        {
            PausePanel.SetActive(true);
            board.currenState = GameState.pause;

        }
        if(!paused && PausePanel.activeInHierarchy)
        {
            PausePanel.SetActive(false);
            board.currenState = GameState.move;
        }
    }

    public void SoundButton()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
            {
                soundButton.image.sprite = musicOnSprite;
                PlayerPrefs.SetInt("Sound", 1);
            }
            else
            {
                soundButton.image.sprite = musicOffSprite;
                PlayerPrefs.SetInt("Sound", 0);
            }
        }
        else
        {
            soundButton.image.sprite = musicOffSprite;
            PlayerPrefs.SetInt("Sound", 1);
        }
        soundManager.AdjustVolume();
    }

    public void PauseGame()
    {
        paused = !paused;
    }

    void ExitGame()
    {
        SceneManager.LoadScene("Splash");
    }
}
