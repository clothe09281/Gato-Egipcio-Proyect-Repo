using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    [Header("要移動的物體")]
    public GameObject Platform;
    [Header("要移動物體開始的點")]
    public Transform StartPoint;
    [Header("要移動物體結束的點")]
    public Transform EndPoint;
    [Header("移動速度")]
    public float MoveSpeed;
    [Header("目標點")]
    public Vector2 target;

    private void Start()
    {
        target = EndPoint.position;
    }

    private void Update()
    {
        Platform.transform.position = Vector2.MoveTowards(Platform.transform.position, target, MoveSpeed * Time.deltaTime);

        if (Platform.transform.position == EndPoint.position)
        {
            target = StartPoint.position;
        }

        if (Platform.transform.position == StartPoint.position)
        {
            target = EndPoint.position;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        other.transform.SetParent(transform);
    }
    private void OnCollisionExit2D(Collision2D other)
    {
        other.transform.SetParent(null);
    }
}
