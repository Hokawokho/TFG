using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeSingleDirection : MonoBehaviour
{
    public bool inverse;
    public float resizeAmount = 1;
    public string resizeDirection = "y" ;
    private Vector3 originalPosition;
    private Vector3 originalScale;
   // private GameObject plane;
    
    // Start is called before the first frame update
    void Start()
    {



        //plane = this.gameObject.transform.getChild(0);


        // Si resizeAmount es 0 (valor no inicializado), asignamos 1
        if (resizeAmount == 0) 
            resizeAmount = 1;

        // Si resizeDirection está vacío o null, asignamos "y"
        if (string.IsNullOrEmpty(resizeDirection)) 
            resizeDirection = "y";

        
        
        //++++++-+AÇÒ DE DALT NO DEURIA SER NECESSARI, COMPROVAR AMB PORTATIL


        originalPosition = transform.position;
        originalScale = transform.localScale;
    }
 
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown (KeyCode.Alpha5)) {
            resize (resizeAmount, resizeDirection);
        }
    }
 
    void resize(float amount, string direction) {

        //ES PER VORE SI ES FA GRAN O MENUT
        // SI invers == TRUE - amount------ invers == FALSE + amount
        float sizeFactor = inverse?  - amount : amount;
        float positionFactor = inverse? - amount/2 : amount/2;


        switch (direction){
            case "x":
                transform.position += new Vector3(positionFactor, 0, 0);
                transform.localScale += new Vector3(sizeFactor, 0, 0);
                break;

            case "y":
                transform.position += new Vector3(0, positionFactor, 0);
                transform.localScale += new Vector3(0, sizeFactor, 0);
                //plane.transform.position += new Vector3(0, -0.01f ,0);

                break;

            case "z":
                transform.position += new Vector3(0, 0, positionFactor);
                transform.localScale += new Vector3(0, 0, sizeFactor);
                break;
        }

        if (transform.localScale.x < originalScale.x || 
            transform.localScale.y < originalScale.y || 
            transform.localScale.z < originalScale.z)
        {
            ResetSize();
        }

        // if (direction == "x" && inverse == false) {
        //     transform.position = new Vector3 (transform.position.x + (amount / 2), transform.position.y, transform.position.z);
        //     transform.localScale = new Vector3 (transform.localScale.x + amount, transform.localScale.y, transform.localScale.z);
        // } else if (direction == "y" && inverse == false) {
        //     transform.position = new Vector3 (transform.position.x, transform.position.y + (amount / 2), transform.position.z);
        //     transform.localScale = new Vector3 (transform.localScale.x, transform.localScale.y + amount, transform.localScale.z);
        // } else if (direction == "z" && inverse == false) {
        //     transform.position = new Vector3 (transform.position.x, transform.position.y, transform.position.z + (amount / 2));
        //     transform.localScale = new Vector3 (transform.localScale.x, transform.localScale.y, transform.localScale.z + amount);
        // }
 
        // if (direction == "x" && inverse == true) {
        //     transform.position = new Vector3 (transform.position.x - (amount / 2), transform.position.y, transform.position.z);
        //     transform.localScale = new Vector3 (transform.localScale.x + amount, transform.localScale.y, transform.localScale.z);
        // } else if (direction == "y" && inverse == true) {
        //     transform.position = new Vector3 (transform.position.x, transform.position.y - (amount / 2), transform.position.z);
        //     transform.localScale = new Vector3 (transform.localScale.x, transform.localScale.y + amount, transform.localScale.z);
        // } else if (direction == "z" && inverse == true) {
        //     transform.position = new Vector3 (transform.position.x, transform.position.y, transform.position.z - (amount / 2));
        //     transform.localScale = new Vector3 (transform.localScale.x, transform.localScale.y, transform.localScale.z + amount);
        // }
 
    }

    void ResetSize()
    {
        transform.localScale = originalScale;
        transform.position = originalPosition;
    }



        // TODO ++++++++++++++++++++++++++++++++

        //Método para reajustar el tamaño entre el plano y el cubo tras un aumento de terreno
        

}
