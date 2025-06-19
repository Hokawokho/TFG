using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockingTrigger : MonoBehaviour
{

    GridManager gridManager;
    private int _rotationCase = 1;
    // Start is called before the first frame update



    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        Rotation.OnCaseChanged += HandleCaseChanged;
    }

    void OnDisable()
    {
        Rotation.OnCaseChanged -= HandleCaseChanged;
    }

    private void HandleCaseChanged(int newCase)
    {
        _rotationCase = newCase;
    }



    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Tile")){

            Tile tile = other.GetComponent<Tile>();
            Vector2Int cords = tile.cords;
            gridManager.BlockNode(cords);
            
        }

    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Tile"))
        {

            Tile tile = other.GetComponent<Tile>();
            Vector2Int cords = tile.cords;
            var rotation = FindObjectOfType<Rotation>();
            // Solo desbloquear si la tile NO debe estar bloqueada en este case
            if (!tile.IsBlockedInCase(_rotationCase))
            {
                Debug.Log($"current case es: {_rotationCase}");
                gridManager.UnblockNode(cords);
                Debug.Log($"Casilla desbloqueada: ({cords.x}, {cords.y})");
            }
        }
    }

}
