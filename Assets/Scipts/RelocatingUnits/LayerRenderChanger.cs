using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;



public class LayerRenderChanger : MonoBehaviour
{

    [System.Serializable]
    public class RenderInfo
    {

        public string layerName;

        public MeshRenderer meshRenderer;

        public bool isTouching;

    }

    public RenderInfo[] renderers;

    MeshRenderer currentActive = null;

    Coroutine delayedDisableCoroutine = null;

    bool ignoreCollisions = false;

    private RaycastDebugger previousDebugger = null;
    private FolowingUnit previousFollower = null;
    private MeshRenderer prevMesh;

    public MeshRenderer invMesh;
    

    //Esto es para mantener el renderer de la unidad inferior tras rotar
    //MeshRenderer stickyRenderer = null;

    //private Rotation rotationScript;

    public void SuspendCollisions()
    {
        ignoreCollisions = true;
    }

    public void ResumeCollisions()
    {
        //La crida al métode estaba en Rotation, ara en UnitController

        ignoreCollisions = false;
        // forzamos un recálculo inmediato al reanudar, 
        // porque puede que durante la rotación hubiera un "exit" pendiente o simplemente 
        // para asegurarnos de que el estado quede correcto
        // UpdateActiveRenderer();
    }


    void Start()
    {
        //rotationScript = FindObjectOfType<Rotation>();
        foreach (var unit in renderers)
        {
            if (unit.meshRenderer != null)
                unit.meshRenderer.enabled = false;

            unit.isTouching = false;
        }

        // ActivateDefaultRenderer();

        // if (renderers.Length > 0)
        // {
        //     RenderInfo fallback = renderers[renderers.Length - 1];
        //     if (fallback.meshRenderer != null)
        //     {
        //         fallback.meshRenderer.enabled = true;
        //         currentActive = fallback.meshRenderer;
        //     }
        // }

    }

