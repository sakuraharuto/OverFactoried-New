using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimVfxPool : BasePool<Animator>
{
    /// <summary>
    /// 特效持续时间
    /// </summary>
    float _duration = 0.2f;

    /// <summary>
    /// 初始化对象池
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="duration"></param>
    public void Initialize(Animator prefab ,float duration)
    {
        base.Initialize(prefab, 10, 25, true);
        _duration = duration;

        //预生成
        for (int i = 0; i < 10; i++)
        {
            this.Get().transform.position = Vector3.one * -999;
        }
    }

    /// <summary>
    /// 获取对象
    /// </summary>
    /// <param name="obj"></param>
    protected override void OnGetItem(Animator obj)
    {
        base.OnGetItem(obj);

        //获取对象时开启动画和sprite显示
        obj.enabled = true;
        obj.GetComponent<SpriteRenderer>().enabled = true;

        //等待持续时间结束后清除
        DOTween.Sequence().AppendInterval(_duration).AppendCallback(() =>
        {
            Release(obj);
        }).Play();
    }

    protected override void OnReleaseItem(Animator obj)
    {
        base.OnReleaseItem(obj);

        //关闭对象动画及sprite显示
        obj.enabled = false;
        obj.GetComponent<SpriteRenderer>().enabled = false;
    }
}
