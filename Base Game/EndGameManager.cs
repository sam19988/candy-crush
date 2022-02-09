using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum gameType
{
    Moves,
    Time
}

[System.Serializable]
public class endGameRequirments
{
    public gameType GameType;
    public int counterValue = 0;

}
public class EndGameManager : MonoBehaviour
{
    
    public endGameRequirments requirments;
    public GameObject movesLabel;
    public GameObject timeLabel;
    public GameObject youWinPanel;
    public GameObject tryAgainPanel;
    public Text counter;
    public int currentCounterValue;

    private Board board;
    private float timerSeconds;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        setGametype();
        setUpGame();
    }

    // Update is called once per frame
    void Update()
    {
        if(requirments.GameType == gameType.Time && currentCounterValue>0)
        {
            timerSeconds -= Time.deltaTime;
            if (timerSeconds <= 0)
            {
                decreaseCounter();
                timerSeconds = 1;
            }
        }
    }

    void setUpGame()
    {
        currentCounterValue = requirments.counterValue;
        if(requirments.GameType == gameType.Moves)
        {
            movesLabel.SetActive(true);
            timeLabel.SetActive(false);
        }
        else
        {
            timerSeconds = 1;
            movesLabel.SetActive(false);
            timeLabel.SetActive(true);
        }

        counter.text = currentCounterValue.ToString();
        
    }

    public void decreaseCounter()
    { 
        if(board.currentState!= gameState.pause)
        {
            currentCounterValue--;
            counter.text = currentCounterValue.ToString();
            if (currentCounterValue <= 0)
            {
                loseGame();
            }
        }

    }

    public void winGame()
    {
        youWinPanel.SetActive(true);
        board.currentState = gameState.win;
        currentCounterValue = 0;
        counter.text = currentCounterValue.ToString();
        fadeInController fade = FindObjectOfType<fadeInController>();
        fade.gameOver();
    }

    public void loseGame()
    {
        board.currentState = gameState.lose;
        tryAgainPanel.SetActive(true);
        currentCounterValue = 0;
        counter.text = currentCounterValue.ToString();
        fadeInController fade = FindObjectOfType<fadeInController>();
        fade.gameOver();

    }
    void setGametype()
    {
        if(board.World != null)
        {
            if (board.level <board.World.levels.Length && board.level >= 0)
            {
                if (board.World.levels[board.level] != null)
                {
                    requirments = board.World.levels[board.level].EGR;
                }
            }
        }
    }
}
