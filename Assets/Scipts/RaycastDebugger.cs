using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RaycastDebugger : MonoBehaviour
{

    public float rayDistance = 10f; // Distancia del Raycast
    public Vector3 customDirection = new Vector3(-5.5f, -5.5f, -5.5f);

    // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    // Update is called once per frame
    void Update()
    {

        Vector3 direction = customDirection.normalized;
        Vector3 startPosition = transform.position;

        if(Physics.Raycast(startPosition, direction, out RaycastHit hit, rayDistance)){

            Debug.LogWarning($"RAYO IMPACTÓ a {hit.collider.name} en {hit.point}");
            Debug.DrawRay(startPosition, direction * hit.distance, Color.red, 3f);

        }

        else{

            Debug.Log($" RAYO NO IMPACTÓ a nada. Dirección: {direction}");
            Debug.DrawRay(startPosition, direction * rayDistance, Color.blue, 3f);
        }
    }
}
