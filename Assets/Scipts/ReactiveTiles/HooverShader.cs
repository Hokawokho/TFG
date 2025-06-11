using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HooverShader : MonoBehaviour
{

    private MeshRenderer meshRenderer;
    private GridManager gridManager;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
        gridManager = FindObjectOfType<GridManager>();


    }

    void OnMouseEnter()
    {
        if (!PauseMenu.GameIsPaused)
        {
            Vector2Int coords = gridManager.GetCoordinatesFromPosition(transform.position);

            Node node = gridManager.GetNode(coords);

            if (node != null && node.walkable)
            {
                meshRenderer.enabled = true;
            }
        }
    }

    void OnMouseExit()
    {
        meshRenderer.enabled = false;
    }
    
   

}
