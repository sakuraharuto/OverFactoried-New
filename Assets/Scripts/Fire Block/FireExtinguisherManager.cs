using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 灭火器管理组件
/// </summary>
public class FireExtinguisherManager : SerializedMonoBehaviour
{
    /// <summary>
    /// 单例
    /// </summary>
    public static FireExtinguisherManager instance;

    [SerializeField, Tooltip("初始生成位置")]
    Transform _initalPos;
    [SerializeField, Tooltip("遮罩")]
    SpriteMask _mask;

    [SerializeField, LabelText("灭火器及其权重"),DictionaryDrawerSettings(KeyLabel ="灭火器预制件", ValueLabel = "权重")]
    Dictionary<FireExtinguisher, float> _extinguishers = new Dictionary<FireExtinguisher, float>();

    private void Awake()
    {
        //设置单例
        instance = this;    
    }

    IEnumerator Start()
    {
        //初始生成三个
        for (int i = 0; i < 3; i++)
        {
            Create();
            yield return new WaitForSeconds(0.3f);
        }
    }

    /// <summary>
    /// 在指定位置生成一个灭火器
    /// </summary>
    public void Create()
    {
        //按权重获取
        var prefab = Get();

        //生成
        var fe = Instantiate(prefab, transform);
        fe.transform.localScale = Vector3.one;
        fe.transform.position = _initalPos.position;
        fe.transform.parent = _mask.transform;
    }


    /// <summary>
    /// 按权重随机获取一个灭火器
    /// </summary>
    /// <returns></returns>
    FireExtinguisher Get()
    {
        FireExtinguisher result = null;

        //计算权重和
        float max = 0;
        foreach (var item in _extinguishers)
        {
            max += item.Value;
        }

        //随机获取
        float random = Random.Range(0, max);
        max = 0;
        foreach (var item in _extinguishers)
        {
            max += item.Value;
            if (random <= max)
            {
                result = item.Key;

                break;
            }
        }

        return result;
    }
}
