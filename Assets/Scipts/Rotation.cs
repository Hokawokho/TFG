using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.MemoryProfiler;
using UnityEngine;

public class Rotation : MonoBehaviour
{

    public float rotationAngle = 90f; // Ángulo de rotación en grados
    public float rotationSpeed = 200f; // Velocidad de rotación en grados por segundo
    public bool isRotating = false;
    private float targetRotation;


    private GridManager gridManager;

    private UnitController unitController;

    private List<FolowingUnit> downFollowers = new List<FolowingUnit>();
    private List<TopFolowingUnit> topFollowers = new List<TopFolowingUnit>();

    public KeyCode keyToPress = KeyCode.Space;

    private int currentCase = 1;
    //Açò per al bloqueig de cada tile segons l'angle de rotació

    public ConnectionBlocker connectionBlocker;

    private LayerRenderChanger layerRenderChanger;

    private ChangingShaderTopTiles changingShaderTopTiles;
    //Millor ficar-ho des de l'editor així podem tindre replicats del mateix script sense problema






    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();

        unitController = FindObjectOfType<UnitController>();

        layerRenderChanger = FindObjectOfType<LayerRenderChanger>();

        //Si fot el bloqueig a l'inici, llevar esta línea
        ApplyBlockingToAllTiles(currentCase);
        
        if (connectionBlocker != null)
        {
            connectionBlocker.ApplyBlocksForCase(currentCase);
        }

    }

    
    void Update()
    {


        if(Input.GetKeyDown(keyToPress) &&!isRotating){

            ChangingShaderTopTiles.ClearAllHighlights();

            GatherAllFollowers();

            unitController.DeselectCurrentUnit();

            foreach (var unit in downFollowers)
            {

                //esconder la UI de las unidades.
                // CanvasGroup previousCanvas = unit.GetComponentInChildren<CanvasGroup>();
                // if (previousCanvas != null)
                //     previousCanvas.alpha = 0f;





                unit.UpdateFollowerPosition();
            }


            targetRotation = transform.eulerAngles.y + rotationAngle;
            //EL 'Mathf.Repeat' ES PER A PROVAR SI EN EL 'case 0' HO LLIG MILLOR
            gridManager.ResetNodes();
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


    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 150, 30), transform.eulerAngles.ToString());
        GUI.Label(new Rect(10, 40, 150, 30), isRotating.ToString());

    }

    private IEnumerator RotateSmoothly()
    {
        isRotating = true;
        if (layerRenderChanger != null)
            layerRenderChanger.SuspendCollisions();



        while (Mathf.Abs(Mathf.Repeat(transform.eulerAngles.y - targetRotation, 360)) > 0.1f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, targetRotation, 0), rotationSpeed * Time.deltaTime);
            yield return null;
        }
        transform.rotation = Quaternion.Euler(0, targetRotation, 0);
        // Asegurar que la rotación finaliza exactamente en el ángulo deseado
        isRotating = false;
        // if (layerRenderChanger != null)
        //     //layerRenderChanger.ResumeCollisions();
            



        // BlockNodeBasedOnRotation();
        OnRotationFinished();


        yield return null; // Esperamos un frame para asegurar que la rotación ha terminado completamente

        //PARA MOVER EL TOP-UNIT DONDE DIGA DOWN-UNIT

        foreach (var unit in topFollowers)
        {
            yield return StartCoroutine(unit.MoveToRaycastHit());
        }


        yield return null; // Esperamos otro frame por seguridad


        //PARA PONER EL SOURCE DEL CONSTRAINT OTRA VEZ EN EL TOP-UNIT

        foreach (var unit in downFollowers)
        {
            unit.FollowerToParent();

        }
    }


    private void OnRotationFinished()
    {
        int rotationState = Mathf.RoundToInt(targetRotation / 90f) % 4;
        currentCase = rotationState + 1;

        // APLICA bloqueo/desbloqueo a **todas** las Tiles con el nuevo case
        ApplyBlockingToAllTiles(currentCase);
        
        if (connectionBlocker != null)
            connectionBlocker.ApplyBlocksForCase(currentCase);
    }

    private void ApplyBlockingToAllTiles(int caseNumber)
    {
        Tile[] allTiles = FindObjectsOfType<Tile>();

        foreach (Tile t in allTiles)
        {
            t.ApplyBlockForCase(caseNumber);
        }
    }

}
