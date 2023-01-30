using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public static class GameUtility
{
    /// <summary>
    /// 自由落体运动
    /// </summary>
    /// <param name="endValue">目标位置</param>
    /// <param name="acceleration">加速度</param>
    /// <returns></returns>
    public static Tween DoFall(this Transform transform, Vector3 endValue, float acceleration)
    {
        //路程
        float x = Vector3.Distance(transform.position, endValue);
        //时间
        float time = Mathf.Sqrt(2f * x / acceleration);
        //动画
        return transform.DOMove(endValue, time).SetEase(_FallEasy);

        //时间曲线
        float _FallEasy(float time, float duration, float overshootOrAmplitude, float period)
        {
            return (0.5f * acceleration * time * time) / x;
        }
    }

    /// <summary>
    /// 删除a集合内ab交集
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="a"></param>
    /// <param name="b"></param>
    public static void RemoveAtList<T>(this List<T> a, List<T> b)
    {
        if (a != null && b != null)
        {
            for (int i = 0; i < b.Count; i++)
            {
                if (a.Contains(b[i]))
                {
                    a.Remove(b[i]);
                }
            }
        }
    }

    /// <summary>
    /// 列表洗牌
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public static void Shuffle<T>(this List<T> list)
    {
        if (list != null) 
        {
            for (int i = 0; i < list.Count; i++)
            {
                T tmp = list[i];
                int pos = Random.Range(0, list.Count - 1);
                list[i] = list[pos];
                list[pos] = tmp;
            }
        }
    }
}
