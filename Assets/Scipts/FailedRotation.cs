using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.Animations;


public class FailedRotation : MonoBehaviour
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

    private RaycastDebugger previousDebugger;

    private FolowingUnit previousFollower;





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

        //Si fot el bloqueig a l'inici, llevar esta línea
        ApplyBlockingToAllTiles(currentCase);
        
        if (connectionBlocker != null)
        {
            connectionBlocker.ApplyBlocksForCase(currentCase);
        }

    }


    void Update()
    {


        if (Input.GetKeyDown(keyToPress) && !isRotating)
        {
            // RaycastDebugger selectedDebugger;
            // FolowingUnit selectedFollower;
            // RelocatingUnitConstraints(out selectedDebugger, out selectedFollower);


            // **3. ASIGNAR A TODOS LOS TopFolowingUnit**
            GatherAllFollowers(); // asegúrate de volver a poblar topFollowers

            ChangingShaderTopTiles.ClearAllHighlights();

            //GatherAllFollowers();

            unitController.DeselectCurrentUnit();

            // 2) para cada unidad, sacamos su debugger/follower y se los pasamos
            foreach (var changer in layerRenderChangers)
            {
                var mesh = changer.GetCurrentActiveRenderer();
                if (mesh == null) continue;
                var dbg = mesh.GetComponentInParent<RaycastDebugger>();
                var fol = mesh.GetComponentInParent<FolowingUnit>();
                RelocatingUnitConstraints(dbg, fol);

                // asignamos el debugger sólo a las TopFolowingUnit de esta raíz
                foreach (var top in topFollowers)
                    if (top.transform.root == fol.transform.root)
                        top.raycastDebugger = dbg;
            }

            // 3) aplicamos todas las constraints y luego rotamos
            StartCoroutine(ApplyConstraintThenRotate(downFollowers));


            // targetRotation = transform.eulerAngles.y + rotationAngle;
            // //EL 'Mathf.Repeat' ES PER A PROVAR SI EN EL 'case 0' HO LLIG MILLOR
            // gridManager.ResetNodes();
            // StartCoroutine(RotateSmoothly());



        }
    }

    // private void RelocatingUnitConstraints(out RaycastDebugger selectedDebugger, out FolowingUnit selectedFollower)
    private void RelocatingUnitConstraints(RaycastDebugger selectedDebugger, FolowingUnit selectedFollower)
    {
        // selectedDebugger = null;
        // selectedFollower = null;



        // if (layerRenderChanger != null)
        // {
        //     MeshRenderer activeMesh = layerRenderChanger.GetCurrentActiveRenderer();
        //     if (activeMesh != null)
        //         selectedDebugger = activeMesh.GetComponentInParent<RaycastDebugger>();
        //     selectedFollower = activeMesh.GetComponentInParent<FolowingUnit>();
        // }
        // Obtener debugger y follower desde la primera coincidencia de layerRenderChangers
        // foreach (var changer in layerRenderChangers)
        // {



        // var activeMesh = changer.GetCurrentActiveRenderer();
        // if (activeMesh != null)
        // {
        //     selectedDebugger = activeMesh.GetComponentInParent<RaycastDebugger>();
        //     selectedFollower = activeMesh.GetComponentInParent<FolowingUnit>();
        //    break;
        // }
        //}


        // 2. DESACTIVAR EL VIEJO Y ACTIVAR EL NUEVO
        // ——— Debugger ———
        // if (previousDebugger != null)
        //     previousDebugger.enabled = false;
        // if (selectedDebugger != null)
        //     selectedDebugger.enabled = true;
        // previousDebugger = selectedDebugger;
        // desactivar todos los debuggers de esta unidad y activar sólo el suyo
        
        if (selectedFollower != null)
        {
            var root = selectedFollower.transform.root;
            foreach (var d in root.GetComponentsInChildren<RaycastDebugger>())
                d.enabled = false;
            if (selectedDebugger != null)
                selectedDebugger.enabled = true;
        }

        // ——— Follower ———
        // if (previousFollower != null)
        //     previousFollower.enabled = false;
        // if (selectedFollower != null)
        //     selectedFollower.enabled = true;
        // previousFollower = selectedFollower;
        
        // desactivar todos los followers de esta unidad y activar sólo el suyo
        if (selectedFollower != null)
        {
            var root = selectedFollower.transform.root;
            foreach (var f in root.GetComponentsInChildren<FolowingUnit>())
                f.enabled = false;
            selectedFollower.enabled = true;
        }
        //StartCoroutine(ApplyFollowerConstraintNextPhysics(selectedFollower));

        if (selectedFollower != null)
        {
            // Obtener todas las unidades de seguimiento en escena
            FolowingUnit[] allFollowers = FindObjectsOfType<FolowingUnit>();
            foreach (var follower in allFollowers)
            {
                // sólo los bottom‐units de la misma unidad seleccionada
                if (follower == selectedFollower || follower.transform.root != selectedFollower.transform.root)
                    continue;

                // Buscar componente PositionConstraint en el padre
                PositionConstraint constraint = follower.GetComponentInParent<PositionConstraint>();
                if (constraint != null)
                {

                    // Guardar posición mundial antes del cambio de fuente
                    Vector3 worldPos = follower.transform.position;

                    // Limpiar fuentes anteriores
                    for (int i = constraint.sourceCount - 1; i >= 0; i--)
                    {
                        constraint.RemoveSource(i);
                    }
                    // Añadir nueva fuente apuntando al follower activo
                    ConstraintSource newSource = new ConstraintSource
                    {
                        sourceTransform = selectedFollower.transform,
                        weight = 1f
                    };
                    constraint.AddSource(newSource);
                    // Ajustar el offset para mantener la posición mundial
                    constraint.translationOffset = worldPos - selectedFollower.transform.position;
                    constraint.constraintActive = true;

                }
            }
        }
    }

    IEnumerator ApplyConstraintThenRotate(List<FolowingUnit> downFollowers)
    {
        // Esperar al siguiente paso de física para asegurar colisión
        yield return new WaitForFixedUpdate();

         // Esperamos fixed update y actualizamos cada follower
        foreach (var follower in downFollowers)
        {
            yield return new WaitForFixedUpdate();
            follower.UpdateFollowerPosition();
        }

        // Arrancar rotación
        targetRotation = transform.eulerAngles.y + rotationAngle;
        gridManager.ResetNodes();
        yield return StartCoroutine(RotateSmoothly());
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
