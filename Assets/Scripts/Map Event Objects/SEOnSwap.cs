using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 宝石交换音效
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class SEOnSwap : MonoBehaviour
{
    [Tooltip("音效")]
    public AudioClip clip;

    AudioSource _audio;

    // Start is called before the first frame update
    void Start()
    {
        _audio = GetComponent<AudioSource>();
        MapManager.Instance.OnSwap.AddListener((a, b) =>
        {
            _audio.PlayOneShot(clip);
        });
    }
}
