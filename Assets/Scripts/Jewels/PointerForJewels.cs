using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 指针宝石指向组件
/// </summary>
public class PointerForJewels : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    #region Static Value
    /// <summary>
    /// 当前指针所在的宝石
    /// </summary>
    public static Jewel CurrentPointer { get; private set; }
    #endregion

    #region Static Event
    /// <summary>
    /// 宝石指向全局事件:任意宝石被指向时触发
    /// </summary>
    public static event Action<Jewel> OnPointerGlobalEvent;
    #endregion

    Jewel _jewel;

    /// <summary>
    /// 初始化静态量
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void InitStatic()
    {
        CurrentPointer = null;
        OnPointerGlobalEvent = null;
    }

    private void Start()
    {
        _jewel = GetComponent<Jewel>();
    }

    #region PointerHandler

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GameManager.Instance.IsPause) return;//游戏暂停时不执行该ES

        CurrentPointer = _jewel;
        OnPointerGlobalEvent?.Invoke(_jewel);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (GameManager.Instance.IsPause) return;//游戏暂停时不执行该ES

        if (CurrentPointer.Equals(_jewel))
        {
            CurrentPointer = null;
            OnPointerGlobalEvent?.Invoke(null);
        }
    }

    #endregion
}
