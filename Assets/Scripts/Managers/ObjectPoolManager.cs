using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

public class ObjectPoolManager : MonoBehaviour
{
    public enum PoolType
    {
        None,
        Cable,
        OxygenContainer,
    }


    static Dictionary<PoolType, ObjectPool<GameObject>> objectPools;
    static Dictionary<PoolType, GameObject> parentObjects;

    void Awake()
    {
        objectPools = new Dictionary<PoolType, ObjectPool<GameObject>>();
        parentObjects = new Dictionary<PoolType, GameObject>();
        SetupEmpties();
    }

    void SetupEmpties()
    {
        for (int i = 0; i < Enum.GetValues(typeof(PoolType)).Length; i++)
        {
            PoolType poolType = (PoolType)i;
            GameObject emptyParent = new(poolType + "Pool");
            emptyParent.transform.SetParent(transform);
            parentObjects[poolType] = emptyParent;
        }
    }

    static void CreatePool(GameObject prefab, Vector3 pos, Quaternion rot, PoolType poolType)
    {
        ObjectPool<GameObject> pool = new(
            () => CreateObject(prefab, pos, rot, poolType),
            OnGetObject,
            OnReleaseObject,
            OnDestroyObject,
            false,
            0,
            50
        );

        objectPools[poolType] = pool;
    }

    static GameObject CreateObject(GameObject prefab, Vector3 pos, Quaternion rot, PoolType poolType)
    {
        if (prefab.activeSelf)
        {
            prefab.SetActive(false);
        }

        GameObject obj = Instantiate(prefab, pos, rot);
        obj.name = $"{prefab.name}_Pooled_{poolType}";
        if (obj.activeSelf)
        {
            obj.SetActive(false);
        }

        obj.transform.SetParent(parentObjects[poolType].transform); // Set parent for gameobject
        return obj;
    }

    static void OnGetObject(GameObject obj)
    {
    }

    static void OnReleaseObject(GameObject obj)
    {
        if (obj.activeSelf)
        {
            obj.SetActive(false);
        }
    }

    static void OnDestroyObject(GameObject obj)
    {
    }

    static T SpawnObject<T>(GameObject prefab, Vector3 pos, Quaternion rot, PoolType poolType)
        where T : Object
    {
        if (!objectPools.ContainsKey(poolType))
        {
            CreatePool(prefab, pos, rot, poolType);
        }

        ObjectPool<GameObject> pool = objectPools[poolType];
        GameObject obj = pool.Get();
        if (obj == null)
        {
            return null;
        }

        obj.transform.position = pos;
        obj.transform.rotation = rot;
        obj.SetActive(true);
        if (typeof(T) == typeof(GameObject))
        {
            return obj as T;
        }

        T component = obj.GetComponent<T>();
        if (component != null)
        {
            return component;
        }

        Debug.LogError(
            $"Object of type {typeof(T)} not found on {obj.name}. Ensure the prefab has the required component.");
        return null;
    }

    public static T SpawnObject<T>(T typePrefab, Vector3 spawnPos, Quaternion spawnRotation,
        PoolType poolType = PoolType.None) where T : Component
    {
        return SpawnObject<T>(typePrefab.gameObject, spawnPos, spawnRotation, poolType);
    }

    public static GameObject SpawnObject(GameObject objectToSpawn, Vector3 spawnPos, Quaternion spawnRotation,
        PoolType poolType = PoolType.None)
    {
        return SpawnObject<GameObject>(objectToSpawn, spawnPos, spawnRotation, poolType);
    }

    public static void ReturnObjectToPool(GameObject obj, PoolType poolType)
    {
        if (objectPools.TryGetValue(poolType, out ObjectPool<GameObject> pool))
        {
            if (pool != null)
            {
                pool.Release(obj);
                return;
            }
        }

        Debug.LogWarning($"No pool found for type {poolType}. Object not returned to pool.");
    }
}