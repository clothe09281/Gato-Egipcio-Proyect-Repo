using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle : MonoBehaviour
{
    public string puzzlename = "position";
    [Header("拼圖碎片")]
    public GameObject[] fragment;

    private bool isMouseDown = false;
    public bool isClicked = false;
    private bool overlap = false;
    private Vector3 oldposition;
    private GameObject target = null;
    private GameObject mouse;

    private void Start()
    {
        // 隨機初始位置
        for (int i = 0; i < fragment.Length; i++)
        {
            fragment[i].transform.position = new Vector3(Random.Range(-7, 7), Random.Range(-3.5f, 3.5f), 0);
        }

        mouse = GameObject.Find("mouse");
    }

    private void Update()
    {
        // 使mouse始終跟著鼠標走
        mouse.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));
        isMouseDown = Input.GetMouseButton(0);      // 鼠標左鍵按下時，isMouse為true

        if (!isMouseDown && isClicked)              // item被拖動過程中鼠標放開
        {
            isClicked = false;
            if (!overlap) target.transform.position = oldposition;
        }
        // 在item被拖動過程中保證其始終跟著mouse走，並時刻判定其是否與對應node重合
        if (isClicked)
        {
            target.transform.position = transform.position;
            overlap = target.GetComponent<Item>().isChongHe;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (isMouseDown && !isClicked && other.gameObject.tag == "puzzle")
        {
            isClicked = true;
            target = GameObject.Find(other.gameObject.name);
            oldposition = other.transform.position;
            puzzlename = "position_" + other.gameObject.name[8];
        }
    }
}
