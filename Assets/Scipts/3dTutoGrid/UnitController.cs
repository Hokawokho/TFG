using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using TMPro;
using UnityEngine;

public class UnitController : MonoBehaviour
{

    Transform selectedUnit;
    bool unitSelected = false;

    [SerializeField] float movementSpeed = 1f;
    GridManager gridManager;


    List<Node> path = new List<Node>();
    Pathfinding pathFinder;



    // Start is called before the first frame update
    void Start()
    {

        gridManager = FindObjectOfType<GridManager>();

        pathFinder = FindObjectOfType<Pathfinding>();

        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            //Si se hace click Derecho donde este el raton

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //Marca amb un ray on estiga el nostre ratolí
            RaycastHit hit;
            //Detecta si el ray colisiona con algún objecte

            bool hasHit = Physics.Raycast(ray, out hit);
            //Açò guarda el que s'ha seleccionat amb un ray en HIT

            if(hasHit){

                if(hit.transform.tag == "Tile"){

                    Vector2Int tileCords = hit.transform.GetComponent<Tile>().cords;
                    Debug.Log($"Has hecho clic en la casilla: {tileCords}");

                    if(gridManager.GetNode(tileCords) != null && !gridManager.GetNode(tileCords).walkable) {
                        Debug.Log("No se puede mover en esta casilla");
                        return;
                    }

                    if(unitSelected ){

                        //Vector2Int targetCords = hit.transform.GetComponent<Labeler>().cords;
                        Vector2Int targetCords = hit.transform.GetComponent<Tile>().cords;
                        Vector2Int startCords = new Vector2Int((int)selectedUnit.transform.position.x, (int)selectedUnit.transform.position.z) / gridManager.UnityGridSize;
                        pathFinder.SetNewDestination(startCords, targetCords);
                        RecalculatePath(true);

                        //selectedUnit.transform.position = new Vector3(targetCords.x, selectedUnit.position.y, targetCords.y);
                    }

                }


                if(hit.transform.tag == "Unit"){

                    selectedUnit = hit.transform;
                    unitSelected = true;

                }


            }


        }
        
    }

    void RecalculatePath(bool resetPath){

        Vector2Int coordinates = new Vector2Int();
        if(resetPath){

            coordinates = pathFinder.StartCords;
        }
        else{

            coordinates =gridManager.GetCoordinatesFromPosition(transform.position);
        }
        StopAllCoroutines();
        path.Clear();
        path = pathFinder.GetNewPath(coordinates);
        StartCoroutine(FollowPath());
    }
   
   
    IEnumerator FollowPath(){
    
    //IEnumerator es para CORRUTINAS+++++++++++++++++++
    //mes info en notes rapides

        for(int i = 1; i < path.Count; i++)
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

            while(travelPercent < 1f){
                //mientras no se aclance el destino

                travelPercent += Time.deltaTime * movementSpeed;
                selectedUnit.position = Vector3.Lerp(startPosition, endPosition, travelPercent);
                                                        //Lerp == Mueve suavemente la unidad de start a end++++++++++++++++++++
                yield return new WaitForEndOfFrame();
                //Açò es per a acabar la Corrutina+++++++++++
            }

        }

        //Aço es per a desseleccionar la unitat++++++++++++++
        unitSelected = false;
        selectedUnit = null;



    }
}
