using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;

/// <summary>
/// 宝石选择组件
/// </summary>
public class SelectForJewel : MonoBehaviour, IPointerClickHandler
{
    #region Static Value
    /// <summary>
    /// 当前选择的宝石
    /// </summary>
    public static Jewel CurrentSelect { get; private set; }
    #endregion

    #region Static Event
    /// <summary>
    /// 宝石选择全局事件:任意宝石被选中时触发
    /// </summary>
    public static event Action<Jewel> OnSelectGlobalEvent;

    #endregion

    /// <summary>
    /// 初始化静态量
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void InitStatic()
    {
        CurrentSelect = null;
        OnSelectGlobalEvent = null;
    }

    Jewel _jewel;


    private void Start()
    {
        _jewel = GetComponent<Jewel>();
    }

    /// <summary>
    /// 宝石被选中
    /// </summary>
    void Select()
    {
        Jewel old = CurrentSelect;

        if (old == null)
        {
            //之前未选中宝石, 则选中当前宝石
            _Select();
        }
        else if (old.Equals(_jewel))
        {
            //之前选中宝石为当前宝石
            //...忽略...
        }
        else
        {
            //之前选中宝石非当前宝石，判断位置是否相邻
            if (_jewel.PositionAdjacent(old))
            {
                //相邻，则开始交换
                StartCoroutine(MapManager.Instance.Swap(old, _jewel));
                //设置当前选中为空
                CurrentSelect = null;
                OnSelectGlobalEvent?.Invoke(null);
            }
            else
            {
                //不相邻，则选中当前宝石
                _Select();
            }
        }

        
        void _Select()
        {
            //设置选中对象
            CurrentSelect = _jewel;
            //广播选择事件
            OnSelectGlobalEvent?.Invoke(_jewel);

            //Debug.Log($"选择{_jewel.gridPos} : {_jewel.gameObject.name}");
        }
    }

    #region PointerHandler
    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.Instance.IsPause) return;//游戏暂停时不执行该ES

        //仅地图正常状态时可选中（宝石交换及下落进行时不可选中）
        if (MapManager.Instance.status.Equals(MapStatus.Default))
        {
            Select();
        }
    }

    #endregion
}
