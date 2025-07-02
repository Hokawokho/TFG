using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TurnManager : MonoBehaviour
{

    public enum GameState { START, PLAYERTURN, ENEMYTURN, WIN, LOST }

    public GameState State;

    public List<UnitEntity> playerUnits = new List<UnitEntity>();
    public List<UnitEntity> enemyUnits = new List<UnitEntity>();

    private UnitController unitController;

    public KeyCode keyToResetMovement = KeyCode.R;

    public KeyCode keyToEndTurn = KeyCode.F;

    private List<UnitMovementData> unitMovemenList;

    // Contadores para fase de despliegue

    //private bool isPlacingUnits = false;
    private int placedCount = 0;

    public Animator turnAnims;

    public int maxRotationsPerTurn = 1;

    public int remainingRotations;

    public int totalPlayerTurns = 10;
    public int remainingPlayerTurns;
    
    


    // Start is called before the first frame update
    void Start()
    {

        remainingPlayerTurns = totalPlayerTurns;


        if (turnAnims == null)
            Debug.LogError("No se encuentra el turnAnims");

        unitController = FindObjectOfType<UnitController>();

        unitMovemenList = unitController.unitMovementList;

        RegisterExistingUnits();

        ResetAll();
        // Registro de unidades ya colocadas en escena


        //TODO: RETOCAR  PARA EN START de 'SetupGame()' HACER ALGO MÁS-+-+-+-+
        StartCoroutine(SetupGame());


    }


    void ResetAll()
    {

        foreach (var data in unitMovemenList)
        {

            Debug.Log("Movimiento de Unidades reseteado");
            data.ResetMovement();
        }

        foreach (var data in playerUnits) {
            data.ResetActions();
        }

        foreach (var data in enemyUnits)
        {
            data.ResetActions();
        }
        

        
        

    }

    // Registra unidades en el mapa (colocadas en el editor)
    private void RegisterExistingUnits()
    {
        UnitEntity[] all = FindObjectsOfType<UnitEntity>();
        foreach (var unit in all)
        {
            if (unit.GetComponent<Player>() != null)
                playerUnits.Add(unit);
            else
                enemyUnits.Add(unit);

            // Suscribir para detectar muertes
            //????????????????????????????????????????????????????????
            unit.OnDieEvent += CheckEndConditions;
        }
    }

    // Hook para registro dinámico (por si a futuro spawneo nuevas)
    public void RegisterUnit(UnitEntity unit)
    {
        if (unit.GetComponent<Player>() != null)
            playerUnits.Add(unit);
        else
            enemyUnits.Add(unit);
            
        unit.OnDieEvent += CheckEndConditions;
        }

    // Update is called once per frame
    private void Update()
    {
        
        // if (Input.GetKeyDown(keyToResetMovement))
        // {
        //     Debug.Log("Reseteando movimiento");
        //     ResetAll();
        // }


        //Botón Finalizar Turno

        if (Input.GetKeyDown(keyToEndTurn) && (State == GameState.PLAYERTURN || State == GameState.ENEMYTURN))
        {
            
            var next = State == GameState.PLAYERTURN
                ? GameState.ENEMYTURN
                : GameState.PLAYERTURN;
            //ChangingShaderTopTiles.ClearAllHighlights();
            StartCoroutine(ChangeState(next));
        }



        // if (State == GameState.PLAYERTURN)
        // {
        //     bool done = true;
        //     foreach (var unit in playerUnits)
        //         if (unit.HasActionsRemaining)
        //         //TODO: setear las condiciones de Actions Remaining
        //         {
        //             done = false;
        //             break;
        //         }
        //     if (done)
        //         StartCoroutine(ChangeState(GameState.ENEMYTURN));
        // }
    }

    private void OnGUI()
    {
        Rect labelRect = new Rect(10, 70, 200, 30);
        string text = $"Turno : {State}";
        GUI.Label(labelRect, text);

    }


    private IEnumerator SetupGame()
    {
        State = GameState.START;

        //Ejecuta la animación
        turnAnims.Play("StartState");
        

        // TODO: Spawn or position player and enemy units
        // Example: UnitSpawner.Instance.SpawnAllUnits();

        yield return new WaitForSeconds(1f);
        // 2) Iniciar la fase de despliegue
        ShowStartingTiles();
        //isPlacingUnits = true;
        placedCount = 0;
        yield return StartCoroutine(HandlePlacement());
        //isPlacingUnits = false;

        // 3) Limpiar resaltados y pasar a turno de jugador
        //ChangingShaderTopTiles.ClearAllHighlights();

        // After setup, go to player turn
        if (placedCount >= playerUnits.Count)
        {
            // State = GameState.PLAYERTURN;
            // OnPlayerTurnStart();

            StartCoroutine(ChangeState(GameState.PLAYERTURN));
        }
    }

     private void ShowStartingTiles()
    {
        var allTiles = FindObjectsOfType<Tile>();
        foreach (var tile in allTiles)
        {
            if (tile.startingPoint)
            {
                var shader = tile.GetComponentInChildren<ChangingShaderTopTiles>();
                if (shader != null)
                {
                    var mr = shader.GetComponent<MeshRenderer>();
                    if (mr != null) mr.enabled = true;
                }
            }
        }
    }

    private IEnumerator HandlePlacement()
    {
        // Mientras queden unidades por colocar...
        while (placedCount < playerUnits.Count)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit) && hit.transform.CompareTag("Tile"))
                {
                    var tile = hit.transform.GetComponent<Tile>();
                    if (tile != null && tile.startingPoint)
                    {
                        // Desactivar resaltado de este tile
                        var shader = hit.transform.GetComponentInChildren<ChangingShaderTopTiles>();
                        if (shader != null)
                        {
                            var mr = shader.GetComponent<MeshRenderer>();
                            if (mr != null) mr.enabled = false;
                        }

                        // Posicionar la unidad siguiente
                        var unit = playerUnits[placedCount];
                        Vector3 tilePos = hit.transform.position;
                        float currentY = unit.transform.position.y;
                        unit.transform.position = new Vector3(tilePos.x, currentY, tilePos.z);
                        placedCount++;
                    }
                }
            }
            yield return null;
        }
    }







    private void OnPlayerTurnStart()
    {


        

        Debug.Log("Player Turn Start");
        ChangingShaderTopTiles.ClearAllHighlights();
        

        // Reset actions for all player units
        //foreach (var unit in playerUnits)
        //{
        //ResetAll();
        //}

        // Notify UI
        //UIManager.Instance.UpdateTurnText("Player");
        ResetAll();
    }



        //TODO:  Provar si 'ResetActions() millor sense el foreach de cada unitat.'

    private void OnEnemyTurnStart()
    {
        Debug.Log("Enemy Turn Start");

        // Reset actions for all enemy units
        // foreach (var unit in enemyUnits)
        // {
        //unit.ResetActions();
       //ResetAll();

        // }

        //UIManager.Instance.UpdateTurnText("Enemy");

        // Start enemy AI
        StartCoroutine(HandleEnemyTurn());
    }







    private IEnumerator HandleEnemyTurn()
    {
        // Simple AI: loop through enemy units
        foreach (var unit in enemyUnits)
        {
            var ai = unit.GetComponent<EnemyAIController>();
            if (unit.IsAlive && ai != null)
            {
                yield return ai.PerformAI();
                if (State == GameState.LOST)
                    yield break;
                // Wait until AI action done
                //yield return StartCoroutine(unit.PerformAI());

            }
        }


        // yield break;
        // TODO: AÑADIR LOS METODO DE LLAMADA A IA AQUI


        if (State == GameState.ENEMYTURN)
            StartCoroutine(ChangeState(GameState.PLAYERTURN));
    }

    private IEnumerator ChangeState(GameState newState)
    {

        // Si vamos a comenzar el turno enemigo, antes deseleccionamos
        if (newState == GameState.ENEMYTURN || newState == GameState.PLAYERTURN)
        {
            unitController.DeselectCurrentUnit();
        }


        State = newState;
        yield return new WaitForSeconds(0.3f);

        switch (newState)
        {
            case GameState.PLAYERTURN:
                OnPlayerTurnStart();
                remainingRotations = maxRotationsPerTurn;
                // Reproducir animación
                turnAnims.Play("StateChange_in");
                break;

            case GameState.ENEMYTURN:

                remainingPlayerTurns--;
                // 2) Comprobación global de fin de partida
                CheckEndConditions();
                if (State == GameState.ENEMYTURN)
                {
                    OnEnemyTurnStart();
                    // Reproducir animación
                    turnAnims.Play("StateChangeEnemy_in");
                }
                break;

            case GameState.WIN:
                Debug.Log("You Win!");
                // Reproducir animación
                turnAnims.Play("Victory");
                SceneManager.LoadScene(0, LoadSceneMode.Single);
                // TODO: Show win screen
                break;

            case GameState.LOST:
                Debug.Log("You Lose!");
                // Reproducir animación
                turnAnims.Play("Defeat");
                yield return new WaitForSeconds(turnAnims.GetCurrentAnimatorStateInfo(0).length);
                SceneManager.LoadScene(0, LoadSceneMode.Single);
                // TODO: Show lose screen
                break;
        }
    }

    // Call this to check win/lose conditions
    public void CheckEndConditions()
    {
        bool allEnemiesDead = true;
        bool allPlayersDead = true;

        foreach (var unit in enemyUnits)
            if (unit.IsAlive)
                allEnemiesDead = false;
        foreach (var unit in playerUnits)
            if (unit.IsAlive)
                allPlayersDead = false;

        if (allEnemiesDead )
            StartCoroutine(ChangeState(GameState.WIN));
        else if (allPlayersDead || remainingPlayerTurns <= 0)
            StartCoroutine(ChangeState(GameState.LOST));
    }
}
