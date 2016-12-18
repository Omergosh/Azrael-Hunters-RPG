using UnityEngine;
using System.Collections;

public class NPC_Dialogue : MonoBehaviour {

    public string message = "Hi there";

	public void Interact() {
        Debug.Log(message);
    }
}
