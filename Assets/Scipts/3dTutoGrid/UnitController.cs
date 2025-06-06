using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//using System.Numerics;
using TMPro;
using UnityEngine;

public class UnitController : MonoBehaviour
{

    public Transform selectedUnit;
    public bool unitSelected = false;

    private Transform lastSelectedUnit = null;

    [SerializeField] float movementSpeed = 1f;
    GridManager gridManager;



    List<Node> path = new List<Node>();
    public Pathfinding pathFinder;

    [SerializeField] public List<UnitMovementData> unitMovementList = new List<UnitMovementData>();

    //Teclas de ataque    
    public KeyCode keyToCloseAttack = KeyCode.A;
    public KeyCode keyToRangeAttack = KeyCode.D;
    public KeyCode keyToConfirmAttack = KeyCode.E;

    private ObjectShooter shooter;


    public enum AttackMode { None, Melee, Range }
    private AttackMode currentAttackMode = AttackMode.None;

    private TurnManager turnManager;

    public bool isMoving = false;

    // public KeyCode keyToResetMovement;

    // Start is called before the first frame update
    void Awake()
    {

        gridManager = FindObjectOfType<GridManager>();

        pathFinder = FindObjectOfType<Pathfinding>();

        turnManager = FindObjectOfType<TurnManager>();

        // foreach (var data in unitMovementList)
        // {

        //     Debug.Log("Movimiento de Unidades reseteado");
        //     data.ResetMovement();
        // }


    }





    //+++-++-+-+-+COSTE-ADD+++++++++++++INICIO
    public UnitMovementData GetUnitData(GameObject unit)
    {

        //Debug.Log($"La unidad es: {unit}");

        return unitMovementList.Find(data => data.unitData == unit);

        //
    }
    //+++-++-+-+-+COSTE-ADD+++++++++++++FIN



    // Update is called once per frame
    void Update()
    {
        HandleMouseKeys();
        HandleHotKeys();


        //RESETEAR MOVEMENT -> F
        // if (Input.GetKeyDown(keyToResetMovement))
        // {
        //     foreach (var data in unitMovementList)
        //     {
        //         data.ResetMovement();
        //     }
        // }
    }

    private void HandleMouseKeys()
    {
        if (!Input.GetMouseButtonDown(0)) return;


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //Marca amb un ray on estiga el nostre ratolí
        // RaycastHit hit;
        // bool hasHit = Physics.Raycast(ray, out hit);
        //Açò guarda el que s'ha seleccionat amb un ray en HIT
        // if (hasHit)

        //D'esta forma es més ràpid que dalt
        if (Physics.Raycast(ray, out var hit))


        {
            if (hit.transform.tag == "Tile")
            {

                Vector2Int tileCords = hit.transform.GetComponent<Tile>().cords;
                Debug.Log($"Casilla seleccionada: {tileCords.x}, {tileCords.y}");

                if (gridManager.GetNode(tileCords) != null && !gridManager.GetNode(tileCords).walkable)
                {
                    Debug.Log("No se puede mover en esta casilla");
                    return;
                }

                MoveUnitTo(tileCords);

            }


            if (hit.transform.tag == "Unit")
            {

                //TODO: Quan acabe amb el debugging aplicar açò també en el turno del jugador pals enemics (Després ja implementar selecció estil FE)    

                //Si turno del enemigo no se pueden seleccionar los Player+++
                if (turnManager.State == TurnManager.GameState.ENEMYTURN)
                {
                    if (hit.transform.GetComponent<Player>() != null)
                        return;
                }

                if (unitSelected && hit.transform == selectedUnit)
                {
                    DeselectCurrentUnit();
                    return;
                }




                SelectUnit(hit.transform);
            }
        }

    }


    // private void HandleHotKeys()
    // {
    //     if (!unitSelected) return;

    //     Vector2Int unitCoords = gridManager.GetCoordinatesFromPosition(selectedUnit.position);
    //     //ChangingShaderTopTiles.ClearAllHighlights();

    //     // Resalta las 4 adyacentes
    //     if (Input.GetKeyDown(keyToCloseAttack))
    //     {
    //         ChangingShaderTopTiles.ClearAllHighlights();
    //         ChangingShaderTopTiles.HighlightTilesAround(unitCoords, gridManager);
    //     }

    //     if (Input.GetKeyDown(keyToRangeAttack))
    //     {
    //         ChangingShaderTopTiles.ClearAllHighlights();
    //         ChangingShaderTopTiles.HighlightLineTiles(unitCoords, gridManager);
    //     }





    // }

