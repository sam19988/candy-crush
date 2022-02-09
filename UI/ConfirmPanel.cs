using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class ConfirmPanel : MonoBehaviour
{
    [Header("Level Information")]
    public string levelToLoad;
    public int level;
    private GameData gameData;
    private int starsActive;
    private int highScore;

    [Header("UI Prop")]
    public Image[] stars;
    public Text highScoreText;
    public Text starsText;
   

    private void OnEnable()
    {
        gameData = FindObjectOfType<GameData>();
        LoadData();
        starsActivation();
        setText();
    }

   public void cancle()
    {
        this.gameObject.SetActive(false);
    }

   public  void play()
    {
        PlayerPrefs.SetInt("Current Level", level - 1);
        SceneManager.LoadScene(levelToLoad);
    }

    void starsActivation()
    {
        for (int i=0;i<starsActive;i++)
        {
            stars[i].enabled = true;
        }
    }

    void LoadData()
    {
        if (gameData != null)
        {
            starsActive = gameData.saveData.stars[level - 1];
            highScore = gameData.saveData.highScores[level - 1];
        }
    }

    void setText()
    {
        highScoreText.text = highScore.ToString();
        starsText.text = starsActive.ToString() + " / 3";
    }
}
