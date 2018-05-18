using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PlayerControlsOverworld : MonoBehaviour {

    public Vector3 pos;                    // For movement

    // Battle stats
    public float health; // Obsolete?                
    public float exp; // Obsolete?
    public List<PlayerCharacterData> party;

    public float speed = 4.0f;      // Speed of movement
    public float interactDelay = 1.0f;  // Delay after talking
    private float timestamp;    // For keeping time
	public bool paused = false;
    public bool canMove = true;
    public bool interacting = false;
    public Vector2 directionV;  // Direction of player
    public int layerMaskCollisions = 1 << 8;
    public int layerMaskInteracts = 1 << 9;
    GameObject canvas;
	GameObject UI_DialogueSystem;
	GameObject UI_PauseMenu;
	GameObject UI_PauseMenuRoot;
    GameObject UI_PauseMenuParty;
    GameObject UI_PauseMenuJournal;
    GameObject UI_PauseMenuQuit;
    GameObject UI_SaveCrystal;

	public bool pauseButtonDown = false;
	public bool pauseButtonPrevious = false;

    void Start()
    {

        if(GameManager.control != null) {
            party = GameManager.control.party;    // Setting stats on loading into level
            pos = GameManager.control.pos;          // Take the gameManager position


            if (GameManager.control.pos != Vector2.zero && GameManager.control.dir != Vector2.zero)
            {

                pos = GameManager.control.pos;          // Take the gameManager position
		        directionV = GameManager.control.dir;   // Take the gameManager direction
                Debug.Log("Gamemanager position obtained");
            }
            else
            {
                pos = transform.position;          // Take the starting pos
                directionV = Vector2.down;   // Take the starting pos
            }
        }
        else
        {
            pos = transform.position;          // Take the starting pos
            directionV = Vector2.down;   // Take the starting pos
        }
        transform.position = Vector3.MoveTowards(transform.position, pos, 1000);   // Moving to new pos instantly thanks to HIGH SPEEDS

        canvas = GameObject.Find("Canvas");   // Finding the Canvas gameObject
        try
        {
            UI_DialogueSystem = canvas.transform.Find("UI_DialogueSystem").gameObject;
        }
        catch
        {
               // TODO? DialogueSystem can't be found, should probably be made persistant?
        }

        UI_PauseMenu = canvas.transform.Find("UI_PauseMenu").gameObject;
		UI_PauseMenuRoot = UI_PauseMenu.transform.Find("UI_PauseMenuRoot").gameObject;
        UI_PauseMenuParty = UI_PauseMenu.transform.Find("UI_PauseMenuParty").gameObject;
        UI_PauseMenuJournal = UI_PauseMenu.transform.Find("UI_PauseMenuJournal").gameObject;
        UI_PauseMenuQuit = UI_PauseMenu.transform.Find("UI_PauseMenuQuit").gameObject;
        UI_SaveCrystal = canvas.transform.Find("UI_SaveCrystal").gameObject;
    }

    void Update()
    {
		if (!paused) {
			if (canMove) {
				if (Input.GetAxisRaw ("Horizontal") < 0 && transform.position == pos) {   // Left
					directionV = Vector2.left;
					if (CollisionCheck (directionV)) {
						GameObject objectHit = InteractCheck (directionV);
						pushing (objectHit);
						// Walk into wall sound effect
					} else {
						pos += 2 * Vector3.left;
					}
				}
				if (Input.GetAxisRaw ("Horizontal") > 0 && transform.position == pos) {   // Right
					directionV = Vector2.right;
					if (CollisionCheck (directionV)) {
						{
							GameObject objectHit = InteractCheck (directionV);
							pushing (objectHit);
							// Walk into wall sound effect
						}
					} else {
						pos += 2 * Vector3.right;
					}
				}
				if (Input.GetAxisRaw ("Vertical") > 0 && transform.position == pos) {   // Up
					directionV = Vector2.up;
					if (CollisionCheck (directionV)) {
						{
							GameObject objectHit = InteractCheck (directionV);
							pushing (objectHit);
							// Walk into wall sound effect
						}
					} else {
						pos += 2 * Vector3.up;
					}
				}
				if (Input.GetAxisRaw ("Vertical") < 0 && transform.position == pos) {   // Down
					directionV = Vector2.down;
					if (CollisionCheck (directionV)) {
						{
							GameObject objectHit = InteractCheck (directionV);
							pushing (objectHit);
							// Walk into wall sound effect
						}
					} else {
						pos += 2 * Vector3.down;
					}
				}
			}

	        transform.position = Vector3.MoveTowards(transform.position, pos, Time.deltaTime * speed);    // Moving to new pos
		}

        // User interaction with a delay of how quickly it can be done equal to interactDelay
        if (Input.GetAxisRaw("Interact") > 0 & Time.time >= timestamp)
        {
            timestamp = Time.time + interactDelay;
            GameObject objectHit = InteractCheck(directionV);
            if (objectHit != null)
            {
                // Interacting with person or object
                if (objectHit.GetComponent<NPC_Behavior>() && interacting == false)
                {
                    NPC_Behavior NPC = objectHit.GetComponent<NPC_Behavior>();
                    UI_DialogueSystem.SetActive(true);
                    interacting = true;
                    canMove = false;
                    StartCoroutine(NPC.Interact());
                }
                // Interacting with savepoint
                if (objectHit.GetComponent<SaveCrystal>() && interacting == false)  
                {
                    //SaveCrystal saveCrystal = objectHit.GetComponent<SaveCrystal>();
                    UI_SaveCrystal.SetActive(true);
                    interacting = true;
                    canMove = false;
                    //saveCrystal.save();
                }
                // Interacting with door, teleporter or something similar
                if (objectHit.GetComponent<TransferPoint>() && interacting == false)
                {
                    TransferPoint transferPoint = objectHit.GetComponent<TransferPoint>();

                    //interacting = true;
                    //canMove = false;
                    gameManagerUpdate();
                    transferPoint.transfer();   // Set to HYPERSPEED, WE'RE GOING PLACES. 
                    updatePos();
                }
                // Interacting with enemy or something similar that starts an encounter immediately
                if (objectHit.GetComponent<hostileEncounter>() && interacting == false)
                {
                    hostileEncounter hostileEncounter = objectHit.GetComponent<hostileEncounter>();

                    //interacting = true;
                    //canMove = false;
                    gameManagerUpdate();

                    hostileEncounter.startBattle();   // Starts the battle
                    updatePos();
                }
            }
		}

        // Pause, pause, pause everything please! bring up the menu make everything stop
		if (Input.GetAxisRaw ("Pause") > 0) {
			pauseButtonDown = true;
		} else {
			pauseButtonDown = false;
		}
		if(pauseButtonDown == true && pauseButtonPrevious == false){
			if (paused) {
                // Unpausing
				UI_PauseMenuRoot.SetActive (false);
				UI_PauseMenu.SetActive (false);
                UI_PauseMenuParty.SetActive(false);
                UI_PauseMenuJournal.SetActive(false);
                paused = false;
			} else {
				if (!UI_DialogueSystem.activeSelf) {
                    // If the dialogue box is not open/active
					UI_PauseMenu.SetActive (true);
					UI_PauseMenuRoot.SetActive (true);
					paused = true;
				}
			}
		}
        // Abort! abort! you know, dialogue
        if (Input.GetAxisRaw("Back") > 0) 
        {
			if (paused) {
                //Unpausing
				UI_PauseMenuRoot.SetActive (false);
				UI_PauseMenu.SetActive (false);
				paused = false;
			}
			if (UI_DialogueSystem.activeSelf) {
				UI_DialogueSystem.SetActive (false);
                canMove = true;
                interacting = false;
			}
        }

		pauseButtonPrevious = pauseButtonDown;
    }

    bool CollisionCheck(Vector2 direction)
    {
        if (Physics2D.Raycast(transform.position, direction, 2.0f, layerMaskCollisions))
        {
            return true;
        }
        return false;
    }

    GameObject InteractCheck(Vector2 direction)
    {
        RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, direction, 2.0f, layerMaskInteracts);
        if(raycastHit2D)
        {
            return raycastHit2D.collider.gameObject;
        }
        return null;
    }

	public void PauseMenuButtons(string pressedButton = "resume")
	{
		switch (pressedButton) {
			case "resume":
				UI_PauseMenuRoot.SetActive (false);
				UI_PauseMenu.SetActive (false);
                UI_SaveCrystal.SetActive(false);    // Added to avoid making a second resume for save menus, technically doesn't belong here
				paused = false; // See above comment
                interacting = false;    // See above comment
                canMove = true; // See above comment
                break;
			case "party":
                UI_PauseMenuParty.SetActive(true);
				UI_PauseMenuRoot.SetActive (false);
				break;
			case "missions":
				//UI_PauseMenuRoot.SetActive (false);
				//UI_PauseMenu.SetActive (false);
				break;
			case "journal":
				UI_PauseMenuRoot.SetActive (false);
                UI_PauseMenuJournal.SetActive(true);
				break;
			case "quit":
                UI_PauseMenuRoot.SetActive (false);
                UI_PauseMenuQuit.SetActive (true);
				break;
            case "yes": //PauseMenuQuit yes button
                SceneManager.LoadScene("mainMenu");
                break;
            case "no":  //PauseMenuQuit no button
                UI_PauseMenuRoot.SetActive(true);
                UI_PauseMenuQuit.SetActive(false);
                break;
        }
	}

    void pushing(GameObject objectHit)
    {

        if (objectHit != null)
        {
            if (objectHit.GetComponent<NPC_Behavior>())
            {
                NPC_Behavior NPC = objectHit.GetComponent<NPC_Behavior>();
                NPC.Push(directionV);   // Pushing things
            }
        }
    }

    // Updating the gameManager before loading a new scene or loading into battle
    public void gameManagerUpdate()
    {
        GameManager.control.party = party;
        GameManager.control.pos = pos;
        GameManager.control.dir = directionV;
    }

    public void updatePos()
    {
        pos = GameManager.control.pos;
        directionV = GameManager.control.dir;
    }
}