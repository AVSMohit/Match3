using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource[] destroyNoise;
    public AudioSource bgMusic;
    
    private void Start()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
            {
                bgMusic.Play();
                bgMusic.volume = 0;
            }
            else
            {
                bgMusic.Play();
                bgMusic.volume = 1;
            }
        }
        else
        {
            bgMusic.Play();
            bgMusic.volume = 1;
        }
    }

    public void AdjustVolume()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
            {
                bgMusic.volume = 0;
            }
            else
            {
                bgMusic.volume =1;
            }
        }
    }
    public void PlayRandomDestroyNoise()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 1)
            {
                int clipToPlay = Random.Range(0, destroyNoise.Length);
                bgMusic.Play();
                destroyNoise[clipToPlay].Play();

            }
        }
        else
        {
            int clipToPlay = Random.Range(0, destroyNoise.Length);

            destroyNoise[clipToPlay].Play();
        }

    }

}
