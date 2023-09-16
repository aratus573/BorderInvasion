using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public bool isActive;
        public GameObject prefab;
        public Transform parent;
        public int size;
    }

    #region Singleton

    public static ObjectPooler Instance;
    
    private void Awake() 
    {
        Instance = this;    
    }

    #endregion

    #region  Usage


    // Set
    // ObjectPooler yourObjectPoolerName;
    //
    // void Start()
    // {
    //      yourObjectPoolerName = ObjectPooler.Instance;
    // }
    //
    // YourFunction()
    // {
    //      yourObjectPoolerName.SpawnFromPool (string tag, Vector3 position, Quaternion rotation);
    // }

    #endregion

    public Dictionary<string, Queue<GameObject>> poolDictionary;


    [Space]
    public List<Pool> listOfPools;

    private void Start() 
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in listOfPools)
        {
            if (!pool.isActive)
            {
                return;
            }
            
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab, pool.parent, true);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool (string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag -" + tag + " doesn't exist.");
            return null;
        }
        GameObject objToSpawn = poolDictionary[tag].Dequeue();

        objToSpawn.SetActive(true);
        objToSpawn.transform.position = position;
        objToSpawn.transform.rotation = rotation;

        IPooledObject pooledObject = objToSpawn.GetComponent<IPooledObject>();

        if (pooledObject != null)
        {
            // call OnObjectSpawn() every time spawn from pool.
            // this is a bit like Awake()
            pooledObject.OnObjectSpawn();
        }

        poolDictionary[tag].Enqueue(objToSpawn);
        return objToSpawn;

    }



}
