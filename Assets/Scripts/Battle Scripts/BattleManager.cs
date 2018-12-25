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
    public List<ItemData> playerInventory;
    public List<GameObject> playerCharacterTiles;
    public List<Transform> enemyList;
    public List<Transform> aliveEnemyList;
    public List<EnemyData> enemiesPassedIn;
    public List<GameObject> enemyTiles;

    int playerTurnsRemaining;

    public GameObject UI_combatText;
    public RectTransform UI_bgTop;
    public RectTransform UI_bgBottom;
    public Text combatText; // Text component indicating what's happening
    public string combatTextString; // Text string itself

    public GameObject selectedCharacter;
    public GameObject selectedEnemy;
    public GameObject selectedTile;
    public ItemData apple;
    public enum phaseState {
        SELECTINGCHAR,      // Player is selecting a character
        SELECTINGACTION,    // Player is selecting an action
        SELECTINGTILE,      // Selecting tile for either attack or movement
        SELECTINGENEMY,     // Player is selecting an enemy to attack
        PROCESSING,     // Something is happening!
        ENEMYTURN,      // It's getting real!
        NEWROUND,   // New turn order at the end of each round, plus checking Buffs/Debuffs
        VICTORY,    // Battle won
        DEFEAT      // Battle lost
    }

    // Used for delaying enemies from all attacking instantly
    private float nextEnemyActionDelay = 2.0F;
    private float nextEnemyAction = 0.0F;

    int currentEnemyToAct = 0;
    int enemiesLeftToAct = 0;

    public phaseState currentState;

    //  BATTLE GRID VISUALIZATION //
    //       |7|3|    |3|7|       //
    //      |6|2|      |2|6|      //
    //     |5|1|        |1|5|     //
    //    |4|0|          |0|4|    //
    //   ALLIES          ENEMIES  //

    void Start() {// Initializing things for battle such as models and stuff

        playerInventory = GameManager.control.inventory;    // Loading inventory

        // Set references to Background UI panels
        UI_bgTop = GameObject.Find("Top Background UI").GetComponent<RectTransform>(); // Finding GameObject
        UI_bgBottom = GameObject.Find("Bottom Background UI").GetComponent<RectTransform>(); // Finding GameObject

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
            // If enemy list is provided, clear field of pre-existing entities, then generate combatants
            foreach (Transform child in transform)   // Adding each combatant to one of two lists: player or enemy
            {
                Destroy(child.gameObject);
            }

            // Finds ally based off prefab name and generates them
            int playerNum = 0;
            int enemyNum = 0;
            int i = 0;
            int j = 0;
            foreach (PlayerCharacterData allyCharacter in playerCharactersPassedIn)
            {
                // TODO: Determine grid position and do stuff in full
                // First, put allies with filled in 'gridPos' on grid
                // Second, put allies with empty (-1) 'gridPos' on grid
                GameObject newAlly = (GameObject)Instantiate(Resources.Load(allyCharacter.name), new Vector3(playerCharacterTiles[allyCharacter.gridPos].GetComponent<GridTile>().getX(), playerCharacterTiles[allyCharacter.gridPos].GetComponent<GridTile>().getY(), 0), Quaternion.identity, GameObject.Find("BattleManager").transform);
                newAlly.GetComponent<BaseCombatant>().gainEXP(allyCharacter.exp);
                newAlly.GetComponent<BaseCombatant>().currentHealth = allyCharacter.health;
                newAlly.GetComponent<BaseCombatant>().gridPosition = allyCharacter.gridPos;
                newAlly.GetComponent<BaseCombatant>().partyIndex = playerCharactersPassedIn.IndexOf(allyCharacter);
                playerCharacterTiles[allyCharacter.gridPos].GetComponent<GridTile>().isOccupied = true;
                counterAllies++;

                // Generate UI
                GameObject playerUI = (GameObject)Instantiate(Resources.Load("combatUI"), new Vector3(GameObject.Find("Canvas/Combatants UI/playerCombatantsUI").transform.position.x, GameObject.Find("Canvas/Combatants UI/playerCombatantsUI").transform.position.y), Quaternion.identity, GameObject.Find("Canvas/Combatants UI/playerCombatantsUI").transform);
                playerUI.name = "Player" + playerNum;
                playerNum++;
                playerUI.transform.Translate(new Vector3(-438f + i, 212f+j));
                i += 322;
                if (i >= 1000)
                {
                    j += 59;
                    i = 0;
                }
            }

            i = 0;
            j = 0;
            // Finds enemy based off prefab name and generates it
            foreach (EnemyData enemy in enemiesPassedIn)
            {
                //Same deal with enemies (take in level and 
                GameObject newEnemy = (GameObject)Instantiate(Resources.Load(enemy.name), new Vector3(enemyTiles[enemy.gridPos].GetComponent<GridTile>().getX(), enemyTiles[enemy.gridPos].GetComponent<GridTile>().getY(), 0), Quaternion.identity, GameObject.Find("BattleManager").transform);
                newEnemy.GetComponent<BaseCombatant>().setLevel(enemy.level);
                newEnemy.GetComponent<BaseCombatant>().setExpByLevel(enemy.level);
                newEnemy.GetComponent<BaseCombatant>().gridPosition = enemy.gridPos;
                enemyTiles[enemy.gridPos].GetComponent<GridTile>().isOccupied = true;
                counterEnemies++;

                // Generate UI
                GameObject enemyUI = (GameObject)Instantiate(Resources.Load("combatUI"), new Vector3(GameObject.Find("Canvas/Combatants UI/enemyCombatantsUI").transform.position.x, GameObject.Find("Canvas/Combatants UI/enemyCombatantsUI").transform.position.y), Quaternion.identity, GameObject.Find("Canvas/Combatants UI/enemyCombatantsUI").transform);
                enemyUI.name = "Enemy" + enemyNum;
                enemyNum++;
                enemyUI.transform.Translate(new Vector3(-438f + i, -366f + j));
                i += 322;
                if (i >= 1000)
                {
                    j -= 59;
                    i = 0;
                }
            }
        }


        UI_combatText = GameObject.Find("Combat Text"); // Finding GameObject
        combatText = UI_combatText.GetComponent<Text>();    // Referencing text component

        // Add each combatant to one of two lists: player or enemy
        foreach (Transform child in transform)
        {
            if (child.GetComponent<BaseCombatant>() != null && !child.GetComponent<BaseCombatant>().Equals(null))
            {
                if (child.GetComponent<BaseCombatant>().isPlayerCharacter == true)
                {
                    playerCharacterList.Add(child);
                    alivePlayerCharacterList.Add(child);
                }

                else
                {
                    enemyList.Add(child);
                    aliveEnemyList.Add(child);
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
            player.GetComponent<BaseCombatant>().ID = playerID;
            playerID++;
            player.GetComponent<BaseCombatant>().battleManagerStart();
        }
        enemyList.Sort(SortByGridPos);
        foreach (Transform enemy in enemyList)
        {
            enemy.GetComponent<BaseCombatant>().ID = enemyID;
            enemyID++;
            enemy.GetComponent<BaseCombatant>().battleManagerStart();
        }

        // Actions equal to number of characters
        playerTurnsRemaining = alivePlayerCharacterList.Count;

        // Intimidation phase goes here

        // Intimidation phase ends

        currentState = phaseState.SELECTINGCHAR;   // Initialize to player select turn
    }

    // Update is called once per frame
    void Update() {
        switch (currentState)
        {
            case (phaseState.SELECTINGCHAR):    // Selecting a character to use

                if (aliveEnemyList.Count == 0)
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
                        if (hit.collider.GetComponent<BaseCombatant>() != null && hit.collider.GetComponent<BaseCombatant>().canAct == true && hit.collider.GetComponent<BaseCombatant>().isPlayerCharacter == true)
                        {
                            //Debug.Log("You clicked a Friendly Chip!");
                            //Debug.Log("Target Position: " + hit.collider.gameObject.transform.position);
                            selectedCharacter = hit.transform.gameObject;
                            selectedCharacter.GetComponent<SpriteRenderer>().color = new Color32(0, 100, 25, 150);  // is selected
                            currentState = phaseState.SELECTINGACTION;
                            combatText.text = "Select an action";
                            break;
                        }
                        currentState = phaseState.SELECTINGCHAR; // ALERT: Redundant and pointless, unless I'm missing something? -Omer
                    }

                }

                break;

            case (phaseState.SELECTINGACTION):
                // Anything that needs to happen while the player is selecting an action
                break;

            case (phaseState.SELECTINGTILE):
                if (Input.GetMouseButtonDown(0))
                {
                    // Converting Mouse Pos to 2D (vector2) World Pos
                    RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));

                    if (hit)
                    {
                        if (hit.collider.GetComponent<GridTile>() != null)
                        {
                            Debug.Log("You clicked on a GridTile!");
                            selectedTile = hit.transform.gameObject;
                            if (hit.collider.GetComponent<GridTile>().isOccupied == false) { // Checking if tile is occupied
                                executeMove(selectedCharacter, selectedTile);    // Doing the move calculations

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
                    // Converting Mouse Pos to 2D (vector2) World Pos
                    RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));

                    if (hit)
                    {
                        if (hit.collider.GetComponent<BaseCombatant>() != null && hit.collider.GetComponent<BaseCombatant>().isPlayerCharacter == false)
                        {
                            //Debug.Log("You clicked an Enemy Chip!");
                            //Debug.Log("Target Position: " + hit.collider.gameObject.transform.position);
                            selectedEnemy = hit.transform.gameObject;
                            executeAttack(selectedCharacter, selectedEnemy);    // Doing the damage calculations

                            selectedCharacter.GetComponent<BaseCombatant>().canAct = false;
                            selectedCharacter.GetComponent<SpriteRenderer>().color = new Color32(25, 25, 25, 100);  // Character now inactive coloured
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
                // Animations or something probably goes here too

                playerTurnsRemaining = 0;
                foreach (Transform character in playerCharacterList)    // Checking if any players still have to go
                {
                    // MYSTERY BUG
                    if (character.gameObject.GetComponent<BaseCombatant>().canAct == true)
                    {
                        playerTurnsRemaining++;
                    }
                }

                if (playerTurnsRemaining > 0)
                {
                    currentState = phaseState.SELECTINGCHAR;    // Player still has actions left
                }
                else
                {
                    currentEnemyToAct = 0;
                    enemiesLeftToAct = aliveEnemyList.Count;
                    nextEnemyAction = Time.time + nextEnemyActionDelay; // Setting delay
                    currentState = phaseState.ENEMYTURN;    // Enemy time
                }

                if (aliveEnemyList.Count == 0)    // Checking if enemies have been defeated
                {
                    currentState = phaseState.VICTORY;
                }

                break;

            case (phaseState.ENEMYTURN):
                // Enemy does stuff
                if (Time.time >= nextEnemyAction)
                {
                    nextEnemyAction = Time.time + nextEnemyActionDelay;// Resetting timer and starting next action
                    Transform enemy = aliveEnemyList[currentEnemyToAct];
                    selectedCharacter = enemy.gameObject;
                    if (alivePlayerCharacterList.Count > 0)
                    {
                        selectedEnemy = alivePlayerCharacterList[Random.Range(0, alivePlayerCharacterList.Count)].gameObject;
                        // Pick random action here out of available actions
                        // (Dependent on 'mood')
                        executeAttack(selectedCharacter, selectedEnemy);    // Only action currently available
                    }
                    else
                    {
                        executePass(selectedCharacter);
                    }
                    currentEnemyToAct += 1;
                    enemiesLeftToAct -= 1;
                    if (enemiesLeftToAct == 0) {
                        currentState = phaseState.NEWROUND;
                    }
                }

                break;

            case (phaseState.NEWROUND):

                bool battleLost = true; // We assume battle is lost unless someone is still alive

                foreach (Transform character in playerCharacterList)    // Players can act again
                {
                    if (character.gameObject.GetComponent<BaseCombatant>().conscious == true)
                    {
                        character.gameObject.GetComponent<BaseCombatant>().canAct = true;
                        character.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);   // Can act again
                        battleLost = false;
                    }
                }

                foreach (Transform enemy in aliveEnemyList)
                {
                    enemy.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);   // Can act again
                }

                if (battleLost)
                {
                    currentState = phaseState.DEFEAT;
                    break;
                }

                currentState = phaseState.SELECTINGCHAR;

                break;

            case (phaseState.VICTORY):
                // Award EXP and Rewards (items, catalysts) either here or in GameManager
                Debug.Log("Battle won!");
                // EXP
                // -First find out how much EXP to award in total
                int expToGain = 0;
                foreach (Transform enemy in enemyList)
                {
                    expToGain += enemy.GetComponent<BaseCombatant>().rewardExperience();
                    Debug.Log("EXP to gain: " + expToGain);
                }
                Debug.Log("Total EXP: " + expToGain);
                // -Then award the same amount of EXP to all party members
                // -NOTE: This approach may be revisited if EXP scaling needs to be implemented
                foreach (Transform character in playerCharacterList)
                {
                    // Award EXP. If level ups and stuff ensue, give notices and stuff in the UI
                    character.GetComponent<BaseCombatant>().gainEXP(expToGain);
                }
                Debug.Log(playerCharacterList[0].GetComponent<BaseCombatant>().currentEXP);
                // Item Drops
                List<ItemData> lootGained = new List<ItemData>();
                foreach(Transform enemy in enemyList){
                    lootGained.AddRange(enemy.GetComponent<BaseCombatant>().dropLootRewards());
                }
                GameManager.control.inventory.AddRange(lootGained);
                // Update certain current party details in GameManager (hit points, EXP)
                //foreach (PlayerCharacterData partyMember in GameManager.control.party)
                foreach (Transform character in playerCharacterList)
                {
                    // Identify which character in playerCharacterList corresponds to this party member
                    int pIndex = character.GetComponent<BaseCombatant>().partyIndex;
                    // Update HP
                    GameManager.control.party[pIndex].health = character.GetComponent<BaseCombatant>().currentHealth;
                    // Update EXP
                    GameManager.control.party[pIndex].exp = character.GetComponent<BaseCombatant>().currentEXP;
                }

                // Potentially update inventory to account for items used, if it's not already done elsewhere

                // Return to map/cutscene
                GameManager.control.mostRecentBattleWon = true;
                SceneManager.LoadScene(GameManager.control.currentMapScene);
                break;

            case (phaseState.DEFEAT):
                Debug.Log("You lose!");
                SceneManager.LoadScene("mainMenu");
                break;
        }
    }

    public void Attack()    // Function needed for each different possible action, possible animations go here?
    {
        // Selected character's attacks should show
        if (currentState == phaseState.SELECTINGACTION)
        {
            currentState = phaseState.SELECTINGENEMY;
        }

    }


    public void executeAttack(GameObject attacker, GameObject defender) // Probably needs more parameters at some point
    {
        // Determine if attack hits or is avoided
        bool attackHits = false;
        float hitChance = 0.75f - (float)(defender.GetComponent<BaseCombatant>().agilityStat - attacker.GetComponent<BaseCombatant>().agilityStat) / 100;
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

        // Damage calculation
        damage = attacker.GetComponent<BaseCombatant>().attackStat; // DMG = Attack*power

        // This allows attacks to hit from 90% to 110% of intended damage for damage variation
        float damageVariation = Random.Range(0.9f, 1.1f);    
        damage = (int)(damage * damageVariation);

        damage -= (int)(defender.GetComponent<BaseCombatant>().defenseStat / 1.7935); // DMG reduction = Def*Armor/1.7935
        // Attacks always deal at least 1 damage
        if (damage < 1) 
        {
            damage = 1;
        }
        if (attackHits)
        {
            defender.GetComponent<BaseCombatant>().takeDamage(damage);
        }

        // TODO: Play animation of attacker and when it finishes, continue?

        // Placeholder for animations: Console + UI Text
        string message;
        if (attackHits)
        {
            message = attacker.GetComponent<BaseCombatant>().characterName + " dealt " + damage + " damage to " + defender.GetComponent<BaseCombatant>().characterName;
            Debug.Log(attacker + " dealt " + damage + " to " + defender);
        }
        else
        {
            message = attacker.GetComponent<BaseCombatant>().characterName + " missed!";
            Debug.Log(attacker + " missed!");
        }

        combatTextString = (message);
        combatText.text = combatTextString;

        defender.GetComponent<BaseCombatant>().updateHealthBar();

        if (defender.GetComponent<BaseCombatant>().conscious == false)
        {
            if (defender.GetComponent<BaseCombatant>().isPlayerCharacter == false)
            {
                aliveEnemyList.Remove(defender.transform);
                // Remove defeated foe from the battle grid
                enemyTiles[defender.GetComponent<BaseCombatant>().gridPosition].GetComponent<GridTile>().isOccupied = false;
                defender.SetActive(false); // Abilities exist to resurrect enemies, so at least some ofthis data needs to be available
                // Destroy(defender);  // A defender has been slain!
            }
            else
            {
                defender.GetComponent<SpriteRenderer>().color = new Color32(100, 100, 100, 125);
                Debug.Log(defender + " has fallen!");
                alivePlayerCharacterList.Remove(defender.transform);
            }

        }
    }

    public void Move()  // Move the character
    {
        if (currentState == phaseState.SELECTINGACTION)
        {
            currentState = phaseState.SELECTINGTILE;
        }

    }

    public void executeMove(GameObject selectedCharacter, GameObject tile)
    {
        int pastID = selectedCharacter.GetComponent<BaseCombatant>().gridPosition;
        playerCharacterTiles[pastID].GetComponent<GridTile>().isOccupied = false;
        selectedCharacter.GetComponent<BaseCombatant>().gridPosition = tile.GetComponent<GridTile>().id;
        tile.GetComponent<GridTile>().isOccupied = true;

        selectedCharacter.transform.position = new Vector3(tile.GetComponent<GridTile>().getX(), tile.GetComponent<GridTile>().getY(), 0);
        selectedCharacter.GetComponent<BaseCombatant>().canAct = false;
        selectedCharacter.GetComponent<SpriteRenderer>().color = new Color32(25, 25, 25, 100);  // Character now inactive coloured

        combatTextString = "Select a character";
        combatText.text = combatTextString;

    }

    public void Items()  // Check Inventory + potentially use items
    {
        if (currentState == phaseState.SELECTINGACTION)
        {
            executeItems(selectedCharacter);
        }

    }

    public void executeItems(GameObject selectedCharacter)
    {
        Debug.Log("Items:");
        foreach(ItemData item in playerInventory)
        {
            Debug.Log(item.name);
        }

    }

    public void Pass()  // Skip the character's turn
    {
        if (currentState == phaseState.SELECTINGACTION)
        {
            executePass(selectedCharacter);
            currentState = phaseState.PROCESSING;
        }

    }

    public void executePass(GameObject selectedCharacter)
    {
        selectedCharacter.GetComponent<BaseCombatant>().canAct = false;
        selectedCharacter.GetComponent<SpriteRenderer>().color = new Color32(25, 25, 25, 100);  // Character now inactive coloured
    }

    // C#
    static int SortByGridPos(Transform p1, Transform p2)
    {
        return p1.gameObject.GetComponent<BaseCombatant>().gridPosition.CompareTo(p2.gameObject.GetComponent<BaseCombatant>().gridPosition);
    }

}
