using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "UnitType", menuName = "UnitType")]
public class UnitType : ScriptableObject

{
    public int hitPoints, maxHitPoints, initialHitPoints, movement;
    public string attackType;
}
