using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    [SerializeField] Vector2Int startCords;
    public Vector2Int StartCords {get {return startCords;}}

    [SerializeField] Vector2Int targetCords;
    public Vector2Int TargetCords {get {return targetCords;}}

    Node startNode;
    Node targetNode;
    Node currentNode;

    Queue<Node> frontier = new Queue<Node>();
    Dictionary<Vector2Int, Node> reached = new Dictionary<Vector2Int, Node>();

    GridManager gridManager;
    Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();

    Vector2Int[] searchOrder = {Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };
        //Açò es per a marcar l'ordre en que mirarem els posibles nodes, primer el de la dreta, despres esquerra, dalt i baix


    private void Awake()
    {
        gridManager = FindObjectOfType<GridManager>();
        if(gridManager != null)
        {
            grid = gridManager.Grid;
        }
    }





    public List<Node> GetNewPath(){

        return GetNewPath(startCords);        

    }

    public List<Node> GetNewPath(Vector2Int coordinates){

        gridManager.ResetNodes();

        BreadthFirstSearch(coordinates);
        return BuildPath();
    }

    //Este nos devuelve una lista con los nodos del path
    //lo reseteamos cada vez porque cada casilla tendra un path diferente ca vegada







    void BreadthFirstSearch(Vector2Int coordinates){

        startNode.walkable = true;
        targetNode.walkable = true;

        frontier.Clear();
        reached.Clear();

        bool isRunning = true;

        frontier.Enqueue(grid[coordinates]);
        //Esto encola nuestro nodo donde empezamos

        reached.Add(coordinates, grid[coordinates]);
        // esto marca de 1ras el nodo base

        while (frontier.Count > 0 && isRunning == true){

            currentNode = frontier.Dequeue();
            currentNode.explored = true;
            ExploreNeighbors();
            if(currentNode.cords == targetCords){
                
                isRunning = false;
                currentNode.walkable = false;
                //este ultim es pa que no es puga anar al Tile on estiga una unitat
            }
        }
        //El bucle para calcular el path hasta que no haya mas nodos (fronteras)
    }





    void ExploreNeighbors(){

        List<Node> neighbors = new List<Node>();

        foreach(Vector2Int direction in searchOrder){

            Vector2Int neighborCoords = currentNode.cords + direction;

            if(grid.ContainsKey(neighborCoords)){

                //neighbors.Add(grid[neighborCoords]);

                //RETOQUE PARA CONECTADOS NODES++++++++++++++++++++++
                Node neighbor = grid[neighborCoords];

                //EXTRA PA CONECTADOS NODES
                if (!currentNode.blockedConnections.Contains(direction) && 
                !neighbor.blockedConnections.Contains(-direction)) 
                {
                    neighbors.Add(neighbor);
                }
            }

            
            //Açò es per a no afegir nodes que no estiguen en el Grid
        }

        // Este metode fa una llista on guarda les cords dels veins calculats en sarchOrder

        foreach(Node neighbor in neighbors){

            if(!reached.ContainsKey(neighbor.cords) && neighbor.walkable){
                //1er mirem que no em passat per ell i que es pot accedir a este

                neighbor.connectTo = currentNode;
                reached.Add(neighbor.cords, neighbor);
                frontier.Enqueue(neighbor);
            }
            //Afegim els veins que no tenim en frontera i que es poden accedir a aquest
            //Açò es fara fins que el bucle de l'algoritme ixca del loop++++++++++++++++++++

        }
    }    


    List<Node> BuildPath(){

        List<Node> path = new List<Node>();
        Node currentNode = targetNode;
        
        path.Add(currentNode);
        currentNode.path = true;
        
        while(currentNode.connectTo!= null){

            currentNode = currentNode.connectTo;
            path.Add(currentNode);
            currentNode.path = true;
        }
        
        path.Reverse();
        //Se hace reverse porque el camino se hace desde el objetivo hasta el inicio usando connectTo
        return path;
    }


    public void NotifyRecievers(){

        BroadcastMessage("RecalculatePath", false, SendMessageOptions.DontRequireReceiver);
        //Aço llança cualsevol metode que es diga "RecualculatePath" que esta en UnitController+++++++++++++++++++++++++++++++++++++++++++++
    }

    public void SetNewDestination(Vector2Int startCoordinates, Vector2Int targetCoordinates){

        startCords = startCoordinates;
        targetCords = targetCoordinates;
        startNode = grid[this.startCords];
        targetNode = grid[this.targetCords];
        GetNewPath();

        //Açò es pa resetear l'algoritme
    }
}
