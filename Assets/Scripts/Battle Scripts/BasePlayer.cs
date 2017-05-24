﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasePlayer: MonoBehaviour {

    // player or enemy stats
    public string characterName;
    public int currentHealth = 150;
    public int maxHealth = 275;
    public int attackStat = 21;
    public int defenseStat = 24;
    public int agilityStat = 19;
    public int techStat = 16;

    public int actionPoints = 5;    // minor action costs 2, major costs 3
    public bool isDead = false;
    public bool canMove = true;
    public bool canAct = true;
    public bool conscious = true;   // player character thing. Specifically indicates death (HP <= 0), NOT a sleep status effect

    public bool isPlayerCharacter;  // distinguish between player and enemy characters

    public GameObject characterUI;
    public Image healthBar;
    public Text healthText;
    public Text attackText;
    public Text defenseText;
    public Text agilityText;
    public Text techText;
    public int ID;
    

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        /*
        //temporary health bar tester
        if(currentHealth > 0)
        {
            currentHealth -= 1;
        }*/
        //updateHealthBar();
	}

    public void battleManagerStart()
    {
        //Set up and update relevant UI elements
        if (isPlayerCharacter == true)
        {
            characterUI = GameObject.Find("Combatants UI/playerCombatantsUI/Player" + ID);
        }
        else
        {
            characterUI = GameObject.Find("Combatants UI/enemyCombatantsUI/Enemy" + ID);
        }

        healthBar = characterUI.transform.GetChild(4).GetComponent<Image>();
        healthText = characterUI.transform.GetChild(2).GetComponent<Text>();
        attackText = characterUI.transform.GetChild(9).GetComponent<Text>();
        defenseText = characterUI.transform.GetChild(10).GetComponent<Text>();
        agilityText = characterUI.transform.GetChild(11).GetComponent<Text>();
        techText = characterUI.transform.GetChild(12).GetComponent<Text>();

        updateStats();
        updateHealthBar();
    }

    public void takeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            conscious = false;
        }
    }

    public void updateStats()
    {
        //Update display
        attackText.text = attackStat.ToString();
        defenseText.text = defenseStat.ToString();
        agilityText.text = agilityStat.ToString();
        techText.text = techStat.ToString();
    }

    public void updateHealthBar()
    {
        healthText.text = currentHealth.ToString() + " HP";
        healthBar.transform.localScale = new Vector3(Mathf.Clamp(((float)currentHealth/ maxHealth), 0, 1), healthBar.transform.localScale.y, healthBar.transform.localScale.z);
    }
}
