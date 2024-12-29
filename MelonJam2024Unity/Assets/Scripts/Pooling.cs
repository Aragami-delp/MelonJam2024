using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pooling<T> where T : MonoPoolItem
{
    private List<T> _poolItems = new();
    private T _prefab;

    public Pooling(T prefab)
    {
        _prefab = prefab;
    }

    public T GetNewItem(Vector2 position, Transform parent)
    {
        if (_poolItems.Where(x => x.gameObject.activeSelf).ToList().Count == 0)
        {
            T retInst = GameObject.Instantiate(_prefab, parent);
            retInst.transform.position = position;
            return retInst;
        }

        T retVal = _poolItems[0];
        _poolItems.RemoveAt(0);
        retVal.transform.SetParent(parent);
        retVal.transform.position = position;
        retVal.ResetValues();
        retVal.gameObject.SetActive(true);
        return retVal;
    }

    public void DisablePoolItem(T item)
    {
        item.gameObject.SetActive(false);
    }
}
