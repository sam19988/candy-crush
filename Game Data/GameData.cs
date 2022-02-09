using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class SaveData
{
    public bool [] isActive;  // check whether the level is actve or not 
    public int[] highScores;
    public int[] stars;
}

public class GameData : MonoBehaviour
{
    public static GameData gameData;
    public  SaveData saveData;


    // Start is called before the first frame update
    private void Awake()
    {
        if (gameData == null)
        {
            DontDestroyOnLoad(this.gameObject);
            gameData = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        Load();

    }


    public void Save()
    {
        // creating a binary formatter which can read binary files
        BinaryFormatter formatter = new BinaryFormatter();
       
        // create a route from the program to the file 
        FileStream file = File.Open(Application.persistentDataPath + "Player.dat",FileMode.Create);
        
        // create a copy save data
        SaveData data = new SaveData();
        data = saveData;
        // doing the saving process 
        formatter.Serialize(file, data);
        file.Close();
    }
    
    public void Load()
    {
        // check if the save game file exists 
        if (File.Exists(Application.persistentDataPath + "Player.dat"))
        {
            // creating a binary formatter 
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream file = File.Open(Application.persistentDataPath + "Player.dat", FileMode.Open);

            saveData = formatter.Deserialize(file) as SaveData;
            file.Close();
        }
    }

    private void OnDisable()
    {
        Save();
    }

    private void OnApplicationQuit()
    {
        Save();
    }
}
