using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockFrameRate : MonoBehaviour
{
    [Tooltip("最大帧率")]
    public int maxFrame = 90;

    private void Awake()
    {
        Application.targetFrameRate = maxFrame;
    }
}
