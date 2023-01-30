using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 场景地图数据集
/// </summary>
[DefaultExecutionOrder(-1)]
public class MapData : MonoBehaviour
{
    /// <summary>
    /// 单例
    /// </summary>
    public static MapData Instance { get; private set; }

    [Tooltip("场景地图数据")]
    public MapInfo mapInfo;
    [Tooltip("场景物品数据")]
    public MapItemInfo mapItemInfo;

    private void Awake()
    {
        //设置单例
        Instance = this;
    }
}
