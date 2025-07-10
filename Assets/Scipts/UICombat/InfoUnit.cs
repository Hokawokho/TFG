using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InfoUnit : MonoBehaviour
{
    public GameObject meleeAttackGO;
    public GameObject rangeAttackGO;
    // public TextMeshProUGUI meleeAttackText;

    public GameObject movementGO;
    public TextMeshProUGUI movementText;
    public GameObject unitProfileGO;
    public GameObject enemyProfileGO;

    // Estos se guardan internamente para actualizar dinámicamente
    private UnitEntity currentEntity;
    private UnitMovementData currentMovementData;



    
    private void Awake()
    {
        // Llama a SetUnit con null para desactivar toda la UI inicialmente
        SetUnit(null, null);
    }
    
    //ESTE ANTIGUO LEE LAS ACCIONES EN VEZ DE EL DAÑO (QUE EN REALIDAD LO Q LEE ES SI ES A RANGO O NO)

    // public void SetUnit(UnitEntity entity, UnitMovementData movementData)
    // {
    //     currentEntity = entity;
    //     currentMovementData = movementData;

    //     bool hasUnit = entity != null && movementData != null;
    //     meleeAttackGO.SetActive(hasUnit);
    //     movementGO.SetActive(hasUnit);
    //     unitProfileGO.SetActive(hasUnit);

    //     if (hasUnit)
    //     {
    //         // Actualiza el texto una sola vez; Update() se encargará de refrescarlo si cambian los valores.
    //         // Sustituye 'actionPointsRemaining' por tu propiedad real en UnitEntity
    //         bool isPlayer = entity.GetComponent<Player>() != null;
    //         unitProfileGO.SetActive(isPlayer);
    //         enemyProfileGO.SetActive(!isPlayer);

    //         // Textos dinámicos
    //         meleeAttackText.text = ": " + entity.currentActions;
    //         movementText.text =  ": " + movementData.remainingTiles.ToString();
    //     }
    //     else
    //     {
    //         // Oculta ambos perfiles
    //         unitProfileGO. SetActive(false);
    //         enemyProfileGO.SetActive(false);
    //     }
    // }

    public void SetUnit(UnitEntity entity, UnitMovementData movementData)
    {
        currentEntity = entity;
        currentMovementData = movementData;

        bool hasUnit = entity != null && movementData != null;
        
        meleeAttackGO.SetActive(false); // Inicialmente apagados
        rangeAttackGO.SetActive(false);
        
        movementGO.SetActive(hasUnit);
        unitProfileGO.SetActive(hasUnit);

        if (hasUnit)
        {
            // Actualiza el texto una sola vez; Update() se encargará de refrescarlo si cambian los valores.
            // Sustituye 'actionPointsRemaining' por tu propiedad real en UnitEntity
            bool isPlayer = entity.GetComponent<Player>() != null;
            unitProfileGO.SetActive(isPlayer);
            enemyProfileGO.SetActive(!isPlayer);

            // Textos dinámicos
            string type = entity.unitType.name.ToLower();

            if (type.Contains("range"))
            {
                rangeAttackGO.SetActive(true);
                // meleeAttackText.text = ": 1 ";
            }
            else
            {
                meleeAttackGO.SetActive(true);
                // meleeAttackText.text = ": 2";
            }
            movementText.text = ": " + movementData.remainingTiles.ToString();
        }
        else
        {
            // Oculta ambos perfiles
            unitProfileGO.SetActive(false);
            enemyProfileGO.SetActive(false);
        }
    }

    void Update()
    {
        // Si la UI está activa, refrescamos los valores por si cambian en tiempo de ejecución
        if (currentEntity != null && currentMovementData != null)
        {
            // meleeAttackText.text = ": " + currentEntity.currentActions;
            movementText.text    = ": " + currentMovementData.remainingTiles.ToString();
        }
    }
}