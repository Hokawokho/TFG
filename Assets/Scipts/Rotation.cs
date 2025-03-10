using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{

    public float rotationAngle = 90f; // Ángulo de rotación en grados
    public float rotationSpeed = 200f; // Velocidad de rotación en grados por segundo
    private bool isRotating = false;
    private float targetRotation;

    private List<Vector2Int> previousBlockedNodes = new List<Vector2Int>();
    //Llista per a guardar els nodes bloquejats pels girs
    private GridManager gridManager;
    //Açò es per a cridar a les funcions de l'escript i bloquejar nodes



    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();

        previousBlockedNodes.Add(new Vector2Int(5,5));
        gridManager.BlockNode(new Vector2Int(5,5));
        //Aquí es per a inicialitzar la llista de nodes bloquejats

        
    }

    
    void Update()
    {


        if(Input.GetKeyDown(KeyCode.R) &&!isRotating){
            targetRotation = transform.eulerAngles.y + rotationAngle;
            //EL 'Mathf.Repeat' ES PER A PROVAR SI EN EL 'case 0' HO LLIG MILLOR
            gridManager.ResetNodes();
            StartCoroutine(RotateSmoothly());
        
        }
        
        
        
        

    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 150, 30), transform.eulerAngles.ToString());
        GUI.Label(new Rect(10, 40, 150, 30), isRotating.ToString());

    }

    private System.Collections.IEnumerator RotateSmoothly()
    {
        isRotating = true;
        while (Mathf.Abs(Mathf.Repeat(transform.eulerAngles.y - targetRotation, 360)) > 0.1f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, targetRotation, 0), rotationSpeed * Time.deltaTime);
            yield return null;
        }
        transform.rotation = Quaternion.Euler(0, targetRotation, 0); // Asegurar que la rotación finaliza exactamente en el ángulo deseado
        isRotating = false;

        BlockNodeBasedOnRotation();
    }

    private void BlockNodeBasedOnRotation()
    {
        if(gridManager == null) return;

        foreach (Vector2Int node in previousBlockedNodes)
        {
            gridManager.UnblockNode(node);
        }
        previousBlockedNodes.Clear();   

       // gridManager.ResetNodes(); // Reset tots els nodes per a bloquejar nous

       
        Vector2Int newBlockedNode = Vector2Int.zero;
        int rotationState = Mathf.RoundToInt(targetRotation / 90f) % 4; // 0, 1, 2, 3 para cada rotación

        switch(rotationState)
        {
            case 0:
                newBlockedNode = new Vector2Int(5,5);
                break;

            case 1:
                newBlockedNode =new Vector2Int(2,2);
                break;

            case 2:
                newBlockedNode =new Vector2Int(3,3);
                break;

            case 3:
                newBlockedNode =new Vector2Int(4,4);
                break;


        }
        gridManager.BlockNode(newBlockedNode);
        previousBlockedNodes.Add(newBlockedNode);
    }

}



// if(Input.GetKey(KeyCode.R)){

        //     transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // }