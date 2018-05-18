using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasePlayer: MonoBehaviour {

    // Progression (Level + XP)
    public int level = 1;
    public int currentEXP = 0;
    public int requiredEXP = 100;

    // Player or Enemy stats (current)
    public string characterName;
    public int currentHealth;
    public int maxHealth;
    public int attackStat;
    public int defenseStat;
    public int agilityStat;
    public int techStat;

    // Base stats
    public float healthModifier = 1.0f;
    public int attackBase;
    public int defenseBase;
    public int agilityBase;
    public int techBase;

    public int gridPosition = -1;   // Used to determine front/backline stuff.
                                    // -If unspecified at start of battle (-1), position on grid is randomly assigned from open spaces.
    public int actionPoints = 5;    // Minor action costs 2, major costs 3
    public bool isDead = false;
    public bool canMove = true;
    public bool canAct = true;
    public bool conscious = true;   // Player characters only. Specifically indicates death (HP <= 0), NOT a sleep status effect

    public bool isPlayerCharacter;  // Distinguish between player and enemy characters

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
        // Set up and update relevant UI elements
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

        updateStatDisplays();
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

    public bool gainEXP(int expToGain)
    {
        // Return true if combatant levels up, false if not
        currentEXP += expToGain;
        requiredEXP = level * 100;      // Update required EXP
        if(currentEXP >= requiredEXP)
        {
            setLevel(level + 1);
            requiredEXP = level * 100;
            return true;                // Return true; combatant leveled up
        }
        return false;
    }

    public void setLevel(int newLevel)
    {
        // Sets level and updates Health/Stats
        /*
        if(newLevel == -1) // Use '-1' to refresh Health/Stats without changing level
        {
            newLevel = level;
        }
        level++;
        requiredEXP = level * 100;
        updateHealth();
        updateStats();
        */
        level = newLevel;
        requiredEXP = level * 100;
        updateHealth();
        updateStats();
    }

    public void setExpByLevel(int levelToReach = -1)
    {
        if (levelToReach <= 1)
        {
            currentEXP = 0;
        }
        else if(levelToReach == 2)
        {
            currentEXP = 100;
        }
        else
        {
            currentEXP = 0;
            level = 1;
            while(level < levelToReach)
            {
                currentEXP += level * 100;
                level += 1;
            }
        }
        requiredEXP = level * 100;
    }

    public void updateHealth()
    {
        // Calculates health based on level
        // Update Maximum Hit Points
        int oldHealth = maxHealth;
        maxHealth = Mathf.FloorToInt(maxHealth + ((level - 1.0f) * 10.0f * healthModifier));

        // Resolve Current Hit Points
        currentHealth += (maxHealth - oldHealth);
    }

    public void updateStats()
    {
        // Calculates stats based on level
        // First, reset temp to level 1 stats (base stats)
        //int tempLevel = 1;
        float attack = attackBase * level;
        float defense = defenseBase * level;
        float agility = agilityBase * level;
        float tech = techBase * level;
 
        // Increase temp stats based on level
        /*while(tempLevel < level)
        {
            attack += ((3.6f + ((attackBase - 20.0f) * 0.1f)) * (tempLevel - 1));
            defense += ((3.6f + ((defenseBase - 20.0f) * 0.1f)) * (tempLevel - 1));
            agility += ((3.6f + ((agilityBase - 20.0f) * 0.1f)) * (tempLevel - 1));
            tech += ((3.6f + ((techBase - 20.0f) * 0.1f)) * (tempLevel - 1));
            tempLevel++;
            
        }*/

        // Update to temp stats
        attackStat = Mathf.FloorToInt(attack);
        defenseStat = Mathf.FloorToInt(defense);
        agilityStat = Mathf.FloorToInt(agility);
        techStat = Mathf.FloorToInt(tech);
    }

    public void updateStatDisplays()
    {
        // Update display
        attackText.text = attackStat.ToString();
        defenseText.text = defenseStat.ToString();
        agilityText.text = agilityStat.ToString();
        techText.text = techStat.ToString();
    }

    public void updateHealthBar()
    {
        // Update Health Bar
        healthText.text = currentHealth.ToString() + " HP";
        healthBar.transform.localScale = new Vector3(Mathf.Clamp(((float)currentHealth/ maxHealth), 0, 1), healthBar.transform.localScale.y, healthBar.transform.localScale.z);
    }
}
