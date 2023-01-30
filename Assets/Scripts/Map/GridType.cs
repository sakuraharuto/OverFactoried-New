using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 格子类型
/// </summary>
[System.Flags]
public enum GridType
{
    Jewel = 0b1,
    Obstacles = 0b10
}

