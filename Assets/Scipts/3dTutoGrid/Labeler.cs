using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


[ExecuteAlways]
//Açò fa que es faça sempre++++++++++

public class Labeler : MonoBehaviour
{
    TextMeshPro label;
    public Vector2Int cords = new Vector2Int();
    GridManager gridManager;


    //Aço de ací es per a els numeros, tot visual
    [SerializeField] Color defaultColor = Color.white;
    [SerializeField] Color blockedColor = Color.red;
    [SerializeField] Color exploredColor = Color.yellow;
    [SerializeField] Color pathColor = new Color(1f, 0.5f, 0f);

    private void Awake()
    {
        gridManager = FindObjectOfType<GridManager>();
        //Aço buscara elements que tinga 'GridManager'
        label = GetComponentInChildren<TextMeshPro>();
        //Agafa el label, que es el fill de 'Tile' pq estem ficanto en este
        
        //AÑADIDO VISUAL A POSTERIORI++--+-+-+-+-+-+-+-+--+
        label.enabled = false;
        
        
        DisplayCords();
        
       
    }

    private void Update()
    {
        //AÑADIDO VISUAL A POSTERIORI++--+-+-+-+-+-+-+-+--+
        if(!Application.isPlaying){

            label.enabled = true;
        }

        //Aquí es quan es fa la mateixa cosa que fem en Awake, pero per cada frame
        DisplayCords();
        transform.name = cords.ToString();


        //AÑADIDO VISUAL A POSTERIORI++--+-+-+-+-+-+-+-+--+
        ToggleLables();
        SetLabelColor();
    }



    private void DisplayCords()
    {
        if (!gridManager) { return; }
        cords.x = Mathf.RoundToInt(transform.position.x / gridManager.UnityGridSize);
        cords.y = Mathf.RoundToInt(transform.position.z / gridManager.UnityGridSize);
        //Aço es pa pillar la posició de 'Tile' i pa ficaro a posteriori en el Label

        label.text = $"{cords.x}, {cords.y}";
    }





    //AÑADIDO VISUAL A POSTERIORI++--+-+-+-+-+-+-+-+--+
    void SetLabelColor(){

        if(gridManager == null) { return; }

        Node node = gridManager.GetNode(cords);
        
        if(node == null) {return;}

        if(!node.walkable){
            label.color = blockedColor;
        }

        else if(node.path){
            label.color = pathColor;
        }
        else if(node.explored){
            label.color = exploredColor;
        }
        else{
            label.color = defaultColor;
        }
    }

    //AÑADIDO VISUAL A POSTERIORI++--+-+-+-+-+-+-+-+--+
    void ToggleLables(){
        
        if(Input.GetKeyDown(KeyCode.C)){
            label.enabled =!label.IsActive();
        }
    }
}
