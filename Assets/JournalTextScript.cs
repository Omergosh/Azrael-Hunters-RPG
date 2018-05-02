using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JournalTextScript : MonoBehaviour {

    public Button buttonComponent;
    public Text journalTitle;
    public string journalText;
    
    public JournalScrollList scrollList;

    private JournalEntry journalEntry;

    // Use this for initialization
    void Start () {
        buttonComponent.onClick.AddListener(HandleClick);
        //journalTitle.
	}

    public void HandleClick()
    {
        scrollList.refreshDisplay(journalText);
    }
}
