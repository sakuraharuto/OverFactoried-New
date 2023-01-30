using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// 组件对象池基类
/// </summary>
/// <typeparam name="T"></typeparam>
public class BasePool<T> : MonoBehaviour where T : Component
{
    [SerializeField, Tooltip("对象预制件")] protected T prefab;

    /// <summary>
    /// Unity对象池
    /// </summary>
    ObjectPool<T> pool;

    /// <summary>
    /// 初始化对象池
    /// </summary>
    /// <param name="defaultSize"></param>
    /// <param name="maxSize"></param>
    /// <param name="collectionCheck"></param>
    protected void Initialize(T prefab, int defaultSize, int maxSize, bool collectionCheck = true)
    {
        this.prefab = prefab;
        pool = new ObjectPool<T>(OnCreateItem, OnGetItem, OnReleaseItem, OnDestroyItem, collectionCheck, defaultSize, maxSize);
    }
    protected virtual T OnCreateItem() => Instantiate(prefab, transform);
    protected virtual void OnGetItem(T obj) => obj.gameObject.SetActive(true);
    protected virtual void OnReleaseItem(T obj) => obj.gameObject.SetActive(false);
    protected virtual void OnDestroyItem(T obj) => Destroy(obj.gameObject);

    /// <summary>
    /// 获取对象
    /// </summary>
    /// <returns></returns>
    public T Get() => pool.Get();
    /// <summary>
    /// 释放对象
    /// </summary>
    /// <param name="obj"></param>
    public void Release(T obj) => pool.Release(obj);
    /// <summary>
    /// 清除对象池
    /// </summary>
    public void Clear() => pool.Clear();
}
