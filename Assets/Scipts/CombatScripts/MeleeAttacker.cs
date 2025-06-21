using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeleeAttacker : MonoBehaviour
{
    [Tooltip("El tag en el ObjectPooler para tu prefab de melee-hitbox")]
    public string poolTag = "meeleHitbox";

    [SerializeField] private LayerMask meleeLayerMask; // en el Inspector pon “InvGridCollider”
    private UnitEntity _entity;
    private GridManager _gridManager;

    void Awake()
    {
        _entity      = GetComponentInParent<UnitEntity>();
        _gridManager = FindObjectOfType<GridManager>();
    }

    public bool TryMeleeAttack(Ray ray)
    {
        if (!_entity.HasActionsRemaining) return false;

        // 1) Raycast solo contra meleeLayerMask
        if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, meleeLayerMask))
            return false;

        // 2) El resto de tu chequeo sigue igual
        if (!hit.transform.CompareTag("Tile")) return false;
        var tile = hit.transform.GetComponent<Tile>();
        if (tile == null) return false;

        Vector2Int tileCords = tile.cords;
        Vector2Int unitCords = _gridManager.GetCoordinatesFromPosition(_entity.transform.position);
        bool isAdjacent = (Mathf.Abs(tileCords.x - unitCords.x) == 1 && tileCords.y == unitCords.y)
                       || (Mathf.Abs(tileCords.y - unitCords.y) == 1 && tileCords.x == unitCords.x);
        if (!isAdjacent) return false;

        // 3) Consumir acción y spawnear hitbox
        _entity.UseAction();
        Vector3 spawnPos = _gridManager.GetPositionFromCoordinates(tileCords);
        spawnPos.y = _entity.transform.position.y;
        ObjectPooler.Instance.SpawnFromPool(poolTag, spawnPos, Quaternion.identity, _entity.gameObject);
        return true;
    }
}
