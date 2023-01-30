using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 冻块
/// </summary>
public class Freeze : MonoBehaviour
{
    private void OnEnable()
    {
        //设置冻块
        GetComponent<Jewel>().type |= JewelType.Freeze;
        GetComponent<Jewel>().InitialPos();
        MapManager.Instance.GridMap[GetComponent<Jewel>().gridPos.x, GetComponent<Jewel>().gridPos.y] = GridType.Obstacles;
    }

    private void OnDisable()
    {
        //关闭冻块
        GetComponent<Jewel>().type -= JewelType.Freeze;
        MapManager.Instance.GridMap[GetComponent<Jewel>().gridPos.x, GetComponent<Jewel>().gridPos.y] = GridType.Jewel;
    }
}
