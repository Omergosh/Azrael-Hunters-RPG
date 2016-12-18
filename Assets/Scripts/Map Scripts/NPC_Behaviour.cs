using UnityEngine;
using System.Collections;

public class NPC_Behaviour : MonoBehaviour
{

    public string message = "Hi there";
    public bool moveable = false;
    public Vector2 directionV;
    Vector2 pos;                    // For movement
    public float speed = 1024.0f;      // Speed of movement
    public int layerMaskCollisions = 1 << 8;
    public int layerMaskInteracts = 1 << 9;
    private Vector3 displacement;   //So the moving object doesn't raycast into itself

    void Start()
    {
        pos = transform.position;          // Take the initial position
    }

    public void Interact()
    {
        Debug.Log(message);
    }

    public void Push(Vector2 directionV)
    {
        if (moveable)
        {
            if (CollisionCheck(directionV))
            {
                Debug.Log("I'm already at a wall.");
                //Walk into wall sound effect
            }
            else
            {
                Debug.Log("Pushing me around I see!");
                pos += (directionV);
            }
            transform.position = Vector2.MoveTowards(transform.position, pos, speed);    // Moving to new pos
        }
    }

    bool CollisionCheck(Vector2 direction)
    {
        displacement = 1.5f * (Vector3)direction;   // making sure the raycast doesn't start on the object
        if (Physics2D.Raycast(transform.position + displacement, direction, 0.25f, layerMaskCollisions))    //0.25 is just distance to check to the next square without entering the one after it
        {
            return true;
        }
        return false;
    }
}