using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour {

    //This class manages turn order, actions and calls on other classes to appropriately apply methods

    public List<Transform> playerCharacterList;
    public List<string> playerCharactersPassedIn;
    public List<GridTile> playerCharacterTiles;
    public List<Transform> enemyList;
    public List<string> enemiesPassedIn;
    public List<GridTile> enemyTiles;

    int playerTurnsRemaining;

    public GameObject UI_combatText;
    public Text combatText; // text component indicating what's happening
    public string combatTextString; // text string itself

    public GameObject selectedCharacter;
    public GameObject selectedEnemy;

    public enum phaseState {
        SELECTINGCHAR,      // player is selecting a character
        SELECTINGACTION,    // player is selecting an action
        SELECTINGENEMY,     // player is selecting an enemy to attack
        PROCESSING,     // something is happening!
        ENEMYTURN,      // It's getting real!
        NEWROUND,   // new turn order at the end of each round, plus checking Buffs/Debuffs
        VICTORY,    // battle won
        DEFEAT      // battle lost
    }

    public phaseState currentState;

    //  BATTLE GRID VISUALIZATION //
    //       |7|3|    |7|3|       //
    //      |6|2|      |6|2|      //
    //     |5|1|        |5|1|     //
    //    |4|0|          |4|0|    //
    //   ALLIES          ENEMIES  //

    void Start () {// initializing things for battle such as models and stuff
        // Generate ally grid positions
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                GridTile tile = (GridTile)ScriptableObject.CreateInstance("GridTile");
                tile.setID((i * 4) + j);
                tile.setX((i * -2.4f) - ((3 - j) * 1.2f) - 27.6f);
                tile.setY((j * 2.0f) + 10);
                playerCharacterTiles.Add(tile);
            }
        }

        // Generate enemy grid positions
        for (int i = 0; i < 2; i++)
        {        
            for (int j = 0; j < 4; j++)
            {
                GridTile tile = (GridTile)ScriptableObject.CreateInstance("GridTile");
                tile.setID((i * 4) + j);
                tile.setX((i * 2.4f) - (j * 1.2f) - 18);
                tile.setY((j * 2.0f) + 10);
                enemyTiles.Add(tile);
            }
        }

        // Counts how many allies/enemies have been spawned in
        int counterAllies = 0;
        int counterEnemies = 0;

        if(GameManager.control != null)
        {
            enemiesPassedIn = GameManager.control.enemies;
        }
        if (enemiesPassedIn.Count > 0)
        {
            //If enemy list is provided, clear field of pre-existing entities, then generate combatants
            foreach (Transform child in transform)   // Adding each combatant to one of two lists: player or enemy
            {
                Destroy(child.gameObject);
            }

            // Finds ally based off prefab name and generates them
            foreach (string allyName in playerCharactersPassedIn)
            {
                Instantiate(Resources.Load(allyName), new Vector3(playerCharacterTiles[counterAllies].getX(), playerCharacterTiles[counterAllies].getY(), 0), Quaternion.identity, GameObject.Find("BattleManager").transform);
                counterAllies++;    //TODO: allies still need UI generated
            }

            // Finds enemy based off prefab name and generates it
            foreach (string enemyName in enemiesPassedIn)
            {
                Instantiate(Resources.Load(enemyName), new Vector3(enemyTiles[counterEnemies].getX(), enemyTiles[counterEnemies].getY(), 0), Quaternion.identity, GameObject.Find("BattleManager").transform);
                counterEnemies++;    //TODO: enemies still need UI generated
            }
        }
        

        UI_combatText = GameObject.Find("Combat Text"); // finding GameObject
        combatText = UI_combatText.GetComponent<Text>();    // referencing text component

        // For determining appropriate UI
        int playerID = 1;
        int enemyID = 1;

        // Adding each combatant to one of two lists: player or enemy
        foreach (Transform child in transform)
        {
            if(child.GetComponent<BasePlayer>() != null && child.GetComponent<BasePlayer>().isPlayerCharacter == true)
            {
                playerCharacterList.Add(child);
                child.GetComponent<BasePlayer>().ID = playerID;
                playerID++;
            }

            else
            {
                enemyList.Add(child);
                child.GetComponent<BasePlayer>().ID = enemyID;
                enemyID++;
            }
            child.GetComponent<BasePlayer>().battleManagerStart();
        }

        playerTurnsRemaining = playerCharacterList.Count;   // actions equal to num of characters


        // Intimidation phase goes here

        // Intimidation phase ends

        currentState = phaseState.SELECTINGCHAR;   // initialize to player select turn
    }
	
	// Update is called once per frame
	void Update () {
        switch (currentState)
        {
            case (phaseState.SELECTINGCHAR):    // selecting a character to use

                if(enemyList.Count == 0)
                {
                    currentState = phaseState.VICTORY;
                    break;
                }

                if (Input.GetMouseButtonDown(0))
                {
                    //Converting Mouse Pos to 2D (vector2) World Pos
                    RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));

                    if (hit)
                    {
                        if(hit.collider.GetComponent<BasePlayer>() != null && hit.collider.GetComponent<BasePlayer>().canAct == true && hit.collider.GetComponent<BasePlayer>().isPlayerCharacter == true)
                        {
                            Debug.Log("You clicked a Friendly Chip!");
                            Debug.Log("Target Position: " + hit.collider.gameObject.transform.position);
                            selectedCharacter = hit.transform.gameObject;
                            selectedCharacter.GetComponent<SpriteRenderer>().color = new Color32(0, 100, 25, 150);  //is selected
                            currentState = phaseState.SELECTINGACTION;
                            break;
                        }
                        currentState = phaseState.SELECTINGCHAR;
                    }

                }

                break;

            case (phaseState.SELECTINGACTION):
                // anything that needs to happen while the player is selecting an action
                break;

            case (phaseState.SELECTINGENEMY):
                if (Input.GetMouseButtonDown(0))
                {
                    //Converting Mouse Pos to 2D (vector2) World Pos
                    RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));

                    if (hit)
                    {
                        if(hit.collider.GetComponent<BasePlayer>() != null && hit.collider.GetComponent<BasePlayer>().isPlayerCharacter == false)
                        {
                            Debug.Log("You clicked an Enemy Chip!");
                            Debug.Log("Target Position: " + hit.collider.gameObject.transform.position);
                            selectedEnemy = hit.transform.gameObject;
                            executeAttack(selectedCharacter, selectedEnemy);    // doing the damage calculations

                            selectedCharacter.GetComponent<BasePlayer>().canAct = false;
                            selectedCharacter.GetComponent<SpriteRenderer>().color = new Color32(25, 25, 25, 100);  // character now inactive coloured
                            currentState = phaseState.PROCESSING;
                            break;
                        }
                    }

                }
                break;

            case (phaseState.PROCESSING):
                // animations or something probably goes here too

                playerTurnsRemaining = 0;
                foreach (Transform character in playerCharacterList)    // checking if any players still have to go
                {
                    //MYSTERY BUG
                    if (character.gameObject.GetComponent<BasePlayer>().canAct == true) //BUG: Null reference here after a PC is made to attack, for some reason
                    {
                        playerTurnsRemaining++;
                    }
                }

                if(playerTurnsRemaining > 0)
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
                    selectedEnemy = playerCharacterList[Random.Range(0, playerCharacterList.Count)].gameObject; // need to adjust for unconscious players
                    while(selectedEnemy.GetComponent<BasePlayer>().conscious == false)
                    {
                        selectedEnemy = playerCharacterList[Random.Range(0, playerCharacterList.Count)].gameObject;
                    }

                    // Pick random action here out of available actions
                    // (Dependent on 'mood')
                    executeAttack(selectedCharacter, selectedEnemy);    // only action currently available
                }

                currentState = phaseState.NEWROUND;

                break;

            case (phaseState.NEWROUND):

                bool battleLost = true; // we assume battle is lost unless someone is still alive

                foreach (Transform character in playerCharacterList)    // players can act again
                {
                    if(character.gameObject.GetComponent<BasePlayer>().conscious == true)
                    {
                        character.gameObject.GetComponent<BasePlayer>().canAct = true;
                        character.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);   // can act again
                        battleLost = false;
                    }
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
        // selected characters attacks should show
        if(currentState == phaseState.SELECTINGACTION)
        {
            currentState = phaseState.SELECTINGENEMY;
        }

    }


    public void executeAttack(GameObject attacker, GameObject defender) // probably needs more parameters at some point
    {
        //Determine if attack hits or is dodged
        bool attackHits = true;

        int damage;

        //Damage calculation
        damage = attacker.GetComponent<BasePlayer>().attackStat; //DMG = Attack*power
        damage -= (int)(defender.GetComponent<BasePlayer>().defenseStat / 1.7935); //DMG reduction = Def*Armor/1.7935
        if (damage < 0)
        {
            damage = 0;
        }
        defender.GetComponent<BasePlayer>().currentHealth -= damage;

        //TODO: Play animation of attacker and when it finishes, continue?

        //Placeholder for animations: Console + UI Text
        Debug.Log(attacker + " dealt " + damage + " to " + defender);
        combatTextString = (attacker.GetComponent<BasePlayer>().characterName + " dealt " + damage + " damage to " + defender.GetComponent<BasePlayer>().characterName);
        combatText.text = combatTextString;

        defender.GetComponent<BasePlayer>().updateHealthBar();

        if (defender.GetComponent<BasePlayer>().currentHealth <= 0)
        {
            defender.GetComponent<BasePlayer>().currentHealth = 0;  // health can't go below 0
            if (defender.GetComponent<BasePlayer>().isPlayerCharacter == false)
            {
                enemyList.Remove(defender.transform);
                Destroy(defender);  // a defender has been slain!
            }
            else
            {
                defender.GetComponent<BasePlayer>().conscious = false;
                defender.GetComponent<SpriteRenderer>().color = new Color32(100, 100, 100, 125);
                Debug.Log(defender + " has fallen!");
            }

        }
    }

}
