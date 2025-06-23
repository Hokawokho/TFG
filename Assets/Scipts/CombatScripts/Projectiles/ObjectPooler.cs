using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;





public class ObjectPooler : Singleton<ObjectPooler>
                            //Eso convierte a ObjectPooler en un singleton, accesible fácilmente desde cualquier parte del código con ObjectPooler.Instance.
{

    [System.Serializable]
//Sin [System.Serializable], no podría ver ni editar la lista pools desde el Editor de Unity. La clase existiría, pero sería completamente invisible para el Inspector.


    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    public GameObject lookAtTarget;

    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            poolDictionary.Add(pool.tag, objectPool);
        }
    }
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion orientation, GameObject owner)
    {
        if (!poolDictionary.ContainsKey(tag))
    {
        Debug.LogError($"[ObjectPooler] No existe pool con tag '{tag}'");
        return null;
    }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();
        objectToSpawn.SetActive(true);

        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = orientation;
        AttackHitbox hitbox = objectToSpawn.GetComponent<AttackHitbox>();
        
        hitbox.owner = owner;

        if(hitbox == null)
            Debug.Log("No encuentra el collider del padre");
        
        var look = objectToSpawn.GetComponent<LookAtConstraint>();
        if (look != null && lookAtTarget != null)
        {
            for (int i = look.sourceCount - 1; i >= 0; i--)
                look.RemoveSource(i);
            // Creamos el ConstraintSource apuntando a lookAtTarget
            ConstraintSource src = new ConstraintSource
            {
                sourceTransform = lookAtTarget.transform,
                
                weight = 1f

            };
            look.AddSource(src);
            look.constraintActive = true;
            look.enabled = true;
            Debug.Log("Constraint FLECHA activado ");


            var shooterComp = owner.GetComponentInChildren<ObjectShooter>();        
            if (shooterComp != null)                                               
            {                                                                      
                Vector3 d = shooterComp.currentDirection;                           
                float zOffset = 0f;                                                 
                if      (d == Vector3.left)    zOffset = 145.23f;                   
                else if (d == Vector3.forward) zOffset =  -145.23f;                   
                else if (d == Vector3.right)   zOffset = -32.13f;                   
                else if (d == Vector3.back)    zOffset = 32.13f;                  

                look.rotationOffset = new Vector3(0f, 0f, zOffset);                 
            }
        }

        poolDictionary[tag].Enqueue(objectToSpawn);
        return objectToSpawn;
    }
}

