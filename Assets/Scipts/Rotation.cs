using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    private UnitController unitController;
    private List<Transform> unitsRedirigir = new List<Transform>(); // Lista de todas las unidades seleccionadas
    //GIR-ADD+-+-+-+-+-+-+-+-+





    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();

        previousBlockedNodes.Add(new Vector2Int(5,5));
        gridManager.BlockNode(new Vector2Int(5,5));
        //Aquí es per a inicialitzar la llista de nodes bloquejats

        unitController = FindObjectOfType<UnitController>();
        //Açó es per a moure de una casilla x a una y en Gir
        //GIR-ADD+-+-+-+-+-+-+-+-+
    }

    
    void Update()
    {


        if(Input.GetKeyDown(KeyCode.R) &&!isRotating){

            SelectAllUnits();


            //FOLLOWER-ADD+-+-+-+-+-+-+-+-+INITIAL

            
            // if(followerUnit != null)
            // {

            //     followerUnit.ChangeFollowing();
            // }
            //FOLLOWER-ADD+-+-+-+-+-+-+-+-+FINAL


           // MovePositionRotation(new Vector2Int(6,6), new Vector2Int(8,8));
            //GIR-ADD+-+-+-+-+-+-+-+-+

            targetRotation = transform.eulerAngles.y + rotationAngle;
            //EL 'Mathf.Repeat' ES PER A PROVAR SI EN EL 'case 0' HO LLIG MILLOR
            gridManager.ResetNodes();
            StartCoroutine(RotateSmoothly());

            
        
        }
    }

    void SelectAllUnits (){

        unitsRedirigir.Clear();
        GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");

        foreach (GameObject unit in units)
        {
            unitsRedirigir.Add(unit.transform);
        }

    }


    
    //GIR-ADD+-+-+-+-+-+-+-+-+INICI
    // private void MovePositionRotation(Vector2Int from, Vector2Int to)
    private void MovePositionRotation(Dictionary<Vector2Int, Vector2Int> casillasRedir)
    {
        if(unitController == null || unitsRedirigir.Count == 0) return;

        foreach(Transform unit in unitsRedirigir){

            int posX = Mathf.RoundToInt(unit.position.x)/ gridManager.UnityGridSize;
            int posY = Mathf.RoundToInt(unit.position.z)/ gridManager.UnityGridSize;
            Vector2Int unitPosition = new Vector2Int(posX, posY);

            //gridManager.UnblockNode(unitPosition);

            if(casillasRedir.ContainsKey(unitPosition))
            {
                Vector2Int dest = casillasRedir[unitPosition];
                Debug.Log($"Unidad detectada en {unitPosition}, moviéndola a {dest}");

                

                unitController.selectedUnit = unit;
                unitController.unitSelected = true;
                //Açò es per a marcar la unitat com a selecciona

                // unitController.pathFinder.SetNewDestination(unitPosition, dest);
                // unitController.RecalculatePath(true);
                Vector3 pos = gridManager.GetPositionFromCoordinates(dest);;
                pos.y = unitController.selectedUnit.position.y;
                unitController.selectedUnit.position = pos;
                //+-++-+-+

                gridManager.BlockNode(dest);
                unitController.RecalculatePath(true, false);

                //NO HACE FALTA--pero si ya encontramos la unidad nos podemos salir de este.
            }
        }
        return;
    }
    //GIR-ADD+-+-+-+-+-+-+-+-+FIN

   
    private void unselectAllUnits(){

        unitsRedirigir.Clear();

    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 150, 30), transform.eulerAngles.ToString());
        GUI.Label(new Rect(10, 40, 150, 30), isRotating.ToString());

    }

    private IEnumerator RotateSmoothly()
    {
        isRotating = true;
        while (Mathf.Abs(Mathf.Repeat(transform.eulerAngles.y - targetRotation, 360)) > 0.1f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, targetRotation, 0), rotationSpeed * Time.deltaTime);
            yield return null;
        }
        transform.rotation = Quaternion.Euler(0, targetRotation, 0); 
        // Asegurar que la rotación finaliza exactamente en el ángulo deseado
        isRotating = false;

        BlockNodeBasedOnRotation();

        unselectAllUnits();
    }

    private void BlockNodeBasedOnRotation()
    {
        if(gridManager == null) return;

        // UnblockPreviousNodes();

       // gridManager.ResetNodes(); // Reset tots els nodes per a bloquejar nous

       
        Vector2Int newBlockedNode = Vector2Int.zero;
        int rotationState = Mathf.RoundToInt(targetRotation / 90f) % 4; // 0, 1, 2, 3 para cada rotación
        Dictionary<Vector2Int, Vector2Int> casillasRedir = new Dictionary<Vector2Int, Vector2Int>();
        switch(rotationState)
        {
            case 0:
                newBlockedNode = new Vector2Int(5,5);
                break;

            case 1:
                newBlockedNode =new Vector2Int(2,2);
                // Dictionary<Vector2Int, Vector2Int> casillasRedir = new Dictionary<Vector2Int, Vector2Int>();

                // {
                //     { new Vector2Int(6,6), new Vector2Int(8,8) },
                //     { new Vector2Int(7,7), new Vector2Int(2,2) }
                // };
                casillasRedir.Add(new Vector2Int(3,0), new Vector2Int(0,6));
                casillasRedir.Add(new Vector2Int(7,7), new Vector2Int(4,4));
                gridManager.UnblockNode(new Vector2Int(3,0));
                gridManager.UnblockNode(new Vector2Int(7,7));

                MovePositionRotation(casillasRedir);
                break;

            case 2:
                newBlockedNode =new Vector2Int(3,3);
                break;

            case 3:
                newBlockedNode =new Vector2Int(4,4);
                //MovePositionRotation(new Vector2Int(6,6), new Vector2Int(8,8));
                break;


        }
        gridManager.BlockNode(newBlockedNode);
        previousBlockedNodes.Add(newBlockedNode);
    }

//     private void UnblockPreviousNodes()
// {
//     foreach (Vector2Int node in previousBlockedNodes)
//     {
//         gridManager.UnblockNode(node);
//     }
//     previousBlockedNodes.Clear();
// }
    


    

}



// if(Input.GetKey(KeyCode.R)){

        //     transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // }