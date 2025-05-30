using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] 
    private GameObject mouseIndicator, cellIndicator;
    

    [SerializeField] 
    private InputManager inputManager;

    [SerializeField] 
    private GridLayout grid;


    private void Update(){


        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        mouseIndicator.transform.position = mousePosition;
        cellIndicator.transform.position = grid.CellToWorld(gridPosition);


    }
}
