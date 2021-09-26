using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseDrag : MonoBehaviour
{
    [Header("要拖曳的物件")]
    public GameObject item;

    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        // item = GameObject.Find("聖甲蟲(紫大)").GetComponent<GameObject>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            item.transform.position = Input.mousePosition;
        }
    }
    /**
    private void OnMouseDown()
    {
        offset = item.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    }

    private void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.position = curPosition;
    }
    **/
}
