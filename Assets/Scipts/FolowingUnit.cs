using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class FolowingUnit : MonoBehaviour
{

    private PositionConstraint positionConstraint;
    
    public bool isFirstActive = true;
    //Açò per a alternar entre el primer i el segon constraint

    private Transform detectedTarget = null;

    private Vector3 offsetSource1 = new Vector3(-5.5f,-5.5f,-5.5f);
    
    //Offset altura base ESTIMADO
    public Vector3 offsetSource2 = new Vector3 (0,0.512f,0);
    //Offset altura 1 ESTIMADO
    //private Vector3 offsetSource2 = new Vector3 (0,1.512f,0);
    //private Vector3 offsetSource2 = new Vector3 (0,1.025f,0);
    
    //Offset altura 2 ESTIMADO
    //private Vector3 offsetSource2 = new Vector3 (0,2.045f,0);


    public  RaycastDebugger raycastDebugger;

    // Start is called before the first frame update
    void Start()
    {
        positionConstraint = GetComponent<PositionConstraint>();

        if(raycastDebugger == null){
            raycastDebugger = FindObjectOfType<RaycastDebugger>();
        }

        //REVISAR+++++++++++
        //Este if no es necesari pq ho estic tocant des de l'editor, pero per si de cas esta.
        //Si no fique mes elements a futur ELIMINAR if-+-+-+-+-+-+-+
        if(positionConstraint.sourceCount >=2){

            ConstraintSource source1 = positionConstraint.GetSource(0);
            source1.weight = 1;
            positionConstraint.SetSource(0, source1);

            

            positionConstraint.translationOffset = offsetSource1;



        }
        
        
    }

    // Update is called once per frame
    // void Update()
    // {

    //     detectedTarget = raycastDebugger.detectedHit;


    //     if(Input.GetKeyDown(KeyCode.Alpha1)){
    //         //Provat en aço DEMA-+-+-+-+-+-+-+
    //         isFirstActive = !isFirstActive;

    //         //  Guardamos la posición actual antes de cambiar
    //         Vector3 currentPosition = transform.position;

            

            

    //         ConstraintSource source1 = positionConstraint.GetSource(0);
    //         source1.weight = isFirstActive? 1 : 0;
    //         positionConstraint.SetSource(0, source1);

          


            
    //         if(!isFirstActive && detectedTarget != null && positionConstraint.sourceCount < 2){
    //             ConstraintSource source2 = new ConstraintSource{
    //                 sourceTransform = detectedTarget,
    //                 weight = 1f
    //             };
    //             positionConstraint.AddSource(source2);

    //             Collider targetCollider = detectedTarget.GetComponent<Collider>();

    //             if(targetCollider!= null){
    //                 float peakCollider = targetCollider.bounds.max.y;
    //                 positionConstraint.translationOffset = new Vector3(0, peakCollider + offsetSource2.y/*- detectedTarget.position.y*/, 0);

    //             }

                
                
    //         }
    //         else{
    //             if(positionConstraint.sourceCount > 1){

    //                 positionConstraint.RemoveSource(1);
    //             }

    //             positionConstraint.translationOffset = offsetSource1;
    //         }
            
            

            
    //          Debug.Log($"Following: {(isFirstActive ? "Source 1" : detectedTarget.name)}");
    //     }
    // }

    void Update()
    {



        // if(Input.GetKeyDown(KeyCode.Alpha1)){
        //     isFirstActive = !isFirstActive;;
        //     if(!isFirstActive){
        //     UpdateFollowerPosition();
        //     if (raycastDebugger != null)
        //     {
        //         Debug.Log($"[Raycast Emisor] Se está teniendo en cuenta el raycast lanzado por: {raycastDebugger.gameObject.name}");
        //         }
        //     }
        //     else{

        //         FollowerToParent();
        //     }
            
        // }
        // if (Input.GetKeyDown(KeyCode.Alpha3))
        //     {
        //         UpdateFollowerPosition();
        //     }

        
    }


    public void UpdateFollowerPosition() {
    

        //isFirstActive = !isFirstActive;
        detectedTarget = raycastDebugger.detectedHit;

        ConstraintSource source1 = positionConstraint.GetSource(0);
        // source1.weight = isFirstActive ? 1 : 0;
        source1.weight = 0;
        positionConstraint.SetSource(0, source1);
        
        if (positionConstraint.sourceCount > 1)
            {
                positionConstraint.RemoveSource(1);
            }

        if (/*!isFirstActive &&*/ detectedTarget != null && positionConstraint.sourceCount < 2)
        {
            ConstraintSource source2 = new ConstraintSource
            {
                sourceTransform = detectedTarget,
                weight = 1f
            };
            positionConstraint.AddSource(source2);

            Collider targetCollider = detectedTarget.GetComponent<Collider>();
            if (targetCollider != null)
            {
                float peakCollider = targetCollider.bounds.max.y;
                positionConstraint.translationOffset = new Vector3(0, peakCollider + offsetSource2.y, 0);
            }

        }//Debug.Log($"Following: {(isFirstActive ? "Source 1" : detectedTarget.name)}");
    }

     public void FollowerToParent(){

        {
            if (positionConstraint.sourceCount > 1)
            {
                positionConstraint.RemoveSource(1);
            }
            positionConstraint.translationOffset = offsetSource1;

            ConstraintSource source1 = positionConstraint.GetSource(0);
            source1.weight = isFirstActive ? 1 : 0;
            positionConstraint.SetSource(0, source1);
        }
        Debug.Log($"Following: {(isFirstActive ? "Source 1" : detectedTarget.name)}");
     }
    
}
