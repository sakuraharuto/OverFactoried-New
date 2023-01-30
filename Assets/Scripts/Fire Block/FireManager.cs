using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

/// <summary>
/// 火灾
/// </summary>
[DefaultExecutionOrder(100)]
public class FireManager : SerializedMonoBehaviour
{
    [SerializeField, LabelText("火灾方块字典容器"), DictionaryDrawerSettings(KeyLabel = "火灾类型", ValueLabel = "物体预制件")]
    Dictionary<FireType, GameObject> fireObjsDic = new Dictionary<FireType, GameObject>();

    [SerializeField, Tooltip("场上最大共存数量")]
    float _maxCapacity = 5;

    [SerializeField, LabelText("特定事件1的类型")]
    private FireType _one;
    [SerializeField, LabelText("特定事件2的类型")]
    private FireType _two;
    [SerializeField, LabelText("特定事件4的类型")]
    private FireType _four;
    [SerializeField, LabelText("特定事件5的类型")]
    private FireType _five;

    [SerializeField, Tooltip("初始概率")]
    float _initialProbability = 0;
    [SerializeField, ReadOnly, LabelText("当前概率")]
    float _currenProbability;
    [SerializeField, Tooltip("每消除增加的概率")]
    float _increasedProbability = 3;
    [SerializeField, Tooltip("可以进行判定的概率阈值")]
    float _threshold = 11;

    Queue<GameObject> fireObjsQueue;
    List<GameObject> currentFires = new List<GameObject>();

    private void Awake()
    {
        //初始化数据
        _currenProbability = _initialProbability;
    }

    private void Start()
    {
        //生成火灾集合
        List<GameObject> list = new List<GameObject>();
        foreach (var item in fireObjsDic)
        {
            for (int i = 0; i < 3; i++) //每种生成3个
            {
                //生成火灾方块
                var obj = Instantiate(item.Value, transform);
                obj.SetActive(false);

                obj.GetComponent<FireBlock>().type = item.Key;
                //添加回收事件
                obj.GetComponent<FireBlock>().OnRemove.AddListener((a) =>
                {
                    currentFires.Remove(a.gameObject);
                    fireObjsQueue.Enqueue(a.gameObject);
                });

                list.Add(obj);
            }
        }

        //洗牌
        list.Shuffle();

        //火灾事件位置变换
        //第一个特定事件1放到开头
        for (int x = 0; x < list.Count; x++)
        {
            if (list[x].GetComponent<FireBlock>().type.HasFlag(_one))
            {
                var tmp = list[x];
                list[x] = list[0];
                list[0] = tmp;

                break;
            }
        }
        //第一个特定事件2放到第二位
        for (int x = 0; x < list.Count; x++)
        {
            if (list[x].GetComponent<FireBlock>().type.HasFlag(_two))
            {
                var tmp = list[x];
                list[x] = list[1];
                list[1] = tmp;

                break;
            }
        }
        //前半段的特定事件4,5 Swap到后面
        for (int x = 0; x < list.Count / 2; x++)
        {
            if (list[x].GetComponent<FireBlock>().type.HasFlag(_five) || list[x].GetComponent<FireBlock>().type.HasFlag(_four))
            {
                var tmp = list[x];
                list[x] = list[list.Count - 1 - x];
                list[list.Count - 1 - x] = tmp;
            }
        }

        //初始化队列
        fireObjsQueue = new Queue<GameObject>(list);

        //订阅主动消除事件：增加概率
        MapManager.Instance.OnMatchForSwap.AddListener(IncreaseProbability);
        //订阅消除结束事件：概率生成火灾方块
        MapManager.Instance.OnMatchEnd.AddListener(ProbabiltyCreate);
    }

    /// <summary>
    /// 概率生成
    /// </summary>
    private void ProbabiltyCreate()
    {
        if (_currenProbability > _threshold && currentFires.Count < _maxCapacity) //满足判定条件：大于阈值且未达到最大容量
        {
            if (Random.value < _currenProbability / 100f)//判定成功
            {
                //生成
                Create();

                //初始化概率
                _currenProbability = _initialProbability;
            }
        }
    }

    public void Create()
    {
        //获取火灾方块
        var obj = fireObjsQueue.Dequeue();
        currentFires.Add(obj);

        //生成坐标
        Vector2Int pos;
        do 
        {
            int posX = Random.Range(0, MapManager.Instance.Length);
            int posY = Random.Range(0, MapManager.Instance.Hight);
            pos = new Vector2Int(posX, posY);
        } while(MapManager.Instance.GridMap[pos.x, pos.y] != GridType.Jewel);


        //清除位置宝石
        MapManager.Instance.JewelsMap[pos.x, pos.y].Remove();

        //设置坐标
        obj.GetComponent<Jewel>().SetGridPosition(pos.x, pos.y);
        
        //开始火灾方块
        obj.SetActive(true);
    }

    /// <summary>
    /// 增加概率
    /// </summary>
    /// <param name="arg0"></param>
    private void IncreaseProbability(List<List<Jewel>> arg0)
    {
        _currenProbability += _increasedProbability;
    }
}
