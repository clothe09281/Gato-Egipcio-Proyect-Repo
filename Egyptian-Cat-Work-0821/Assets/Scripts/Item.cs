using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public bool isChongHe = false;          // �O�_�P�������node���X

    private GameObject mouseObject;

    // Use this for initialization
    private void Start()
    {
        mouseObject = GameObject.Find("mouse");
    }

    // �i�J������position�ɡA�NisChongHe�m��true
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "position")
        {
            if (other.gameObject.name == mouseObject.GetComponent<Puzzle>().puzzlename) isChongHe = true;
        }
    }

    // ���}������position�ɡA�NisChongHe�m��false
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "position")
        {
            if (other.gameObject.name == mouseObject.GetComponent<Puzzle>().puzzlename) isChongHe = false;
        }
    }

    // ��ۨ��Q��ʨ������position�B�é�}��
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "position")
        {
            if (other.gameObject.name == mouseObject.GetComponent<Puzzle>().puzzlename && !mouseObject.GetComponent<Puzzle>().isClicked)
            {
                this.transform.tag = "position";
                this.transform.position = other.transform.position;
            }
        }
    }
}
