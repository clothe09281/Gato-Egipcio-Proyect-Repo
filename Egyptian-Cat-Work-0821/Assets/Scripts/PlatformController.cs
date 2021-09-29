using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    [Header("�n���ʪ�����")]
    public GameObject Platform;
    [Header("�n���ʪ���}�l���I")]
    public Transform StartPoint;
    [Header("�n���ʪ��鵲�����I")]
    public Transform EndPoint;
    [Header("���ʳt��")]
    public float MoveSpeed;
    [Header("�ؼ��I")]
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
