//using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class JournalEntry
{
    public string journalTitle;
    public Sprite icon;
    public string text = "This is placeholder text";
}

public class JournalScrollList : MonoBehaviour {

    public List<JournalEntry> journalEntryList;
    public Transform contentPanel;
    public Text journalTextView;

    // Use this for initialization
    void Start () {
	}
	
    public void refreshDisplay(string newText)
    {
        journalTextView.text = newText;
    }
}
