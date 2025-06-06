using System.Collections;
using System.Collections.Generic;
using UnityEngine;



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

        // 5.2) Si ninguno toca, forzamos activar “H0”
        // if (toActivate == null)
        // {
        //     // Buscamos en el array la clave “H0”, o por convención el último elemento
        //     for (int i = 0; i < renderers.Length; i++)
        //     {
        //         if (renderers[i].layerName == "H0")
        //         {
        //             toActivate = renderers[i].meshRenderer;
        //             break;
        //         }
        //     }
        //     // Si no tuvieras clave "H0", podrías asumir que está en renderers[renderers.Length-1]
        //     // toActivate = renderers[renderers.Length - 1].meshRenderer;
        // }

        // 5.3) Si el que debería estar activo ya está activo, no hacemos nada


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
    // IEnumerator DisableAfterDelay(MeshRenderer oldRenderer, float delay)
    // {
    //     yield return new WaitForSeconds(delay);

    //     // Sólo lo apagamos si ya no es el active (por si en ese 0.1s se cambió nuevamente)
    //     if (oldRenderer != currentActive)
    //     {
    //         oldRenderer.enabled = false;
    //     }

    //     delayedDisableCoroutine = null;
    // }

}

