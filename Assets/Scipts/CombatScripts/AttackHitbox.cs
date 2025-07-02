using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public int damage;
    public string damageOnlyToTag;

    public GameObject owner;

    public float lifetime;
    float _spawnTime;


    void OnEnable()
    {
        _spawnTime = Time.time;
    }


    void OnTriggerEnter(Collider other)
    {

        if (other.transform.root.gameObject == owner)
            return;


        if (damageOnlyToTag == "" || other.CompareTag(damageOnlyToTag))
        {
            UnitEntity hit = other.gameObject.GetComponentInParent<UnitEntity>();
            if (hit != null)
            {
                hit.TakeDamage(damage);
                Debug.Log($"Has golpeado a {hit.name}. Vida restante: {hit.CurrentHealth}");
            }
            Die();
        }
    }
    private void Update()
    {
        // Vector pos = Camera.main.WorldToViewportPoint(transform.position);
        // if (pos.x < -0.1 || pos.x > 1.1)
        //     Die();

        if (Time.time > _spawnTime + lifetime)
            Die();

    }
    private void Die()
    {
        gameObject.SetActive(false);
    }
}
