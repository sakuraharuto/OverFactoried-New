using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// buff基类(抽象的）
/// </summary>
[DefaultExecutionOrder(20)]
public abstract class BaseBuff : MonoBehaviour
{
    protected virtual void OnEnable()
    {
        //物体激活时开启buff
        Buff();
    }
    protected virtual void OnDisable()
    {
        //物体关闭时停止Buff
        Stop();
    }

    /// <summary>
    /// buff内容
    /// </summary>
    protected abstract void Buff();

    /// <summary>
    /// 停止buff（buff的反）
    /// </summary>
    protected abstract void Stop();
}
