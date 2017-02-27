using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour {

    //This class manages turn order, actions and calls on other classes to appropriately apply methods

    public List<Transform> playerCharacterList;
    public List<Transform> enemyList;
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


	void Start () {// initializing things for battle such as models and stuff

        UI_combatText = GameObject.Find("Combat Text"); // finding GameObject
        combatText = UI_combatText.GetComponent<Text>();    // referencing text component

        int playerID = 1;
        int enemyID = 1;    //for determining appropriate UI
        foreach (Transform child in transform)   // adding each combatant to one of two lists: player or enemy
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

        playerTurnsRemaining = playerCharacterList.Count;   //actions equal to num of characters


        // intimidation phase here

        // intimidation ends

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
                    if (character.gameObject.GetComponent<BasePlayer>().canAct == true)
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

                    // pick random action here out of available actions
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

    public void Attack()    // function needed for each different possible action
    {
        // selected characters attacks should show
        if(currentState == phaseState.SELECTINGACTION)
        {
            currentState = phaseState.SELECTINGENEMY;
        }

    }


    public void executeAttack(GameObject attacker, GameObject defender) // probably needs more parameters at some point
    {

        int damage;

        damage = attacker.GetComponent<BasePlayer>().attackStat;
        defender.GetComponent<BasePlayer>().currentHealth -= damage;

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
