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
    public HitPoints hitpoints;

    public int maxActions = 2;
    public int currentActions;

    public bool invulnerable = false;

    private Animator[] animators;

    private AudioManager audioManager;

    protected virtual void Start()
    {
        hitpoints = Instantiate(hitpoints);
        hitpoints.hitPoints = hitpoints.initialHitPoints;
        //ResetActions();

        //var root = selectedUnit.parent;
        Transform unitRoot = transform.parent;
        animators = unitRoot.GetComponentsInChildren<Animator>(true);

        audioManager = FindObjectOfType<AudioManager>();
        OnDieEvent += PlayDeathAudio;

    }

    public void Wake()
    {
        enabled = true;
        OnWakeEvent?.Invoke();
    }


    public bool IsAlive => hitpoints.hitPoints > 0;

    public bool HasActionsRemaining => currentActions > 0;


    public void TakeDamage(int damage)
    {
        if (!enabled)
            return;
        if (invulnerable)
            return;

        hitpoints.hitPoints -= damage;
        if (hitpoints.hitPoints <= 0)
        {
            hitpoints.hitPoints = 0;
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
        foreach (var anim in animators)
            anim.SetTrigger("Die");
        OnDieEvent?.Invoke();
        enabled = false;
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

    
    public int CurrentHealth => hitpoints.hitPoints;
}
