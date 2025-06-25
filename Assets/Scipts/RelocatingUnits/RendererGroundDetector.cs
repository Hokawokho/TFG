using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RendererGroundDetector : MonoBehaviour
{

    public LayerRenderChanger renderChanger;
    // Start is called before the first frame update

    public string layerRenderGround;

    public string visualLayerMask = "Ground";

    // public ImmuneRaycast immuneRaycast;

    public Transform currentCollision { get; private set; }

    string myLayer;
    void Start()
    {
        renderChanger = GetComponentInParent<LayerRenderChanger>();
        // immuneRaycast = GetComponentInParent<ImmuneRaycast>();
        if (renderChanger == null)
        {
            Debug.LogError($"[{name}] No tiene LayerRendererChanger en el padre");
            return;
        }

        // if (immuneRaycast == null)
        // {
        //     Debug.LogError($"[{name}] No tiene ImmuneRaycast en el padre immediato del mesh");
        //     return;
        // }

        // // Suscribirse al evento de cambio de estado de hit del raycast
        // immuneRaycast.OnHitStateChanged += OnImmuneRaycastHitStateChanged;


        // if (transform.parent == null)
        // {
        //     Debug.LogError($"[RendererGroundDetector:{name}] No tiene padre, no puede leer la capa.");
        // }
        int myLayerIndex = gameObject.layer;
        myLayer = LayerMask.LayerToName(myLayerIndex);
    }

    void OnTriggerStay(Collider other)
    {
        int otherLayer = other.gameObject.layer;
        if (otherLayer == LayerMask.NameToLayer(layerRenderGround))
        {
            currentCollision = other.transform;
            renderChanger.SetTouching(myLayer, true);
//            Debug.Log($"[RendererGroundDetector] Principal enter '{myLayer}' con '{LayerMask.LayerToName(otherLayer)}'");
            // immuneRaycast.TriggerRay();
        }
        if (otherLayer == LayerMask.NameToLayer(visualLayerMask))
         {
            renderChanger.SetSecondaryTouching(myLayer, true);
//            Debug.Log($"[RendererGroundDetector] Secundaria enter '{myLayer}' con '{visualLayerMask}'");
         }
    }

    void OnTriggerExit(Collider other)
    {
        int otherLayer = other.gameObject.layer;
        if (otherLayer == LayerMask.NameToLayer(layerRenderGround))

            if (currentCollision == other.transform)
                currentCollision = null;
        renderChanger.SetTouching(myLayer, false);

        if (otherLayer == LayerMask.NameToLayer(visualLayerMask))
         {
            renderChanger.SetSecondaryTouching(myLayer, false);
            Debug.Log($"[RendererGroundDetector] Secundaria exit '{myLayer}'");
         }
    }
    
    //  // Callback para activar o desactivar invMesh según el estado del raycast
    // private void OnImmuneRaycastHitStateChanged(bool hit)
    // {
    //     if (renderChanger.invMesh != null)
    //     {
    //         renderChanger.invMesh.enabled = hit;
    //     }
    // }
}
