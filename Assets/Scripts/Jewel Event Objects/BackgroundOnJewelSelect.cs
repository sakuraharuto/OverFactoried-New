using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 宝石被选中时显示GameObject
/// </summary>
public class BackgroundOnJewelSelect : MonoBehaviour
{
    void Start()
    {
        //订阅宝石选择全局事件
        SelectForJewel.OnSelectGlobalEvent += Select;
    }

    private void OnDestroy()
    {
        //销毁时取消订阅
        SelectForJewel.OnSelectGlobalEvent -= Select;
    }

    void Select(Jewel jewel)
    {
        if (jewel != null)
        {
            //指向宝石则确定位置并显示
            transform.parent = jewel.transform;
            transform.localScale = Vector3.one;
            transform.localPosition = Vector3.zero;

            gameObject.SetActive(true);
        }
        else
        {
            //否则隐藏
            gameObject.SetActive(false);
            transform.parent = null;
        }
    }
}
