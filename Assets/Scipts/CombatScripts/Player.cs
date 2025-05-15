using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    //public float force;
    private Rigidbody rb;

    private UnitEntity lifetime;
    //private Animator anim;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        lifetime = GetComponent<UnitEntity>();
        //anim = GetComponent<Animator>();
    }
    void Update()
    {

    }

    private void OnEnable()
    {
        lifetime.OnHitEvent += OnHit;
        lifetime.OnDieEvent += OnDie;
    }
    private void OnDisable()
    {
        lifetime.OnHitEvent -= OnHit;
        lifetime.OnDieEvent -= OnDie;
    }
    private void OnDie()
    {
        Debug.Log("Game over");
        //CameraManager.Instance.ShakeCamera(5);
        //Destroy(gameObject);
        //anim.SetTrigger("Die");
    }
    private void OnHit(int damage)
    {
        Debug.Log("Player hit");
        //CameraManager.Instance.ShakeCamera(damage);
        //anim.SetTrigger("Hit");
    }
}