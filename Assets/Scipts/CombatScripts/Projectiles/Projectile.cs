using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    public Vector3 direction;
    //TODO: modificar la direcció de Vector3 per a que apunte a la direcció q vulgam

    public float speed;
    private Rigidbody rb;

    // public float maxDistance = 1f; 
    private Vector3 startPosition;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        // 1) Cada vez que salga del pool, marcamos su punto de inicio
        startPosition = transform.position;
        // y forzamos la velocidad de salida
        // rb.velocity = direction * speed;
    }



    // void OnEnable()
    // {
    //     // Resetear la velocidad al salir del pool
    //     if (rb != null)
    //         rb.velocity = direction.normalized * speed;
    // }

    void Update()
    {
        // startPosition = transform.position;
        rb.velocity = direction * speed;
        // float traveled = Vector3.Distance(startPosition, transform.position);
        // if (traveled >= maxDistance)
        // {
        //     // Vuélvelo al pool desactivándolo
        //     gameObject.SetActive(false);
        // }
    }
}
