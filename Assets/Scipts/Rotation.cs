using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{

    public float rotationAngle = 90f; // Ángulo de rotación en grados
    public float rotationSpeed = 200f; // Velocidad de rotación en grados por segundo
    private bool isRotating = false;
    private float targetRotation;



    void Start()
    {
        
    }

    // Update is called once per frame
    
    void Update()
    {


        if(Input.GetKeyDown(KeyCode.R) &&!isRotating){
            targetRotation = transform.eulerAngles.y + rotationAngle;
            StartCoroutine(RotateSmoothly());
        
        }
        
        
        
        

    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 150, 30), transform.eulerAngles.ToString());
        GUI.Label(new Rect(10, 40, 150, 30), isRotating.ToString());

    }

    private System.Collections.IEnumerator RotateSmoothly()
    {
        isRotating = true;
        while (Mathf.Abs(Mathf.Repeat(transform.eulerAngles.y - targetRotation, 360)) > 0.1f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, targetRotation, 0), rotationSpeed * Time.deltaTime);
            yield return null;
        }
        transform.rotation = Quaternion.Euler(0, targetRotation, 0); // Asegurar que la rotación finaliza exactamente en el ángulo deseado
        isRotating = false;
    }
}



// if(Input.GetKey(KeyCode.R)){

        //     transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // }