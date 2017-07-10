using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour {

    //This class manages turn order, actions and calls on other classes to appropriately apply methods

    public List<Transform> playerCharacterList;
    public List<Transform> alivePlayerCharacterList;
    public List<PlayerCharacterData> playerCharactersPassedIn;
    public List<GameObject> playerCharacterTiles;
    public List<Transform> enemyList;
    public List<EnemyData> enemiesPassedIn;
    public List<GameObject> enemyTiles;

    int playerTurnsRemaining;

    public GameObject UI_combatText;
    public Text combatText; // text component indicating what's happening
    public string combatTextString; // text string itself

    public GameObject selectedCharacter;
    public GameObject selectedEnemy;
    public GameObject selectedTile;

    public enum phaseState {
        SELECTINGCHAR,      // player is selecting a character
        SELECTINGACTION,    // player is selecting an action
        SELECTINGTILE,      // selecting tile for either attack or movement
        SELECTINGENEMY,     // player is selecting an enemy to attack
        PROCESSING,     // something is happening!
        ENEMYTURN,      // It's getting real!
        NEWROUND,   // new turn order at the end of each round, plus checking Buffs/Debuffs
        VICTORY,    // battle won
        DEFEAT      // battle lost
    }

    public phaseState currentState;

    //  BATTLE GRID VISUALIZATION //
    //       |7|3|    |3|7|       //
    //      |6|2|      |2|6|      //
    //     |5|1|        |1|5|     //
    //    |4|0|          |0|4|    //
    //   ALLIES          ENEMIES  //

    void Start() {// initializing things for battle such as models and stuff
        // Generate ally grid positions
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                GameObject tile = (GameObject)Instantiate(Resources.Load("GridTile"), new Vector3(0, 0, 0), Quaternion.identity, GameObject.Find("AllyGrid").transform);
                tile.AddComponent<GridTile>();
                tile.AddComponent<BoxCollider2D>();
                tile.GetComponent<GridTile>().setID((i * 4) + j);
                tile.GetComponent<GridTile>().setX((i * -2.4f) - ((3 - j) * 1.2f) - 27.6f);
                tile.GetComponent<GridTile>().setY((j * 2.0f) + 10);
                tile.transform.Translate(tile.GetComponent<GridTile>().getX(), tile.GetComponent<GridTile>().getY()-1, 0);

                playerCharacterTiles.Add(tile);
            }
        }

        // Generate enemy grid positions
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                GameObject tile = (GameObject)Instantiate(Resources.Load("GridTile"), new Vector3(0,0,0), Quaternion.identity, GameObject.Find("EnemyGrid").transform);
                tile.AddComponent<GridTile>();
                tile.AddComponent<BoxCollider2D>();
                tile.GetComponent<GridTile>().setID((i * 4) + j);
                tile.GetComponent<GridTile>().setX((i * 2.4f) - (j * 1.2f) - 18);
                tile.GetComponent<GridTile>().setY((j * 2.0f) + 10);
                tile.transform.Translate(tile.GetComponent<GridTile>().getX(), tile.GetComponent<GridTile>().getY()-1, 0);

                enemyTiles.Add(tile);
            }
        }

        // Counts how many allies/enemies have been spawned in
        int counterAllies = 0;
        int counterEnemies = 0;

        if (GameManager.control != null)
        {
            enemiesPassedIn = GameManager.control.enemies;
            playerCharactersPassedIn = GameManager.control.party;
        }
        if (enemiesPassedIn.Count > 0)
        {
            //If enemy list is provided, clear field of pre-existing entities, then generate combatants
            foreach (Transform child in transform)   // Adding each combatant to one of two lists: player or enemy
            {
                Destroy(child.gameObject);
            }

            // Finds ally based off prefab name and generates them
            foreach (PlayerCharacterData allyCharacter in playerCharactersPassedIn)
            {
                //TODO: Determine grid position and do stuff in full
                //First, put allies with filled in 'gridPos' on grid
                //Second, put allies with empty (-1) 'gridPos' on grid
                GameObject newAlly = (GameObject)Instantiate(Resources.Load(allyCharacter.name), new Vector3(playerCharacterTiles[allyCharacter.gridPos].GetComponent<GridTile>().getX(), playerCharacterTiles[allyCharacter.gridPos].GetComponent<GridTile>().getY(), 0), Quaternion.identity, GameObject.Find("BattleManager").transform);
                newAlly.GetComponent<BasePlayer>().gainEXP(allyCharacter.exp);
                newAlly.GetComponent<BasePlayer>().currentHealth = allyCharacter.health;
                newAlly.GetComponent<BasePlayer>().gridPosition = allyCharacter.gridPos;
                playerCharacterTiles[allyCharacter.gridPos].GetComponent<GridTile>().isOccupied = true;
                counterAllies++;    //TODO: allies still need UI generated
            }

            // Finds enemy based off prefab name and generates it
            foreach (EnemyData enemy in enemiesPassedIn)
            {
                //Same deal with enemies (take in level and 
                GameObject newEnemy = (GameObject)Instantiate(Resources.Load(enemy.name), new Vector3(enemyTiles[enemy.gridPos].GetComponent<GridTile>().getX(), enemyTiles[enemy.gridPos].GetComponent<GridTile>().getY(), 0), Quaternion.identity, GameObject.Find("BattleManager").transform);
                newEnemy.GetComponent<BasePlayer>().setLevel(enemy.level);
                newEnemy.GetComponent<BasePlayer>().setExpByLevel(enemy.level);
                newEnemy.GetComponent<BasePlayer>().gridPosition = enemy.gridPos;
                enemyTiles[enemy.gridPos].GetComponent<GridTile>().isOccupied = true;
                counterEnemies++;    //TODO: enemies still need UI generated
            }
        }


        UI_combatText = GameObject.Find("Combat Text"); // finding GameObject
        combatText = UI_combatText.GetComponent<Text>();    // referencing text component

        // Add each combatant to one of two lists: player or enemy
        foreach (Transform child in transform)
        {
            if (child.GetComponent<BasePlayer>() != null && !child.GetComponent<BasePlayer>().Equals(null))
            {
                if (child.GetComponent<BasePlayer>().isPlayerCharacter == true)
                {
                    playerCharacterList.Add(child);
                    alivePlayerCharacterList.Add(child);
                }

                else
                {
                    enemyList.Add(child);
                }
            }
            else
            {
                //Invalid child object found in BattleManager
                Debug.Log("ERROR: BattleManager contains invalid child object");
            }
        }

        // For determining appropriate UI
        int playerID = 0;
        int enemyID = 0;

        // Sort each list and assign ID here
        playerCharacterList.Sort(SortByGridPos);
        foreach (Transform player in playerCharacterList)
        {
            player.GetComponent<BasePlayer>().ID = playerID;
            playerID++;
            player.GetComponent<BasePlayer>().battleManagerStart();
        }
        enemyList.Sort(SortByGridPos);
        foreach (Transform enemy in enemyList)
        {
            enemy.GetComponent<BasePlayer>().ID = enemyID;
            enemyID++;
            enemy.GetComponent<BasePlayer>().battleManagerStart();
        }

        // Actions equal to number of characters
        playerTurnsRemaining = alivePlayerCharacterList.Count;

        // Intimidation phase goes here

        // Intimidation phase ends

        currentState = phaseState.SELECTINGCHAR;   // initialize to player select turn
    }

    // Update is called once per frame
    void Update() {
        switch (currentState)
        {
            case (phaseState.SELECTINGCHAR):    // selecting a character to use

                if (enemyList.Count == 0)
                {
                    currentState = phaseState.VICTORY;
                    break;
                }

                if (Input.GetMouseButtonDown(0))
                {
                    // Converting Mouse Pos to 2D (vector2) World Pos
                    RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));

                    if (hit)
                    {
                        if (hit.collider.GetComponent<BasePlayer>() != null && hit.collider.GetComponent<BasePlayer>().canAct == true && hit.collider.GetComponent<BasePlayer>().isPlayerCharacter == true)
                        {
                            //Debug.Log("You clicked a Friendly Chip!");
                            //Debug.Log("Target Position: " + hit.collider.gameObject.transform.position);
                            selectedCharacter = hit.transform.gameObject;
                            selectedCharacter.GetComponent<SpriteRenderer>().color = new Color32(0, 100, 25, 150);  // is selected
                            currentState = phaseState.SELECTINGACTION;
                            combatText.text = "Select an action";
                            break;
                        }
                        currentState = phaseState.SELECTINGCHAR; //ALERT: Redundant and pointless, unless I'm missing something? -Omer
                    }

                }

                break;

            case (phaseState.SELECTINGACTION):
                // anything that needs to happen while the player is selecting an action
                break;

            case (phaseState.SELECTINGTILE):
                if (Input.GetMouseButtonDown(0))
                {
                    //Converting Mouse Pos to 2D (vector2) World Pos
                    RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));

                    if (hit)
                    {
                        if (hit.collider.GetComponent<GridTile>() != null)
                        {
                            Debug.Log("You clicked on a GridTile!");
                            selectedTile = hit.transform.gameObject;
                            if(hit.collider.GetComponent<GridTile>().isOccupied == false) { //checking if tile is occupied
                                executeMove(selectedCharacter, selectedTile);    // doing the move calculations

                                currentState = phaseState.PROCESSING;
                                break;
                            }
                            
                        }
                    }
                    else
                    {
                        Debug.Log("You clicked something, not sure what!");
                    }

                }
                else
                {
                    combatText.text = "Select a tile.";
                }
                break;

            case (phaseState.SELECTINGENEMY):
                if (Input.GetMouseButtonDown(0))
                {
                    //Converting Mouse Pos to 2D (vector2) World Pos
                    RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));

                    if (hit)
                    {
                        if (hit.collider.GetComponent<BasePlayer>() != null && hit.collider.GetComponent<BasePlayer>().isPlayerCharacter == false)
                        {
                            //Debug.Log("You clicked an Enemy Chip!");
                            //Debug.Log("Target Position: " + hit.collider.gameObject.transform.position);
                            selectedEnemy = hit.transform.gameObject;
                            executeAttack(selectedCharacter, selectedEnemy);    // doing the damage calculations

                            selectedCharacter.GetComponent<BasePlayer>().canAct = false;
                            selectedCharacter.GetComponent<SpriteRenderer>().color = new Color32(25, 25, 25, 100);  // character now inactive coloured
                            currentState = phaseState.PROCESSING;
                            break;
                        }
                    }

                }
                else
                {
                    combatText.text = "Select a target";
                }
                break;

            case (phaseState.PROCESSING):
                // animations or something probably goes here too

                playerTurnsRemaining = 0;
                foreach (Transform character in playerCharacterList)    // checking if any players still have to go
                {
                    //MYSTERY BUG
                    if (character.gameObject.GetComponent<BasePlayer>().canAct == true)
                    {
                        playerTurnsRemaining++;
                    }
                }

                if (playerTurnsRemaining > 0)
                {
                    currentState = phaseState.SELECTINGCHAR;    // player still has actions left
                }
                else
                {
                    currentState = phaseState.ENEMYTURN;    // enemy time
                }

                if (enemyList.Count == 0)    // checking if enemies have been defeated
                {
                    currentState = phaseState.VICTORY;
                }

                break;

            case (phaseState.ENEMYTURN):
                //Enemy does stuff
                foreach (Transform enemy in enemyList)
                {
                    selectedCharacter = enemy.gameObject;
                    if (alivePlayerCharacterList.Count > 0)
                    {
                        selectedEnemy = alivePlayerCharacterList[Random.Range(0, alivePlayerCharacterList.Count)].gameObject;
                        // Pick random action here out of available actions
                        // (Dependent on 'mood')
                        executeAttack(selectedCharacter, selectedEnemy);    // only action currently available
                    }
                    else
                    {
                        executePass(selectedCharacter);
                    }

                }

                currentState = phaseState.NEWROUND;

                break;

            case (phaseState.NEWROUND):

                bool battleLost = true; // we assume battle is lost unless someone is still alive

                foreach (Transform character in playerCharacterList)    // players can act again
                {
                    if (character.gameObject.GetComponent<BasePlayer>().conscious == true)
                    {
                        character.gameObject.GetComponent<BasePlayer>().canAct = true;
                        character.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);   // can act again
                        battleLost = false;
                    }
                }

                foreach (Transform enemy in enemyList)
                {
                    enemy.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);   // can act again
                }

                if (battleLost)
                {
                    currentState = phaseState.DEFEAT;
                    break;
                }

                currentState = phaseState.SELECTINGCHAR;

                break;

            case (phaseState.VICTORY):
                Debug.Log("Battle won!");
                SceneManager.LoadScene("Map Exploration");
                break;

            case (phaseState.DEFEAT):
                Debug.Log("You lose!");
                SceneManager.LoadScene("mainMenu");
                break;
        }
    }

    public void Attack()    // function needed for each different possible action, possible animations go here?
    {
        // selected character's attacks should show
        if (currentState == phaseState.SELECTINGACTION)
        {
            currentState = phaseState.SELECTINGENEMY;
        }

    }


    public void executeAttack(GameObject attacker, GameObject defender) // probably needs more parameters at some point
    {
        //Determine if attack hits or is avoided
        bool attackHits = false;
        float hitChance = 0.75f - (float)(defender.GetComponent<BasePlayer>().agilityStat - attacker.GetComponent<BasePlayer>().agilityStat) / 100;
        Debug.Log("Hit chance: " + hitChance);
        if (hitChance < 0.50f)
        {
            hitChance = 0.50f;
        }
        if (hitChance > 1.0f)
        {
            hitChance = 1.0f;
        }
        float hitRoll = Random.value;
        Debug.Log("Hit roll: " + hitRoll);
        if (hitRoll <= hitChance)
        {
            attackHits = true;
        }

        int damage;

        //Damage calculation
        damage = attacker.GetComponent<BasePlayer>().attackStat; //DMG = Attack*power
        damage -= (int)(defender.GetComponent<BasePlayer>().defenseStat / 1.7935); //DMG reduction = Def*Armor/1.7935
        if (damage < 1) // Attacks always deal at least 1 damage
        {
            damage = 1;
        }
        if (attackHits)
        {
            defender.GetComponent<BasePlayer>().takeDamage(damage);
        }

        //TODO: Play animation of attacker and when it finishes, continue?

        //Placeholder for animations: Console + UI Text
        string message;
        if (attackHits)
        {
            message = attacker.GetComponent<BasePlayer>().characterName + " dealt " + damage + " damage to " + defender.GetComponent<BasePlayer>().characterName;
            Debug.Log(attacker + " dealt " + damage + " to " + defender);
        }
        else
        {
            message = attacker.GetComponent<BasePlayer>().characterName + " missed!";
            Debug.Log(attacker + " missed!");
        }

        combatTextString = (message);
        combatText.text = combatTextString;

        defender.GetComponent<BasePlayer>().updateHealthBar();

        if (defender.GetComponent<BasePlayer>().conscious == false)
        {
            if (defender.GetComponent<BasePlayer>().isPlayerCharacter == false)
            {
                enemyList.Remove(defender.transform);
                // Remove defeated foe from the battle grid
                enemyTiles[defender.GetComponent<BasePlayer>().gridPosition].GetComponent<GridTile>().isOccupied = false;
                Destroy(defender);  // A defender has been slain!
            }
            else
            {
                defender.GetComponent<SpriteRenderer>().color = new Color32(100, 100, 100, 125);
                Debug.Log(defender + " has fallen!");
                alivePlayerCharacterList.Remove(defender.transform);
            }

        }
    }

    public void Move()  // move the character
    {
        if (currentState == phaseState.SELECTINGACTION)
        {
            currentState = phaseState.SELECTINGTILE;
        }

    }

    public void executeMove(GameObject selectedCharacter, GameObject tile)
    {
        int pastID = selectedCharacter.GetComponent<BasePlayer>().gridPosition;
        playerCharacterTiles[pastID].GetComponent<GridTile>().isOccupied = false;
        selectedCharacter.GetComponent<BasePlayer>().gridPosition = tile.GetComponent<GridTile>().id;
        tile.GetComponent<GridTile>().isOccupied = true;

        selectedCharacter.transform.position = new Vector3(tile.GetComponent<GridTile>().getX(), tile.GetComponent<GridTile>().getY(), 0);
        selectedCharacter.GetComponent<BasePlayer>().canAct = false;
        selectedCharacter.GetComponent<SpriteRenderer>().color = new Color32(25, 25, 25, 100);  // character now inactive coloured

        combatTextString = "Select a character";
        combatText.text = combatTextString;

    }

    public void Pass()  // move the character
    {
        if (currentState == phaseState.SELECTINGACTION)
        {
            executePass(selectedCharacter);
            currentState = phaseState.PROCESSING;
        }

    }

    public void executePass(GameObject selectedCharacter)
    {
        selectedCharacter.GetComponent<BasePlayer>().canAct = false;
        selectedCharacter.GetComponent<SpriteRenderer>().color = new Color32(25, 25, 25, 100);  // character now inactive coloured
    }

    // C#
    static int SortByGridPos(Transform p1, Transform p2)
    {
        return p1.gameObject.GetComponent<BasePlayer>().gridPosition.CompareTo(p2.gameObject.GetComponent<BasePlayer>().gridPosition);
    }

}
