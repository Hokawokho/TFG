using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ImmuneRaycast : MonoBehaviour
{


    public float rayDistance = 10f; // Distancia del Raycast
    public Vector3 customDirection = new Vector3(5.5f, 5f, 5.5f);

    // public LayerMask ignoreLayer;
    public LayerMask targetLayer;

    public Transform detectedHit { get; private set; } = null;

    public event Action<bool> OnHitStateChanged;
    private bool lastHitState = false;
    public bool IsCurrentlyHit => lastHitState;

    // Start is called before the first frame update
    private bool autoFire = true;

    // Extraemos el raycast en un método
    public void TriggerRay()
    {
        // Lanza el raycast una vez y lo dibuja con duración larga
        PerformRaycast(duration: 5f);
        autoFire = true;
    }

    public void StopRay()
    {
        autoFire = false;
        UpdateHit(false);
    }

    void Update()
    {
        if (autoFire)
        {
            // Mantener el dibujo del rayo cada frame (duración = Time.deltaTime)
            PerformRaycast(duration: Time.deltaTime);
        }
    }

    private void PerformRaycast(float duration)
    {
        Vector3 direction = customDirection.normalized;
        Vector3 startPosition = transform.position;
        bool hit = Physics.Raycast(startPosition, direction, out RaycastHit info, rayDistance, targetLayer);
        if (hit)
        {
            detectedHit = info.transform;
            Debug.DrawRay(startPosition, direction * info.distance, Color.green, duration);
        }
        else
        {
            detectedHit = null;
            Debug.DrawRay(startPosition, direction * rayDistance, Color.blue, duration);
        }
        UpdateHit(hit);
    }
     private void UpdateHit(bool hit)
    {
        if (hit != lastHitState)
        {
            lastHitState = hit;
            OnHitStateChanged?.Invoke(hit);
        }
    }
}