using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Manager_Sensor : MonoBehaviour
{
    public static Manager_Sensor instance = null;

    public GameObject UI_Canvas;
    public Camera UI_Camera;

    public RectTransform Ray_position;

    public float Prev_Ray_position_x;
    public float Prev_Ray_position_y;

    GraphicRaycaster GR;
    PointerEventData PED;

    public GameObject BALLPrefab; 
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
        //Init
        Ray_position = this.gameObject.GetComponent<RectTransform>();
        Prev_Ray_position_x = 0f;
        Prev_Ray_position_y = 0f;
    }

    public GameObject Get_UIcanvas()
    {
        return UI_Canvas;
    }
    public Camera Get_UIcamera()
    {
        return UI_Camera;
    }
    public void Set_RayPosition(RectTransform RayPos)
    {
        Ray_position = RayPos;
        //상태 판별
    }
    public void Set_PrevRayPosition(float x, float y)
    {
        Prev_Ray_position_x = x;
        Prev_Ray_position_y = y;

        //상태 판별
    }

}
