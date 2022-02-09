using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LevelButton : MonoBehaviour
{
    [Header("level prop")]
    public Image[] stars;
    public Text levelText;
    public int level;
    public GameObject confirmPAnel;

    [Header("Activation prop")]
    public bool isActive;
    public Sprite activeSprite;
    public Sprite lockedSprite;
    private Image buttonImage;
    private Button mybutton;

    private GameData gameData;
    private int starsActive;

    // Start is called before the first frame update
    void Start()
    {
        buttonImage = GetComponent<Image>();
        mybutton = GetComponent<Button>();
        gameData = FindObjectOfType<GameData>();
        LoadData();
        showLevel();
        decideSprite();
        starsActivation();
    }

   public void confrimPanel(int Lvl)
    {
        confirmPAnel.GetComponent<ConfirmPanel>().level = Lvl;
        confirmPAnel.SetActive(true);
    }

    void decideSprite()
    {
        if (isActive)
        {
            buttonImage.sprite = activeSprite;
            mybutton.enabled = true;
            levelText.enabled = true;
        }
        else
        {
            buttonImage.sprite = lockedSprite;
            mybutton.enabled = false;
            levelText.enabled = false;
        }
    }

    void showLevel()
    {
        levelText.text = level.ToString();
    }

    void starsActivation()
    {
        for( int i=0;i<starsActive;i++)
        {
            stars[i].enabled = true;

        }
    }

    void LoadData()
    {
        if (gameData != null)
        {
            if (gameData.saveData.isActive[level - 1])
            {
                isActive = true; 
            }
            else
            {
                isActive = false;
            }

            // decide how many stars to active 
            starsActive = gameData.saveData.stars[level - 1];
        }
    }
}
