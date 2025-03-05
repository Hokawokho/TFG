using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Grid {
    // No tiene MonoBehaviour porque no se le va a poner a un GameObject

    /*
    private int width;
    private int height;
    private float cellSize;
    private int[,] gridArray;
    private TextMesh[,] debugTextArray;
    //Esta es pa fer Debugging**************************************

    public Grid(int width, int height, float cellSize)
    {

        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        
        gridArray = new int[width, height];
        debugTextArray = new TextMesh[width, height];

        for (int x = 0; x < gridArray.GetLength(0); x++){
            for (int y = 0; y < gridArray.GetLength(x); y++){

                debugTextArray[x,y] = TextGrid.CreateWorldText(gridArray[x, y].ToString(), null, GetWorldPosition(x, y) + new Vector3(cellSize,cellSize) * 5f, 30, Color.white, TextAnchor.MiddleCenter);
                //Aço es pa fer el grid de Text, LLEVAR A POSTERIORI++++++++++++++++++++++
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y+1), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x+1, y), Color.white, 100f);
                      //Este metode pilla un lloc d'INICI i FIN pa dibuixar les linees del Grid
            }

        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);

       
       
        SetValue (2,1,56);

        }
    }
    private Vector3 GetWorldPosition(int x, int y){
           // return new Vector3(x * cellSize, 0, y * cellSize);
        return new Vector3(x, y) * cellSize;

    }

    public void SetValue (int x, int y, int value) {

        if (x >= 0 &&  y >= 0 && y < height) {

            gridArray[x,y] = value;
            debugTextArray[x, y].text = gridArray[x, y].ToString();
            //Esta es pa fer Debugging**************************************
        }


    }*/
            
}

