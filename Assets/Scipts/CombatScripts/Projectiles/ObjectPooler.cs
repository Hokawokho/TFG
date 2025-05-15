using System.Collections;
using System.Collections.Generic;
using UnityEngine;





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
        GameObject objectToSpawn = poolDictionary[tag].Dequeue();
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = orientation;
        AttackHitbox hitbox = objectToSpawn.GetComponent<AttackHitbox>();
        hitbox.owner = owner;

        if(hitbox == null)
            Debug.Log("No encuentra el collider del padre");
        

        poolDictionary[tag].Enqueue(objectToSpawn);
        return objectToSpawn;
    }
}

