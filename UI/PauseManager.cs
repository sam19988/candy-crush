using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel;
    private Board board;
    public bool paused = false;
    public Image soundButton;
    public Sprite musicON;
    public Sprite musicOFF;
    private soundManager SM;
    // Start is called before the first frame update
    void Start()
    {
        SM = FindObjectOfType<soundManager>();
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound")== 0)
            {
                soundButton.sprite = musicOFF;
            }
            else
            {
                soundButton.sprite = musicON;
            }
        }
        else
        {
            soundButton.sprite = musicON;
        }
        pausePanel.SetActive(false);
        board = GameObject.FindWithTag("Board").GetComponent<Board>();
    }

    // Update is called once per frame
    void Update()
    {
        if(paused && !pausePanel.activeInHierarchy)
        {
            pausePanel.SetActive(true);
            board.currentState = gameState.pause;
        }
        if(!paused && pausePanel.activeInHierarchy)
        {
            pausePanel.SetActive(false);
            board.currentState = gameState.move;
        }
    }

    public void pauseGame()
    {
        paused = !paused;
    }

    public void exitGame()
    {
        SceneManager.LoadScene("Splash");
    } 

    public void SoundButton()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
            {
                soundButton.sprite = musicON;
                PlayerPrefs.SetInt("Sound", 1);
                SM.adjustVolume();
            }
            else
            {
                soundButton.sprite = musicOFF;
                PlayerPrefs.SetInt("Sound", 0);
                SM.adjustVolume();
            }
        }
        else
        {
            soundButton.sprite = musicOFF;
            PlayerPrefs.SetInt("Sound", 1);
            SM.adjustVolume();
        }
    }
}
