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

    private HealthBar healthBar;

    private InfoUnit infoUnit;

    public bool isMoving = false;

    private Animator[] selectedAnimators;

    private AudioManager audioManager;

    private bool tileBlockedOnSelect;

    private Rotation rotation;

    // public KeyCode keyToResetMovement;

    // Start is called before the first frame update
    void Awake()
    {

        gridManager = FindObjectOfType<GridManager>();

        pathFinder = FindObjectOfType<Pathfinding>();

        turnManager = FindObjectOfType<TurnManager>();

        healthBar = FindObjectOfType<HealthBar>();

        infoUnit = FindObjectOfType<InfoUnit>();

        audioManager = FindObjectOfType<AudioManager>();

        rotation = FindObjectOfType<Rotation>();

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

        if (unitSelected)
        {
            var selEntity = selectedUnit.GetComponent<UnitEntity>();
            if (selEntity != null && !selEntity.IsAlive)
            {
                DeselectCurrentUnit();
                return;
            }
        }

        if (PauseMenu.GameIsPaused || isMoving)
            return;

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


        if (rotation.isRotating) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //Marca amb un ray on estiga el nostre ratolí
        // RaycastHit hit;
        // bool hasHit = Physics.Raycast(ray, out hit);
        //Açò guarda el que s'ha seleccionat amb un ray en HIT
        // if (hasHit)

        //D'esta forma es més ràpid que dalt
        // if (Physics.Raycast(ray, out var hit))
        // {
             // ─── MODO ATAQUE ─────────────────────────────────────────────
            if (currentAttackMode != AttackMode.None)
        {
            // Raycast ignorando layer 'Player'
            int playerLayer = LayerMask.NameToLayer("Player");
            int mask = ~(1 << playerLayer);
            if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, mask))
            {
                // Nada seleccionado: deseleccionar unidad
                ExitAttackMode();
                DeselectCurrentUnit();
                return;
            }

            // Solo clicks en casillas (Tile)
            if (hit.transform.CompareTag("Tile"))
            {
                //Del Tile, filtramos para pillar solo el hijo que tiene el script de Shader Top Tiles
                var shaders = hit.transform.GetComponentsInChildren<ChangingShaderTopTiles>();
                MeshRenderer highlight = shaders
                .Select(s => s.GetComponent<MeshRenderer>())
                .FirstOrDefault(m => m != null);
                
                if (highlight != null && highlight.enabled)
                {
                    // Calcular dirección y disparar
                    var tileComp = hit.transform.GetComponent<Tile>();
                    var target = tileComp.cords;
                    var origin = gridManager.GetCoordinatesFromPosition(selectedUnit.position);
                    var delta = target - origin;
                    Vector3 dir = Vector3.zero;
                    if (delta.x != 0 && delta.y == 0)
                        dir = delta.x > 0 ? Vector3.right : Vector3.left;
                    else if (delta.y != 0 && delta.x == 0)
                        dir = delta.y > 0 ? Vector3.forward : Vector3.back;
                    shooter.currentDirection = dir;
                    ConfirmAttack();
                }
                else
                {
                    // Tile no está highlight: cancelar modo y deseleccionar
                    ExitAttackMode();
                    DeselectCurrentUnit();
                }
            }
            else
            {
                // No es Tile: cancelar modo y deseleccionar
                ExitAttackMode();
                DeselectCurrentUnit();
            }
            return;
        }
        //──────────── GESTIÓN DE MOVIMIENTO ─────────────────────────
           // Normal: raycast sin máscara
        if (Physics.Raycast(ray, out var rawHit))
        {
            var hit = rawHit;

            // Movimiento en Tile
            if (hit.transform.CompareTag("Tile"))
            {
                

                //COMENTAR PODER MOVER A LAS UNIDADES ENEMIGAS
                if (turnManager.State == TurnManager.GameState.PLAYERTURN
                && selectedUnit != null
                && selectedUnit.GetComponent<Player>() == null)
                    return;
                    
                if (tileBlockedOnSelect)
                    return;

                Vector2Int tileCords = hit.transform.GetComponent<Tile>().cords;
                Debug.Log($"Tile clicada en: {tileCords.x}, {tileCords.y}");

                if (gridManager.GetNode(tileCords) != null && !gridManager.GetNode(tileCords).walkable)
                    return;
                MoveUnitTo(tileCords);
                return;
            }

            // Selección de Unidades
            if (hit.transform.CompareTag("Unit"))
            {       
        
                if (turnManager.State == TurnManager.GameState.ENEMYTURN || turnManager.State == TurnManager.GameState.START)
                {
                    var clickedEntity = hit.transform.GetComponent<UnitEntity>();
                    if (clickedEntity != null && !clickedEntity.IsAlive) return;
                    if (hit.transform.GetComponent<Player>() != null) return;
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

    private void HandleHotKeys()
    {
        if (!unitSelected) return;
        if (tileBlockedOnSelect)
            return;


        CanvasGroup canvas = selectedUnit.GetComponentInChildren<CanvasGroup>();


        //Vector2Int unitCoords = gridManager.GetCoordinatesFromPosition(selectedUnit.position);
        //ChangingShaderTopTiles.ClearAllHighlights();

        // Resalta las 4 adyacentes
        if (Input.GetKeyDown(keyToCloseAttack))
        {
            if (currentAttackMode == AttackMode.Melee)
                ExitAttackMode();
            else
            {
                var selEntity = selectedUnit.GetComponent<UnitEntity>();
                if (selEntity != null && !selEntity.HasActionsRemaining)
                {
                    Debug.Log("No tienes acciones para cuerpo a cuerpo");
                    return;
                }
                EnterAttackMode(AttackMode.Melee);
                if (canvas != null)
                    canvas.alpha = 0f;
            }
        }

        if (Input.GetKeyDown(keyToRangeAttack))
        {
            if (currentAttackMode == AttackMode.Range)
                ExitAttackMode();
            else
            {
                var selEntity = selectedUnit.GetComponent<UnitEntity>();
                if (selEntity != null && !selEntity.HasActionsRemaining)
                {
                    Debug.Log("No tienes acciones para cuerpo a cuerpo");
                    return;
                }
                EnterAttackMode(AttackMode.Range);
                if (canvas != null)
                    canvas.alpha = 0f;
            }
        }

        if (currentAttackMode == AttackMode.Range && Input.GetKeyDown(keyToConfirmAttack))
        {

            ConfirmAttack();
        }

        if (currentAttackMode == AttackMode.Melee && Input.GetKeyDown(keyToConfirmAttack))
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
        if (shooter != null)
        {
            if (mode == AttackMode.Melee)
                shooter.poolTag = "meleeHitbox";
            else if (mode == AttackMode.Range)
                shooter.poolTag = "arrow-proj";
        }
    }


    private void ConfirmAttack()
    {

        if (shooter == null)
        {

            Debug.LogWarning("La unidad no tiene Weapon");
            return;
        }

        if (audioManager != null)
            PlayMeleeAudioEvent();

        if (selectedAnimators == null || selectedAnimators.Length == 0)
            {
                Debug.LogWarning($"No se encontraron animators para la unidad «{selectedUnit?.name}»");
                // Si prefieres, puedes salir aquí:
                return;
            }

        bool fired = shooter.TryShoot();
        if (!fired)
            Debug.Log("No puede disparar (cooldown o sin acciones)");
        else
        {
            bool isEnemy = selectedUnit.GetComponent<Player>() == null;
             var entity = selectedUnit.GetComponent<UnitEntity>();
            entity.UseAction();
            foreach (var anim in selectedAnimators)
            {
                if (currentAttackMode == AttackMode.Melee || isEnemy)
                {
                    anim.SetTrigger("Attack");
                    Debug.LogWarning("Ataca cuerpo a cuerpo");
                }
                else if (currentAttackMode == AttackMode.Range)
                    anim.SetTrigger("Shoot");
            }
            ExitAttackMode();
        }
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
        if (turnManager.State != TurnManager.GameState.START)
            ChangingShaderTopTiles.ClearAllHighlights();

        if (unitSelected)
        {

            //Vector2Int targetCords = hit.transform.GetComponent<Labeler>().cords;
            Vector2Int targetCords = tileCords;
            Vector2Int startCords = gridManager.GetCoordinatesFromPosition(selectedUnit.position);



            int distance = CalculatePathCost(startCords, targetCords);
            var unitData = GetUnitData(selectedUnit.gameObject);

            if (distance <= unitData.remainingTiles)
            {
                unitData.remainingTiles -= distance;
                // RecalculatePath(true, true);
                RecalculatePath(true);
            }
            else
            {

                Debug.Log("Esta demasiado lejos");
                CanvasGroup canvas = selectedUnit.GetComponentInChildren<CanvasGroup>();
                if (canvas != null)
                    canvas.alpha = 0f;
            }

            //selectedUnit.transform.position = new Vector3(targetCords.x, selectedUnit.position.y, targetCords.y);
        }
    }

    public void SelectUnit(Transform unit)
    {

        // Si había una unidad seleccionada antes, ocultar su interfaz
        if (lastSelectedUnit != null)
        {
            CanvasGroup previousCanvas = selectedUnit.GetComponentInChildren<CanvasGroup>(true);
            if (previousCanvas != null)
                previousCanvas.alpha = 0f;
        }


        selectedUnit = unit;
        unitSelected = true;
        var root = selectedUnit.parent;
        selectedAnimators = root.GetComponentsInChildren<Animator>();

        CanvasGroup canvas = selectedUnit.GetComponentInChildren<CanvasGroup>();
        if (canvas != null)
            canvas.alpha = 1f;
        lastSelectedUnit = selectedUnit; // actualizar

        //AUDIO DE SELECCIÓN DE UNIDAD
        if (unit.GetComponent<Player>() != null)
        {
            if (audioManager != null && audioManager.selectAudio != null)
                audioManager.PlayOneShot(audioManager.selectAudio);
        }

        // Esto para actualizar HealthBar-+-+-++
        var entity = selectedUnit.GetComponent<UnitEntity>();
        if (healthBar != null)
            healthBar.SetUnit(entity);



        // Obtener coords y datos de movimiento
        Vector2Int unitCoords = gridManager.GetCoordinatesFromPosition(unit.position);
        var unitData = GetUnitData(unit.gameObject);

        var trigger = unit.GetComponent<BlockingTrigger>();
        int currentCase = trigger._rotationCase;
        Tile tileComponent = FindObjectsOfType<Tile>().FirstOrDefault(t => t.cords == unitCoords);
        tileBlockedOnSelect = tileComponent.IsBlockedInCase(currentCase);

        if (unitData != null)
        {
            if (infoUnit != null)
                infoUnit.SetUnit(entity, unitData);

            //Volver a tener en cuenta colisiones para el mesh del layer corresponent
            var layerChanger = unit.GetComponentInParent<LayerRenderChanger>();
            if (layerChanger != null)
            {
                layerChanger.ResumeCollisions();
            }
            // Resaltar todas las tiles al alcance de la unidad
            if (!tileBlockedOnSelect)
                ChangingShaderTopTiles.HighlightCostTiles(unitCoords, gridManager, unitData.remainingTiles);
        }


    }

    public void DeselectCurrentUnit()
    {
        // Si había una unidad seleccionada, ocultamos su UI
        if (selectedUnit != null)
        {
            if (infoUnit != null)
                infoUnit.SetUnit(null, null);

            if (healthBar != null)
                healthBar.SetUnit(null);


            var canvas = selectedUnit.GetComponentInChildren<CanvasGroup>();
            if (canvas != null)
                canvas.alpha = 0f;
        }

        //AUDIO DE DESELECCIONAR UNIDAD
        if (selectedUnit != null && selectedUnit.GetComponent<Player>() != null)
        {
            if (audioManager != null && audioManager.unselectAudio != null)
                audioManager.PlayOneShot(audioManager.unselectAudio);
        }

        selectedUnit = null;
        unitSelected = false;
        lastSelectedUnit = null;

        // Limpiar tiles 
        ChangingShaderTopTiles.ClearAllHighlights();

        // reseteo de HealthBar
        // (la dejaremos a 0)
        if (lastSelectedUnit != null)
        {
            if (healthBar != null)
                healthBar.SetUnit(null);
        }
        // ——————————————————————————
    }


    public int CalculatePathCost(Vector2Int start, Vector2Int target)
    {


        pathFinder.SetNewDestination(start, target);
        List<Node> pathCost = pathFinder.GetNewPath();
        int cost = pathCost.Count - 1;

        //UnitMovementData unitData = GetUnitData(selectedUnit.gameObject);
        //GetUnitData(selectedUnit.gameObject);

        return cost;

    }

    //public void RecalculatePath(bool resetPath, bool followPath)
    public void RecalculatePath(bool resetPath)
    {



        //FORMA DE OPTIMIZAR-HO
        Vector2Int coordinates = resetPath ? pathFinder.StartCords : gridManager.GetCoordinatesFromPosition(transform.position);

        StopAllCoroutines();
        path.Clear();
        path = pathFinder.GetNewPath(coordinates);

        //Debug.Log($"Nodos en el camino: {path.Count}");


        if (path.Count > 1)
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
        CanvasGroup canvas = selectedUnit.GetComponentInChildren<CanvasGroup>();
        if (canvas != null)
            canvas.alpha = 1f;

        foreach (var anim in selectedAnimators)
            anim.SetBool("isMoving", true);
        // Debug.Log($"[UC] FollowPath START  count={path.Count}");
        //IEnumerator es para CORRUTINAS+++++++++++++++++++
        //mes info en notes rapides

        for (int i = 1; i < path.Count; i++)
        // int i = 0 seria la unidad en su posición actual
        {
            //Debug.Log($"[UC]  Step {i}/{path.Count - 1}: {path[i - 1].cords} → {path[i].cords}");


            Vector3 startPosition = selectedUnit.position;

            Vector3 endPosition = gridManager.GetPositionFromCoordinates(path[i].cords);
            float travelPercent = 0f;
            //porcentaje de progreso de movimiento

            //Esto es para mantener la altura de la unidad cuando se mueva
            endPosition.y = selectedUnit.position.y + travelPercent;

            //selectedUnit.LookAt(endPosition);
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


        if (canvas != null)
        {
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
        foreach (var anim in selectedAnimators)
            anim.SetBool("isMoving", false);
        //Debug.Log("[UC] FollowPath END");

    }
    
    
    //GESTIÓN DE AUDIO CLIPS
    //TODO: Quitar las declaraciones inferiores y poner toda la gestión de ataques en UnitEntity con serializedObjects
    public void PlayMeleeAudioEvent()
    {
        audioManager.PlayOneShot(audioManager.meleeAudio);
    }

    public void PlayRangeAudioEvent()
    {
        audioManager.PlayOneShot(audioManager.rangeAudio);
    }

}
