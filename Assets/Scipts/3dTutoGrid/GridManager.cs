using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] Vector2Int gridSize;

    [SerializeField] int unityGridSize;

    private BlockingTrigger trigger;

    public int UnityGridSize{

        get { return unityGridSize; }}

    
    Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();
              //la clau (la Def) es la posició del Node 
    //Açò es una estructura de datos per a guardar parelles de valors clau (Paraula, Def)
    public Dictionary<Vector2Int, Node> Grid { 
        get { return grid;}
    }

    private void Awake()
    {
        CreateGrid();
        trigger = FindObjectOfType<BlockingTrigger>();


    }

    private void Update()
    {   
        if(Input.GetKeyDown(KeyCode.Alpha2))
        ShowBlockNodes();   
    }

    public Node GetNode(Vector2Int coordinates)
    {

        if (grid.ContainsKey(coordinates))
        {
            return grid[coordinates];
        }
        else
        {
            return null;
        }
    }

    public void BlockNode(Vector2Int coordinates){

        if(grid.ContainsKey(coordinates)){

            grid[coordinates].walkable = false;
           // FindObjectOfType<Pathfinding>().GetNewPath();
            
            //LA ÚLTIMA LINEA RESETEA L'ALGORITME, LLEVAR DESPRÉS.....+++++++++++++++++++
        }
    }
    
    
    public void UnblockNode(Vector2Int coordinates)
    {   //Gaste esta funció pa desbloquejar durant l'execució de la rotació

        if (grid.ContainsKey(coordinates))
        {
            grid[coordinates].walkable = true;
        }
    }


    public void ResetNodes(){

        foreach(KeyValuePair<Vector2Int, Node> entry in grid){

            entry.Value.connectTo = null;
            entry.Value.explored = false;
            entry.Value.path = false;
        }
    }

    



    public Vector2Int GetCoordinatesFromPosition(Vector3 position){

        Vector2Int coordinates = new Vector2Int();

        // coordinates.x = Mathf.RoundToInt(position.x / unityGridSize);
        // coordinates.y = Mathf.RoundToInt(position.z / unityGridSize);
        
        
        
        coordinates.x = Mathf.FloorToInt(position.x / unityGridSize);
        coordinates.y = Mathf.FloorToInt(position.z / unityGridSize);
        
        //++REDONDEJAR CAP A BAIX PER ELS NUMS NEGATIUS+++++++++++++
        

        return coordinates;
    }


    public Vector3 GetPositionFromCoordinates(Vector2Int coordinates){

        Vector3 position = new Vector3();

        position.x = coordinates.x * unityGridSize;
        position.z = coordinates.y * unityGridSize;

        return position;
    }



    //TODO: Llevar açò al final, es sols per vore els nodes Blocked++++++++++++++++++++++

    void ShowBlockNodes(){

        Debug.Log("Blocked nodes: " + string.Join(",", BlockedNodeList()));
    }


    public List<Vector2Int> BlockedNodeList()
    {
        List<Vector2Int> blockedNodes = new List<Vector2Int>();
        foreach (var entry in grid)
        {

            if (!entry.Value.walkable)
            {

                blockedNodes.Add(entry.Key);
            }
        }
        return blockedNodes;
    }

    // private void CreateGrid()
    // {
    //     // for (int x = 0; x < gridSize.x; x++)
    //     // {
    //     //     for (int y = 0; y < gridSize.y; y++)

    //     //ESTE ES EL CORRECTE
    //     for (int x = -gridSize.x; x < gridSize.x; x++)
    //     {
    //         for (int y = -gridSize.y; y < gridSize.y; y++)
    //         {

    //             Vector2Int cords = new Vector2Int(x, y);
    //             Node node = new Node(cords, true);
    //             grid.Add(cords, node);

    //             //AÇÒ DE BAIX MANTINDREU COMENTAT

    //             //  GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
    //             //  Vector3 position = new Vector3(cords.x * unityGridSize, 0f, cords.y * unityGridSize);
    //             //  cube.transform.position = position;
    //             //  cube.transform.SetParent(transform);


    //         }
    //     }
    // }
    private void CreateGrid()
    {
        grid.Clear();
        // Busca todos los Tiles y usa sus coords reales
        foreach (var tile in FindObjectsOfType<Tile>())
        {
            Vector2Int c = tile.cords;
            grid[c] = new Node(c, !tile.IsBlockedInCase(1));
        }
    }


    //AÇO ES PER A BLOQUEJAR FRONTERES+++++++++++++++++++
    public void BlockConnection(Vector2Int from, Vector2Int to)
    {
        if (grid.ContainsKey(from) && grid.ContainsKey(to))
        {
            Vector2Int direction = to - from;
            grid[from].blockedConnections.Add(direction);
            grid[to].blockedConnections.Add(-direction);
        }
    }

}
