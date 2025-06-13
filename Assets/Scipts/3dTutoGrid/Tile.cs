using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{


    [SerializeField] bool blockInCase1;
    [SerializeField] bool blockInCase2;
    [SerializeField] bool blockInCase3;
    [SerializeField] bool blockInCase4;
    [SerializeField] public bool startingPoint;

    //Açò es pa ficar si 'BLOCKED' des del editor. Si no es necesari per a després simplement llevar-ho!!!!!!!!!!!!!!!!
    public Vector2Int cords;

    GridManager gridManager;

    bool isCurrentlyBlocked = false;

    void Awake()
    {

        SetCords();
    }

    void Start()
    {
        ApplyBlockForCase(1);
        // if (blocked)
        // {
        //     gridManager.BlockNode(cords);

        // }
        //Açò es fa soles a l'inici del programa, retocar després-+-+-++-+-+-+-+-+-+!!!!!!!!!!!!!

    }

    private void SetCords()
    {
        gridManager = FindObjectOfType<GridManager>();
        int x = (int)transform.position.x;
        int z = (int)transform.position.z;

        cords = new Vector2Int(x / gridManager.UnityGridSize, z / gridManager.UnityGridSize);
    }
    
    public void ApplyBlockForCase(int caseNumber)
    {
        bool isBlocked = false;
        switch (caseNumber)
        {
            case 1: isBlocked = blockInCase1; break;
            case 2: isBlocked = blockInCase2; break;
            case 3: isBlocked = blockInCase3; break;
            case 4: isBlocked = blockInCase4; break;
            default: isBlocked = false; break;
        }

        //if (isBlocked && !isCurrentlyBlocked)
         if (isBlocked )
        {
            // Bloquear
            gridManager.BlockNode(cords);
            // isCurrentlyBlocked = true;
        }
        // else if (!isBlocked && isCurrentlyBlocked)
        else if (!isBlocked)
        {
            gridManager.UnblockNode(cords);
            // isCurrentlyBlocked = false;
        }
    }
}
