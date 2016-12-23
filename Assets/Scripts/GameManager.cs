using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager control;

    public float health;
    public float exp;
    public string scene;

	void Awake ()
    {
		if(control == null)
        {
            DontDestroyOnLoad(gameObject);
            control = this;
        }
        else if(control != this)
        {
            Destroy(gameObject);
        }
	}

    public void Save(int saveNumber)    //saves file number "saveNumber"
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/playerInfo" + saveNumber + ".dat");

        PlayerData data = new PlayerData();
        data.health = health;
        data.exp = exp;
        data.scene = scene;

        bf.Serialize(file, data);
        file.Close();
        Debug.Log("Data saved");
    }

    public void Load(int saveNumber)    //loads file number "saveNumber"
    {
        if(File.Exists(Application.persistentDataPath + "/playerInfo" + saveNumber + ".dat"))
        {
            Debug.Log("Loading data");
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo" + saveNumber + ".dat", FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();

            health = data.health;
            exp = data.exp;
            SceneManager.LoadScene(data.scene);
        }
        else
        {
            Debug.Log("No data to load");
        }
    }
}

[Serializable]  //allows for class to be written to a file
class PlayerData
{
    public float health;
    public float exp;
    public string scene;
}
