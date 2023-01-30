using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

/// <summary>
/// 地图对象管理组件
/// </summary>
[DefaultExecutionOrder(5)]
public class MapManager : MonoBehaviour
{
    /// <summary>
    /// 地图管理器单例
    /// </summary>
    public static MapManager Instance { get; private set; }

    [SerializeField, Tooltip("宝石生成工厂")]
    private JewelFactory _jewelFactory;

    #region 场景数据
    MapInfo _mapInfo => MapData.Instance.mapInfo;
    MapItemInfo _mapItemInfo => MapData.Instance.mapItemInfo;

    /// <summary>
    /// 具有的宝石类型
    /// </summary>
    List<JewelType> _jewelTypes;

    /// <summary>
    /// 格子大小
    /// </summary>
    public Vector3 GridSize { get; private set; }

    /// <summary>
    /// 地图长
    /// </summary>
    public int Length { get; private set; }
    /// <summary>
    /// 地图高
    /// </summary>
    public int Hight { get; private set; }

    /// <summary>
    /// 格子地图
    /// </summary>
    public GridType[,] GridMap { get; private set; }
    /// <summary>
    /// 宝石地图
    /// </summary>
    public Jewel[,] JewelsMap  { get; private set; }

    #endregion

    [ReadOnly]
    public MapStatus status = MapStatus.Default;

    #region 场景事件
    /// <summary>
    /// 全局宝石交换事件
    /// </summary>
    public UnityEvent<Jewel, Jewel> OnSwap = new UnityEvent<Jewel, Jewel>();
    /// <summary>
    /// 主动（交换）消除宝石事件
    /// </summary>
    public UnityEvent<List<List<Jewel>>> OnMatchForSwap = new UnityEvent<List<List<Jewel>>>();
    /// <summary>
    /// 宝石序列消除事件（三消，四消，五消）
    /// </summary>
    public UnityEvent<List<Jewel>> OnMatchList = new UnityEvent<List<Jewel>>();
    /// <summary>
    /// 全局每个宝石消除事件
    /// </summary>
    public UnityEvent<Jewel> OnJewelMatch = new UnityEvent<Jewel>();
    /// <summary>
    /// 全局每个宝石生成事件
    /// </summary>
    public UnityEvent<Jewel> OnJewelCreate = new UnityEvent<Jewel>();
    /// <summary>
    /// 结束消除事件
    /// </summary>
    public UnityEvent OnMatchEnd = new UnityEvent();
    #endregion

    private void Awake()
    {
        //初始化单例
        Instance = this;

        //初始化数据
        Length = _mapInfo.GridX;
        Hight = _mapInfo.GridY;
        GridSize = new Vector3(Mathf.Abs( _mapInfo.MapX / Length), _mapInfo.MapY / Hight, 1);
        _jewelTypes = _mapItemInfo.JewelObjects.Keys.ToList();
    }

    private void Start()
    {
        //生成格子地图数据
        CreatGridMapData();
        //生成宝石地图
        CreatJewelMap();
    }
    #region CreateMap
    /// <summary>
    /// 生成宝石地图
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void CreatJewelMap()
    {
        //生成初始宝石地图的类型数据
        JewelType[,] typeMap = InitialJewelMapData();

        //生成宝石
        JewelsMap = new Jewel[Length, Hight];
        for (int x = 0; x < Length; x++)
        {
            for (int y = 0; y < Hight; y++)
            {
                if (!typeMap[x,y].Equals(JewelType.None))
                {
                    //从工厂获取一个宝石
                    var jewel = _jewelFactory.GetJewelWithType(typeMap[x,y]);
                    //加入集合
                    JewelsMap[x, y] = jewel;
                    //设置宝石位置
                    jewel.transform.position = GetGridPostion(x,y);
                    jewel.transform.localScale = new Vector3(Mathf.Abs(GridSize.x), Mathf.Abs(GridSize.y), 1);
                    jewel.gridPos = new Vector2Int(x, y);
                }
            }
        }

        JewelType[,] InitialJewelMapData()
        {
            var result = new JewelType[Length, Hight];

            for (int i = 0; i < Length; i++)
            {
                for (int j = 0; j < Hight; j++)
                {
                    if (GridMap[i,j].HasFlag(GridType.Jewel))
                    {
                        //设置一个类型数组
                        List<JewelType> tmp = new List<JewelType>();
                        tmp.AddRange(_jewelTypes);

                        //删去x-2及y-2两个位置的类型 保证初始不会出现3消
                        if (i - 2 >= 0)
                        {
                            tmp.Remove(result[i-2, j]);
                        }
                        if (j - 2 >= 0)
                        {
                            tmp.Remove(result[i, j - 2]);
                        }

                        //随机选取一个类型存入数据
                        result[i,j] = tmp[Random.Range(0, tmp.Count)];
                    }
                }
            }

            return result;
        }
    }

