using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 火灾方块组件
/// </summary>
public class FireBlock : MonoBehaviour
{
    [Tooltip("火灾类型")]
    public FireType type;

    /// <summary>
    /// 清除火灾方块事件
    /// </summary>
    public UnityEvent<FireBlock> OnRemove = new UnityEvent<FireBlock> ();

    public void Start()
    {
        //添加冻块
        gameObject.AddComponent<Freeze>();
    }

    /// <summary>
    /// 使用灭火器消除
    /// </summary>
    /// <param name="extinguisher"></param>
    /// <returns></returns>
    public bool Remove(FireType extinguisher)
    {
        if (extinguisher.HasFlag(type)) //可消除
        {
            OnRemove?.Invoke(this);

            //消除
            gameObject.SetActive(false);

            MapManager.Instance.Match(GetComponent<Jewel>());

            return true;
        }

        return false;
    }
}
