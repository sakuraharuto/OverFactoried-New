using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 灭火器
/// </summary>
public class FireExtinguisher : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public FireType type;
    [field : SerializeField, Tooltip("回收价值")]
    public int Gold { get; private set; } = 20;

    [SerializeField]
    GameObject _sprite;

    void Start()
    {
        //设置蒙版内可见
        _sprite.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
    }

    #region Drag
    GameObject _tmp;
    Vector3 _offset;
    public void OnBeginDrag(PointerEventData eventData)
    {
        //创建临时拖拽图像
        _tmp = Instantiate(_sprite);
        _tmp.transform.position = _sprite.transform.position;
        //正常显示
        _tmp.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.None;
        //原图像半透明
        _sprite.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);

        
        //设置偏移
        _offset = _tmp.transform.position - Camera.main.ScreenToWorldPoint(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //移动
        _tmp.transform.position = Camera.main.ScreenToWorldPoint(eventData.position) + _offset;

        //移动到灭火器垃圾桶时高亮显示垃圾桶
        if (RecycleFireExitinguisher.Instance.IsPointer)
        {
            RecycleFireExitinguisher.Instance.Highlight();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Destroy(_tmp);
        //原图像恢复
        _sprite.GetComponent<SpriteRenderer>().color = Color.white;

        //移动到灭火器垃圾桶时回收
        if (RecycleFireExitinguisher.Instance.IsPointer)
        {
            RecycleFireExitinguisher.Instance.Recycle(this);

            Remove();
            return;
        }

        var jewel = PointerForJewels.CurrentPointer;
        if (jewel != null) 
        {
            FireBlock fire;
            //指向的宝石为火灾宝石
            if (jewel.TryGetComponent<FireBlock>(out fire))
            {
                //灭火成功
                if (fire.Remove(type))
                {
                    Remove();
                }
                else
                {
                    //灭火失败 -10s
                    GameManager.Instance.RemainingTime -= 10;
                    Remove();
                }
            }
            return;
        }
    }

    #endregion

    /// <summary>
    /// 销毁对象
    /// </summary>
    void Remove()
    {
        //0.1s后销毁
        Destroy(gameObject, 0.1f);
        //生成新的
        FireExtinguisherManager.instance.Create();
    }
}
