using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockingTrigger : MonoBehaviour
{

    GridManager gridManager;
    // Start is called before the first frame update
    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Tile")){

            Tile tile = other.GetComponent<Tile>();
            Vector2Int cords = tile.cords;
            gridManager.BlockNode(cords);
            
        }

    }
    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Tile")){
            
            Tile tile = other.GetComponent<Tile>();
            Vector2Int cords = tile.cords;
            gridManager.UnblockNode(cords);
        }
    }

}
