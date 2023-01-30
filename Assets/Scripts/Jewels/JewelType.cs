using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 宝石类型
/// </summary>
[System.Flags]
public enum JewelType
{
    None = 0,
    A = 0b1,
    B = 0b10,
    C = 0b100,
    D = 0b1000,
    E = 0b10000,
    /// <summary>
    /// 冻块
    /// </summary>
    Freeze = 0b100000
}
