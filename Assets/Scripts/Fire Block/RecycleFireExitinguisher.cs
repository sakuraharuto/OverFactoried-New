using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 灭火器的垃圾桶
/// </summary>
public class RecycleFireExitinguisher : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    /// <summary>
    /// 单例
    /// </summary>
    public static RecycleFireExitinguisher Instance { get; private set; }

    /// <summary>
    /// 指针是否在垃圾桶上
    /// </summary>
    public bool IsPointer { get; private set; } = false;

    private void Awake()
    {
        Instance = this;
    }
    [SerializeField]
    GameObject _vfx;
    /// <summary>
    /// 回收
    /// </summary>
    /// <param name="fireExtinguisher"></param>
    public void Recycle(FireExtinguisher fireExtinguisher)
    {
        //回收价值
        GameManager.Instance.Gold += fireExtinguisher.Gold;
        //关闭高亮显示
        TurnOffHighlight();

        //播放特效及音效
        _vfx.gameObject.SetActive(false);
        _vfx.gameObject.SetActive(true);
    }

    [SerializeField]
    SpriteRenderer _spriteRenderer;
    bool _isHighlight = false;

    /// <summary>
    /// 高亮显示
    /// </summary>
    public void Highlight()
    {
        if (!_isHighlight)
        {
            _spriteRenderer.color = Color.white;

            _isHighlight = true;
        }
    }
    /// <summary>
    /// 关闭高亮显示
    /// </summary>
    public void TurnOffHighlight()
    {
        if (_isHighlight)
        {
            _spriteRenderer.color = Color.gray;

            _isHighlight = false;   
        }
    }

    #region Pointer
    public void OnPointerEnter(PointerEventData eventData)
    {
        IsPointer = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsPointer = false;
        TurnOffHighlight();
    }
    #endregion
}
