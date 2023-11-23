using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager_Sensor : MonoBehaviour
{
    public static Manager_Sensor instance = null;

    public GameObject UI_Canvas;
    public Camera UI_Camera;


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
        
    }

    public GameObject Get_UIcanvas()
    {
        return UI_Canvas;
    }
    public Camera Get_UIcamera()
    {
        return UI_Camera;
    }
}