    private void HandleHotKeys()
    {
        if (!unitSelected) return;

        //Vector2Int unitCoords = gridManager.GetCoordinatesFromPosition(selectedUnit.position);
        //ChangingShaderTopTiles.ClearAllHighlights();

        // Resalta las 4 adyacentes
        if (Input.GetKeyDown(keyToCloseAttack))
        {
            if (currentAttackMode == AttackMode.Melee)
                ExitAttackMode();
            else
                EnterAttackMode(AttackMode.Melee);
        }

        if (Input.GetKeyDown(keyToRangeAttack))
        {
            if (currentAttackMode == AttackMode.Range)
                ExitAttackMode();
            else
                EnterAttackMode(AttackMode.Range);
        }

        if (currentAttackMode == AttackMode.Range && Input.GetKeyDown(keyToConfirmAttack))
        {

            ConfirmAttack();
        }


    }


    private void EnterAttackMode(AttackMode mode)
    {
        //ExitAttackMode();
        ChangingShaderTopTiles.ClearAllHighlights();
        currentAttackMode = mode;

        Vector2Int unitCoords = gridManager.GetCoordinatesFromPosition(selectedUnit.position);

        if (mode == AttackMode.Melee)
            ChangingShaderTopTiles.HighlightTilesAround(unitCoords, gridManager);

        else if (mode == AttackMode.Range)
            ChangingShaderTopTiles.HighlightLineTiles(unitCoords, gridManager);

        shooter = selectedUnit.GetComponentInChildren<ObjectShooter>();
    }


    private void ConfirmAttack()
    {

        if (shooter == null)
        {

            Debug.LogWarning("La unidad no tiene Weapon");
            return;
        }
        bool fired = shooter.TryShoot();
        if (!fired)
            Debug.Log("No puede disparar (cooldown o sin acciones)");
        else
            ExitAttackMode();

    }


    //Per a tornar a mostrar el movimient i no el rango de ataque+++
    private void ExitAttackMode()
    {

        currentAttackMode = AttackMode.None;
        ChangingShaderTopTiles.ClearAllHighlights();

        Vector2Int unitCoords = gridManager.GetCoordinatesFromPosition(selectedUnit.position);
        var unitData = GetUnitData(selectedUnit.gameObject);
        if (unitData != null)
        {
            // Resaltar todas las tiles al alcance de la unidad
            ChangingShaderTopTiles.HighlightCostTiles(unitCoords, gridManager, unitData.remainingTiles);
        }
    }


    public void MoveUnitTo(Vector2Int tileCords)
    {

        //Esto es para quitar los tiles de ataque
        ChangingShaderTopTiles.ClearAllHighlights();

        if (unitSelected)
        {

            //Vector2Int targetCords = hit.transform.GetComponent<Labeler>().cords;
            Vector2Int targetCords = tileCords;
            Vector2Int startCords = gridManager.GetCoordinatesFromPosition(selectedUnit.position);

            // int distance = CalculatePathCost(startCords, targetCords);

            //COSTE - ADD+++++++++++++ INICIO

            // pathFinder.SetNewDestination(startCords, targetCords);
            // List<Node> pathPosible = pathFinder.GetNewPath();

            //UnitMovementData unitData = GetUnitData(selectedUnit.gameObject);
            // if (unitData != null && distance <= unitData.maxTiles) 
            // {

            //     RecalculatePath(true, true);
            // }
            //COSTE - ADD+++++++++++++ FIN


            int distance = CalculatePathCost(startCords, targetCords);
            var unitData = GetUnitData(selectedUnit.gameObject);

            //pathFinder.SetNewDestination(startCords, targetCords);
            //List<Node> pathPosible = pathFinder.GetNewPath();
            //CalculatePathCost(startCords, targetCords);
            // if(pathPosible.Count > 0){

            //     RecalculatePath(true,true);
            // } 
            // else{

            //     Debug.LogWarning("Camino no encontrad por coste");
            // }
            if (distance <= unitData.remainingTiles)
            {
                unitData.remainingTiles -= distance;
                RecalculatePath(true, true);
            }
            else
            {

                Debug.LogWarning("Esta demasiado lejos");
                CanvasGroup canvas = selectedUnit.GetComponentInChildren<CanvasGroup>();
                if (canvas != null)
                    canvas.alpha = 0f;
            }

            //selectedUnit.transform.position = new Vector3(targetCords.x, selectedUnit.position.y, targetCords.y);
        }
    }

    private void SelectUnit(Transform unit)
    {

        // Si había una unidad seleccionada antes, ocultar su interfaz
        if (lastSelectedUnit != null)
        {
            CanvasGroup previousCanvas = selectedUnit.GetComponentInChildren<CanvasGroup>();
            if (previousCanvas != null)
                previousCanvas.alpha = 0f;
        }


        selectedUnit = unit;
        unitSelected = true;

        CanvasGroup canvas = selectedUnit.GetComponentInChildren<CanvasGroup>();
        if (canvas != null)
            canvas.alpha = 1f;
        lastSelectedUnit = selectedUnit; // actualizar
    

        // Obtener coords y datos de movimiento
        Vector2Int unitCoords = gridManager.GetCoordinatesFromPosition(unit.position);
        var unitData = GetUnitData(unit.gameObject);
        if (unitData != null)
        {   
            //Volver a tener en cuenta colisiones para el mesh del layer corresponent
            var layerChanger = unit.GetComponentInParent<LayerRenderChanger>();
            if (layerChanger != null)
            {
                layerChanger.ResumeCollisions();
            }
            // Resaltar todas las tiles al alcance de la unidad
            ChangingShaderTopTiles.HighlightCostTiles(unitCoords, gridManager, unitData.remainingTiles);
        }


    }

