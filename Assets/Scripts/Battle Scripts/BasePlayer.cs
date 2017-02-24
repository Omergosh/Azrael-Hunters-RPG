using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasePlayer: MonoBehaviour {

    //public GameObject player or enemy;
    public string characterName;
    public int currentHealth = 728;
    public int maxHealth = 1525;
    public int attackStat = 20;

    public bool isDead = false;
    public bool canMove = true;
    public bool canAct = true;

    public bool isPlayerCharacter;
    public Image healthBar;
    public Text healthText;
    

	// Use this for initialization
	void Start () {
        //healthBar = transform.FindChild("HealthBar").gameObject.GetComponent<Image>();
        //healthText = transform.FindChild("HealthText").gameObject.GetComponent<Text>();
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

    void updateHealthBar()
    {
        healthText.text = currentHealth.ToString() + " HP";
        healthBar.transform.localScale = new Vector3(Mathf.Clamp(((float)currentHealth/ maxHealth), 0, 1), healthBar.transform.localScale.y, healthBar.transform.localScale.z);
    }
}
