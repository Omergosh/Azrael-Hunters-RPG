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
    public string currentMapScene; // Map that player is on
    public Vector2 pos;
    public Vector2 dir;
    GameObject player;

    public List<PlayerCharacterData> party;
    public List<ItemData> inventory;

    public List<EnemyData> enemies;    // List of enemies for battle
    public List<string> overworldEnemies;   // Keeps track of all enemies on overworld map
    public string enemyEncounter;
    public bool mostRecentBattleWon;


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
        overworldEnemies.Clear();
    }

    public void Reset()
    {
        scene = "";
        pos = Vector2.zero;
        dir = Vector2.zero;

        // Reset party members to default
        party.Clear();
        // Add Chip to party
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
        ItemData apple = new ItemData
        {
            name = "Apple",
            quantity = 1,
            isEquipment = false,
            isCatalyst = false
        };
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
        data.inventory = inventory;

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
            inventory = data.inventory;
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
    public List<ItemData> inventory;
    /* Other things to save:
     *  -Unlocked Areas
     *  -Unlocked Journal Entries (encountered Azrael, NPCs, Places, etc.)
     *  -Missions Completed
     *  -Mission Progress (Current)
     *  -Plot Points / Cutscenes triggered
    */
}

[Serializable]  // Allows for class to be written to a file
public class PlayerCharacterData
{
    public string name;
    public int level;
    public int health;
    public int exp;
    public int gridPos;
    public ItemData equipmentSlot1;
    public ItemData equipmentSlot2;
    public ItemData armorSlot;
    public List<ItemData> catalystSlots; //Slots A-B-C-D
    /* Other things to save:
     *  -Skill Tree Progression (which skills did the player choose to progress so far?)
    */
}

[Serializable]  // Allows for class to be written to a file
public class PlayerInventoryData
{
    //Data for the entire inventory of the player
    //Does this include currently equipped items? Answer for now: yes
    public List<ItemData> items;
}

[Serializable]  // Allows for class to be written to a file
public class ItemData
{
    //Data for a given item as saved - no implementation or additional details necessary
    public string name;
    public int quantity;
    public bool isEquipment;
    public bool isCatalyst;
}