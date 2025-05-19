using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectShooter : MonoBehaviour
{
    
    public string poolTag;
    public float creationRate = .5f;
    public KeyCode keyToPress;

    private float timeOfLastSpawn;

    public Vector3 currentDirection = new Vector3(0f, 0f, 1f);
    
    void Start()
    {
        timeOfLastSpawn = -creationRate;
    }
    void Update()
    {


        ShotDirection();



        if (Input.GetKeyDown(keyToPress) &&
           Time.time >= timeOfLastSpawn + creationRate)
        {
            GameObject projectile = ObjectPooler.Instance.SpawnFromPool(poolTag, transform.position, Quaternion.identity, transform.root.gameObject);



            //Esto es para pasar el owner al disparo y que no se detecte a si mismo
            AttackHitbox hitbox = projectile.GetComponent<AttackHitbox>();
            if (hitbox != null)
            {
                // Asigna como owner el root del shooter (tu personaje)
                hitbox.owner = transform.root.gameObject;
            }

            //Esto es para setear la dirección en la cual se disparará
            Projectile proj = projectile.GetComponent<Projectile>();
            if (proj != null)
            {
                proj.direction = currentDirection.normalized;   // Asignar dirección seleccionada
            }


            timeOfLastSpawn = Time.time;
        }
    }


    void ShotDirection(){

        // Cambiar dirección con flechas
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentDirection = new Vector3(0, 0, -1);
            Debug.Log("Dirección de disparo: " + currentDirection);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentDirection = new Vector3(0, 0, 1);
            Debug.Log("Dirección de disparo: " + currentDirection);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentDirection = new Vector3(1, 0, 0);
            Debug.Log("Dirección de disparo: " + currentDirection);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentDirection = new Vector3(-1, 0, 0);
            Debug.Log("Dirección de disparo: " + currentDirection);
        }

        // Mostrar dirección actual al pulsar C
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("La dirección seleccionada para el próximo disparo es: " + currentDirection);
        }
    }
}