    public void DeselectCurrentUnit()
    {
        // Si había una unidad seleccionada, ocultamos su UI
        if (selectedUnit != null)
        {
            var canvas = selectedUnit.GetComponentInChildren<CanvasGroup>();
            if (canvas != null)
                canvas.alpha = 0f;
        }

        selectedUnit = null;
        unitSelected = false;
        lastSelectedUnit = null;

        // Limpiar tiles 
        ChangingShaderTopTiles.ClearAllHighlights();
    }


    public int CalculatePathCost(Vector2Int start, Vector2Int target)
    {


        pathFinder.SetNewDestination(start, target);
        List<Node> pathCost = pathFinder.GetNewPath();
        int cost = pathCost.Count - 1;
        Debug.Log($"Costo del camino: {cost}, Nodos en el camino: {pathCost.Count}");

        string pathCoords = string.Join(" -> ",
                pathCost.Select(n => $"{n.cords.x},{n.cords.y}")
            );
        Debug.Log($"Casillas en el camino: {pathCoords}");
        

        //UnitMovementData unitData = GetUnitData(selectedUnit.gameObject);
        //GetUnitData(selectedUnit.gameObject);

        return cost;

    }

    public void RecalculatePath(bool resetPath, bool followPath)
    {



        //FORMA DE OPTIMIZAR-HO
        Vector2Int coordinates = resetPath ? pathFinder.StartCords : gridManager.GetCoordinatesFromPosition(transform.position);

        //Vector2Int coordinates = new Vector2Int();
        // if(resetPath){
        //     coordinates = pathFinder.StartCords;
        // }
        // else{
        //     coordinates =gridManager.GetCoordinatesFromPosition(transform.position);
        // }




        //19/3------------Desbloquear tile de la posició anterior,
        //Al rotar per alguna raó no funciona, per això esta acó.
        // Vector2Int previousCoords = gridManager.GetCoordinatesFromPosition(transform.position);
        // gridManager.UnblockNode(previousCoords);

        //19/3------------------FIN
        //---+-+-+ PER ARA NO FUNCIONA, NO HI HA CAP CAMBI


        StopAllCoroutines();
        path.Clear();
        path = pathFinder.GetNewPath(coordinates);

        //Debug.Log($"Nodos en el camino: {path.Count}");


        if (followPath && path.Count > 0)
        {
            StartCoroutine(FollowPath());
        }

        else
        {

            Debug.LogWarning("No hay camino posible");
        }
    }


    IEnumerator FollowPath()
    {
        isMoving = true;

        //IEnumerator es para CORRUTINAS+++++++++++++++++++
        //mes info en notes rapides

        for (int i = 1; i < path.Count; i++)
        // int i = 0 seria la unidad en su posición actual
        {


            Vector3 startPosition = selectedUnit.position;

            Vector3 endPosition = gridManager.GetPositionFromCoordinates(path[i].cords);
            float travelPercent = 0f;
            //porcentaje de progreso de movimiento

            //Esto es para mantener la altura de la unidad cuando se mueva
            endPosition.y = selectedUnit.position.y + travelPercent;

            selectedUnit.LookAt(endPosition);
            //Esta linea es per si foren figures complexes, que miren a la endPosition (es a dir que es giren)

            while (travelPercent < 1f)
            {
                //mientras no se aclance el destino

                travelPercent += Time.deltaTime * movementSpeed;
                selectedUnit.position = Vector3.Lerp(startPosition, endPosition, travelPercent);
                //Lerp == Mueve suavemente la unidad de start a end++++++++++++++++++++
                yield return new WaitForEndOfFrame();
                //Açò es per a acabar la Corrutina+++++++++++
            }
        }
        //Aço es per a desseleccionar la unitat++++++++++++++


        if (selectedUnit != null)
        {
            CanvasGroup canvas = selectedUnit.GetComponentInChildren<CanvasGroup>();
            if (canvas != null)
                canvas.alpha = 1f;

            Vector2Int unitCoords = gridManager.GetCoordinatesFromPosition(selectedUnit.position);
            var unitData = GetUnitData(selectedUnit.gameObject);
            ChangingShaderTopTiles.HighlightCostTiles(unitCoords, gridManager, unitData.remainingTiles);
        }

        //Si interesa deseleccionar la unitat descomentar açò.
        // unitSelected = false;
        // lastSelectedUnit= null;
        // selectedUnit = null;
        isMoving = false;

    }
    
}
