using System.Collections.Generic;
using System.IO;
using UnityEngine;

public struct Label
{
    public string Name;
    public string Path;
    public int Id;
}

/// <summary>
/// 需要再加入池子内的对象的数量控制，对象池内对象的清除，内存释放，清除某个对象池（这时需要处理ID），玩家可以根据id创建对象
/// 池子的名字命名再考虑考虑
/// </summary>
public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance;
    [SerializeField]private Dictionary<Label, ObjectPool> objectPoolDic = new Dictionary<Label, ObjectPool>();
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            if (Instance != this)
            {
                Destroy(gameObject);
                Instance = this;
            }
        }
        DontDestroyOnLoad(Instance);
    }


    #region 外部调用，申请一个对象

    /// <summary>
    /// 根据传入的对象，创建对象，设置父物体
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="location"></param>
    /// <param name="rotation"></param>
    /// <param name="parent"></param>
    /// <param name="delayDe"></param>
    /// <returns></returns>
    public Transform Spawn(GameObject obj, Vector3 location, Quaternion rotation, Transform parent, float delayDe = -1)
    {
        var objectPool = GetObjectPoolFromGameObject(obj);
        return GetObjectFromObjectPool(objectPool, location, rotation, parent, delayDe);
    }

    /// <summary>
    ///  根据传入的对象，创建对象，不设置父物体
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="location"></param>
    /// <param name="rotation"></param>
    /// <param name="delayDe"></param>
    /// <returns></returns>
    public Transform Spawn(GameObject obj, Vector3 location, Quaternion rotation, float delayDe = -1)
    {
        var objectPool = GetObjectPoolFromGameObject(obj);
        return GetObjectFromObjectPool(objectPool, location, rotation, null, delayDe);
    }

    /// <summary>
    /// 根据路径生成对象，设置父物体
    /// </summary>
    /// <param name="path"></param>
    /// <param name="location"></param>
    /// <param name="rotation"></param>
    /// <param name="parent"></param>
    /// <param name="delayDe"></param>
    /// <returns></returns>
    public Transform Spawn(string path, Vector3 location, Quaternion rotation, Transform parent, float delayDe = -1)
    {
        var objectPool = GetObjectPoolFromPath(path);
        return GetObjectFromObjectPool(objectPool, location, rotation, parent, delayDe);
    }

    /// <summary>
    /// 根据路径生成对象，不设置父物体
    /// </summary>
    /// <param name="path"></param>
    /// <param name="location"></param>
    /// <param name="rotation"></param>
    /// <param name="delayDe"></param>
    /// <returns></returns>
    public Transform Spawn(string path, Vector3 location, Quaternion rotation, float delayDe = -1)
    {
        var objectPool = GetObjectPoolFromPath(path);
        return GetObjectFromObjectPool(objectPool, location, rotation, null, delayDe);
    }

    #endregion

    #region 新建池

    /// <summary>
    /// 新建一个池
    /// </summary>
    /// <param name="_path"></param>
    /// <param name="_name"></param>
    /// <param name="_id"></param>
    /// <returns></returns>
    private ObjectPool NewAObjectPool(string _path, string _name, int _id, GameObject objectPrefab = null)
    {
        var label = new Label()
            {Path = _path, Name = _name, Id = _id};
        var objectPool = new GameObject(_path);
        if (objectPrefab == null)
        {
            objectPool.AddComponent<ObjectPool>().Init(label);
        }
        else
        {
            objectPool.AddComponent<ObjectPool>().Init(label, objectPrefab);
        }

        objectPool.transform.parent = transform;
        objectPoolDic.Add(label, objectPool.GetComponent<ObjectPool>());
        Debug.Log("new  "+ label.Name);
        return objectPool.GetComponent<ObjectPool>();
    }

    #endregion

    #region 外部调用，获得某个池子的标签（目前拿到标签还没有啥用）

    /// <summary>
    /// 根据路径获得对象池的标签
    /// </summary>
    /// <param name="path"></param>
    /// <param name="_label"></param>
    /// <returns></returns>
    private bool GetLabelFromPath(string path, out Label _label)
    {
        foreach (var item in objectPoolDic.Keys)
        {
            if (item.Path.Equals(path))
            {
                _label = item;
                return true;
            }
        }

        _label = default;
        return false;
    }

    /// <summary>
    /// 根据对象获得对象池标签
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public Label GetLabelFromObject(GameObject obj)
    {
        return GetObjectPoolFromGameObject(obj).label;
    }

    #endregion


    #region 从池中拿对象

    /// <summary>
    /// 从对应的对象池中拿对象
    /// </summary>
    /// <param name="objectPool"></param>
    /// <param name="location"></param>
    /// <param name="rotation"></param>
    /// <param name="parent"></param>
    /// <param name="delayDe"></param>
    /// <returns></returns>
    private Transform GetObjectFromObjectPool(ObjectPool objectPool, Vector3 location, Quaternion rotation,
        Transform parent,
        float delayDe = -1)
    {
        var tran = parent == null
            ? objectPool.SpawnObject(location, rotation)
            : objectPool.SpawnObject(location, rotation, parent);
/*        if (delayDe >= 0)
        {

            this.AttachTimer(delayDe, () => DeSpawn(tran.gameObject));
        }*/

        return tran;
    }

    #endregion


    #region 根据申请者传入的信息，找到对应的对象池，若没有则创建一个

    /// <summary>
    /// 根据路径获得对象池，没有就创建一个
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private ObjectPool GetObjectPoolFromPath(string path)
    {
        return !GetLabelFromPath(path, out var label)
            ? NewAObjectPool(path, Path.GetFileNameWithoutExtension(path), objectPoolDic.Count)
            : objectPoolDic[label];
    }

    /// <summary>
    /// 根据对象获得对象池，没有就创建一个
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private ObjectPool GetObjectPoolFromGameObject(GameObject obj)
    {
        foreach (var VARIABLE in objectPoolDic.Values)
        {
            if (VARIABLE.objectPrefab.Equals(obj))
            {
                return VARIABLE;
            }
        }

        return NewAObjectPool("FromScript", obj.name, objectPoolDic.Count, obj);
    }

    #endregion


    #region 外部调用，提前增加池内对象的数量

    /// <summary>
    /// 增加对象池中的对象数量
    /// </summary>
    /// <param name="path"></param>
    /// <param name="quantity"></param>
    public void AddPoolObjectNumber(string path, int quantity)
    {
        var objectPool = !GetLabelFromPath(path, out var label)
            ? NewAObjectPool(path, Path.GetFileNameWithoutExtension(path), objectPoolDic.Count)
            : objectPoolDic[label];

        objectPool.AddPoolObjectNumber(quantity);
    }

    /// <summary>
    /// 增加对象池中的对象数量
    /// </summary>
    /// <param name="path"></param>
    /// <param name="targetQuantity"></param>
    public void AddPoolObjectNumberTo(string path, int targetQuantity)
    {
        var objectPool = !GetLabelFromPath(path, out var label)
            ? NewAObjectPool(path, Path.GetFileNameWithoutExtension(path), objectPoolDic.Count)
            : objectPoolDic[label];

        objectPool.AddPoolObjectNumberTo(targetQuantity);
    }

    #endregion

    #region 外部调用，回收对象

    /// <summary>
    /// 回收单个对象
    /// </summary>
    /// <param name="target"></param>
    public void DeSpawn(GameObject target)
    {
        if (!target.TryGetComponent(out PoolLabel targetLabel))
        {
            Debug.LogError("The targetObject does have PoolLabel");
        }

        var label = targetLabel.label;


        if (!objectPoolDic.ContainsKey(label))
        {
            Debug.LogError($"ObjectPool does have object=>{label.Name}");
            Debug.LogError($"ObjectPool does have object=>{label.Path}");
            Debug.LogError($"ObjectPool does have object=>{label.Id}");
            foreach (var item in objectPoolDic)
            {
                Debug.Log(item.Key.Name);
                Debug.Log(item.Key.Path);
                Debug.Log(item.Key.Id);
                Debug.Log("  5");
            }
            return;
        }

        objectPoolDic[label].DeSpawnObject(target);
    }

    /// <summary>
    /// 回收某一个对象池的所有对象
    /// </summary>
    /// <param name="path"></param>
    public void DeSpawnAll(string path)
    {
        GetLabelFromPath(path, out var label);


        if (!objectPoolDic.ContainsKey(label))
        {
            Debug.LogError($"ObjectPool does have object=>{label}");
            return;
        }

        objectPoolDic[label].DeSpawnObjectAll();
    }

    /// <summary>
    /// 回收所有对象池的所有对象
    /// </summary>
    public void DeSpawnAllPool()
    {
        foreach (var item in objectPoolDic)
        {
            item.Value.DeSpawnObjectAll();
        }
    }

    #endregion

    #region 外部调用，删除某个对象

    /// <summary>
    /// 删除单个对象
    /// </summary>
    /// <param name="target"></param>
    public void DestroyObject(GameObject target)
    {
        if (!target.TryGetComponent(out PoolLabel targetLabel))
        {
            Debug.LogError("The targetObject does have PoolLabel");
        }

        var label = targetLabel.label;


        if (!objectPoolDic.ContainsKey(label))
        {
            Debug.LogError($"ObjectPool does have object=>{label}");
            return;
        }

        objectPoolDic[label].DestroyObject(target);
    }

    public void DestroyPool(string path, bool isDestroyUsing)
    {
        if (GetLabelFromPath(path, out var _label))
        {
            var objectPool = GetObjectPoolFromPath(path);
            objectPool.DestroySelf(isDestroyUsing);
            objectPoolDic.Remove(_label);
        }
    }

    #endregion
}