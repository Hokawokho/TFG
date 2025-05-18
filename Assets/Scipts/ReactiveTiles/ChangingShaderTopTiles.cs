using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangingShaderTopTiles : MonoBehaviour
{

    private MeshRenderer meshRenderer;
    private Tile parentTile;
    private static Dictionary<Vector2Int, ChangingShaderTopTiles> tilesMap = new Dictionary<Vector2Int, ChangingShaderTopTiles>();
    void Awake()
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

        Vector2Int[] dirs = {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

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




    public void Show() => meshRenderer.enabled = true;
    public void Hide() => meshRenderer.enabled = false;
    

}
