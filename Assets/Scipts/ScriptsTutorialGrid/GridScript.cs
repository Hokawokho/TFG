using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridScript

    //No tiene MonoBehaviour porque no se va estar en un GameObject.++++++++++

{
    private int width;
    private int height;
    private int[,]gridArray;
    //Esto es para hecer un array bidimensional

    public GridScript(int width, int height){
        
        //Aqui estamos haciendo un constructor, por eso Nombre class == metodo++++++++


        this.width = width;
        this.height = height;
        gridArray = new int[width, height];

        Debug.Log("Grid created with dimensions: " + width + "x" + height);


    }
}
