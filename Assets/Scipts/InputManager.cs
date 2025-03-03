using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] 
    private Camera sceneCamera;
    //AÇÒ PA DETECTAR LA POSICIÓ DEL MOUSE

    private Vector3 lastPosition;

    [SerializeField]
    private LayerMask placementLayerMask;
    //AÇÒ PA MARCAR QUIN PLANO TE QUE REACCIONAR



    public Vector3 GetSelectedMapPosition(){

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = sceneCamera.nearClipPlane;
        //mousePos.z es para que no se puedan seleccionar cosas q no esten renderizadas por la camara


        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        //Esto se hace para que la camara haga un rayo desde la posicion del mouse hasta el plano, seleccionando asi el objeto que hago click

        RaycastHit hit;
        //Este anterior es el resultado

        if (Physics.Raycast(ray, out hit, 100, placementLayerMask)){
                                            //Este es per a marcar el Layer que volem gastar com a Grid
            lastPosition = hit.point;
        }
        return lastPosition;




    }
}
