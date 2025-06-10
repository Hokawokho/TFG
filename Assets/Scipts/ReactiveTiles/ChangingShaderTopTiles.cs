using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangingShaderTopTiles : MonoBehaviour
{

    private MeshRenderer meshRenderer;
    private Tile parentTile;
    private static Dictionary<Vector2Int, ChangingShaderTopTiles> tilesMap = new Dictionary<Vector2Int, ChangingShaderTopTiles>();
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;        

        parentTile = GetComponentInParent<Tile>();
        if (parentTile != null)
            tilesMap[parentTile.cords] = this;
        else
            Debug.LogError("ChangingTiles: no se encontró Tile en ningún padre.");
    }

    void OnDestroy()
    {
        if (parentTile != null)
            tilesMap.Remove(parentTile.cords);
    }

    // Desactiva todos los highlights
    public static void ClearAllHighlights()
    {
        foreach (var t in tilesMap.Values)
            t.Hide();
    }

    // Activa sólo las 4 adyacentes a `center`
    public static void HighlightTilesAround(Vector2Int center, GridManager grid)
    {
        ClearAllHighlights();

        Vector2Int[] dirs = {Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right};
            
            
            
            
        

        foreach (var dir in dirs)
        {
            Vector2Int neigh = center + dir;
            // Comprueba que exista nodo y, si quieres, que sea caminable:
            Node node = grid.GetNode(neigh);
            if (node != null /* && node.walkable */ && tilesMap.TryGetValue(neigh, out var tile))
                                    //out var tile==> LO QUE HACE ES GUARDAR EL VALUE DE 'neigh'
                                        //             EN TILE (es a dir es una var que pilla
                                        //el valor quan TryGetValue dona true, y el vertader valor el pilla tile)  
            {
                tile.Show();
            }
        }
    }


    public static void HighlightLineTiles(Vector2Int center, GridManager grid)
    {
        // 1) Limpiar cualquier highlight previo
        ClearAllHighlights();

        Vector2Int[] dirs = {Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right};

        // 3) Para cada dirección, avanzamos paso a paso hasta que se acabe la rejilla
        foreach (var dir in dirs)
        {
            int step = 1;
            while (true)
            {
                Vector2Int pos = center + dir * step;
                // 3a) Salir si estamos fuera de la rejilla
                if (grid.GetNode(pos) == null)
                    break;

                // 3b) Salir si el nodo está bloqueado
                //if (!grid.GetNode(pos).walkable)
                //    break;

                // 3c) Si hay instancia de ChangingShaderTopTiles, mostrarla
                if (tilesMap.TryGetValue(pos, out var tile))
                    tile.Show();
                else
                    // si no hay tile (por cómo esté construido el grid), paramos
                    break;

                step++;
            }
        }
    }

    public static void HighlightCostTiles(Vector2Int center, GridManager grid, int range)
    {
        ClearAllHighlights();

        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        var visited = new HashSet<Vector2Int> { center };
        //a diferencia del Map este es sols 1 valor, val pa vore si ja esta guardat o no
        // millor HashSet pa busqueda en amplaria != profunditat (on es millor una pila)

        var queue = new Queue<(Vector2Int pos, int dist)>();
        queue.Enqueue((center, 0));


        // Llista auxiliar pa guardar lo que hi ha que resaltar
        var tilesToShow = new List<Vector2Int>();

        // Recorre en anchura hasta 'range'
        while (queue.Count > 0)
        {
            var (pos, dist) = queue.Dequeue();

            // No queremos resaltar la casilla de origen (dist == 0)
            if (dist > 0 && tilesMap.TryGetValue(pos, out var tile))
                tilesToShow.Add(pos);

            // Si ya llegamos al rango máximo, no expandimos más
            if (dist == range)
                continue;

            Node currentNode = grid.GetNode(pos);
            if (currentNode == null)
                continue;

            // Explorar vecinos
            foreach (var dir in dirs)
            {
                var next = pos + dir;
                if (visited.Contains(next))
                    continue;


                //visited.Add(next);

                Node neighborNode = grid.GetNode(next);

                if (neighborNode != null
                    && neighborNode.walkable
                    && !currentNode.blockedConnections.Contains(dir)
                    && !neighborNode.blockedConnections.Contains(-dir))
                {
                    visited.Add(next);
                    queue.Enqueue((next, dist + 1));
                }
            }
        }

        foreach (var pos in tilesToShow)
        {

            if (tilesMap.TryGetValue(pos, out var tile))
                tile.Show();
        }
}



    public void Show() => meshRenderer.enabled = true;
    public void Hide() => meshRenderer.enabled = false;
    

}
