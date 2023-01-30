using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 宝石被指向时显示GameObject
/// </summary>
public class BackgroundOnJewelPointer : MonoBehaviour
{
    void Start()
    {
        //订阅宝石指向全局事件
        PointerForJewels.OnPointerGlobalEvent += Pointer;
    }

    private void OnDestroy()
    {
        //销毁时取消订阅
        PointerForJewels.OnPointerGlobalEvent -= Pointer;
    }

    void Pointer(Jewel jewel)
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
