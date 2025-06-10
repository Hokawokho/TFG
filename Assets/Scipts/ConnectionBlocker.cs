using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct ConnectionPair
{
    public Vector2Int from;   // Coordenadas del nodo origen
    public Vector2Int to;     // Coordenadas del nodo destino

    public ConnectionPair(Vector2Int from, Vector2Int to)
    {
        this.from = from;
        this.to = to;
    }
}

public class ConnectionBlocker : MonoBehaviour
{
    public GridManager gridManager;

    public List<ConnectionPair> case1Blocks = new List<ConnectionPair>();

    public List<ConnectionPair> case2Blocks = new List<ConnectionPair>();

    public List<ConnectionPair> case3Blocks = new List<ConnectionPair>();

    public List<ConnectionPair> case4Blocks = new List<ConnectionPair>();

    public void ApplyBlocksForCase(int caseNumber)
    {
        if (gridManager == null)
        {
            Debug.LogError("[ConnectionBlocker] No hay referencia a GridManager.");
            return;
        }

        ClearAllBlockedConnections();

        List<ConnectionPair> toBlock = GetBlocksListForCase(caseNumber);

        // 3) Aplicar cada par a través de GridManager.BlockConnection(...)
        foreach (var pair in toBlock)
        {
            gridManager.BlockConnection(pair.from, pair.to);
        }
    }

    private List<ConnectionPair> GetBlocksListForCase(int caseNumber)
    {
        switch (caseNumber)
        {
            case 1: return case1Blocks;
            case 2: return case2Blocks;
            case 3: return case3Blocks;
            case 4: return case4Blocks;
            default:
                Debug.LogWarning($"[ConnectionBlocker] Case {caseNumber} inválido. Se usa lista vacía.");
                return new List<ConnectionPair>();
        }
    }
    
    private void ClearAllBlockedConnections()
    {
        foreach (var node in gridManager.Grid)
        {
            node.Value.blockedConnections.Clear();
        }
    }

}

