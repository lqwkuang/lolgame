using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class baseAttri : MonoBehaviour
{
    private static baseAttri instance;
    public int Speed;

    public static baseAttri Instance { get => instance; set => instance = value; }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log($"Speed{Speed}");
        }
    }
}
