using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyAIController : MonoBehaviour
{


    public UnitEntity selfEntity;
    //TODO:   llevar açò pa que siga a mes de un enemic
    private GridManager gridManager;
    private TurnManager turnManager;
    private Pathfinding pathfinding;
    private UnitController unitController;

    private LayerRenderChanger layerRenderChanger;

    public int AttackDistance = 4;

    void Awake()
    {
        gridManager = FindObjectOfType<GridManager>();
        turnManager = FindObjectOfType<TurnManager>();
        pathfinding = FindObjectOfType<Pathfinding>();
        unitController = FindObjectOfType<UnitController>();
        layerRenderChanger = GetComponentInParent<LayerRenderChanger>();

        //TODO: Retocar pa pillar totes les unitats enemigues
        //selfEntity = FindObjectOfType<UnitEntity>();

    }

    // public IEnumerator PerformAI()
    // {


    // }

    public IEnumerator PerformAI()
    {
        if (selfEntity.invulnerable)
        {
            Debug.Log($"[EnemyAI] {selfEntity.name} está invulnerable: salto de turno");
            yield break;  // termina esta corrutina → TurnManager pasará a la siguiente unidad
        }


        // 1. Obtener todas las unidades jugador vivas
        List<UnitEntity> targets = turnManager.playerUnits.FindAll(u => u.IsAlive && !u.invulnerable);
        if (targets.Count == 0) yield break;

        // 2. Escoger la mejor según criterio (ej: menos vida)
        UnitEntity bestTarget = targets.OrderBy(t => Vector3.Distance(transform.position, t.transform.position)).ThenBy(t => t.CurrentHealth).First();

        Vector2Int selfPos = gridManager.GetCoordinatesFromPosition(transform.position);
        Vector2Int targetPos = gridManager.GetCoordinatesFromPosition(bestTarget.transform.position);

        // 3. Buscar tiles válidas para atacar (línea recta, distancia <= 4)
        List<Vector2Int> validTiles = FindTilesToAttackFrom(targetPos, AttackDistance);
        validTiles.RemoveAll(tile => tile == selfPos);
        Dictionary<Vector2Int, int> pathCosts = new Dictionary<Vector2Int, int>();
        

        Scenario bestScenario = new Scenario(); // por defecto, -100000

        foreach (var pos in validTiles)
        {
            if (!gridManager.GetNode(pos).walkable) continue;

            int pathCost = unitController.CalculatePathCost(selfPos, pos);
            if (pathCost == 0 || pathCost > 99)
            // if (pathCost == 1 || pathCost > 99)
            {
                Debug.Log($"Coste del path de la IA no rentable, pathcost = {pathCost}");
                continue;
            }
            pathCosts[pos] = pathCost;

            float value = EvaluateScenario(bestTarget, pathCost);
            if (value > bestScenario.scenarioValue)
            {
                bestScenario = new Scenario(value, selfPos, pos, true);
            }
        }

        var unitData = unitController.GetUnitData(gameObject);

        // 4. Mover a la mejor casilla
        if (bestScenario.scenarioValue > -100000)
        {
            int costToBestTile = pathCosts[bestScenario.targetTile];

            if (costToBestTile <= unitData.remainingTiles)
            {
                Debug.Log($"El mejor escenario para el enemigo es la posición [{bestScenario.targetTile}]");
                yield return MoveTo(bestScenario.targetTile);

                yield return new WaitForSeconds(0.2f);
                yield return ShootAt(bestTarget);
            }
            else
        {
            Debug.Log($" No hay posiciones desde las que atacar. Me acerco al objetivo...");
            yield return ApproachTargetTile(bestScenario.targetTile, selfPos, unitData.remainingTiles);

        }
        }

        

        yield return new WaitForSeconds(0.5f);
    }







    private List<Vector2Int> FindTilesToAttackFrom(Vector2Int targetPos, int maxRange)
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        for (int i = 1; i <= maxRange; i++)
        {
            positions.Add(targetPos + Vector2Int.up * i);
            positions.Add(targetPos + Vector2Int.down * i);
            positions.Add(targetPos + Vector2Int.left * i);
            positions.Add(targetPos + Vector2Int.right * i);
        }
        return positions;
    }



    private float EvaluateScenario(UnitEntity target, int pathCost)
    {

        float healtScore = 100 - target.hitpoints.hitPoints;
        float distancePenalty = pathCost * 5;
        return healtScore - distancePenalty;
    }

    public IEnumerator MoveTo(Vector2Int position)
    {
        layerRenderChanger.ResumeCollisions();
        //TODO: No es podria passar directament el controller desde el awake com tot lo demés???
        //UnitController controller = FindObjectOfType<UnitController>();
        unitController.selectedUnit = transform;
        unitController.unitSelected = true;
        unitController.SelectUnit(transform);
        // 1) Recalcular la ruta teniendo en cuenta los nodos bloqueados
        Vector2Int currentPos = gridManager.GetCoordinatesFromPosition(transform.position);

        // Debug.Log($"[AI] MoveTo from {currentPos} to {position}");
        pathfinding.SetNewDestination(currentPos, position);

        pathfinding.NotifyRecievers();   // dispara RecalculatePath en UnitController
        // 2) Ahora sí, mueve el UnitController con la ruta recién calculada
        unitController.MoveUnitTo(position);
        yield return new WaitUntil(() => !unitController.isMoving);
        // Debug.Log("[AI] MoveTo FIN – now at " + gridManager.GetCoordinatesFromPosition(transform.position));

        //TODO: Espera extra tras acabar de moverse
        //yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator ShootAt(UnitEntity target)
    {
        var shooter = GetComponentInChildren<ObjectShooter>();

        //TODO: per ara sols dispara 1 tipo de projectil, cambiar en el futur.-+-+-+-+-+-+

        //TODO: repasar lo de baix-+-+-+-+
        yield return new WaitForSeconds(0.5f);

        if (shooter != null)
        {

            Vector3 toTarget = target.transform.position - transform.position;

            Vector3 shotDirection = Vector3.zero;

            if (Mathf.Abs(toTarget.x) > Mathf.Abs(toTarget.z))
            {
                shotDirection = toTarget.x > 0 ? Vector3.right : Vector3.left;
            }
            else
            {
                shotDirection = toTarget.z > 0 ? Vector3.forward : Vector3.back;
            }

            shooter.currentDirection = shotDirection.normalized;

            Debug.Log($"{gameObject.name} apunta en dirección {shooter.currentDirection} hacia {target.gameObject.name}");


            bool fired = shooter.TryShoot();
            if (fired)
            {
                Transform unitRoot = transform.parent != null ? transform.parent : transform;
                Animator[] animators = unitRoot.GetComponentsInChildren<Animator>(true);
                foreach (var anim in animators)
                    anim.SetTrigger("Attack");
                Debug.Log($"{gameObject.name} ataca a {target.gameObject.name} desde la posición {transform.position}");
            }
            else
            {
                Debug.Log($"{gameObject.name} intentó disparar, pero no pudo (cooldown, sin acciones, etc.)");
            }
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} no tiene un ObjectShooter asignado");
        }
    }

    private IEnumerator ApproachTargetTile(Vector2Int finalTarget, Vector2Int selfPos, int maxTiles)
    {
        pathfinding.SetNewDestination(selfPos, finalTarget);
        List<Node> fullPath = pathfinding.GetNewPath();
    if (fullPath == null || fullPath.Count <= 1)
        {
            Debug.LogWarning("[IA] No se puede calcular un camino válido");
            yield break;
        }

    // Quitamos el nodo actual (posición de la unidad)
    fullPath.RemoveAt(0);

    // Recortamos el camino según los tiles disponibles
    int stepsToTake = Mathf.Min(maxTiles, fullPath.Count);
    if (stepsToTake <= 0)
    {
        Debug.Log("[IA] No puedo moverme más esta ronda.");
        yield break;
    }

    Vector2Int bestReachableTile = fullPath[stepsToTake - 1].cords;
    Debug.Log($"[IA] Me acerco a {bestReachableTile} (lo más cerca posible)");
    yield return MoveTo(bestReachableTile);


    }

}
