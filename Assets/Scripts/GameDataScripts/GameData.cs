using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UIElements;
using Unity.Services.Analytics;
using Unity.Services.Core;
using Unity.VisualScripting;

[Serializable]
public class SaveData
{
    public bool[] isAcive;
    public int[] highScore;
    public int[] stars;

}
public class GameData : MonoBehaviour
{
    public static GameData gameData;
    public SaveData saveData;



    // Start is called before the first frame update
    void Awake()
    {
        if(gameData == null)
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

    async void Start()
    {
        await UnityServices.InitializeAsync();



        AnalyticsService.Instance.StartDataCollection();
    }

    private void OnDisable()
    {
        Save();
    }
    private void OnApplicationQuit()
    {
        Save();
    }
    private void OnApplicationPause()
    {
        Save();
    }
    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            Save();
        }
    }

    public void Save()
    {
        //Create a binary formatter which can read binary files;

        BinaryFormatter formatter = new BinaryFormatter();

        //Create a route from program to file
        FileStream file = File.Open(Application.persistentDataPath + "/player.dat",FileMode.Create);

        //Create a copy of save data
        SaveData data = new SaveData();
        data = saveData;
        //Save Data in File
        formatter.Serialize(file, data);

        //Close data stream
        file.Close();

        Debug.Log("Save");
    }

    public void Load()
    {
        //Check if Save game file exists

        if(File.Exists(Application.persistentDataPath + "/player.dat"))
        {
            //create binary formatter
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/player.dat", FileMode.Open);
            saveData = formatter.Deserialize(file) as SaveData;
            file.Close() ;
            Debug.Log("Loaded");
        }
        else
        {
            saveData= new SaveData();
            saveData.isAcive = new bool[100];
            saveData.highScore = new int[100];
            saveData.stars = new int[100];
            saveData.isAcive[0] = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
