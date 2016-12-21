using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class PlayerControlsOverworld : MonoBehaviour {

    Vector3 pos;                    // For movement
    public float speed = 4.0f;      // Speed of movement
	public bool paused = false;
    public bool canMove = true;
    public Vector2 directionV;
    public int layerMaskCollisions = 1 << 8;
    public int layerMaskInteracts = 1 << 9;
    GameObject canvas;
	GameObject UI_DialogueSystem;
	GameObject UI_PauseMenu;
	GameObject UI_PauseMenuRoot;

	public bool pauseButtonDown = false;
	public bool pauseButtonPrevious = false;

    void Start()
    {
        pos = transform.position;          // Take the initial position
		directionV = Vector2.down;
		canvas = GameObject.Find("Canvas");   //finding the Canvas gameObject
		UI_DialogueSystem = canvas.transform.Find("UI_DialogueSystem").gameObject;
		UI_PauseMenu = canvas.transform.Find("UI_PauseMenu").gameObject;
		UI_PauseMenuRoot = UI_PauseMenu.transform.Find("UI_PauseMenuRoot").gameObject;
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
						//Walk into wall sound effect
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
							//Walk into wall sound effect
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
							//Walk into wall sound effect
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
							//Walk into wall sound effect
						}
					} else {
						pos += 2 * Vector3.down;
					}
				}
			}

	        transform.position = Vector3.MoveTowards(transform.position, pos, Time.deltaTime * speed);    // Moving to new pos
		}

        if (Input.GetAxisRaw("Interact") > 0) //user interaction
        {
            GameObject objectHit = InteractCheck(directionV);
            if (objectHit != null)
            {
                if (objectHit.GetComponent<NPC_Behavior>())
                {
                    NPC_Behavior NPC = objectHit.GetComponent<NPC_Behavior>();
                    UI_DialogueSystem.SetActive(true);
                    NPC.Interact(); 
                }
            }
		}
		if (Input.GetAxisRaw ("Pause") > 0) { //pause, pause, pause everything please! bring up the menu make everything stop
			pauseButtonDown = true;
		} else {
			pauseButtonDown = false;
		}
		if(pauseButtonDown == true && pauseButtonPrevious == false){
			if (paused) { //Unpausing
				UI_PauseMenuRoot.SetActive (false);
				UI_PauseMenu.SetActive (false);
				paused = false;
			} else {
				if (!UI_DialogueSystem.activeSelf) { //If the dialogue box is not open/active
					UI_PauseMenu.SetActive (true);
					UI_PauseMenuRoot.SetActive (true);
					paused = true;
				}
			}
		}
        if (Input.GetAxisRaw("Back") > 0) //abort! abort! you know, dialogue
        {
			if (paused) { //Unpausing
				UI_PauseMenuRoot.SetActive (false);
				UI_PauseMenu.SetActive (false);
				paused = false;
			}
			if (UI_DialogueSystem.activeSelf) {
				UI_DialogueSystem.SetActive (false);
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
				paused = false;
				break;
			case "party":
				//UI_PauseMenuRoot.SetActive (false);
				//UI_PauseMenu.SetActive (false);
				//paused = false;
				break;
			case "missions":
				//UI_PauseMenuRoot.SetActive (false);
				//UI_PauseMenu.SetActive (false);
				//paused = false;
				break;
			case "journal":
				//UI_PauseMenuRoot.SetActive (false);
				//UI_PauseMenu.SetActive (false);
				//paused = false;
				break;
			case "quit":
				Application.Quit ();
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
                NPC.Push(directionV);   //pushing things
            }
        }
    }
}