using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasePlayer: MonoBehaviour {

    //public GameObject player;
    public int currentHealth = 728;
    public int maxHealth = 1525;
    public Image HealthBar;
    public Text healthText;
    

	// Use this for initialization
	void Start () {
        HealthBar = transform.FindChild("HealthBar").gameObject.GetComponent<Image>();
        healthText = transform.FindChild("HealthText").gameObject.GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        if(currentHealth > 0)
        {
            currentHealth -= 1;
        }
        updateHealthBar();
	}

    void updateHealthBar()
    {
        healthText.text = currentHealth.ToString() + " HP";
        HealthBar.transform.localScale = new Vector3(Mathf.Clamp(((float)currentHealth/ maxHealth), 0, 1), HealthBar.transform.localScale.y, HealthBar.transform.localScale.z);
    }
}
