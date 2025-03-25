using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class FolowingUnit : MonoBehaviour
{

    private PositionConstraint positionConstraint;
    
    private bool isFirstActive = true;
    //Açò per a alternar entre el primer i el segon constraint

    private Vector3 offsetSource1 = new Vector3(-5.5f,-5.5f,-5.5f);
    private Vector3 offsetSource2 = new Vector3 (0,1.066227912f,0);

    // Start is called before the first frame update
    void Start()
    {
        positionConstraint = GetComponent<PositionConstraint>();


        //REVISAR+++++++++++
        //Este if no es necesari pq ho estic tocant des de l'editor, pero per si decas esta.
        //Si no fique mes elements a futur ELIMINAR if-+-+-+-+-+-+-+
        if(positionConstraint.sourceCount >=2){

            ConstraintSource source1 = positionConstraint.GetSource(0);
            source1.weight = 1;
            positionConstraint.SetSource(0, source1);

            ConstraintSource source2 = positionConstraint.GetSource(1);
            source2.weight = 0;
            positionConstraint.SetSource(1, source2);

            positionConstraint.translationOffset = offsetSource1;



        }
        
        
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKeyDown(KeyCode.Alpha1)){

            isFirstActive = !isFirstActive;

            //  Guardamos la posición actual antes de cambiar
            Vector3 currentPosition = transform.position;

            //  Desactivamos temporalmente el constraint
            positionConstraint.constraintActive = false;

            

            ConstraintSource source1 = positionConstraint.GetSource(0);
            source1.weight = isFirstActive? 1 : 0;
            positionConstraint.SetSource(0, source1);

            ConstraintSource source2 = positionConstraint.GetSource(1);

            //si true 0-> si false 1
            source2.weight = isFirstActive? 0 : 1;
            positionConstraint.SetSource(1, source2);

            //Mantenemos la posición actual del objeto antes de reactivar el constraint
           // transform.position = currentPosition;

            // Vector3 newAtRest = currentPosition - (isFirstActive ? source1.sourceTransform.position : source2.sourceTransform.position);
            // positionConstraint.translationAtRest = newAtRest;

            // Transform activeSource = isFirstActive ? source1.sourceTransform : source2.sourceTransform;


            // Vector3 newAtRest = activeSource.InverseTransformPoint(currentPosition);
            // positionConstraint.translationAtRest = newAtRest;
            

            Vector3 newOffset = currentPosition - (isFirstActive ? source1.sourceTransform.position : source2.sourceTransform.position);
            positionConstraint.translationOffset = newOffset;

            //positionConstraint.translationOffset = isFirstActive?  offsetSource1 : offsetSource2;

            positionConstraint.constraintActive = true;
            
            if(source2.weight != 0){
                Debug.Log($"Following: {positionConstraint.GetSource(1)}");
            }
            else{
                Debug.Log($"Following:  {positionConstraint.GetSource(0)}");
            }
        }
    }
}
