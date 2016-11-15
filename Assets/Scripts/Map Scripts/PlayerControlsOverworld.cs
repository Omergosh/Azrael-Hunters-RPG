using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class PlayerControlsOverworld : MonoBehaviour {

    Vector3 pos;                    // For movement
    public float speed = 4.0f;      // Speed of movement

    void Start()
    {
        pos = transform.position;          // Take the initial position
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.A) && transform.position == pos)   // Left
        {        
            pos += 2 * Vector3.left;
        }
        if (Input.GetKey(KeyCode.D) && transform.position == pos)   // Right
        {        
            pos += 2 * Vector3.right;
        }
        if (Input.GetKey(KeyCode.W) && transform.position == pos)   // Up
        {       
            pos += 2 * Vector3.up;
        }
        if (Input.GetKey(KeyCode.S) && transform.position == pos)   // Down
        {        
            pos += 2 * Vector3.down;
        }

        transform.position = Vector3.MoveTowards(transform.position, pos, Time.deltaTime * speed);    // Moving to new pos

        //if (Input.GetKey(KeyCode.Z) //user interaction
    }
}