    public void SetTouching(string layerName, bool isEntering)
    {
        // if (rotationScript != null && rotationScript.isRotating)
        //     return;
        if (ignoreCollisions)
            return;

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].layerName == layerName)
            {
                renderers[i].isTouching = isEntering;
                break;
            }
        }

        // Recalculamos qué renderer debería estar activo
        // 
        // 2) Si alguien entra, cancelamos cualquier retraso y recalculamos YA
        if (isEntering)
        {
            if (delayedDisableCoroutine != null)
            {
                StopCoroutine(delayedDisableCoroutine);
                delayedDisableCoroutine = null;
            }
            UpdateActiveRenderer();
        }

        else
        {
            if (delayedDisableCoroutine != null)
                StopCoroutine(delayedDisableCoroutine);

            delayedDisableCoroutine = StartCoroutine(DelayedUpdateAfterExit(0.2f));
        }
    }

    IEnumerator DelayedUpdateAfterExit(float delay)
    {
        yield return new WaitForSeconds(delay);
        delayedDisableCoroutine = null;
        UpdateActiveRenderer();
    }


    void UpdateActiveRenderer()
    {
        if (ignoreCollisions)
            return;

        // if (rotationScript != null && rotationScript.isRotating)
        //     return;

        // 5.1) Buscamos, en orden de prioridad, el primer isTouching=true
        MeshRenderer toActivate = null;

        foreach (var unit in renderers)
        {
            if (unit.isTouching)
            {
                toActivate = unit.meshRenderer;
                break;
            }
        }
        if (toActivate == null && renderers.Length > 0)
        {
            toActivate = renderers[renderers.Length - 1].meshRenderer;
        }

        if (toActivate == currentActive)
            return;

        // 5.4) Activamos inmediatamente el nuevo
        if (toActivate != null)
            toActivate.enabled = true;

        // 5.5) Desactivamos el anterior, pero con un retardo de 0.1s
        if (currentActive != null)
        {
            // Si había una corrutina anterior pendiente, la detenemos
            // if (delayedDisableCoroutine != null)
            // {
            //     StopCoroutine(delayedDisableCoroutine);
            //     delayedDisableCoroutine = null;
            // }
            currentActive.enabled = false;

            // Lanzamos la corrutina para desactivar el anterior tras 0.1s
            // delayedDisableCoroutine = StartCoroutine(DisableAfterDelay(currentActive, 0.1f));
        }


        currentActive = toActivate;
    }

    public MeshRenderer GetCurrentActiveRenderer()
    {
        return currentActive;
    }


    public void SetUpScripts_and_RaycastTop()
    {

        var mesh = GetCurrentActiveRenderer();
        if (mesh == null) return;

        // Only update if mesh changed since last time
        // if (prevMesh != mesh)
        // {
            prevMesh = mesh;
            var dbg = mesh.GetComponentInParent<RaycastDebugger>();
            var fol = mesh.GetComponentInParent<FolowingUnit>();
            Debug.Log($"[Rotation] Enabling debugger '{dbg?.gameObject.name ?? "None"}' and follower '{fol?.gameObject.name ?? "None"}'");
            RelocatingUnitConstraints(dbg, fol);

            var top = GetComponentInChildren<TopFolowingUnit>();
            if (top != null)
            {
                top.raycastDebugger = dbg;
            }
            
       // }
    }
    private IEnumerator ApplyConstraint(FolowingUnit fol)
    {
        //Debug.Log("[Rotation] Waiting for next FixedUpdate to apply constraints.");
        // Esperar al siguiente paso de física para asegurar colisión
        yield return new WaitForFixedUpdate();

        if (fol.enabled)
        {
            fol.UpdateFollowerPosition();

        }
    }

    private void RelocatingUnitConstraints( RaycastDebugger selectedDebugger,  FolowingUnit selectedFollower)
    {
        Debug.Log($"[Rotation] RelocatingUnitConstraints started with debugger '{selectedDebugger?.gameObject.name ?? "None"}' and follower '{selectedFollower?.gameObject.name ?? "None"}'");

        // 2. DESACTIVAR EL VIEJO Y ACTIVAR EL NUEVO
        // ——— Debugger ———
        if (previousDebugger != null)
            previousDebugger.enabled = false;
        if (selectedDebugger != null)
            selectedDebugger.enabled = true;
        previousDebugger = selectedDebugger;
        // desactivar todos los debuggers de esta unidad y activar sólo el suyo
        // ——— Follower ———
        if (previousFollower != null)
            previousFollower.enabled = false;
        if (selectedFollower != null)
            selectedFollower.enabled = true;
        previousFollower = selectedFollower;

        //Es fa ací que es quan es deselecciona el script
        StartCoroutine(ApplyConstraint(selectedFollower));

        if (selectedFollower != null)
        {

            FolowingUnit[] allFollowers = GetComponentsInChildren<FolowingUnit>();
            foreach (var follower in allFollowers)
            {
                if (follower == selectedFollower)
                    continue;

                var constraint = follower.GetComponentInParent<PositionConstraint>();
                if (constraint == null)
                {
                    Debug.LogWarning($"[Rotation] No PositionConstraint found on '{follower.gameObject.name}'");
                    continue;
                }

                //Debug.Log($"[Rotation] Updating PositionConstraint for follower '{follower.gameObject.name}'");

                Vector3 worldPos = follower.transform.position;
                //int oldCount = constraint.sourceCount;
                for (int i = constraint.sourceCount - 1; i >= 0; i--)
                    constraint.RemoveSource(i);
                /// Debug.Log($"[Rotation] Cleared {oldCount} sources for follower '{follower.gameObject.name}'");

                var newSource = new ConstraintSource
                {
                    sourceTransform = selectedFollower.transform,
                    weight = 1f
                };
                constraint.AddSource(newSource);
                constraint.translationOffset = worldPos - selectedFollower.transform.position;
                constraint.constraintActive = true;
                Debug.Log($"[Rotation] Constraint on '{follower.gameObject.name}' now follows '{selectedFollower.gameObject.name}'");
                //Debug.Log($"[Rotation] Added new source '{selectedFollower.gameObject.name}' to constraint on '{follower.gameObject.name}' with offset {constraint.translationOffset}");

            }
        }
        else
        {
            Debug.LogWarning("[Rotation] No selectedFollower provided to update constraints.");
        }
    }
}

