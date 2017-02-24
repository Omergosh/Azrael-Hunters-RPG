using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour {

    //This class manages turn order, actions and calls on other classes to appropriately apply methods

    public List<Transform> playerCharacterList;
    public List<Transform> enemyList;

    public GameObject selectedCharacter;

    public enum phaseState {
        SELECTING,     // player is selecting a character
        PROCESSING, // something is happening!
        NEWROUND,   // new turn order at the end of each round, plus checking Buffs/Debuffs
        VICTORY,    // battle won
        DEFEAT     // battle lost
    }

    public phaseState currentState;

	// Use this for initialization
	void Start () {// initializing things for battle such as models and stuff

        foreach(Transform child in transform)   // adding each combatant to one of two lists: player or enemy
        {
            if(child.GetComponent<BasePlayer>() != null)
            {
                playerCharacterList.Add(child);
            }

            else
            {
                enemyList.Add(child);
            }
        }
       

        // intimidation phase here

        // intimidation ends

        currentState = phaseState.SELECTING;   // initialize to player select turn
    }
	
	// Update is called once per frame
	void Update () {
        switch (currentState)
        {
            case (phaseState.SELECTING):    // selecting a character to use

                if (Input.GetMouseButtonDown(0))
                {
                    //Converting Mouse Pos to 2D (vector2) World Pos
                    RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));

                    if (hit.collider.GetComponent<BasePlayer>() != null)
                    {
                        Debug.Log("You clicked a Friendly Chip!");
                        Debug.Log("Target Position: " + hit.collider.gameObject.transform.position);
                        selectedCharacter = hit.transform.gameObject;
                        currentState = phaseState.PROCESSING;
                    }

                }

                if (enemyList.Count == 0)    // checking if enemies have been defeated
                {
                    currentState = phaseState.VICTORY;    // disabled until enemies added
                }

                break;

            case (phaseState.PROCESSING):

                break;

            case (phaseState.NEWROUND):

                break;
            case (phaseState.VICTORY):

                break;
            case (phaseState.DEFEAT):

                break;
        }
	}
}
