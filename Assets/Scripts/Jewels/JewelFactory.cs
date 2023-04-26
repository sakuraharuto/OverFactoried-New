using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 宝石生成工厂
/// </summary>
[DefaultExecutionOrder(1)]
public class JewelFactory : MonoBehaviour
{
    MapInfo _mapInfo => MapData.Instance.mapInfo;
    MapItemInfo _mapItemInfo => MapData.Instance.mapItemInfo;

    /// <summary>
    /// 对象池字典集合
    /// </summary>
    Dictionary<JewelType, JewelPool> _pools = new Dictionary<JewelType, JewelPool>();

    private void Awake()
    {
        //生成对象池
        foreach (var item in _mapItemInfo.JewelObjects)
        {
            //生成对象池GameObject
            GameObject poolObj = new GameObject($"JewelPool:{item.Key.ToString()}");
            //放置层级位置
            poolObj.transform.parent = transform;
            //添加对象池组件
            var pool = poolObj.AddComponent<JewelPool>();
            _pools[item.Key] = pool;
            //初始化对象池
            pool.Initialized(item.Value, _mapInfo);
        }
    }

    /// <summary>
    /// 获取一个对应类型的宝石
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public Jewel GetJewelWithType(JewelType type)
    {
        if (_pools.ContainsKey(type))
        {
            return _pools[type].Get();
          
        } 
        else
        {
            Debug.LogError($"获取宝石失败！mapItemInfo中无类型{type.ToString()}的宝石!");
            return null;
        }
    }
}
