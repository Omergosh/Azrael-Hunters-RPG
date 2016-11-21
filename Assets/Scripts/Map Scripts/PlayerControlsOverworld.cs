using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class PlayerControlsOverworld : MonoBehaviour {

    Vector3 pos;                    // For movement
    public float speed = 4.0f;      // Speed of movement
    public string direction = "right";
    public Vector2 directionV = Vector2.right;

    //Collision stuff
    int colLayerMask;

    void Start()
    {
        pos = transform.position;          // Take the initial position
        colLayerMask = 1 << 8;//LayerMask.NameToLayer("Collisions");
    }

    void Update()
    {
        if (Input.GetAxisRaw("Horizontal") < 0 && transform.position == pos)   // Left
        {
            directionV = Vector2.left;
            if (CheckCollisions(directionV))
            {
                //Try to walk into wall sound effect
            }else
            {
                //Successful walk!
                pos += 2 * Vector3.left;
            }
        }
        if (Input.GetAxisRaw("Horizontal") > 0 && transform.position == pos)   // Right
        {
            directionV = Vector2.right;
            if (CheckCollisions(directionV))
            {
                //Try to walk into wall sound effect
            }
            else
            {
                //Successful walk!
                pos += 2 * Vector3.right;
            }
        }
        if (Input.GetAxisRaw("Vertical") > 0 && transform.position == pos)   // Up
        {
            directionV = Vector2.up;
            if (CheckCollisions(directionV))
            {
                //Try to walk into wall sound effect
            }
            else
            {
                //Successful walk!
                pos += 2 * Vector3.up;
            }
        }
        if (Input.GetAxisRaw("Vertical") < 0 && transform.position == pos)   // Down
        {
            directionV = Vector2.down;
            if (CheckCollisions(directionV))
            {
                //Try to walk into wall sound effect
            }
            else
            {
                //Successful walk!
                pos += 2 * Vector3.down;
            }
        }

        transform.position = Vector3.MoveTowards(transform.position, pos, Time.deltaTime * speed);    // Moving to new pos

        //if (Input.GetAxisRaw("Interact") > 0) //user interaction
        //if (Input.GetAxisRaw("Back") > 0) //abort! abort! you know, dialogue
    }

    bool CheckCollisions(Vector2 direction)
    {
        if (Physics2D.Raycast(transform.position, direction, 2.0f, colLayerMask))
        {
            return true;
        }

        return false;
    }
}