using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    //将点击屏幕的屏幕坐标转换为世界坐标
    private Vector3 mousePositionInWorld;

    private GameObject obj;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            mousePositionInWorld = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
                Input.mousePosition.y,
                Camera.main.transform.position.z > 0
                    ? Camera.main.transform.position.z
                    : -Camera.main.transform.position.z)); //屏幕坐标转换世界坐标
        }

        if (Input.GetMouseButtonDown(0))
        {
            PoolManager.Instance.GetObj("Cube", (obj) => { obj.transform.position = mousePositionInWorld; });
        }
        else if (Input.GetMouseButtonDown(1))
        {
            PoolManager.Instance.GetObj("Sphere", (obj) => { obj.transform.position = mousePositionInWorld; });
        }
    }
}