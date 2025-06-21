using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RendererGroundDetector : MonoBehaviour
{

    public LayerRenderChanger renderChanger;
    // Start is called before the first frame update

    public string LayerRenderGround;

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
        if (other.gameObject.layer == LayerMask.NameToLayer(LayerRenderGround))
        {
            currentCollision = other.transform;
            renderChanger.SetTouching(myLayer, true);
            // immuneRaycast.TriggerRay();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(LayerRenderGround))

            if (currentCollision == other.transform)
                currentCollision = null;
        renderChanger.SetTouching(myLayer, false);
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
