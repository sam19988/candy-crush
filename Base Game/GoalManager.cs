using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlankGoal
{
    public int neededNumber;
    public int collectednumber;
    public Sprite goalSprite;
    public string matchValue;
}
public class GoalManager : MonoBehaviour
{
    public BlankGoal[] levelGoals;
    public List<GoalPanel> currentGoals = new List<GoalPanel>();
    public GameObject goalPrefab;
    public GameObject goalIntroParent;
    public GameObject goalGameParent;

    private EndGameManager endGame;
    private Board board;
    // Start is called before the first frame update
    void Start()
    {
        endGame = FindObjectOfType<EndGameManager>();
        board = FindObjectOfType<Board>();
        getGoals();
        setUpGoals();
    }


    void setUpGoals()
    {
        for(int i = 0; i < levelGoals.Length; i++)
        {
            // create a new goal panle at the goal introParent positon
            GameObject introGoal = Instantiate(goalPrefab, goalIntroParent.transform.position, Quaternion.identity);
            introGoal.transform.SetParent(goalIntroParent.transform);
            introGoal.transform.localScale = Vector3.one;

            // set the image and the text of the goal
            GoalPanel panel = introGoal.GetComponent<GoalPanel>();
            panel.thisSprite = levelGoals[i].goalSprite;
            panel.thisString = "0/" + levelGoals[i].neededNumber;

            // create a new goal panle at the game goal parent
            GameObject gameGoal = Instantiate(goalPrefab, goalGameParent.transform.position, Quaternion.identity);
            gameGoal.transform.SetParent(goalGameParent.transform);
            gameGoal.transform.localScale = Vector3.one;

            // set the image and the text of the goal
             panel = gameGoal.GetComponent<GoalPanel>();
            currentGoals.Add(panel);
            panel.thisSprite = levelGoals[i].goalSprite;
            panel.thisString = "0/" + levelGoals[i].neededNumber;


        }
    }

    public void updateGoals()
    {
        int goalsCompleted = 0;
        for(int i=0; i < levelGoals.Length; i++)
        {
            currentGoals[i].thisText.text = levelGoals[i].collectednumber.ToString() + " / " + levelGoals[i].neededNumber;
            if(levelGoals[i].collectednumber >= levelGoals[i].neededNumber)
            {
                goalsCompleted++;
                currentGoals[i].thisText.text = levelGoals[i].neededNumber.ToString() + " / " + levelGoals[i].neededNumber;
            }
        }
        if(goalsCompleted>= levelGoals.Length)
        {
            if (endGame != null)
            {
                endGame.winGame();
                
            }
        }
    }

    public void compareToGoal(string goalToCompare)
    {
        for(int i = 0; i < levelGoals.Length; i++)
        {
            if (goalToCompare == levelGoals[i].matchValue)
            {
                levelGoals[i].collectednumber++;
            }
        }
    }

    void getGoals()
    {
        if (board.World != null)
        {
            if (board.level < board.World.levels.Length && board.level >= 0)
            {
                if (board.World.levels[board.level] != null)
                {
                    levelGoals = board.World.levels[board.level].levelGoals;
                    for (int i = 0; i < levelGoals.Length; i++)
                    {
                        levelGoals[i].collectednumber = 0;
                    }
                }
            }
        }
    }
}
