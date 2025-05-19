using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    

    [SerializeField] bool blocked;
    //Açò es pa ficar si 'BLOCKED' des del editor. Si no es necesari per a després simplement llevar-ho!!!!!!!!!!!!!!!!
    public Vector2Int cords;

    GridManager gridManager;

    void Awake()
    {
        SetCords();

        if (blocked)
        {
            gridManager.BlockNode(cords);

        }
        //Açò es fa soles a l'inici del programa, retocar després-+-+-++-+-+-+-+-+-+!!!!!!!!!!!!!

    }

    private void SetCords()
    {
        gridManager = FindObjectOfType<GridManager>();
        int x = (int)transform.position.x;
        int z = (int)transform.position.z;

        cords = new Vector2Int(x / gridManager.UnityGridSize, z / gridManager.UnityGridSize);
    }
}
