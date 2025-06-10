using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RendererGroundDetector : MonoBehaviour
{

    public LayerRenderChanger renderChanger;
    // Start is called before the first frame update

    public string LayerRenderGround;

    public Transform currentCollision { get; private set; }

    string myLayer;
    void Start()
    {
        renderChanger = GetComponentInParent<LayerRenderChanger>();
        if (renderChanger == null)
        {
            Debug.LogError($"[{name}] No tiene LayerRendererChanger en el padre");
            return;
        }

        if (transform.parent == null)
        {
            Debug.LogError($"[RendererGroundDetector:{name}] No tiene padre, no puede leer la capa.");
        }
        int parentLayer = transform.parent.gameObject.layer;
        myLayer = LayerMask.LayerToName(parentLayer);
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(LayerRenderGround))
            currentCollision = other.transform;
            renderChanger.SetTouching(myLayer, true);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(LayerRenderGround))

             if (currentCollision == other.transform)
                currentCollision = null;
            renderChanger.SetTouching(myLayer, false);
    }
}
