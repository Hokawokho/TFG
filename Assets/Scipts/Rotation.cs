using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
// using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.Animations;


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

    //private LayerRenderChanger layerRenderChanger;
    private List<LayerRenderChanger> layerRenderChangers = new List<LayerRenderChanger>();

    private ChangingShaderTopTiles changingShaderTopTiles;
    //Millor ficar-ho des de l'editor així podem tindre replicats del mateix script sense problema

    private int updatesDone;
    private int updatesExpected;
    public static event System.Action<int> OnCaseChanged;

    private TurnManager turnManager;

    public static event Action<bool> OnRotationStateChanged;



    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();

        unitController = FindObjectOfType<UnitController>();

        turnManager = FindObjectOfType<TurnManager>();


        layerRenderChangers.Clear();
        foreach (var data in unitController.unitMovementList)
        {
            var unitObj = data.unitData;
            if (unitObj != null && unitObj.transform.parent != null)
            {
                var changer = unitObj.transform.parent.GetComponent<LayerRenderChanger>();
                if (changer != null)
                    layerRenderChangers.Add(changer);
            }
        }
        updatesDone = 0;
        updatesExpected = layerRenderChangers.Count;


        //Si fot el bloqueig a l'inici, llevar esta línea
        ApplyBlockingToAllTiles(currentCase);

        if (connectionBlocker != null)
        {
            connectionBlocker.ApplyBlocksForCase(currentCase);
        }

    }
    void OnEnable()
    {
        FolowingUnit.OnFollowerPositionUpdated += HandleFollowerUpdated;
    }

    void OnDisable()
    {
        FolowingUnit.OnFollowerPositionUpdated -= HandleFollowerUpdated;
    }



    void Update()
    {


        if (Input.GetKeyDown(keyToPress) && !isRotating)
        {
            if (turnManager.State != TurnManager.GameState.PLAYERTURN)
                return;
            if (turnManager.remainingRotations <= 0)
            {
                Debug.Log("No quedan rotaciones este turno");
                return;
            }
            // Consumimos una rotación
            turnManager.remainingRotations--;


            // **3. ASIGNAR A TODOS LOS TopFolowingUnit**
            GatherAllFollowers(); // asegúrate de volver a poblar topFollowers
            ChangingShaderTopTiles.ClearAllHighlights();
            unitController.DeselectCurrentUnit();
            // updatesDone = 0;
            // updatesExpected = layerRenderChangers.Count;



            // Antes de empezar la rotación limpia la lista:
            layerRenderChangers = layerRenderChangers
                .Where(c => c != null && c.gameObject.activeInHierarchy)
                .ToList();

            foreach (var changer in layerRenderChangers)
            {
                //Debug.Log($"[Rotation] Processing LayerRenderChanger on '{changer.gameObject.name}'");

                //DE ACÍ 

                changer.SetUpScripts_and_RaycastTop();


                //FINS ACÍ+-+-+--+-+-


                foreach (var unit in downFollowers)
                {

                }
            }
            // if (cont >= layerRenderChangers.Count)
            // {
            //     targetRotation = transform.eulerAngles.y + rotationAngle;
            //     gridManager.ResetNodes();
            //     StartCoroutine(RotateSmoothly());
            // }
        }
    }
    private void HandleFollowerUpdated(FolowingUnit fol)
    {
        updatesDone++;
        if (updatesDone >= updatesExpected)
        {
            // Reiniciamos el contador para la próxima vez
            updatesDone = 0;

            // Aquí ponemos el mismo código que antes hacías en el `if (cont >= ...)`
            targetRotation = transform.eulerAngles.y + rotationAngle;
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


    // private void OnGUI()
    // {
    //     GUI.Label(new Rect(10, 10, 150, 30), transform.eulerAngles.ToString());
    //     GUI.Label(new Rect(10, 40, 150, 30), isRotating.ToString());

    // }

    private IEnumerator RotateSmoothly()
    {
        isRotating = true;
        OnRotationStateChanged?.Invoke(true);
        // if (layerRenderChanger != null)
        //     layerRenderChanger.SuspendCollisions();
        // Suspender colisiones en todos los LayerRenderChanger
        foreach (var changer in layerRenderChangers)
            changer.SuspendCollisions();




        while (Mathf.Abs(Mathf.Repeat(transform.eulerAngles.y - targetRotation, 360)) > 0.1f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, targetRotation, 0), rotationSpeed * Time.deltaTime);
            yield return null;
        }
        transform.rotation = Quaternion.Euler(0, targetRotation, 0);


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
        isRotating = false;
        OnRotationStateChanged?.Invoke(false);
    }


    private void OnRotationFinished()
    {
        int rotationState = Mathf.RoundToInt(targetRotation / 90f) % 4;
        currentCase = rotationState + 1;

        OnCaseChanged?.Invoke(currentCase);


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

    public int CurrentCase => currentCase;
}
