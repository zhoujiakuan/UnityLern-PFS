using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public Label label;
    [NonSerialized] public GameObject objectPrefab; //对象池存储的预制体


    public void Init(Label _label)
    {
        label = _label;
        objectPrefab = ResourcesLoad();
    }

    public void Init(Label _label, GameObject _objectPrefab)
    {
        label = _label;
        objectPrefab = _objectPrefab;
    }

    private GameObject ResourcesLoad()
    {
        return Resources.Load<GameObject>(label.Path);
    }


    [SerializeField] private int initialQuantity; //初始数量
    [SerializeField] private int maxQuantity = 2000; //最大数量


    private Queue<GameObject> poolQueue = new Queue<GameObject>(); //失活物体队列

    private List<GameObject> spawnPoolList = new List<GameObject>(); //激活物体的集合

    private void Awake()
    {
        while (poolQueue.Count < initialQuantity)
        {
            InstantiateNewObject();
        }
    }

    public void AddPoolObjectNumber(int quantity)
    {
        for (int i = 0; i < quantity; i++)
        {
            InstantiateNewObject();
        }
    }

    public void AddPoolObjectNumberTo(int targetQuantity)
    {
        while (poolQueue.Count < targetQuantity)
        {
            InstantiateNewObject();
        }
    }

    private bool InstantiateNewObject()
    {
        GameObject instantiateObject = null;
        if (!objectPrefab)
        {
            Debug.LogError("对象池的预制体对象为空");
            return false;
        }
        else
        {
            instantiateObject = Instantiate(objectPrefab, Vector3.zero, Quaternion.identity, transform);
            instantiateObject.GetComponent<IPoolObjectBase>()?.OnInstant();
            if(instantiateObject.TryGetComponent<PoolLabel>(out var poolLabel))
            {
                poolLabel.label = label;
            }
            else
            {
                instantiateObject.AddComponent<PoolLabel>().label=label;
            }
            ResetObject(instantiateObject.transform);
        }

        poolQueue.Enqueue(instantiateObject);
        return true;
    }

    /// <summary>
    /// 重置对象
    /// </summary>
    private void ResetObject(Transform tatget)
    {
        tatget.gameObject.SetActive(false);
        tatget.SetParent(transform);
        tatget.eulerAngles = Vector3.zero;
        tatget.localScale = Vector3.one;
        tatget.position = Vector3.zero;
    }

    public Transform SpawnObject(Vector3 location, Quaternion rotation, Transform parent = null)
    {
        if (poolQueue.Count + spawnPoolList.Count >= maxQuantity)
        {
            Debug.LogError("已经超过该池的最大对象数");
            return null;
        }

        GameObject spawn = null;
        while (!spawn) //这里确保从队列里取到了对象，解决了对象池内存储的对象被外部删除时，队列内却还保持着对他的引用从而导致在使用对象池取物体时取到是空物体，导致后续报空出错的问题【不建议出现这种操作】
        {
            if (poolQueue.Count <= 0)
                if (!InstantiateNewObject())
                    return null;
            spawn = poolQueue.Dequeue();
        }

        if (parent)
        {
            spawn.transform.SetParent(parent, false);
        }

        if (spawn.TryGetComponent(out RectTransform rectTransform) || parent)
        {
            spawn.transform.localPosition = location;
        }
        else
        {
            spawn.transform.position = location;
        }


        spawn.transform.rotation = rotation;


        spawn.GetComponent<IPoolObjectBase>()?.OnSpawnBefore();
        spawn.SetActive(true);
        spawn.GetComponent<IPoolObjectBase>()?.OnSpawnEnd();
        spawnPoolList.Add(spawn);
        return spawn.transform;
    }

    public void DeSpawnObject(GameObject target)
    {
        if (spawnPoolList.Count == 0)
            return;
        var deSpawn = target;
        if (spawnPoolList.Contains(deSpawn))
        {
            if (!deSpawn) //若该物体已经被删除，从集合中删除
            {
                spawnPoolList.Remove(target);
                return;
            }

            deSpawn.GetComponent<IPoolObjectBase>()?.OnDeSpawn();
            spawnPoolList.Remove(deSpawn);
            poolQueue.Enqueue(deSpawn);
            ResetObject(deSpawn.transform);
        }
    }

    public void DeSpawnObjectAll()
    {
        while (spawnPoolList.Count > 0)
            DeSpawnObject(spawnPoolList[0]);
    }


    public void DestroyObject(GameObject target)
    {
        if (spawnPoolList.Count == 0)
            return;
        var deSpawn = target;
        if (spawnPoolList.Contains(deSpawn))
        {
            if (!deSpawn) //若该物体已经被删除，从集合中删除
            {
                spawnPoolList.Remove(target);
                return;
            }

            deSpawn.GetComponent<IPoolObjectBase>()?.OnDeSpawn();
            spawnPoolList.Remove(deSpawn);
            Destroy(target);
        }
    }

    public void DestroySelf(bool isDestroyUsing)
    {
        if (isDestroyUsing)
        {
            foreach (var VARIABLE in spawnPoolList)
            {
                DestroyObject(VARIABLE);
            }
        }


        Destroy(gameObject);
    }
}