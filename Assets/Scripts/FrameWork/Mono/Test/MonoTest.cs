using System.Collections;
using UnityEngine;

public class Test2
{
    public Test2()
    {
        MonoController.Instance.StartCoroutine(Test123());

        // MonoManager.Instance.StartCoroutine(Test123());
    }
    
    public void Update()
    {
        Debug.Log("Test2");
    }

    IEnumerator Test123()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            Debug.Log("123123123");
        }
    }
}
public class MonoTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Test2 t = new Test2();
        MonoController.Instance.AddUpdateListener(t.Update);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
