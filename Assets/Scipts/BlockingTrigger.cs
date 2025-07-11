using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockingTrigger : MonoBehaviour
{

    GridManager gridManager;
    public int _rotationCase = 1;
    private Vector2Int? currentBlocked;
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
        // Si la unidad muere y desactiva este objeto, limpiamos la última casilla
        if (currentBlocked.HasValue)
        {
            gridManager.UnblockNode(currentBlocked.Value);
            currentBlocked = null;
        }
        Rotation.OnCaseChanged -= HandleCaseChanged;
    }

    public void HandleCaseChanged(int newCase)
    {
        _rotationCase = newCase;
    }



    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tile"))
        {

            Tile tile = other.GetComponent<Tile>();
            Vector2Int cords = tile.cords;
            gridManager.BlockNode(cords);
            
            currentBlocked = cords;
            
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
                if (currentBlocked.HasValue && currentBlocked.Value == cords)
                    currentBlocked = null;
//                Debug.Log($"Casilla desbloqueada: ({cords.x}, {cords.y})");
            }
        }
    }

}
