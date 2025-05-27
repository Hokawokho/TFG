using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Scenario
{

    public float scenarioValue;

    private Vector2Int startTile;
    private Vector2Int targetTile;

    public bool hasAttacked;
    

    public Scenario(float scenarioValue, Vector2Int startTile, Vector2Int targetTile, bool hasAttacked)
    {
        this.scenarioValue = scenarioValue;
        this.startTile = startTile;
        this.targetTile = targetTile;
        this.hasAttacked = hasAttacked;
    }

    public Scenario()
    {

        this.scenarioValue = -100000;
        this.startTile = new Vector2Int(-1,-1);
        this.targetTile = new Vector2Int(-1,-1);
        this.hasAttacked = false;
    }
}
