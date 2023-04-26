using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 宝石方块组件
/// </summary>
public class Jewel : MonoBehaviour
{
    #region Type
    [Tooltip("宝石类型")] public JewelType type;

    /// <summary>
    /// 判断宝石类型能否一致
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool TypeEqual(Jewel a, Jewel b)
    {
        if (a != null && b != null && !a.type.HasFlag(JewelType.Freeze) && !b.type.HasFlag(JewelType.Freeze))
            return a.type.HasFlag(b.type) || b.type.HasFlag(a.type);
        return false;
    }

    /// <summary>
    /// 能否与另一个宝石类型一致
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool TypeEqual(Jewel other) =>
        TypeEqual(this, other);

    #endregion
    #region Position
    [Tooltip("所在格子位置"), ReadOnly] public Vector2Int gridPos;

    /// <summary>
    /// 判断宝石位置是否相邻
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool PositionAdjacent(Jewel a, Jewel b)
    {
        int distance = Mathf.Abs(a.gridPos.x - b.gridPos.x) + Mathf.Abs(a.gridPos.y - b.gridPos.y);

        return distance == 1;
    }

    /// <summary>
    /// 是否与另一个宝石相邻
    /// </summary>
    /// <param name="ohter"></param>
    /// <returns></returns>
    public bool PositionAdjacent(Jewel ohter) =>
        PositionAdjacent(this, ohter);

    /// <summary>
    /// 设置位置
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void SetGridPosition(int x, int y)
    {
        gridPos.x = x;
        gridPos.y = y;
        MapManager.Instance.JewelsMap[x, y] = this;
    }
    #endregion
    #region Initial
    [Tooltip("宝石实例生成事件:仅关于这个物体")]
    public UnityEvent<Jewel> OnCreate = new UnityEvent<Jewel>();

    public void Initial(Vector3 initPos, Vector3 initSize)
    {
        transform.position = initPos;
        transform.localScale = initSize;
        //广播生成事件
        OnCreate?.Invoke(this);
    }

    public void InitialPos()
    {
        Initial(MapManager.Instance.GetGridPostion(gridPos.x, gridPos.y), 
            new Vector3(Mathf.Abs(MapManager.Instance.GridSize.x), Mathf.Abs(MapManager.Instance.GridSize.y), 1));
        MapManager.Instance.JewelsMap[gridPos.x, gridPos.y] = this;
    }
    #endregion

    #region Behavior
    /// <summary>
    /// 宝石交换移动
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public Tween SwapMove(int x, int y)
    {
        //交换时间
        const float time = 0.2f;

        //移动
        Tween tween = transform.DOMove(MapManager.Instance.GetGridPostion(x, y), time).SetEase(Ease.InQuad).
            OnComplete(() =>
            {
                //设置位置
               SetGridPosition(x, y);
            });

        return tween;
    }

    /// <summary>
    /// 宝石掉落移动
    /// </summary>
    /// <param name="endY"></param>
    /// <returns></returns>
    public Tween FallMove(int x, int y)
    {
        //掉落加速度
        const float acc = 22f;

        //移动
        Tween tween = transform.DoFall(MapManager.Instance.GetGridPostion(x, y), acc * MapManager.Instance.GridSize.y).
            OnComplete(() =>
            {
                //设置位置
                SetGridPosition(x, y);
            });

        return tween;
    }

    #endregion

    #region Remove

    [Tooltip("宝石实例消除事件:仅关于这个物体")]
    public UnityEvent<Jewel, List<List<Jewel>>, List<Jewel>> OnMatch = new UnityEvent<Jewel, List<List<Jewel>>, List<Jewel>>();
    /// <summary>
    /// 宝石消除后事件 仅用于对象池回收
    /// </summary>
    public System.Action<Jewel> OnRemove;

    /// <summary>
    /// 消除宝石
    /// </summary>
    /// <param name="time">消除时间</param>
    /// <param name="matchList">消除序列的列表</param>
    /// <param name="allMatchJewelList">在消除中的所有宝石的集合</param>
    /// <returns></returns>
    public IEnumerator Match(float time, List<List<Jewel>> matchList, List<Jewel> allMatchJewelList)
    {
        //广播该宝石消除事件
        OnMatch?.Invoke(this, matchList, allMatchJewelList);
        yield return new WaitForSeconds(time);

        //清除
        Remove();
    }


    /// <summary>
    /// 清除宝石实例
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public void Remove()
    {
        //消除宝石
        OnRemove?.Invoke(this);
        if (MapManager.Instance.JewelsMap[gridPos.x, gridPos.y] == this)
        {
            MapManager.Instance.JewelsMap[gridPos.x, gridPos.y] = null;
        }
    }

    #endregion
}
