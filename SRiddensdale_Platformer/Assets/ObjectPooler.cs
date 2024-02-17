using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This class was part of a tutorial by Sasquatch B Studios. Thank you for your wonderful tutorial!
/// https://www.youtube.com/watch?v=9O7uqbEe-xc&t=393s&ab_channel=SasquatchBStudios
/// </summary>
public class ObjectPooler : MonoBehaviour
{
    public static List<PooledObjectInfo> ObjectPools = new List<PooledObjectInfo>();

    private static GameObject particleSystemFolder;
    private static GameObject gameObjectsFolder;
    private static GameObject audioSourceFolder;
    private static GameObject objectPoolEmptyHolder;

    public enum PoolType
    {
        ParticleSystem,
        AudioSource,
        GameObject,
        None
    }

    public static PoolType PoolingType;

    public void Awake() => SetupEmpties();

    public void SetupEmpties()
    {
        objectPoolEmptyHolder = new GameObject("Pooled Objects");
        objectPoolEmptyHolder.transform.parent = transform;

        audioSourceFolder = new GameObject("Audio Sources");
        audioSourceFolder.transform.SetParent(objectPoolEmptyHolder.transform);

        gameObjectsFolder = new GameObject("GameObjects");
        gameObjectsFolder.transform.SetParent(objectPoolEmptyHolder.transform);

        particleSystemFolder = new GameObject("Particle Systems");
        particleSystemFolder.transform.SetParent(objectPoolEmptyHolder.transform);
    }

    public static GameObject SpawnObject(GameObject objectToSpawn, Vector3 spawnPosition, Quaternion spawnRotation, PoolType poolType = PoolType.None)
    {
        PooledObjectInfo pool = ObjectPools.Find(p => p.LookupString == objectToSpawn.name);

        // If the pool doesn't exist, create it

        if (pool == null)
        {
            pool = new PooledObjectInfo() { LookupString = objectToSpawn.name };
            ObjectPools.Add(pool);
        }

        // Check if there are any inactive objects in the pool
        GameObject spawnableObj = pool.InactiveObjects.FirstOrDefault();

        if (spawnableObj == null)
        {
            GameObject parentObj = SetParentObject(poolType);
            // if there are no inactive objects, create a new one
            spawnableObj = Instantiate(objectToSpawn.gameObject, spawnPosition, spawnRotation);

            // Set parent if a parent was found
            if (parentObj != null) spawnableObj.transform.SetParent(parentObj.transform);
        }
        else
        {
            spawnableObj.transform.position = spawnPosition;
            spawnableObj.transform.rotation = spawnRotation;
            pool.InactiveObjects.Remove(spawnableObj);
            spawnableObj.SetActive(true);
        }

        return spawnableObj;
    }

    public static void ReturnObjectToPool(GameObject obj)
    {
        string goName = obj.name.Substring(0, obj.name.Length - 7); // takeoff seven to remove clone

        PooledObjectInfo pool = ObjectPools.Find(p => p.LookupString == goName);

        if (pool == null)
        {
            Debug.LogWarning("Trying to release an object that is not pooled: " + obj.name);
            return;
        }
        else
        {
            obj.SetActive(false);
            pool.InactiveObjects.Add(obj);
        }
    }

    private static GameObject SetParentObject(PoolType poolType)
    {
        switch (poolType)
        {
            case PoolType.ParticleSystem:
                return particleSystemFolder;

            case PoolType.GameObject:
                return gameObjectsFolder;

            case PoolType.AudioSource:
                return audioSourceFolder;

            default:
                return null;
        }
    }
}

public class PooledObjectInfo
{
    public string LookupString;
    public List<GameObject> InactiveObjects = new List<GameObject>();
}