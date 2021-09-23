using UnityEngine;
using System.Collections;

/// <summary>
/// 攝影機追蹤玩家
/// 玩家打擊到物件的打擊效果
/// </summary>
public class CameraController : MonoBehaviour
{
    #region 欄位
    [Header("追蹤速度"), Range(0, 100)]
    public float speed = 10;
    [Header("要追蹤的物件名稱")]
    public string nameTarget;
    [Header("左右限制")]
    public Vector2 limitHorizontal;

    /// <summary>
    /// 要追蹤的玩家
    /// </summary>
    private Transform target;
    #endregion

    #region 事件
    private void Start()
    {
        // ※ 很吃效能，所以建議在 Start 內使用
        // 目標變形元件 = 遊戲物件.尋找(物件名稱).變形元件
        target = GameObject.Find(nameTarget).transform;
    }

    private void Update()
    {
        Track();
    }
    #endregion

    #region 方法
    /// <summary>
    /// 追蹤目標
    /// </summary>
    private void Track()
    {
        Vector3 posCamera = transform.position;     // A點：攝影機座標
        Vector3 posTarget = target.position;        // B點：目標物座標

        // 運算後的結果座標 = 取得 A點攝影機 與 B點目標物 之間的座標
        Vector3 posResult = Vector3.Lerp(posCamera, posTarget, speed * Time.deltaTime);
        // 攝影機 Z 軸放回預設 -10 避免看不到2D物件
        posResult.z = -10;

        // 使用夾住 API 限制 攝影機的 左右範圍
        posResult.x = Mathf.Clamp(posResult.x, limitHorizontal.x, limitHorizontal.y);

        // 此物件的座標 指定為 運算後的結果座標
        transform.position = posResult;
    }
    #endregion

    [Header("晃動的值"), Range(0, 5)]
    public float shakeValue = 0.2f;
    [Header("晃動的次數"), Range(0, 20)]
    public int shakeCount = 10;
    [Header("晃動的間隔"), Range(0, 5)]
    public float shakeInterval = 0.3f;

    /// <summary>
    /// 晃動效果
    /// </summary>
    public IEnumerator ShakeEffect()
    {
        Vector3 posOriginal = transform.position;               // 取得攝影機晃動前的座標
        for (int i = 0; i < shakeCount; i++)                    // 迴圈執行座標改變
        {
            Vector3 posShake = posOriginal;

            if (i % 2 == 0) posShake.x -= shakeValue;           // i 為 偶數 就 往左
            else posShake.x += shakeValue;                      // i 為 奇數 就 往右

            transform.position = posShake;
            yield return new WaitForSeconds(shakeInterval);
        }

        transform.position = posOriginal;                       // 攝影機恢復原始座標
    }
}