    /// <summary>
    /// 生成格子地图数据
    /// </summary>
    void CreatGridMapData()
    {
        //按设定长宽初始化
        GridMap = new GridType[Length, Hight];
        //赋值
        for (int i = 0; i < Length; i++)
        {
            for (int j = 0; j < Hight; j++)
            {
                if (_mapInfo.ObstaclesData[i,j])
                {
                    //设置空挡（障碍物）
                    GridMap[i, j] = GridType.Obstacles;
                }
                else
                {
                    //设置宝石
                    GridMap[i, j] = GridType.Jewel;
                }
            }
        }
    }
    #endregion
    #region Command
    /// <summary>
    /// 交换宝石
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public IEnumerator Swap(Jewel a, Jewel b)
    {
        //设置地图状态
        status = MapStatus.Swap;
        //广播交换事件
        OnSwap?.Invoke(a, b);
        //交换
        var squence = DOTween.Sequence();
        squence.Join(a.SwapMove(b.gridPos.x, b.gridPos.y));
        squence.Join(b.SwapMove(a.gridPos.x, a.gridPos.y));
        //等待交换结束
        yield return squence.Play().WaitForCompletion();

        //检测消除
        List<Jewel> decisionList = new List<Jewel>();//判定列表
        decisionList.Add(a);
        decisionList.Add(b);
        var matchList = DecisionMatch(decisionList);//可消除集合

        //无可消除宝石
        if (matchList == null)
        {
            //换回位置
            //交换
            var squence2 = DOTween.Sequence();
            squence2.Join(a.SwapMove(b.gridPos.x, b.gridPos.y));
            squence2.Join(b.SwapMove(a.gridPos.x, a.gridPos.y));
            //等待交换结束
            yield return squence2.Play().WaitForCompletion();

            //设置地图状态
            status = MapStatus.Default;
            //返回
            yield break;
        }
        else //可消除
        {
            //设置消除状态
            status = MapStatus.Fall;

            yield return null;//休息一帧

            //广播主动（交换）消除宝石事件
            OnMatchForSwap?.Invoke(matchList);

            //开始消除
            yield return StartCoroutine(Match(matchList));

            //广播结束消除事件
            OnMatchEnd?.Invoke();
            yield return new WaitForEndOfFrame();
            //结束消除状态
            status = MapStatus.Default;
        }
    }

