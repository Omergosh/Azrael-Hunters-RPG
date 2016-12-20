using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;   //allows lists (used in dialogue)

public class NPC_Behavior : MonoBehaviour {

    public bool moveable = false;
    private Vector2 directionV;
    private Vector2 pos;             // For movement
    private float speed = 8.0f;      // Speed of movement
    public int layerMaskCollisions = 1 << 8;
    public int layerMaskInteracts = 1 << 9;
    private Vector3 displacement;   //So the moving object doesn't raycast into itself

    //Dialogue stuff
    private GameObject UI_DialogueSystem;
    private GameObject UI_DialogueText;
    private Text myText;
    public List<string> myStrings = new List<string>(); //using lists for strings

    void Start()
    {
        pos = transform.position;          // Take the initial position
    }

    //interacting with objects/people
    public void Interact()
    {
        UI_DialogueSystem = GameObject.Find("UI_DialogueSystem");
        UI_DialogueText = GameObject.Find("UI_DialogueText");   //finding the UI_DialogueText gameObject
        myText = UI_DialogueText.GetComponent<Text>();          //references the text object in UI_DialogueText
        foreach(string myString in myStrings)
        {
            myText.text = myString;
            Debug.Log(myString);
            //yield return new WaitForSecondsRealtime(1);
        }
        //UI_DialogueSystem.SetActive(false);
    }

    public void Push(Vector2 directionV)
    {
        if (moveable)
        {
            if (CollisionCheck(directionV))
            {
                //Walk into wall sound effect
            }
            else
            {
                pos += 2 * (directionV);
            }
            transform.position = Vector3.Lerp(transform.position, pos, speed);    // Moving to new pos
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
}
