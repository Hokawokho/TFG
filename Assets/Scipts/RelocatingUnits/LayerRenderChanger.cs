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
        public SpriteRenderer spriteRenderer;

        public bool isTouching;

        public bool visualTouching;

    }

    public RenderInfo[] renderers;

    // MeshRenderer currentActive = null;
    SpriteRenderer currentActive = null;

    Coroutine delayedDisableCoroutine = null;

    bool ignoreCollisions = false;

    private RaycastDebugger previousDebugger = null;
    private FolowingUnit previousFollower = null;
    // private MeshRenderer prevMesh;
    private SpriteRenderer prevMesh;

    private ImmuneRaycast currentImmune = null;
    // public MeshRenderer invMesh;
    public SpriteRenderer invMesh;

    public UnitEntity unitEntity;

    private UnitController unitController;


    public void SuspendCollisions()
    {
        ignoreCollisions = true;
    }

    public void ResumeCollisions()
    {
        //La crida al métode estaba en Rotation, ara en UnitController

        ignoreCollisions = false;

        UpdateActiveRenderer();
    }


    void Start()
    {
        //rotationScript = FindObjectOfType<Rotation>();
        foreach (var unit in renderers)
        {
            if (unit.spriteRenderer != null)
                unit.spriteRenderer.enabled = false;

            unit.isTouching = false;
        }

        unitController = FindObjectOfType<UnitController>();

    }

    void Update()
    {
        // Solo para la unidad seleccionada: asegurar que al menos el sprite principal esté activo
        if (unitController != null && unitController.unitSelected)
        {
            var selEntity = unitController.selectedUnit?.GetComponent<UnitEntity>();
            if (selEntity == unitEntity)
            {
                bool anyActive = false;
                foreach (var info in renderers)
                {
                    if (info.spriteRenderer != null && info.spriteRenderer.enabled)
                    {
                        anyActive = true;
                        break;
                    }
                }
                if (!anyActive)
                {
                    // Reactivar el primero con isTouching según prioridad
                    foreach (var info in renderers)
                    {
                        if (info.isTouching)
                        {
                            info.spriteRenderer.enabled = true;
                            Debug.Log($"[LayerRenderChanger] Update: reactived '{info.layerName}' por isTouching");
                            break;
                        }
                    }
                }
            }
        }
        //  // Mientras la unidad se mueve, garantizar al menos un sprite activo (original)
        // if (unitController != null && unitController.isMoving)
        // {
        //     bool anyActive = false;
        //     foreach (var info in renderers)
        //     {
        //         if (info.spriteRenderer != null && info.spriteRenderer.enabled)
        //         {
        //             anyActive = true;
        //             break;
        //         }
        //     }
        //     if (!anyActive)
        //     {
        //         Debug.Log("[LayerRenderChanger] Update: no hay sprites activos, buscando colisión actual");
        //         foreach (var det in GetComponentsInChildren<RendererGroundDetector>())
        //         {
        //             if (det.currentCollision != null)
        //             {
        //                 SetTouching(det.layerRenderGround, true);
        //                 Debug.Log($"[LayerRenderChanger] Update: activado layer '{det.layerRenderGround}' por movimiento");
        //                 break;
        //             }
        //         }
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

            // ── PATCH: asegurar que el sprite se activa aunque ya fuera currentActive ──
            //ESTO ES PARA CUANDO PASA DE VisualTouching a isTouching en el mismo renderer
            // for (int j = 0; j < renderers.Length; j++)
            // {
            //     if (renderers[j].layerName == layerName)
            //     {
            //         renderers[j].spriteRenderer.enabled = true;
            //         Debug.Log($"[LayerRenderChanger] Patch SetTouching: forcé enable de '{layerName}'");
            //         break;
            //     }
            // }

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
        // MeshRenderer toActivate = null;
        SpriteRenderer toActivate = null;

        foreach (var unit in renderers)
        {
            if (unit.isTouching)
            {
                toActivate = unit.spriteRenderer;
                break;
            }
        }
        if (toActivate == null && renderers.Length > 0)
        {
            toActivate = renderers[renderers.Length - 1].spriteRenderer;
        }

        if (toActivate == currentActive)
            return;

        // 5.4) Activamos inmediatamente el nuevo
        if (toActivate != null)
        {
            toActivate.enabled = true;
            var immune = toActivate.transform.parent.GetComponent<ImmuneRaycast>();
            if (immune != null){
                 if (currentImmune != null)
                    currentImmune.OnHitStateChanged -= HandleImmuneHitStateChanged;

                immune.enabled = true;
                immune.OnHitStateChanged += HandleImmuneHitStateChanged;
                currentImmune = immune;
            }
                
        }

        // 5.5) Desactivamos el anterior, pero con un retardo de 0.1s
        if (currentActive != null)
        {

            currentActive.enabled = false;
                var oldImmune = currentActive.transform.parent.GetComponent<ImmuneRaycast>();
            if (oldImmune != null)
            {
                
                oldImmune.OnHitStateChanged -= HandleImmuneHitStateChanged;
                oldImmune.enabled = false;
            }
                

                // Lanzamos la corrutina para desactivar el anterior tras 0.1s
            // delayedDisableCoroutine = StartCoroutine(DisableAfterDelay(currentActive, 0.1f));
        }


        currentActive = toActivate;
    }

        public void SetSecondaryTouching(string layerName, bool isEntering)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i].layerName == layerName)
                {
                    renderers[i].visualTouching = isEntering;
                    // Habilita si primaria o visual está activa
                    bool shouldEnable = renderers[i].isTouching || renderers[i].visualTouching;
                    renderers[i].spriteRenderer.enabled = shouldEnable;
//                    Debug.Log($"[LayerRenderChanger] Secondary '{layerName}' = {isEntering}, sprite.enabled = {shouldEnable}");
                    break;
                }
            }
        }

    private void HandleImmuneHitStateChanged(bool hit)
    {
        if (hit)
        {
            if (invMesh != null) invMesh.enabled = true;
            if (unitEntity != null) unitEntity.invulnerable = true;
        }
        else
        {
            if (invMesh != null) invMesh.enabled = false;
            if (unitEntity != null) unitEntity.invulnerable = false;
        }
    }

    // public MeshRenderer GetCurrentActiveRenderer()
    public SpriteRenderer GetCurrentActiveRenderer()
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

            }
        }
        else
        {
            Debug.LogWarning("[Rotation] No selectedFollower provided to update constraints.");
        }
    }
}

