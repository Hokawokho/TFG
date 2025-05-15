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
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    // void OnEnable()
    // {
    //     // Resetear la velocidad al salir del pool
    //     if (rb != null)
    //         rb.velocity = direction.normalized * speed;
    // }

    void Update()
    {
        rb.velocity = direction * speed;
    }
}
