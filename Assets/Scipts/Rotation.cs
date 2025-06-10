using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.MemoryProfiler;
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



    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();

        unitController = FindObjectOfType<UnitController>();

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
            // **3. ASIGNAR A TODOS LOS TopFolowingUnit**
            GatherAllFollowers(); // asegúrate de volver a poblar topFollowers
            ChangingShaderTopTiles.ClearAllHighlights();
            unitController.DeselectCurrentUnit();
            // updatesDone = 0;
            // updatesExpected = layerRenderChangers.Count;

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

    // private void RelocatingUnitConstraints( LayerRenderChanger changer, RaycastDebugger selectedDebugger,  FolowingUnit selectedFollower)
    // {
    //     Debug.Log($"[Rotation] RelocatingUnitConstraints started with debugger '{selectedDebugger?.gameObject.name ?? "None"}' and follower '{selectedFollower?.gameObject.name ?? "None"}'");

    //     // 2. DESACTIVAR EL VIEJO Y ACTIVAR EL NUEVO
    //     // ——— Debugger ———
    //     if (previousDebuggers.TryGetValue(changer, out var prevDbg) && prevDbg != null)
    //         prevDbg.enabled = false;
    //     if (selectedDebugger != null)
    //         selectedDebugger.enabled = true;
    //     previousDebuggers[changer] = selectedDebugger;

    //     if (previousFollowers.TryGetValue(changer, out var prevFol) && prevFol != null)
    //         prevFol.enabled = false;
    //     if (selectedFollower != null)
    //         selectedFollower.enabled = true;
    //     previousFollowers[changer] = selectedFollower;

    //     if (selectedFollower != null)
    //     {
    //         FolowingUnit[] allFollowers = FindObjectsOfType<FolowingUnit>();
    //         foreach (var follower in allFollowers)
    //         {
    //             if (follower == selectedFollower || follower.transform.root != selectedFollower.transform.root)
    //                 continue;

    //             PositionConstraint constraint = follower.GetComponentInParent<PositionConstraint>();
    //             if (constraint != null)
    //             {
    //                 //Debug.Log($"[Rotation] Updating PositionConstraint for follower '{follower.gameObject.name}'");

    //                 Vector3 worldPos = follower.transform.position;
    //                 //int oldCount = constraint.sourceCount;
    //                 for (int i = constraint.sourceCount - 1; i >= 0; i--)
    //                     constraint.RemoveSource(i);
    //                /// Debug.Log($"[Rotation] Cleared {oldCount} sources for follower '{follower.gameObject.name}'");

    //                 ConstraintSource newSource = new ConstraintSource
    //                 {
    //                     sourceTransform = selectedFollower.transform,
    //                     weight = 1f
    //                 };
    //                 constraint.AddSource(newSource);
    //                 constraint.translationOffset = worldPos - selectedFollower.transform.position;
    //                 constraint.constraintActive = true;
    //                //Debug.Log($"[Rotation] Added new source '{selectedFollower.gameObject.name}' to constraint on '{follower.gameObject.name}' with offset {constraint.translationOffset}");
    //             }
    //             else
    //             {
    //                 Debug.LogWarning($"[Rotation] No PositionConstraint found on '{follower.gameObject.name}'");
    //             }
    //         }
    //     }
    //     else
    //     {
    //         Debug.LogWarning("[Rotation] No selectedFollower provided to update constraints.");
    //     }
    // }

    IEnumerator ApplyConstraintThenRotate(List<FolowingUnit> downFollowers)
    {
        //Debug.Log("[Rotation] Waiting for next FixedUpdate to apply constraints.");
        // Esperar al siguiente paso de física para asegurar colisión
        yield return new WaitForFixedUpdate();

        // Esperamos fixed update y actualizamos cada follower
        foreach (var follower in downFollowers)
        {
            yield return new WaitForFixedUpdate();
            follower.UpdateFollowerPosition();
            //Debug.Log($"[Rotation] Updated follower position for '{follower.gameObject.name}'");

        }

        // Señalamos que ya se aplicaron las constraints
        // Arrancar rotación
        // targetRotation = transform.eulerAngles.y + rotationAngle;
        // gridManager.ResetNodes();
        // yield return StartCoroutine(RotateSmoothly());
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
        
        
        // Asegurar que la rotación finaliza exactamente en el ángulo deseado
        //isRotating = false;




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
