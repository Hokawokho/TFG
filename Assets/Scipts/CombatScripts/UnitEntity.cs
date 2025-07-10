using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitEntity : MonoBehaviour
{
    public delegate void Notify();
    public event Notify OnWakeEvent;
    public event Notify OnDieEvent;

    public delegate void NotifyDamage(int damage);
    public event NotifyDamage OnHitEvent;
    // public HitPoints hitpoints;

    public UnitType unitType;
    public int currentHealth { get; private set; }
    public int currentMovement { get; private set; }
    public string attackType { get; private set; }


    public int maxActions = 2;
    public int currentActions;

    public bool invulnerable = false;

    private Animator[] animators;

    private AudioManager audioManager;

    protected virtual void Start()
    {
        // hitpoints = Instantiate(hitpoints);
        // hitpoints.hitPoints = hitpoints.initialHitPoints;

        currentHealth   = unitType.initialHitPoints;
        // Movimiento
        currentMovement = unitType.movement;
        // Tipo de ataque ("melee" o "range")
        attackType      = unitType.attackType.ToLower();
        ResetActions();

        //var root = selectedUnit.parent;
        Transform unitRoot = transform.parent;
        animators = unitRoot.GetComponentsInChildren<Animator>(true);

        audioManager = FindObjectOfType<AudioManager>();
        OnDieEvent += PlayDeathAudio;

    }

    public void ApplyUnitType(UnitType newType)
    {
        unitType = newType;

        currentHealth = newType.initialHitPoints;
        currentMovement = newType.movement;
        attackType = newType.attackType.ToLower();

        // Si quieres resetear acciones al cambiar de tipo
        ResetActions();
    }


    public void Wake()
    {
        enabled = true;
        OnWakeEvent?.Invoke();
    }


    public bool IsAlive => currentHealth > 0;

    public bool HasActionsRemaining => currentActions > 0;

    public bool TryHeal(int amount)
    {
        int maxHP = unitType.initialHitPoints;
        if (currentHealth >= maxHP)
            return false;
        // ya está al máximo

        currentHealth = Mathf.Min(currentHealth + amount, maxHP);
        return true;
    }



    public void TakeDamage(int damage)
    {
        if (!enabled || invulnerable)
            return;

        // hitpoints.hitPoints -= damage;
        // if (hitpoints.hitPoints <= 0)
        // {
        //     hitpoints.hitPoints = 0;
        //     Die();
        // }

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        else
        {
            foreach (var anim in animators)
                anim.SetTrigger("Hit");
            OnHitEvent?.Invoke(damage);
        }
    }
    public void Die()
    {
        // Lanza la corrutina que maneja la animación y la desactivación
        StartCoroutine(DieRoutine());
    }

    private IEnumerator DieRoutine()
    {
        // 1) Disparar animación y evento
        foreach (var anim in animators)
            anim.SetTrigger("Die");
        OnDieEvent?.Invoke();

        // 2) Esperar a que el Animator entre en estado "Die"
        //    Necesitamos un frame para que el trigger haga efecto
        yield return null;

        // 3) Calcular la duración real del clip de muerte
        float longest = 0f;
        foreach (var anim in animators)
        {
            var clips = anim.GetCurrentAnimatorClipInfo(0);
            if (clips.Length > 0)
                longest = Mathf.Max(longest, clips[0].clip.length);
        }

        // 4) Esperar clip + extra 0.2s
        yield return new WaitForSeconds(longest + 0.2f);

        // 5) Desactivar todo el GO padre (o este si no hay padre)
        var parentTransform = transform.parent;
        if (parentTransform != null)
            parentTransform.gameObject.SetActive(false);
        else
            gameObject.SetActive(false);
    }


    public void ResetActions()
    {

        currentActions = maxActions;

    }

    public void UseAction()
    {
        if (currentActions <= 0) return;
        currentActions--;


    }

    private void PlayDeathAudio()
    {

        if (audioManager != null && audioManager.unitDeathAudio != null)
        {
            Debug.LogWarning("AUDIO MUERTE UNIDAD");
            audioManager.PlayOneShot(audioManager.unitDeathAudio);
        }
        else
        {
            Debug.LogWarning("AudioManager o unitDeathAudio no asignado.");
        }
    }

    
    public int CurrentHealth => currentHealth;
}
