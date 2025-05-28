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

    void Awake()
    {
        gridManager = FindObjectOfType<GridManager>();
        turnManager = FindObjectOfType<TurnManager>();
        pathfinding = FindObjectOfType<Pathfinding>();
        unitController = FindObjectOfType<UnitController>();

        //TODO: Retocar pa pillar totes les unitats enemigues
        //selfEntity = FindObjectOfType<UnitEntity>();

    }

    // public IEnumerator PerformAI()
    // {


    // }

    public IEnumerator PerformAI()
    {
        // 1. Obtener todas las unidades jugador vivas
        List<UnitEntity> targets = turnManager.playerUnits.FindAll(u => u.IsAlive);
        if (targets.Count == 0) yield break;

        // 2. Escoger la mejor según criterio (ej: menos vida)
        UnitEntity bestTarget = targets.OrderBy(t => Vector3.Distance(transform.position, t.transform.position)).ThenBy(t => t.hitpoints).First();

        Vector2Int selfPos = gridManager.GetCoordinatesFromPosition(transform.position);
        Vector2Int targetPos = gridManager.GetCoordinatesFromPosition(bestTarget.transform.position);

        // 3. Buscar tiles válidas para atacar (línea recta, distancia <= 4)
        List<Vector2Int> validTiles = FindTilesToAttackFrom(targetPos, 4);
        Dictionary<Vector2Int, int> pathCosts = new Dictionary<Vector2Int, int>();

        Scenario bestScenario = new Scenario(); // por defecto, -100000

        foreach (var pos in validTiles)
        {
            if (!gridManager.GetNode(pos).walkable) continue;

            int pathCost = unitController.CalculatePathCost(selfPos, pos);
            if (pathCost == -1 || pathCost > 99) continue;

            pathCosts[pos] = pathCost;

            float value = EvaluateScenario(bestTarget, pathCost);
            if (value > bestScenario.scenarioValue)
            {
                bestScenario = new Scenario(value, selfPos, pos, true);
            }
        }

        // 4. Mover a la mejor casilla
        if (bestScenario.scenarioValue > -100000)
        {
            Debug.Log($"El mejor escenario para el enemigo es la posición [{bestScenario.targetTile}]");
            yield return MoveTo(bestScenario.targetTile);

            yield return new WaitForSeconds(0.2f);
            yield return ShootAt(bestTarget);
        }

        else
        {
            Debug.Log($" No hay posiciones desde las que atacar. Me acerco al objetivo...");
            yield return ApproachTarget(bestTarget, validTiles, pathCosts);


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
        //TODO: No es podria passar directament el controller desde el awake com tot lo demés???
        //UnitController controller = FindObjectOfType<UnitController>();
        unitController.selectedUnit = transform;
        unitController.unitSelected = true;
        unitController.MoveUnitTo(position);
        yield return new WaitUntil(() => !unitController.isMoving);

        //TODO: Espera extra tras acabar de moverse
        yield return new WaitForSeconds(0.5f);
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


    private IEnumerator ApproachTarget(UnitEntity bestTarget, List<Vector2Int> validTiles, Dictionary<Vector2Int, int> pathCosts )
    {

        // List<Vector2Int> reachableTiles = new List<Vector2Int>();

        UnitMovementData unitData = unitController.GetUnitData(gameObject);
        List<Vector2Int> reachableTiles = pathCosts
         .Where(p => p.Value <= unitData.remainingTiles)
         .Select(p => p.Key)
         .ToList();
        
        if (reachableTiles.Count > 0)
    {
        Vector2Int bestMove = reachableTiles
            .OrderBy(tile => Vector2Int.Distance(tile, gridManager.GetCoordinatesFromPosition(bestTarget.transform.position))).First();

        Debug.Log($"[IA] Me muevo a {bestMove} para acercarme a {bestTarget.name}");
        yield return MoveTo(bestMove);
    }
    else
    {
        Debug.Log("[IA] No puedo acercarme a ninguna casilla relevante.");
    }
    }
}