    /// <summary>
    ///  消除宝石（递归）
    /// </summary>
    /// <param name="matchList"></param>
    /// <returns></returns>
    private IEnumerator Match(List<List<Jewel>> matchList)
    {
        //交换宝石到消除宝石的等待时间
        const float waitTime = 0.2f;
        var wait = new WaitForEndOfFrame();

        status = MapStatus.Fall;

        for (int i = 0; i < matchList.Count; i++)
        {
            //广播宝石序列消除事件
            OnMatchList?.Invoke(matchList[i]);
        }
        yield return null;

        //提取要消除的所有宝石
        List<Jewel> jewels = new List<Jewel>();
        foreach (var list in matchList)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (!jewels.Contains(list[i]))
                {
                    jewels.Add(list[i]);
                }
            }
        }

        for (int i = 0; i < jewels.Count; i += 6) //防卡帧 每帧6个
        {
            //广播全局宝石消除事件
            OnJewelMatch?.Invoke(jewels[i]);
            if (i + 1 < jewels.Count) OnJewelMatch?.Invoke(jewels[i + 1]);
            if (i + 2 < jewels.Count) OnJewelMatch?.Invoke(jewels[i + 2]);
            if (i + 3 < jewels.Count) OnJewelMatch?.Invoke(jewels[i + 3]);
            if (i + 4 < jewels.Count) OnJewelMatch?.Invoke(jewels[i + 4]);
            if (i + 5 < jewels.Count) OnJewelMatch?.Invoke(jewels[i + 5]);

            yield return null;
        }

        //消除宝石
        for (int i = 0; i < jewels.Count; i += 6) //防卡帧 每帧6个
        {
            StartCoroutine(jewels[i].Match(waitTime, matchList, jewels));
            if (i + 1 < jewels.Count) StartCoroutine(jewels[i + 1].Match(waitTime, matchList, jewels));
            if (i + 2 < jewels.Count) StartCoroutine(jewels[i + 2].Match(waitTime, matchList, jewels));
            if (i + 3 < jewels.Count) StartCoroutine(jewels[i + 3].Match(waitTime, matchList, jewels));
            if (i + 4 < jewels.Count) StartCoroutine(jewels[i + 4].Match(waitTime, matchList, jewels));
            if (i + 5 < jewels.Count) StartCoroutine(jewels[i + 5].Match(waitTime, matchList, jewels));

            yield return null;
        }

        //等待直到最后一个宝石消除完成
        var last = jewels[jewels.Count - 1];
        while (JewelsMap[last.gridPos.x, last.gridPos.y] == last)
        {
            yield return null;
        }
        yield return wait;

        //下落列表
        List<Jewel> fallJewels = new List<Jewel>();
        var squence = DOTween.Sequence();
        //检测要下落的宝石
        for (int x = 0; x < Length; x++)
        {
            int upDetectionCnt = 0;//向上检测计数
            int fallCnt = 0;//下落计数

            for (int y = Hight - 1; y >= 0; y--)
            {
                if (JewelsMap[x, y] == null && GridMap[x,y].HasFlag(GridType.Jewel))
                {
                    bool have = false;
                    //向上检测宝石
                    for (int pos = y - 1; pos >= 0; pos--)
                    {
                        if (JewelsMap[x,pos] != null && GridMap[x, pos].HasFlag(GridType.Jewel))
                        {
                            var fall = JewelsMap[x,pos];

                            //暂时从集合移除
                            JewelsMap[x, pos] = null;
                            //下落宝石
                            squence.Join(fall.FallMove(x, y));
                            fallJewels.Add(fall);

                            fallCnt++;
                            if (fallCnt >= 5) //每下落6个等待一帧 防止卡顿
                            {
                                fallCnt = 0;
                                yield return wait;
                            }

                            have = true;
                            break;
                        }
                    }

                    //检测不到则生成新宝石
                    if (!have)
                    {
                        upDetectionCnt++;
                        var newJewel = Create(x, -upDetectionCnt);
                        //下落新宝石
                        squence.Join(newJewel.FallMove(x, y));
                        fallJewels.Add(newJewel);

                        fallCnt++;
                        if (fallCnt >= 5) //每下落6个等待一帧 防止卡顿
                        {
                            fallCnt = 0;
                            yield return wait;
                        }
                    }
                }
            }
        }

        //等待所有宝石下落
        yield return squence.Play().WaitForCompletion();
        yield return wait;

        //检测消除
        var lastMatchList = DecisionMatch(fallJewels);//可消除集合

        //无可消除宝石
        if (lastMatchList == null)
        {
            //结束消除
            yield return null;//休息一帧
        }
        else //可消除
        {
            yield return null;//休息一帧
            //开始消除
            yield return StartCoroutine(Match(lastMatchList));
        }

        status = MapStatus.Default;
    }

    /// <summary>
    /// 外部调用消除方法但在地图已在消除中时，将消除载入本集合
    /// </summary>
    List<List<Jewel>> _otherTimeMatchList = new List<List<Jewel>>();

    /// <summary>
    /// 消除某个宝石
    /// </summary>
    /// <param name="jewel"></param>
    public void Match(Jewel jewel)
    {
        List<Jewel> jewels = new List<Jewel>();
        jewels.Add(jewel);
        List<List<Jewel>> jewels2 = new List<List<Jewel>>();
        jewels2.Add(jewels);

        if (status != MapStatus.Default) //不在默认状态时载入other
        {
            _otherTimeMatchList.AddRange(jewels2);
        }
        else
        {
            StartCoroutine(Match(jewels2));
        }
    }
    public Jewel Create(int x, int y, bool canSetGridPos = false)
    {
        //从工厂随机生成一个宝石
        var obj = _jewelFactory.GetJewelWithType(_jewelTypes[Random.Range(0, _jewelTypes.Count)]);
        //广播全局宝石生成事件
        OnJewelCreate?.Invoke(obj);
        //初始化
        obj.Initial(GetGridPostion(x, y), new Vector3(Mathf.Abs(GridSize.x), Mathf.Abs(GridSize.y), 1));
        if (canSetGridPos)
        {
            obj.SetGridPosition(x, y);
        }
        return obj;
    }

    /// <summary>
    /// 检测消除
    /// </summary>
    /// <param name="queue"></param>
    /// <returns></returns>
    List<List<Jewel>> DecisionMatch(List<Jewel> decisions)
    {
        List<List<Jewel>> result = new List<List<Jewel>>();

        while (decisions.Count > 0)
        {
            Jewel obj = decisions[0];
            decisions.RemoveAt(0);

            //横向
            var horizontal = _DecisionHorizontal(obj);
            if (horizontal != null)
            {
                //添加消除
                result.Add(horizontal);
                //修剪判定
                decisions.RemoveAtList(horizontal);
            }
            //纵向
            var vertical = _DecisionVertical(obj);
            if (vertical != null)
            {
                //添加消除
                result.Add(vertical);
                //修剪判定
                decisions.RemoveAtList(vertical);
            }
        }
        //外部消除
        if (_otherTimeMatchList.Count > 0)//添加外部消除
        {
            result.AddRange(_otherTimeMatchList);
            _otherTimeMatchList.Clear();
        }

        //存在消除
        if (result.Count > 0)
        {
            return result;
        }
        return null;

        //横向检测
        List<Jewel> _DecisionHorizontal(Jewel obj)
        {
            List<Jewel> jewels = new List<Jewel>();

            //自身
            jewels.Add(obj);

            //左
            for (int x = obj.gridPos.x - 1; x >= 0; x--)
            {
                var left = JewelsMap[x, obj.gridPos.y];
                if (GridMap[x, obj.gridPos.y].HasFlag(GridType.Jewel) && Jewel.TypeEqual(obj, left))
                {
                    jewels.Add(left);
                }
                else
                {
                    break;
                }
            }
            //右
            for (int x = obj.gridPos.x + 1; x < Length; x++)
            {
                var right = JewelsMap[x, obj.gridPos.y];
                if (GridMap[x, obj.gridPos.y].HasFlag(GridType.Jewel) && Jewel.TypeEqual(obj,right))
                {
                    jewels.Add(right);
                }
                else
                {
                    break;
                }
            }

            //判定消除成功
            if (jewels.Count >= 3)
            {
                //返回消除列表
                return jewels;
            }
            //失败返回Null
            return null;
        }
        //纵向检测
        List<Jewel> _DecisionVertical(Jewel obj)
        {
            List<Jewel> jewels = new List<Jewel>();

            //自身
            jewels.Add(obj);

            //上
            for (int y = obj.gridPos.y - 1; y >= 0; y--)
            {
                var up = JewelsMap[obj.gridPos.x, y];
                if (GridMap[obj.gridPos.x, y].HasFlag(GridType.Jewel) && Jewel.TypeEqual(obj, up) )
                {
                    jewels.Add(up);
                }
                else
                {
                    break;
                }
            }
            //下
            for (int y = obj.gridPos.y + 1; y < Hight; y++)
            {
                var down = JewelsMap[obj.gridPos.x, y];
                if (GridMap[obj.gridPos.x, y].HasFlag(GridType.Jewel) && Jewel.TypeEqual(obj, down))
                {
                    jewels.Add(down);
                }
                else
                {
                    break;
                }
            }

            //判定消除成功
            if (jewels.Count >= 3)
            {
                //返回消除列表
                return jewels;
            }
            //失败返回Null
            return null;
        }
    }
    #endregion
    #region Utility

    /// <summary>
    /// 获取格子（x,y)的世界坐标
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Vector3 GetGridPostion(int x, int y)
    {
        Vector3 result = new Vector3(0, 0, transform.position.z);

        //从左往右，从上到下计算
        result.x = transform.position.x + GridSize.x * x;
        result.y = transform.position.y - GridSize.y * y;

        return result;
    }

    #endregion
}
