using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager control;

    public string scene;
    public Vector2 pos;
    public Vector2 dir;
    GameObject player;

    public List<PlayerCharacterData> party;
    public List<PlayerInventoryData> inventory;

    public List<EnemyData> enemies;    // List of enemies for battle

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

    public void Reset()
    {
        scene = "";
        pos = Vector2.zero;
        dir = Vector2.zero;

        //Reset party members to default
        party.Clear();
        //Add Chip to party
        PlayerCharacterData dataChipFletcher = new PlayerCharacterData();
        dataChipFletcher.name = "Chip";
        dataChipFletcher.level = 1;
        dataChipFletcher.exp = 0;
        dataChipFletcher.health = 100;
        dataChipFletcher.gridPos = 0;
        party.Add(dataChipFletcher);
        PlayerCharacterData dataChipFletcher2 = new PlayerCharacterData();
        dataChipFletcher2.name = "Chip";
        dataChipFletcher2.level = 1;
        dataChipFletcher2.exp = 0;
        dataChipFletcher2.health = 100;
        dataChipFletcher2.gridPos = 2;
        party.Add(dataChipFletcher2);

        inventory.Clear();
        PlayerInventoryData apple = new PlayerInventoryData();
        apple.name = "Apple";
        inventory.Add(apple);
    }

    // Saves file number "saveNumber"
    public void Save(int saveNumber)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/playerInfo" + saveNumber + ".dat");

        PlayerData data = new PlayerData();
        data.scene = SceneManager.GetActiveScene().name;

        player = GameObject.FindGameObjectWithTag("Player");
        pos = player.GetComponent<PlayerControlsOverworld>().pos;
        data.posx = (int)pos.x;
        data.posy = (int)pos.y;
        dir = player.GetComponent<PlayerControlsOverworld>().directionV;
        data.dirx = dir.x;
        data.diry = dir.y;
        scene = data.scene;
        data.party = party;

        bf.Serialize(file, data);
        file.Close();
        Debug.Log("Data saved");
    }

    // Loads file number "saveNumber"
    public void Load(int saveNumber)    
    {
        if(File.Exists(Application.persistentDataPath + "/playerInfo" + saveNumber + ".dat"))
        {
            Debug.Log("Loading data");
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo" + saveNumber + ".dat", FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();

            pos = new Vector2(data.posx, data.posy);
            dir = new Vector2(data.dirx, data.diry);
            party = data.party;
            SceneManager.LoadScene(data.scene);
        }
        else
        {
            Debug.Log("No data to load");
        }
    }
}

[Serializable]  // Allows for class to be written to a file
class PlayerData
{
    public string scene;
    public int posx;
    public int posy;
    public float dirx;
    public float diry;
    public List<PlayerCharacterData> party;
}

[Serializable]  // Allows for class to be written to a file
public class PlayerCharacterData
{
    public string name;
    public int level;
    public int health;
    public int exp;
    public int gridPos;
}

[Serializable]  // Allows for class to be written to a file
public class PlayerInventoryData
{
    public string name;
}