using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node 
{
    public Vector2Int cords;
    //IMPORTANT
    //Vector2Int perque soles ens interesen 2 dimensions
    public bool walkable;
    public bool explored;
    public bool path;
    public Node connectTo;

    //Açò es per a implementar que 2 nodes no estiguen conectats encara que estiguen al costat
     public HashSet<Vector2Int> blockedConnections = new HashSet<Vector2Int>();


    public Node(Vector2Int cords, bool walkable)
    {
        this.cords = cords;
        this.walkable = walkable;
    }
}
