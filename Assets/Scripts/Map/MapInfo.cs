using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 地图数据配置SO
/// </summary>
[CreateAssetMenu(menuName = "GameData/MapInfo")]
public class MapInfo : SerializedScriptableObject
{
    /// <summary>
    /// X轴格子数量
    /// </summary>
    [field:SerializeField, SuffixLabel("X轴格子数量", Overlay = true), OnValueChanged(nameof(InitObstaclesData))] 
    public int GridX { get; private set; }

    /// <summary>
    /// Y轴格子数量
    /// </summary>
    [field:SerializeField, SuffixLabel("Y轴格子数量", Overlay = true), OnValueChanged(nameof(InitObstaclesData))] 
    public int GridY { get; private set; }

    /// <summary>
    /// 地图X轴长度
    /// </summary>
    [field:SerializeField, SuffixLabel("地图X轴长度",  Overlay = true),] 
    public float MapX { get; private set; }

    /// <summary>
    /// 地图Y轴长度
    /// </summary>
    [field:SerializeField, SuffixLabel("地图Y轴长度", Overlay = true)] 
    public float MapY { get; private set; }

    /// <summary>
    /// 障碍物网格
    /// </summary>
    [field: SerializeField, TableMatrix(SquareCells = true, HorizontalTitle = "障碍物网格")]
    public bool[,] ObstaclesData { get; private set; } = new bool[0,0];

    [Button("重置障碍物数据")]
    void InitObstaclesData()
    {
        Debug.Log("重置障碍物数据");

        ObstaclesData = new bool[GridX, GridY];
    }
}  
