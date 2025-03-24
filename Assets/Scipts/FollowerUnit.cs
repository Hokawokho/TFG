using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UIElements;

public class FollowerUnit : MonoBehaviour
{


    private PositionConstraint positionConstraint;
    private ConstraintSource  unitConstraint;//originalSource

    private Transform underUnit; //currentFollow

    void Start()
    {
        positionConstraint = GetComponent<PositionConstraint>();
        if (positionConstraint ==null){

            Debug.LogError("No se ha encontrado el componente PositionConstraint");
            return;

        }

        if(positionConstraint.sourceCount > 0){
            
            unitConstraint = positionConstraint.GetSource(0);
            underUnit = unitConstraint.sourceTransform; 
            Debug.Log($"Fuente inicial asignada desde el editor: {underUnit.name}");


        }

        
    }

    public void ChangeFollowing(){

        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.green, 1f);

        if(Physics.Raycast(ray, out hit, Mathf.Infinity )){
                                        //Infinity es pa q baixe fins que toque algo Ground Layer
            
            
           //Transform groundTransform = hit.collider.transform;
           Transform hitTransform = hit.collider.transform;
           //ESTO ES SOLO PARA EL DEBUG DE SI LE HA DADO AL TILE+++++++++++


            //if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            if(hit.collider.GetComponent<BoxCollider>() != null)

            {


                Debug.Log($"Ground detectado en: {hitTransform.name}");
                
                //SI EL OBJETO IMPACTADO TIENE LA CAPA GROUND
                if(underUnit != hitTransform)
                {

                   SetNewFollowing(hitTransform);    
                   Debug.Log($"Siguiendo ahora a: {hitTransform.name}");
                }

                else
                {
                    // Volver al objeto asignado desde el editor
                    SetNewFollowing(unitConstraint.sourceTransform);
                    Debug.Log($"Volviendo a la fuente original: {unitConstraint.sourceTransform.name}");
                }
            }
            else
            {
                Debug.LogWarning($"El objeto detectado ({hitTransform.name}) no tiene BoxCollider.");
            }

        }
        else{

            Debug.LogWarning("No se ha detectado ningún objeto en el raycast.");
        }

    // Update is called once per frame
    // void Update()
    // {
        
    // }
}

    public void RestoreFollowing(){

        if(unitConstraint.sourceTransform != null){

            SetNewFollowing(unitConstraint.sourceTransform);
            Debug.Log($"Volver a la fuente original: {unitConstraint.sourceTransform.name}");
        }

    }

    private void SetNewFollowing(Transform newFollowing)
    {
        ConstraintSource newSource = new ConstraintSource
        {
            // sourceTransform = hit.collider.transform,
            //++++++++++++++++ESTO SE HACE EN LA DECLARACIÓN DE 'groundTransform'
            sourceTransform = newFollowing,
            weight = 1f
        };

        positionConstraint.SetSource(1, newSource);
        positionConstraint.constraintActive = true;

        underUnit = newFollowing;
    }
}
