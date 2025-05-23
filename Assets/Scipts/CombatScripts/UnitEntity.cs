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

    protected virtual void Start()
    {
        hitpoints = Instantiate(hitpoints);
        hitpoints.hitPoints = hitpoints.initialHitPoints;
    }

    public void Wake()
    {
        enabled = true;
        OnWakeEvent?.Invoke();
    }
    public void TakeDamage(int damage)
    {
        if (!enabled) return;
        hitpoints.hitPoints -= damage;
         if (hitpoints.hitPoints <= 0)
        {
            hitpoints.hitPoints = 0;
            Die();
        }
        else
        {
            OnHitEvent?.Invoke(damage);
        }
    }
    public void Die()
    {
        OnDieEvent?.Invoke();
        enabled = false;
    }
}
