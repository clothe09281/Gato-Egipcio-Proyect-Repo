using UnityEngine;
using UnityEngine.UI;
using System.Linq;

/// <summary>
/// 敵人基底類別
/// 功能：隨機走動、等待、追蹤玩家、受傷與死亡、狀態檢查能
/// 狀態機：列舉 Enum、判斷式 switch (基礎語法)
/// </summary>
public class BaseEnemy : MonoBehaviour
{
    #region 欄位
    [Header("基本能力")]
    [Range(50, 5000)]
    public float hp = 100;
    [Range(5, 1000)]
    public float attack = 20;
    [Range(1, 500)]
    public float speed = 100.0f;

    /// <summary>
    /// 隨機等待範圍
    /// </summary>
    public Vector2 v2RandomIdle = new Vector2(1, 5);
    /// <summary>
    /// 隨機走路範圍
    /// </summary>
    public Vector2 v2RandomWalk = new Vector2(3, 6);

    // 將私人欄位顯示在屬性面板上
    [SerializeField]
    protected StateEnemy state;
    // public KeyCode key;

    private Rigidbody2D rig;
    private Animator ani;
    private AudioSource aud;
    /// <summary>
    /// 等待時間：隨機
    /// </summary>
    private float timeIdle;
    /// <summary>
    /// 等待：等待用計時器
    /// </summary>
    private float timerIdle;
    /// <summary>
    /// 走路時間：隨機
    /// </summary>
    private float timeWalk;
    /// <summary>
    /// 走路用計時器
    /// </summary>
    private float timerWalk;
    #endregion

    /// <summary>
    /// 玩家類別
    /// </summary>
    protected Player player;
    /// <summary>
    /// 攻擊區域的碰撞
    /// </summary>
    protected Collider2D hit;

    [Header("掉落道具資料：道具、機率")]
    public GameObject goPropHp1;
    public GameObject goPropHp2;
    public GameObject goPropHp3;
    public GameObject goPropAtk1;
    public GameObject goPropAtk2;
    public GameObject goPropAtk3;
    [Range(0, 1)]
    public float propProbability1 = 0.1f;
    [Range(0, 1)]
    public float propProbability2 = 0.2f;
    [Range(0, 1)]
    public float propProbability3 = 0.3f;
    private float ProbabilityHp;
    private float ProbabilityAtk;

    [Header("怪物血條")]
    public Image imgHp;
    public RectTransform posHp;
    public Vector3 v3HpOffset;
    private float hpMax;

    #region 事件
    private void Start()
    {
        #region 取得元件
        rig = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
        aud = GetComponent<AudioSource>();

        player = GameObject.Find("主角貓").GetComponent<Player>();
        #endregion

        #region 初始值設定
        timeIdle = Random.Range(v2RandomIdle.x, v2RandomIdle.y);
        #endregion

        hpMax = hp;
    }
    protected virtual void Update()
    {
        CheckState();
        // Walk();
        WalkInFixedUpdate();
        checkForward();
        // Attack();
        // RandomDirection();
    }
    private void FixedUpdate()
    {
        posHp.position = transform.position + v3HpOffset;
    }

    [Header("檢查前方是否有障礙物或地板球體")]
    public Vector3 checkForwardOffset;
    [Range(0, 1)]
    public float checkForwardRadius = 0.3f;

