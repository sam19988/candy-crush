using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToLevelSelect : MonoBehaviour
{
    public string sceneToLoad;
    private GameData gameData;
    private Board board;
    // Start is called before the first frame update
    void Start()
    {
        gameData = FindObjectOfType<GameData>();
        board = FindObjectOfType<Board>();
    }

    public void winOK()
    {
        if (gameData != null)
        {
            gameData.saveData.isActive[board.level+1] = true;
            gameData.Save();
        }
        SceneManager.LoadScene(sceneToLoad);
    }

    public void loseOK()
    {
        SceneManager.LoadScene(sceneToLoad);

    }
}
