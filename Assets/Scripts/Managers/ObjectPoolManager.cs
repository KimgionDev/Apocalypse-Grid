using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance;

    // Dictionary lưu trữ các hàng đợi (Queue) của các object theo tên
    private Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public GameObject SpawnFromPool(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null) return null;

        string poolKey = prefab.name;

        if (!poolDictionary.ContainsKey(poolKey))
        {
            poolDictionary.Add(poolKey, new Queue<GameObject>());
        }

        GameObject objectToSpawn;

        if (poolDictionary[poolKey].Count > 0)
        {
            // Lấy object có sẵn ra xài
            objectToSpawn = poolDictionary[poolKey].Dequeue();
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;
            objectToSpawn.SetActive(true);
        }
        else
        {
            // Hết hàng thì mới tạo mới
            objectToSpawn = Instantiate(prefab, position, rotation);
            PooledObject pooledObj = objectToSpawn.AddComponent<PooledObject>();
            pooledObj.poolKey = poolKey;
        }

        return objectToSpawn;
    }

    public void ReturnToPool(GameObject obj)
    {
        PooledObject pooledObj = obj.GetComponent<PooledObject>();

        if (pooledObj != null && poolDictionary.ContainsKey(pooledObj.poolKey))
        {
            obj.SetActive(false);
            poolDictionary[pooledObj.poolKey].Enqueue(obj);
        }
        else
        {
            // Đề phòng lỗi (Ví dụ object không sinh ra từ Pool mà bị ép Return)
            Destroy(obj);
        }
    }
}

public class PooledObject : MonoBehaviour
{
    public string poolKey;
}
