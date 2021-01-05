using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    /*预制体*/
    [SerializeField]
    [Header("预制体")]
    private GameObject prefab;
    /*对象池*/
    private Queue<GameObject> pool;


    private void Start()
    {
        if (pool == null)
        {
            pool = new Queue<GameObject>();
        }
    }

    /*对象池弹出对象*/
    public GameObject GetObject()
    {
        if (pool == null)
        {
            pool = new Queue<GameObject>();
        }
        if (pool.Count == 0)
        {
            return GameObject.Instantiate(prefab);
        }
        else
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
    }

    /*对象池回收对象*/
    public void ReturnObject(GameObject obj)
    {
        obj.transform.parent = transform;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
