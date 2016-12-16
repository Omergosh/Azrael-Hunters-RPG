using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class PlayerControlsOverworld : MonoBehaviour {

    Vector3 pos;                    // For movement
    public float speed = 4.0f;      // Speed of movement
    public bool canMove = true;
    public Vector2 directionV;
    public int layerMaskCollisions = 1 << 8;
    public int layerMaskInteracts = 1 << 9;

    void Start()
    {
        pos = transform.position;          // Take the initial position
        directionV = Vector2.down;
    }

    void Update()
    {
        if (canMove)
        {
            if (Input.GetAxisRaw("Horizontal") < 0 && transform.position == pos)   // Left
            {
                directionV = Vector2.left;
                if (CollisionCheck(directionV))
                {
                    GameObject objectHit = InteractCheck(directionV);
                    pushing(objectHit);
                    //Walk into wall sound effect
                }
                else
                {
                    pos += 2 * Vector3.left;
                }
            }
            if (Input.GetAxisRaw("Horizontal") > 0 && transform.position == pos)   // Right
            {
                directionV = Vector2.right;
                if (CollisionCheck(directionV))
                {
                    {
                        GameObject objectHit = InteractCheck(directionV);
                        pushing(objectHit);
                        //Walk into wall sound effect
                    }
                }
                else
                {
                    pos += 2 * Vector3.right;
                }
            }
            if (Input.GetAxisRaw("Vertical") > 0 && transform.position == pos)   // Up
            {
                directionV = Vector2.up;
                if (CollisionCheck(directionV))
                {
                    {
                        GameObject objectHit = InteractCheck(directionV);
                        pushing(objectHit);
                        //Walk into wall sound effect
                    }
                }
                else
                {
                    pos += 2 * Vector3.up;
                }
            }
            if (Input.GetAxisRaw("Vertical") < 0 && transform.position == pos)   // Down
            {
                directionV = Vector2.down;
                if (CollisionCheck(directionV))
                {
                    {
                        GameObject objectHit = InteractCheck(directionV);
                        pushing(objectHit);
                        //Walk into wall sound effect
                    }
                }
                else
                {
                    pos += 2 * Vector3.down;
                }
            }
        }

        transform.position = Vector3.MoveTowards(transform.position, pos, Time.deltaTime * speed);    // Moving to new pos

        if (Input.GetAxisRaw("Interact") > 0) //user interaction
        {
            GameObject objectHit = InteractCheck(directionV);
            if (objectHit != null)
            {
                if (objectHit.GetComponent<NPC_Behavior>())
                {
                    NPC_Behavior NPC = objectHit.GetComponent<NPC_Behavior>();
                    NPC.Interact(); 
                }
            }
        }
        if (Input.GetAxisRaw("Back") > 0) //abort! abort! you know, dialogue
        {

        }
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