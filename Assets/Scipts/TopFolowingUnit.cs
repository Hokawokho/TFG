using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopFolowingUnit : MonoBehaviour
{

    public RaycastDebugger raycastDebugger; // Referencia al RaycastDebugger
    public float heightOffset = 0.5f; 

    // Start is called before the first frame update
    void Start()
    {   
        if(raycastDebugger == null){
        raycastDebugger = FindObjectOfType<RaycastDebugger>();
        }

    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.T))
        // {
        //     MoveToRaycastHit();
        // }
    }
 public IEnumerator  MoveToRaycastHit()
    {
        Transform hitTransform = raycastDebugger.detectedHit;
        if (hitTransform != null)
        {
            Vector3 hitPosition = hitTransform.position;

            // Ajustar posición vertical si el collider tiene altura
            Collider col = hitTransform.GetComponent<Collider>();
            if (col != null)
            {
                hitPosition.y = col.bounds.max.y + heightOffset;
            }

            transform.position = hitPosition;
            Debug.Log($"Unidad '{gameObject.name}' movida a posición: {hitPosition}");
        }
        else
        {
            Debug.LogWarning("No se ha detectado ningún impacto del raycast.");
        }
        yield return null;  // Esperar un frame para evitar que se ejecute el movimiento continuo
    }
}