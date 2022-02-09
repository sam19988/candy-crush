using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    private Board board;
    private GameData gameData;
    public Text scoreText;
    public int score;
    public Image scoreBar;
    private int starsNumber;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        gameData = FindObjectOfType<GameData>();
        if (scoreBar != null)
            scoreBar.fillAmount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = score.ToString();

    }

    public void increaseScore(int amountToIncrease)
    {
        score += amountToIncrease;
        for(int i = 0; i < board.scoareGoals.Length; i++)
        {
            if(score>board.scoareGoals[i] && starsNumber < i + 1)
            {
                starsNumber++;
            }
        }
        if (gameData != null)
        {
            int highScore = gameData.saveData.highScores[board.level];
            if(score > highScore)
            {
                gameData.saveData.highScores[board.level] = score;
            }

            int currentStars = gameData.saveData.stars[board.level];
            if(starsNumber> currentStars)
            {
                gameData.saveData.stars[board.level] = starsNumber;
            }
            gameData.Save();
        }
        UpdateBar();
    }

    private void UpdateBar()
    {
        if (board != null && scoreBar != null)
        {
            int lenght = board.scoareGoals.Length;
            scoreBar.fillAmount = (float)score / (float)board.scoareGoals[lenght - 1];
        }
    }

}
