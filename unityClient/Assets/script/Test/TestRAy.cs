using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRAy : MonoBehaviour
{
    public LayerMask mask;
    public Transform target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        test();
    }
    private void test()
    {
        Ray ray = new Ray(transform.position, (target.position - transform.position).normalized);
        Debug.DrawLine(transform.position, target.position);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 10,1<<LayerMask.NameToLayer("Ground")))
        {
            Debug.Log("66");
            if(hitInfo.transform.name== "target")
            {
                Debug.Log($"¼ì²âµ½{hitInfo.transform.name}");
            }
            
        }
    }
}
