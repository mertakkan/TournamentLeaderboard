using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T>
    where T : Component
{
    private Queue<T> pool = new Queue<T>();
    private T prefab;
    private Transform parent;

    public ObjectPool(T prefab, Transform parent, int initialSize = 10)
    {
        this.prefab = prefab;
        this.parent = parent;

        // Pre-instantiate objects
        for (int i = 0; i < initialSize; i++)
        {
            T obj = Object.Instantiate(prefab, parent);
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public T Get()
    {
        if (pool.Count > 0)
        {
            T obj = pool.Dequeue();
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
        {
            // Create new object if pool is empty
            T obj = Object.Instantiate(prefab, parent);
            obj.gameObject.SetActive(true);
            return obj;
        }
    }

    public void Return(T obj)
    {
        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
    }

    public void ReturnAll(List<T> objects)
    {
        foreach (T obj in objects)
        {
            Return(obj);
        }
        objects.Clear();
    }
}
