using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 场景物体配置SO
/// </summary>
[CreateAssetMenu(menuName = "GameData/MapItemInfo")]
public class MapItemInfo : SerializedScriptableObject
{
    [SerializeField, Tooltip("宝石预制件集合")] 
    List<GameObject> _jewelObjects = new List<GameObject>();

    /// <summary>
    /// 宝石实例字典
    /// </summary>
    [field: SerializeField, ReadOnly, LabelText("JewelObjects字典:"), DictionaryDrawerSettings(KeyLabel = "宝石类型", ValueLabel = "宝石预制件", IsReadOnly = true)] 
    public Dictionary<JewelType, Jewel> JewelObjects { get; private set; } = new Dictionary<JewelType, Jewel>();

#if UNITY_EDITOR
    private void OnValidate()
    {
        CreateJewelObjects();
    }

    /// <summary>
    /// 生成宝石字典集合
    /// </summary>
    private void CreateJewelObjects()
    {
        //遍历填充字典
        Dictionary<JewelType, Jewel> tmp = new Dictionary<JewelType, Jewel>();
        foreach (var item in _jewelObjects)
        {
            Jewel jewel;
            if (item != null && item.TryGetComponent<Jewel>(out jewel))
            {
                tmp[jewel.type] = jewel;
            }
        }
        JewelObjects = tmp;

        //重置集合
        _jewelObjects.Clear();
        foreach (var item in JewelObjects.Values)
        {
            _jewelObjects.Add(item.gameObject);
        }
    }
#endif
}
