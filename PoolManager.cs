using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// OBJECT POOL - Ör.; 10'luk array yaratýyoruz. 
/// Bu arrayin içinden enable disable yaparak obje seçiyoruz. Böylelikle create-destroy yapmak gerekmiyor.
/// CPU'ya iþ düþmüyor.
/// </summary>
[DisallowMultipleComponent]
public class PoolManager : SingletonMonoBehaviour<PoolManager>
{
    [Tooltip("Prefabs to be added to the pool")]
    [SerializeField] private Pool[] pools;

    private Transform objectPoolTransform;
    private Dictionary<int, Queue<Component>> poolDictionary = new();
    
    [Serializable]
    public struct Pool
    {
        public int poolSize;
        public GameObject prefab;
        public string componentType;
    }

    private void Start()
    {
        objectPoolTransform = gameObject.transform;

        for (int i = 0; i < pools.Length; i++)
        {
            CreatePool(pools[i].prefab, pools[i].poolSize, pools[i].componentType);
        }
    }

    private void CreatePool(GameObject prefab, int poolSize, string componentType)
    {
        int poolKey = prefab.GetInstanceID();

        string prefabName = prefab.name;

        GameObject parentGameObject = new(prefabName + "Anchor");

        parentGameObject.transform.SetParent(objectPoolTransform);

        if (!poolDictionary.ContainsKey(poolKey))
        {
            poolDictionary.Add(poolKey, new Queue<Component>());
            for (int i = 0; i < poolSize; i++)
            {
                GameObject newObject = Instantiate(prefab, parentGameObject.transform);
                newObject.SetActive(false);
                poolDictionary[poolKey].Enqueue(newObject.GetComponent(Type.GetType(componentType)));
            }
        }
    }
    /// <summary>
    /// Reuse a gameobject component in the pool.
    /// </summary>
    public Component ReuseComponent(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        int poolKey = prefab.GetInstanceID();

        if (poolDictionary.ContainsKey(poolKey))
        {
            Component componentToReuse = GetComponentFromPool(poolKey);
            ResetObject(position, rotation, componentToReuse, prefab);
            return componentToReuse;
        }
        else
        {
            Debug.Log($"No object pool for {prefab}");
            return null;
        }
    }
    /// <summary>
    /// Get a gameobject component from the pool using the poolKey
    /// </summary>
    private Component GetComponentFromPool(int poolKey)
    {
        Component componentToReuse = poolDictionary[poolKey].Dequeue();
        poolDictionary[poolKey].Enqueue(componentToReuse);
        if (componentToReuse.gameObject.activeSelf)
        {
            componentToReuse.gameObject.SetActive(false);
        }
        return componentToReuse;
    }
    /// <summary>
    /// Reset the gameObject
    /// </summary>
    private void ResetObject(Vector3 position, Quaternion rotation, Component componentToReuse, GameObject prefab)
    {
        componentToReuse.transform.SetPositionAndRotation(position, rotation);
        componentToReuse.gameObject.transform.localScale = prefab.transform.localScale;
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(pools), pools);
    }
#endif
    #endregion
}
