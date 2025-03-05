using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] Vector2Int gridSize;

    [SerializeField] int unityGridSize;

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

        coordinates.x = Mathf.RoundToInt(position.x / unityGridSize);
        coordinates.y = Mathf.RoundToInt(position.z / unityGridSize);

        return coordinates;
    }


    public Vector3 GetPositionFromCoordinates(Vector2Int coordinates){

        Vector3 position = new Vector3();

        position.x = coordinates.x * unityGridSize;
        position.z = coordinates.y * unityGridSize;

        return position;
    }

    private void CreateGrid()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {

                Vector2Int cords = new Vector2Int(x, y);
                Node node = new Node(cords, true);
                grid.Add(cords, node);

                //  GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //  Vector3 position = new Vector3(cords.x * unityGridSize, 0f, cords.y * unityGridSize);
                //  cube.transform.position = position;
                //  cube.transform.SetParent(transform);


            }
        }
    }



}
