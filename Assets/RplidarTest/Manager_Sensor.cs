using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager_Sensor : MonoBehaviour
{
    public static Manager_Sensor instance = null;

    private Canvas UI_Manager;
    private Camera UI_Camera;


    // Start is called before the first frame update
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (instance != this)
                Destroy(this.gameObject);
        }
    }

    void Start()
    {
        UI_Camera = GameObject.FindWithTag("UICamera").GetComponent<Camera>();
        UI_Manager = GameObject.Find("UIManager").GetComponent<Canvas>();
    }

    public Canvas Get_UImanager()
    {
        return UI_Manager;
    }
    public Camera Get_UIcamera()
    {
        return UI_Camera;
    }
}
