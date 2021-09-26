using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public bool isChongHe = false;          // 是否與其對應的node重合

    private GameObject mouseObject;

    // Use this for initialization
    private void Start()
    {
        mouseObject = GameObject.Find("mouse");
    }

    // 進入對應的position時，將isChongHe置為true
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "position")
        {
            if (other.gameObject.name == mouseObject.GetComponent<Puzzle>().puzzlename) isChongHe = true;
        }
    }

    // 離開對應的position時，將isChongHe置為false
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "position")
        {
            if (other.gameObject.name == mouseObject.GetComponent<Puzzle>().puzzlename) isChongHe = false;
        }
    }

    // 當自身被拖動到對應的position處並放開後
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
