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

    private List<FolowingUnit> downFollowers = new List<FolowingUnit>();
    private List<TopFolowingUnit> topFollowers = new List<TopFolowingUnit>();

    //GIR-ADD+-+-+-+-+-+-+-+-+

    //private FolowingUnit folowingUnit;

    //private TopFolowingUnit topFolowingUnit;


    public KeyCode keyToPress = KeyCode.Space;





    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();

        previousBlockedNodes.Add(new Vector2Int(5,5));
        gridManager.BlockNode(new Vector2Int(5,5));
        //Aquí es per a inicialitzar la llista de nodes bloquejats

        unitController = FindObjectOfType<UnitController>();
        //Açó es per a moure de una casilla x a una y en Gir
        //GIR-ADD+-+-+-+-+-+-+-+-+

        // folowingUnit = FindObjectOfType<FolowingUnit>();

        // topFolowingUnit = FindObjectOfType<TopFolowingUnit>();
    }

    
    void Update()
    {


        if(Input.GetKeyDown(keyToPress) &&!isRotating){
            
            //TODO: Revisar si es pot llevar---innecesari per ara+-+-+-+-+-+-
            SelectAllUnits();

            GatherAllFollowers();

            foreach (var unit in downFollowers)
            {
                unit.UpdateFollowerPosition();

                // var downFollower = unit.GetComponent<FolowingUnit>();
                // if (downFollower != null)
                //     downFollower.UpdateFollowerPosition();
            }
            //folowingUnit.UpdateFollowerPosition();
            //GIR-ADD+-+-+-+-+-+-+-+-+

            targetRotation = transform.eulerAngles.y + rotationAngle;
            //EL 'Mathf.Repeat' ES PER A PROVAR SI EN EL 'case 0' HO LLIG MILLOR
            gridManager.ResetNodes();
            //folowingUnit.VisualUnit(true);
            StartCoroutine(RotateSmoothly());

            
        
        }
    }

    private void GatherAllFollowers()
    {

        topFollowers.Clear();
        downFollowers.Clear();

        var allUnits = GameObject.FindGameObjectsWithTag("Unit");
        foreach (var unit in allUnits)
        {

            var downUnit = unit.GetComponent<FolowingUnit>();
            if (downUnit != null) downFollowers.Add(downUnit);

            var topUnit = unit.GetComponent<TopFolowingUnit>();
            if (topUnit != null) topFollowers.Add(topUnit);
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

        yield return null; // Esperamos un frame para asegurar que la rotación ha terminado completamente

        //PARA MOVER EL TOP-UNIT DONDE DIGA DOWN-UNIT

        foreach (var unit in topFollowers)
        {
            yield return StartCoroutine(unit.MoveToRaycastHit());

            // var topFollower = unit.GetComponent<TopFolowingUnit>();
            // if (topFollower != null)
            //     yield return StartCoroutine(topFollower.MoveToRaycastHit());
        }

        // if (topFolowingUnit != null)
        // {
        //     yield return StartCoroutine(topFolowingUnit.MoveToRaycastHit());
        // }

        yield return null; // Esperamos otro frame por seguridad


        //PARA PONER EL SOURCE DEL CONSTRAINT OTRA VEZ EN EL TOP-UNIT

        foreach (var unit in downFollowers)
        {
            unit.FollowerToParent();

            // if (folowingUnit != null)
            // {
            //     folowingUnit.FollowerToParent();
            // }
        }


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

                casillasRedir.Add(new Vector2Int(3,0), new Vector2Int(0,6));
                casillasRedir.Add(new Vector2Int(7,7), new Vector2Int(7,2));
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

        if (previousBlockedNodes.Count > 0)
        {
            Vector2Int lastBlocked = previousBlockedNodes[previousBlockedNodes.Count - 1];
            gridManager.UnblockNode(lastBlocked);
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
