// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class TurnManager : MonoBehaviour
// {

//     public enum GameState { START, PLAYERTURN, ENEMYTURN, WIN, LOST }

//     public GameState State;

//     private List<UnitEntity> playerUnits = new List<UnitEntity>();
//     private List<UnitEntity> enemyUnits = new List<UnitEntity>();



//     // Start is called before the first frame update
//     void Start()
//     {
//         // Registro de unidades ya colocadas en escena
//         RegisterExistingUnits();


//         StartCoroutine(SetupGame());

//     }

//     // Registra unidades en el mapa (colocadas en el editor)
//     private void RegisterExistingUnits()
//     {
//         UnitEntity[] all = FindObjectsOfType<UnitEntity>();
//         foreach (var unit in all)
//         {
//             if (unit.GetComponent<Player>() != null)
//                 playerUnits.Add(unit);
//             else
//                 enemyUnits.Add(unit);
//         }
//     }

//     // Hook para registro dinámico (si spawneas unidades después)
//     public void RegisterUnit(UnitEntity unit)
//     {
//         if (unit.GetComponent<Player>() != null)
//                 playerUnits.Add(unit);
//             else
//                 enemyUnits.Add(unit);
//         }

//     // Update is called once per frame
//     private void Update()
//     {
//         if (State == GameState.PLAYERTURN)
//         {
//              bool done = true;
//             foreach (var unit in playerUnits)
//                 if (unit.HasActionsRemaining)
//                 {
//                     done = false;
//                     break;
//                 }
//             if (done)
//                 StartCoroutine(ChangeState(GameState.ENEMYTURN));
//         }
//     }


//     private IEnumerator SetupGame()
//     {
//         State = GameState.START;

//         // TODO: Spawn or position player and enemy units
//         // Example: UnitSpawner.Instance.SpawnAllUnits();

//         yield return new WaitForSeconds(1f);

//         // After setup, go to player turn
//         State = GameState.PLAYERTURN;
//         OnPlayerTurnStart();
//     }







//     private void OnPlayerTurnStart()
//     {
//         Debug.Log("Player Turn Start");

//         // Reset actions for all player units
//         //foreach (var unit in playerUnits)
//         //{
//             unit.ResetActions();
//         //}

//         // Notify UI
//         //UIManager.Instance.UpdateTurnText("Player");
//     }



//         //TODO:  Provar si 'ResetActions() millor sense el foreach de cada unitat.'

//     private void OnEnemyTurnStart()
//     {
//         Debug.Log("Enemy Turn Start");

//         // Reset actions for all enemy units
//         // foreach (var unit in enemyUnits)
//         // {
//         unit.ResetActions();
//         // }

//         //UIManager.Instance.UpdateTurnText("Enemy");

//         // Start enemy AI
//         StartCoroutine(HandleEnemyTurn());
//     }







//     private IEnumerator HandleEnemyTurn()
//     {
//         // Simple AI: loop through enemy units
//         foreach (var unit in enemyUnits)
//         {
//                 // Wait until AI action done
//                 yield return StartCoroutine(unit.PerformAI());
            
//         }

//         // After all enemy actions, back to player
//         yield return new WaitForSeconds(0.5f);
//         StartCoroutine(ChangeState(GameState.PLAYERTURN));
//     }

//     private IEnumerator ChangeState(GameState newState)
//     {
//         State = newState;
//         yield return new WaitForSeconds(0.3f);

//         switch (newState)
//         {
//             case GameState.PLAYERTURN:
//                 OnPlayerTurnStart();
//                 break;

//             case GameState.ENEMYTURN:
//                 OnEnemyTurnStart();
//                 break;

//             case GameState.WIN:
//                 Debug.Log("You Win!");
//                 // TODO: Show win screen
//                 break;

//             case GameState.LOST:
//                 Debug.Log("You Lose!");
//                 // TODO: Show lose screen
//                 break;
//         }
//     }

//     // Call this to check win/lose conditions
//     public void CheckEndConditions()
//     {
//         bool allEnemiesDead = true;
//         bool allPlayersDead = true;

//         foreach (var unit in enemyUnits)
//             if (unit.IsAlive)
//                 allEnemiesDead = false;
//         foreach (var unit in playerUnits)
//             if (unit.IsAlive)
//                 allPlayersDead = false;

//         if (allEnemiesDead)
//             StartCoroutine(ChangeState(GameState.WIN));
//         else if (allPlayersDead)
//             StartCoroutine(ChangeState(GameState.LOST);
//     }
// }
