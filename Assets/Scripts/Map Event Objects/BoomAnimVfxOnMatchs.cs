using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 消除时爆炸特效（序列帧动画）
/// </summary>
public class BoomAnimVfxOnMatchs : MonoBehaviour
{
    [SerializeField, Tooltip("特效预制件")] Animator _prefab;
    [SerializeField, Tooltip("特效持续时间")] float _duration = 0.5f;
    /// <summary>
    /// 对象池
    /// </summary>
    AnimVfxPool _pool;

    // Start is called before the first frame update
    void Start()
    {
        //生成对象池
        GameObject poolObj = new GameObject("Pool:BoomAnimVfx");
        poolObj.transform.parent = transform;
        _pool = poolObj.AddComponent<AnimVfxPool>();

        //对象初始化
        _pool.Initialize(_prefab,_duration);

        //订阅全局宝石消除事件
        MapManager.Instance.OnJewelMatch.AddListener(a =>
        {
            //生成特效
            var vfx = _pool.Get();
            //设置位置
            vfx.transform.position = a.transform.position;
            vfx.transform.localScale = a.transform.localScale;
        });
    }

   
}
