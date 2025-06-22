using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Turn_RotationInfo : MonoBehaviour
{
    public TextMeshProUGUI rotationText;
    public GameObject rotationUp;
    public GameObject rotationDown;

    private TurnManager turnManager;

    void Start()
    {
        //rotationText.gameObject.SetActive(false);
        rotationDown.SetActive(false);
        rotationUp.SetActive(false);
        turnManager = FindObjectOfType<TurnManager>();
    }

    void Update()
    {
        // Solo actualizamos durante el turno del jugador
        if (turnManager.State == TurnManager.GameState.PLAYERTURN)
        {
            rotationText.text = $"{turnManager.remainingPlayerTurns} turnos restantes";

            // 2) Indicadores: si quedan rotaciones (>0) mostramos rotationUp, si no rotationDown
            rotationUp.SetActive(turnManager.remainingRotations > 0);
            rotationDown.SetActive(turnManager.remainingRotations <= 0);
        }
    }
    
}
