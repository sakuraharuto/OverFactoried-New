using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuffThatReduceGameTime : BaseBuff
{
    protected override void Buff()
    {
        GameManager.Instance.reducedTime += 1;
    }

    protected override void Stop()
    {
        GameManager.Instance.reducedTime -= 1;
    }
}
