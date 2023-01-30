using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// 宝石对象池
/// </summary>
public class JewelPool : BasePool<Jewel>
{
    /// <summary>
    /// 初始化宝石对象池
    /// </summary>
    /// <param name="jewelPrefab"></param>
    /// <param name="mapInfo"></param>
    public void Initialized(Jewel jewelPrefab, MapInfo mapInfo)
    {
        //初始化对象池大小为地图格子数量的35%
        Initialize(jewelPrefab,(int)(mapInfo.GridX * mapInfo.GridY * 0.35f), mapInfo.GridX * mapInfo.GridY, false);
    }

    protected override Jewel OnCreateItem()
    {
        var jewel = base.OnCreateItem();

        //放置宝石物体层级
        jewel.transform.parent = transform;

        //订阅宝石实例消除事件 消除时触发对象池回收
        jewel.OnRemove += Release;

        return jewel;
    }
}
