using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 游戏管理器
/// </summary>
[DefaultExecutionOrder(10)]
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// 单例
    /// </summary>
    /// 

    public GameObject cube;
   

    public static GameManager Instance { get; private set; }

    #region Data
    [field: Header("时间")]
    [field: SerializeField, Tooltip("初始时间")]
    public float InitialTime { get; private set; } = 60f;

    /// <summary>
    /// 当前剩余时间
    /// </summary>
    private float RemainingTime { get; set; }

    [HideInEditorMode, Tooltip("每秒减去时间"), ReadOnly]
    public float reducedTime = 1f;

    [field: Header("金币")]
    [field: SerializeField, Tooltip("金币数量")]
    public float Gold { get; set; } = 0;

    [SerializeField, Tooltip("3消获得金币")]
    private int _goldMatch3 = 100;
    [SerializeField, Tooltip("4消获得金币")]
    private int _goldMatch4 = 200;
    [SerializeField, Tooltip("5消获得金币")]
    private int _goldMatch5 = 300;

    [field: Header("进度")]
    [field: SerializeField, Tooltip("进度")]
    public int Progress { get; private set; } = 0;
    [SerializeField, Tooltip("总进度")]
    private int _progressMax = 100;
    [SerializeField, Tooltip("3消获得进度")]
    private int _progressMatch3 = 1;
    [SerializeField, Tooltip("4消获得进度")]
    private int _progressMatch4 = 2;
    [SerializeField, Tooltip("5消获得进度")]
    private int _progressMatch5 = 3;

    [Header("金币换时间")]
    [SerializeField, Tooltip("需要多少金币")]
    private int _needGold = 100;
    [SerializeField, Tooltip("兑换多少时间")]
    private float _conversionTime = 10f;
    #endregion

    #region UI
    [Header("UI")]
    [SerializeField, Tooltip("时间文本")]
    Text _timeText;
    [SerializeField, Tooltip("金币文本")]
    Text _goldText;
    [SerializeField, Tooltip("进度条")]
    Slider _progressBar;
    [SerializeField, Tooltip("暂停按钮")]
    Button _PauseBtn;
    [SerializeField, Tooltip("金币换时间按钮")]
    Button _buyTimeBtn;
    [SerializeField, Tooltip("游戏胜利面板")]
    GameObject _winPanel;
    [SerializeField, Tooltip("游戏失败面板")]
    GameObject _losePanel;
    [SerializeField, Tooltip("右侧信息面板")]
    public Text _infoBoard;
    #endregion

    public bool IsPause { get; private set; } = false;

    private void Awake()
    {
        //设置单例
        Instance = this;

        //初始化数据
        RemainingTime = InitialTime;
        _progressBar.value = Progress;
        _progressBar.maxValue = _progressMax;

        Time.timeScale = 1f;
    }

    void Start()
    {
        //开始倒计时
        StartCoroutine(nameof(StartGameTime));
        //订阅宝石序列消除事件：增加金币及进度
        MapManager.Instance.OnMatchList.AddListener(AddGold);
        MapManager.Instance.OnMatchList.AddListener(AddProgress);

        // 初始为暂停状态
        PauseGame();
        IsPause = true;
        _PauseBtn.GetComponentInChildren<Text>().text = "Continue";

        //暂停按钮
        _PauseBtn.onClick.AddListener(() =>
        {
            if (!IsPause)
            {
                //暂停游戏
                cube.SetActive(true);
                _PauseBtn.GetComponentInChildren<Text>().text = "Continue";
                PauseGame();
            }
            else
            {
                //继续游戏
                cube.SetActive(false);
                _PauseBtn.GetComponentInChildren<Text>().text = "Pause";
                ContinueGame();
            }
        });
        //金币兑换时间按钮
        _buyTimeBtn.onClick.AddListener(() =>
        {
            if (Gold >= _needGold) //金币足够
            {
                Gold -= _needGold;
                RemainingTime += _conversionTime;
            }
        });
    }
    private void Update()
    {
        if (Time.timeScale <= 0) return; //游戏暂停时不执行该update

        //更新时间文本
        _timeText.text = $"Time: {GameManager.Instance.RemainingTime: 000}";
        //更新金币文本
        _goldText.text = $"Gold: {GameManager.Instance.Gold}";
        //更新进度条
        _progressBar.value = Progress;
    }

    #region Game
    IEnumerator StartGameTime()
    {
        var wait = new WaitForSeconds(1f);
        //倒计时
        while (RemainingTime > 0)
        {
            //等待1s
            yield return wait;

            RemainingTime -= reducedTime;
        }

        //游戏失败结束
        StartCoroutine(nameof(Lose));
    }

    private void AddProgress(List<Jewel> arg0)
    {
        if (arg0.Count == 3)
        {
            Progress += _progressMatch3;
        }
        if (arg0.Count == 4)
        {
            Progress += _progressMatch4;
        }
        if (arg0.Count >= 5)
        {
            Progress += _progressMatch5;
        }

        if (Progress >= _progressMax)
        {
            StartCoroutine(nameof(Win));
        }
    }

    private void AddGold(List<Jewel> arg0)
    {
        if (arg0.Count == 3)
        {
            Gold += _goldMatch3;
        }
        if (arg0.Count == 4)
        {
            Gold += _goldMatch4;
        }
        if (arg0.Count >= 5)
        {
            Gold += _goldMatch5;
        }
    }

    private IEnumerator Win()
    {
        //关闭进度事件
        MapManager.Instance.OnMatchList.RemoveListener(AddProgress);

        yield return null;

        //PauseGame();

        _winPanel.SetActive(true);
    }
    private IEnumerator Lose()
    {
        //关闭进度事件
        MapManager.Instance.OnMatchList.RemoveListener(AddProgress);

        yield return null;

        //PauseGame();

        _losePanel.SetActive(true);
    }
    #endregion

    #region Utility
    /// <summary>
    /// 暂停游戏
    /// </summary>
    public void PauseGame()
    {
        IsPause = true;
        Time.timeScale = 0;
    }
    /// <summary>
    /// 继续游戏
    /// </summary>
    public void ContinueGame()
    {
        IsPause = false;
        Time.timeScale = 1f;
    }

    /// <summary>
    /// 重玩游戏
    /// </summary>
    public void RestartGame()
    {
        DOTween.KillAll();
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }
    #endregion

}
