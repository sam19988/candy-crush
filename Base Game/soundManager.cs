using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soundManager : MonoBehaviour
{
    public AudioSource[] destroySounds;
    public AudioSource backgroundMuisc;

    private void Start()
    {
        backgroundMuisc.Play();
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
                backgroundMuisc.volume = 0;
            else
                backgroundMuisc.volume = 1;
        }
        else
        {
            backgroundMuisc.volume = 1;
        }
    }
    public void playRandomDestroySound()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 1)
            {
                int clipToUse = Random.Range(0, destroySounds.Length);
                destroySounds[clipToUse].Play();
            }
        }
        else
        {
            int clipToUse = Random.Range(0, destroySounds.Length);
            destroySounds[clipToUse].Play();
        }
    }

    public void adjustVolume()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
                backgroundMuisc.volume = 0;
            else
                backgroundMuisc.volume = 1;
        }
    }
}
