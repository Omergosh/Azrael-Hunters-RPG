using UnityEngine;
using System.Collections;

public class NPC_Behavior : MonoBehaviour {

    public string message = "Hi there";
    public bool moveable = false;
    public Vector2 directionV;
    Vector2 pos;                    // For movement
    public float speed = 20.0f;      // Speed of movement
    public int layerMaskCollisions = 1 << 8;
    public int layerMaskInteracts = 1 << 9;
    private Vector3 displacement;   //So the moving object doesn't raycast into itself

    void Start()
    {
        pos = transform.position;          // Take the initial position
    }

    public void Interact() {
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
                Debug.Log(pos);
            }
            transform.position = Vector3.MoveTowards(transform.position, pos, speed);    // Moving to new pos
        }
    }

    bool CollisionCheck(Vector2 direction)
{
        displacement = 1.5f * (Vector3)direction;
        if (Physics2D.Raycast(transform.position + displacement, direction, 0.25f, layerMaskCollisions))
        {
            return true;
        }
        return false;
    }
}
