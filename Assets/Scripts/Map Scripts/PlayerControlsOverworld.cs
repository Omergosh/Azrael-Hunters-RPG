using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class PlayerControlsOverworld : MonoBehaviour {

    Vector3 pos;                    // For movement
    public float speed = 4.0f;      // Speed of movement
    private enum Directions { up, right, down, left};          // direction faced
    private Directions direction;
    Vector3 mydirection;

    void Start()
    {
        pos = transform.position;          // Take the initial position
    }

    void Update()
    {
        if (Input.GetAxisRaw("Horizontal") < 0 && transform.position == pos)   // Left
        {        
            pos += 2 * Vector3.left;
            Directions direction = Directions.left;
        }
        if (Input.GetAxisRaw("Horizontal") > 0 && transform.position == pos)   // Right
        {        
            pos += 2 * Vector3.right;
            Directions direction = Directions.right;
        }
        if (Input.GetAxisRaw("Vertical") > 0 && transform.position == pos)   // Up
        {       
            pos += 2 * Vector3.up;
            Directions direction = Directions.up;
        }
        if (Input.GetAxisRaw("Vertical") < 0 && transform.position == pos)   // Down
        {        
            pos += 2 * Vector3.down;
            Directions direction = Directions.down;
        }

        transform.position = Vector3.MoveTowards(transform.position, pos, Time.deltaTime * speed);    // Moving to new pos

        if (Input.GetAxisRaw("Interact") > 0)   //user interaction
        {
            
            if(direction == Directions.up)
            {
                mydirection = new Vector3(0, 1);
            }
            if (direction == Directions.right)
            {
                mydirection = new Vector3(1, 0);
            }
            if (direction == Directions.down)
            {
                mydirection = new Vector3(0, -1);
            }
            if (direction == Directions.left)
            {
                mydirection = new Vector3(-1, 0);
            }

            if(Physics.Raycast(pos, mydirection, 20.0f))
                {
                    print("There is something there!");
                }
        }
        //if (Input.GetAxisRaw("Back") > 0) //abort! abort! you know, dialogue
    }

}