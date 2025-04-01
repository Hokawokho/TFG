using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperiorRotation : MonoBehaviour
{
    private float currentRotation = 0f;

    [SerializeField] GridManager gridManager;
    
    // Start is called before the first frame update
    void Start()
    {
        //gridManager = FindObjectOfType<GridManager>();
    }

    // Update is called once per frame
    void Update()
    {

    //     if(Input.GetKeyDown(KeyCode.Alpha1)){

    //         currentRotation += 90f;
    //         if (currentRotation >= 360f)
    //         {
    //             currentRotation = 0f; // Reiniciar la rotación
    //         }

    //         transform.rotation = Quaternion.Euler(0, currentRotation, 0);

    //     }
     }

    // public void RotateTopMap(){

    //     currentRotation += 90f;
    //         if (currentRotation >= 360f)
    //         {
    //             currentRotation = 0f; // Reiniciar la rotación
    //         }

    //         transform.rotation = Quaternion.Euler(0, currentRotation, 0);

    // }

    public void RotateTopMap()
    {
        Dictionary<Transform, Vector2Int> previousPositions = new Dictionary<Transform, Vector2Int>();

        // 1. Guardar las posiciones actuales de las unidades antes de rotar
        foreach (Transform unit in transform.GetComponentsInChildren<Transform>())
        {
            if (unit.CompareTag("Unit"))
            {
                int posX = Mathf.RoundToInt(unit.position.x) / gridManager.UnityGridSize;
                int posY = Mathf.RoundToInt(unit.position.z) / gridManager.UnityGridSize;
                Vector2Int unitPosition = new Vector2Int(posX, posY);

                previousPositions[unit] = unitPosition;
            }
        }

        // 2. Aplicar la rotación
        currentRotation += 90f;
        if (currentRotation >= 360f)
        {
            currentRotation = 0f; // Reiniciar la rotación
        }
        transform.rotation = Quaternion.Euler(0, currentRotation, 0);

        // 3. Desbloquear las casillas donde estaban antes de rotar
        foreach (var entry in previousPositions)
        {
            gridManager.UnblockNode(entry.Value);
        }
        
    }
}
