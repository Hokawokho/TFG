using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HitPoints", menuName = "HitPoints")]
public class HitPoints : ScriptableObject
{
    public int hitPoints, maxHitPoints, initialHitPoints;
}
