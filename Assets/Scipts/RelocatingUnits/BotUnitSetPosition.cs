using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BotUnitSetPosition : MonoBehaviour
{

    public string walkableTerrain;
    public Transform currentContact { get; private set; }

    //public LayerMask groundLayer;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(walkableTerrain))
        {

            currentContact = other.transform;
            Debug.Log($"[BotUnitSetPosition] BotUnit '{gameObject.name}' ENTRÓ en Trigger con tile '{currentContact.name}'");
            
        }

    }
    // void OnTriggerExit(Collider other)
    // {
    //     if(other.CompareTag("Tile")){
            
    //         Tile tile = other.GetComponent<Tile>();
    //         Vector2Int cords = tile.cords;
    //         gridManager.UnblockNode(cords);
    //     }
    // }
}