    // 父類別的成員如果希望子類別複寫必須遵循：
    // 1. 修飾詞必須是 public 或 protected - 保護 允許子類別存取
    // 2. 添加關鍵字 virtual 虛擬 - 允許子類別存取
    // 3. 子類別使用 override 複寫
    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0.3f, 0.3f, 0.3f);
        // transform.right 當前物件的右方 (2D模式為前方，紅色箭頭)
        // transform.up 當前物件的上方 (綠色箭頭)
        Gizmos.DrawSphere(
            transform.position +
            transform.right * checkForwardOffset.x +
            transform.up * checkForwardOffset.y,
            checkForwardRadius);
    }
    #endregion

    // 認識陣列
    // 語法：類型後方加上中括號，例如：int[]、float[]、string[]、Vector2[]
    public Collider2D[] hits;
    public Collider2D[] hitResult;

    #region 方法
    /// <summary>
    /// 檢查狀態
    /// </summary>
    private void CheckState()
    {
        switch (state)
        {
            case StateEnemy.idle:
                Idle();
                break;
            case StateEnemy.walk:
                Walk();
                break;
            case StateEnemy.track:
                break;
            case StateEnemy.attack:
                Attack();
                break;
            case StateEnemy.dead:
                Dead();
                break;
        }
    }

    /// <summary>
    /// 等待：隨機秒數後進入到走路狀態
    /// 判定後切換至走路狀態
    /// </summary>
    private void Idle()
    {
        if (timerIdle < timeIdle)           // 如果 計時器 < 等待時間
        {
            timerIdle += Time.deltaTime;    // 累加時間
            ani.SetBool("走路開關", false);  // 關閉走路動畫
            // print("等待");
        }
        else                                // 否則
        {
            RandomDirection();              // 隨機方向
            state = StateEnemy.walk;        // 切換狀態
            timeWalk = Random.Range(v2RandomWalk.x, v2RandomWalk.y);    // 取得隨機走路時間
            timerIdle = 0;
        }
    }

    /// <summary>
    /// 隨機走路
    /// </summary>
    private void Walk()
    {
        // print("走路");

        if (timerWalk < timeWalk)
        {
            timerWalk += Time.deltaTime;
            ani.SetBool("走路開關", true);  // 開啟走路動畫
            // rig.velocity = transform.right * speed * Time.deltaTime;
        }
        else
        {
            state = StateEnemy.idle;
            rig.velocity = Vector2.zero;
            timeIdle = Random.Range(v2RandomIdle.x, v2RandomIdle.y);
            timerWalk = 0;
        }
    }

    /// <summary>
    /// 將物理行為單獨處理並在 FixedUpdate 呼叫
    /// </summary>
    private void WalkInFixedUpdate()
    {
        // 如果 目前狀態 是移動 就 剛體.加速度 = 右邊 * 速度 * 1/50 + 上方 * 地心引力
        if (state == StateEnemy.walk) rig.velocity = transform.right * speed * Time.deltaTime + Vector3.up * rig.velocity.y;
    }

    /// <summary>
    /// 隨機方向：隨機面向右邊或左邊
    /// 值為 0 時，左邊：0, 180, 0
    /// 值為 1 時，右邊：0, 0, 0
    /// </summary>
    private void RandomDirection()
    {
        // 隨機.範圍(最小, 最大) - 整數時不包含最大值 (0, 2) - 隨機取得 0 或 1
        int random = Random.Range(0, 2);

        if (random == 0) transform.eulerAngles = Vector2.up * 180;
        else transform.eulerAngles = Vector2.zero;
    }

    private void checkForward()
    {
        hits = Physics2D.OverlapCircleAll(
            transform.position +
            transform.right * checkForwardOffset.x +
            transform.up * checkForwardOffset.y,
            checkForwardRadius);

        // print("前方碰到的物件：" + hit.name);
        // 兩種情況要轉向，避免撞到障礙物以及掉落
        // 1. 陣列內是空的 - 沒有地方戰力會掉落
        // 2. 陣列內有不是 地板 並且 不是跳台 的物件 - 有障礙物
        // 查詢語言 LinQ：可以查詢陣列資料，例如：是否包含地板、是否有資料等等……

        // 碰到地板、跳台、主角皆不會轉向
        hitResult = hits.Where(x => x.name != "地板" && x.name != "跳台" && x.name != "主角貓" && x.name != "可穿透地板").ToArray();

        // 陣列為空值：陣列數量為零
        // 如果 碰撞數量為零 (前方沒有地方站立) 或者 碰撞結果大於零 (前方有障礙物) 都要轉向
        if (hits.Length == 0)
        {
            // print("前方沒有地板會掉落");
            TurnDirection();
        }
    }

    /// <summary>
    /// 轉向
    /// </summary>
    private void TurnDirection()
    {
        float y = transform.eulerAngles.y;
        if (y == 0) transform.eulerAngles = Vector3.up * 180;
        else transform.eulerAngles = Vector3.zero;
    }

    [Range(0.5f, 5)]
    /// <summary>
    /// 攻擊冷卻時間
    /// </summary>
    public float cdAttack = 3;
    // 陣列：保存相同類型的資料表格，擁有編號與值兩份資料
    // 陣列語法：類型[]
    // 例如： int[]、string[]、GameObject[]、Vector3[]
    [Header("攻擊延遲，可自設數量"), Range(0, 5)]
    public float[] attacksDelay;
    [Header("攻擊完成後隔多久恢復原本狀態"), Range(0, 5)]
    public float afterAttackRestoreOriginal = 1;

    private float timerAttack;

    /// <summary>
    /// 攻擊狀態：執行攻擊並添加冷卻
    /// </summary>
    private void Attack()
    {
        if (timerAttack < cdAttack)
        {
            timerAttack += Time.deltaTime;
            ani.SetBool("走路開關", false);
        }
        else
        {
            AttackMethod();
        }
    }

    /// <summary>
    /// 子類別可以決定該如何攻擊的方法
    /// </summary>
    protected virtual void AttackMethod()
    {
        timerAttack = 0;
        ani.SetTrigger("攻擊觸發1");
        // print("攻擊");
    }

    /// <summary>
    /// 受傷
    /// </summary>
    /// <param name="damage"></param>
    public void Hurt(float damage)
    {
        hp -= damage;
        ani.SetTrigger("受傷觸發");
        imgHp.fillAmount = hp / hpMax;

        if (hp <= 0) Dead();
    }

    /// <summary>
    /// 死亡：死亡動畫、狀態、關閉腳本、碰撞器、加速度以及剛體凍結
    /// </summary>
    private void Dead()
    {
        hp = 0;
        ani.SetBool("死亡開關", true);
        state = StateEnemy.dead;
        GetComponent<CapsuleCollider2D>().enabled = false;      // 關閉碰撞器
        rig.velocity = Vector3.zero;                            // 加速度歸零
        rig.constraints = RigidbodyConstraints2D.FreezeAll;     // 剛體凍結全部
        DropProp();

        // TeleportManager.countAllEnemy--;        // 通知傳送管理將數量 -1

        enabled = false;
    }

    /// <summary>
    /// 死亡後呼叫掉落道具方法，機率性掉落
    /// </summary>
    private void DropProp()
    {
        ProbabilityHp = Random.value;
        ProbabilityAtk = Random.value;

        // 生成(物件,座標,角度)
        // Quaternion.identity 零角度 = Vector3.zero
        if (ProbabilityHp <= propProbability1)
            Instantiate(goPropHp1, transform.position + Vector3.up * 1.5f, Quaternion.identity);
        else if (ProbabilityHp <= propProbability1 + propProbability2)
            Instantiate(goPropHp2, transform.position + Vector3.up * 1.5f, Quaternion.identity);
        else if (ProbabilityHp <= propProbability1 + propProbability2 + propProbability3)
            Instantiate(goPropHp3, transform.position + Vector3.up * 1.5f, Quaternion.identity);

        if (ProbabilityAtk <= propProbability1)
            Instantiate(goPropAtk1, transform.position + Vector3.up * 1.5f, Quaternion.identity);
        else if (ProbabilityAtk <= propProbability1 + propProbability2)
            Instantiate(goPropAtk2, transform.position + Vector3.up * 1.5f, Quaternion.identity);
        else if (ProbabilityAtk <= propProbability1 + propProbability2 + propProbability3)
            Instantiate(goPropAtk3, transform.position + Vector3.up * 1.5f, Quaternion.identity);
    }
    #endregion
}

// 定義列舉
// 1. 使用關鍵字 enum 定義列舉以及包含的選項，可以在類別外額外定義
// 2. 需要有一個欄位定義為此列舉類型
// 語法：修飾詞 enum 列舉名稱 { 選項1，選項2，...，選項N}
public enum StateEnemy
{
    idle, walk, track, attack, dead
}
