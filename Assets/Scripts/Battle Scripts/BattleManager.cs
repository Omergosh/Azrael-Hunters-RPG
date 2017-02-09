using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour {

    //This class manages turn order, actions and calls on other classes to appropriately apply methods


    public List<Transform> combatantsList; 

    public enum phaseState {
        SELECTING,     // player is selecting an action
        STARTBATTLE,    // initializing things for battle like models and stuff
        NEWROUND,   // new turn order at the end of each round
        VICTORY,    // battle won
        DEFEAT     // battle lost
    }

    public phaseState currentState;

	// Use this for initialization
	void Start () {
        combatantsList = new List<Transform>();

    }
	
	// Update is called once per frame
	void Update () {
        switch (currentState)
        {
            case (phaseState.SELECTING):
                //do stuff here
                break;

            case (phaseState.STARTBATTLE):

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
