using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTile : ScriptableObject {

    public float x;
    public float y;
    public int id;

	// Use this for initialization
	void Start () {
		
	}
	
    public void setX(float xCoord){
        x = xCoord;
    }

    public void setY(float yCoord){
        y = yCoord;
    }
    public void setID(int idValue)
    {
        id = idValue;
    }

    public float getX()
    {
        return x;
    }

    public float getY()
    {
        return y;
    }
    public float getID()
    {
        return id;
    }
}
