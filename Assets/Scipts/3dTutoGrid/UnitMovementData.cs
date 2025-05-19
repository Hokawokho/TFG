using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[Serializable]
public class UnitMovementData
{
        public GameObject unitData;
        public int maxTiles;

        public int remainingTiles;

        public float movementSpeed;

        public void ResetMovement()
        {
                remainingTiles = maxTiles;
        }
    
}
