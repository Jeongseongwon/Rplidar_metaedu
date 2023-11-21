using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Threading;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RplidarTest_Ray : MonoBehaviour
{
    public string port;
    //public GameObject Capsule;

    private LidarData[] data;
    private RectTransform Img_Rect_transform;
    private GameObject CANVAS;

    //=====0714
    public GameObject BALLPrefab; //spherePrefab을 받을 변수 
    public GameObject MOUSEPrefab; //spherePrefab을 받을 변수 

    public bool m_onscan = false;
    private Thread m_thread;
    private bool m_datachanged = false;
    //=====
    private Vector3 Temp_position;

    //=====


    //====1012
    public bool Test_check = false;
    double number = 0f;

    public GameObject Guideline;
    public GameObject TESTUI;
    //

    //1015

    private float Resolution_Y = 1080;
    private float Resolution_X = 1920;

    public float min_x;
    public float min_y;
    public float max_x;
    public float max_y;


    private Camera cameraToLookAt;

    public float Test_degree;
    public float Test_distance;

    [SerializeField]
    public GameObject temp_pos;

    public float Sensor_rotation = 0;

    //1121

    public Canvas UI_Canvas;
    GraphicRaycaster GR;
    PointerEventData PED;

    private Camera UI_Camera;

    private float x;
    private float y;
    private float Cal_y;

    private void Awake()
    {
        data = new LidarData[720];
    }

    void Start()
    {
        int result = RplidarBinding.OnConnect(port);
        Debug.Log("Connect on " + port + " result:" + result);

        bool r = RplidarBinding.StartMotor();
        Debug.Log("StartMotor:" + r);

        m_onscan = RplidarBinding.StartScan();
        Debug.Log("StartScan:" + m_onscan);

        if (m_onscan)
        {
            m_thread = new Thread(GenMesh);
            m_thread.Start();
        }

        Img_Rect_transform = this.GetComponent<RectTransform>();
        CANVAS = GameObject.Find("3DCANVAS");
        UI_Camera = GameObject.Find("UICamera").GetComponent<Camera>();

        GR = UI_Canvas.GetComponent<GraphicRaycaster>();
        PED = new PointerEventData(null);

        //guide라인이랑 동기화 기능
        min_x = Guideline.GetComponent<RectTransform>().anchoredPosition.x - (Guideline.GetComponent<RectTransform>().rect.width) / 2;
        min_y = Guideline.GetComponent<RectTransform>().anchoredPosition.y - (Guideline.GetComponent<RectTransform>().rect.height) / 2;
        max_x = Guideline.GetComponent<RectTransform>().anchoredPosition.x + (Guideline.GetComponent<RectTransform>().rect.width) / 2;
        max_y = Guideline.GetComponent<RectTransform>().anchoredPosition.y + (Guideline.GetComponent<RectTransform>().rect.height) / 2;

        //실제 거리를 중간에 보정값이랑 곱한 만큼 보정을 해서 추가가 필요함
        //10cm라 가정하고 50으로 환산
        Cal_y = 0;
        //Cal_y = -50;



    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //    Test_check = !Test_check;


    }
    void GenMesh()
    {
        while (true)
        {
            int datacount = RplidarBinding.GetData(ref data);

            if (datacount == 0)
            {
                Thread.Sleep(5);
            }
            else
            {
                m_datachanged = true;
            }
        }
    }
    //해상도 1920,1080

    // Update is called once per frame
    void FixedUpdate()
    {
        if (m_datachanged)
        {
            for (int i = 0; i < 720; i++)
            {
                //센서 데이터 data[i].theta, distant
                //1. 화면과 센서를 일치화 시키기 위해서 theta를 마이너스 곱해줌, 추가로 회전 시켜주기 위해 Sensor_rotation 추가했고 위에서 아래 방향으로 내려다 보는것 기준으 90도 입력하면 댐
                x = 0.74f * Mathf.Cos((-data[i].theta + Sensor_rotation) * Mathf.Deg2Rad) * data[i].distant;
                y = Cal_y + 540 + 0.74f * Mathf.Sin((-data[i].theta + Sensor_rotation) * Mathf.Deg2Rad) * data[i].distant;

                if (i % 4 == 0)
                {

                   
                    if (x != 0 || y != 0)
                    {
                        if (min_x < x && x < max_x)
                        {
                            if (min_y < y && y < max_y)
                            {
                                if (Test_check)
                                {
                                    //데모용, 마우스
                                    //Guideline.SetActive(false);
                                    //TESTUI.SetActive(false);
                                    GameObject Prefab_pos = Instantiate(MOUSEPrefab, this.transform.position, Quaternion.Euler(0, 0, 0), CANVAS.transform);
                                    Prefab_pos.GetComponent<RectTransform>().anchoredPosition = new Vector3(x + 30, y - 30, 0);
                                    Prefab_pos.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 0);

                                    Debug.Log("MOUSE CLICKED");

                                    //PED.position = Input.mousePosition;
                                    PED.position = Prefab_pos.GetComponent<RectTransform>().anchoredPosition;

                                    List<RaycastResult> results = new List<RaycastResult>();
                                    GR.Raycast(PED, results);


                                    if (results[0].gameObject)
                                    {
                                        Debug.Log(results[0].gameObject.name);
                                    }


                                }
                                else
                                {
                                    //개발자용, 공
                                    Guideline.SetActive(true);
                                    TESTUI.SetActive(true);
                                    GameObject Prefab_pos = Instantiate(BALLPrefab, CANVAS.transform.position, Quaternion.Euler(0, 0, 0), CANVAS.transform);
                                    Prefab_pos.GetComponent<RectTransform>().anchoredPosition = new Vector3(x + 30, y - 30, 0);
                                    Prefab_pos.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 0);


                                    //1. UI 카메라상에서 마우스 클릭한걸 메인 카메라에서 제대로 레이 캐스트가 되는지 확인


                                    // 1. 마우스 포지션으로 메인 카메라로 레이 캐스트 하는거 정상작동 확인함
                                    // 2. UI 캔버스상에 있는 이미지의 위치로 메인 카메라에 투영할 수 있는지 이게 중요함
                                    // 아까 해보자 했던 아이디어는 그냥 그 위치 좌표 그대로 그냥 가져와서 하자는거 였음

                                    //

                                    //실질적으로 공이 찍히는 카메라는 UI 카메라
                                    //레이를 찍을 카메라는 메인 카메라

                                    //UI 캔버스 위에 점을 찍음, UI 캔버스 상에서의 x,y좌표가 메인 카메라에서도 똑같다는거지
                                    //메인 캔버스(3D 오브젝트용) 위에 x,y좌표를 두고 그걸 기준으로 레이를 쏴주는거지

                                    Ray ray = Camera.main.ScreenPointToRay(Prefab_pos.GetComponent<RectTransform>().anchoredPosition);
                                    RaycastHit hit;
                                    if (Physics.Raycast(ray, out hit))
                                    {
                                        Debug.Log(hit.transform.name);
                                    }

                                    ////PED.position = Input.mousePosition;
                                    //PED.position = Prefab_pos.GetComponent<RectTransform>().anchoredPosition;

                                    //List<RaycastResult> results = new List<RaycastResult>();
                                    //GR.Raycast(PED, results);


                                    //if (results.Count>0)
                                    //{
                                    //    Debug.Log(results[0].gameObject.name);
                                    //}

                                }

                                //2D랑 3D를 어떻게 구분할 것인가?
                                //기존의 콘텐츠는 전부 3D 기반으로 만들어져있고
                                //일부 콘텐츠의 경우 2D로 만들어져있음
                                
                                

                            }
                        }
                    }

                }
            }
            m_datachanged = false;
        }

    }
   
    void OnDestroy()
    {
        RplidarBinding.EndScan();
        RplidarBinding.EndMotor();
        RplidarBinding.OnDisconnect();
        RplidarBinding.ReleaseDrive();

        //StopCoroutine(GenMesh());

        m_thread?.Abort();

        m_onscan = false;
    }
}
